using JapanischTrainer.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace JapanischTrainer.Database
{
    public static class DataManager
    {
        #region Fields

        private static DBContext context;
        
        #endregion

        #region Public Methods

        #region Database

        public static void CreateDBContext()
        {
            using (context = new DBContext(DBContext.isoStorageDBConnectionString))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                }
            }
            context = null;
        }

        public static void ConnectToLocalStorageDatabase()
        {
            if (context != null)
            {
                return;
            }

            context = new DBContext(DBContext.isoStorageDBConnectionString);
        }

        public static void CloseConnection()
        {
            if (context == null)
            {
                return;
            }

            context.Dispose();
            context = null;
        }

        #endregion

        #region Load Tables
        
        public static void LoadLessons()
        {
            AppData.Lessons = context.GetLessons();
        }

        public static void LoadLessons(Lesson.EType type)
        {
            AppData.Lessons = context.GetLessons(type);
        }

        public static void LoadWords(int lessonID)
        {
            AppData.SelectedLesson = context.GetLesson(lessonID);
            AppData.Words = context.GetWords(AppData.SelectedLesson);
        }

        public static void LoadWords(Lesson lesson)
        {
            AppData.Words = context.GetWords(lesson);
            AppData.SelectedLesson = lesson;
        }

        public static void LoadWords(Lesson[] lessons)
        {
            AppData.Words = context.GetWords(lessons);
        }

        public static void LoadWords(Lesson[] lessons, Word.EType[] types)
        {
            AppData.Words = context.GetWords(lessons, types);
        }
        
        public static void LoadSentences()
        {
            AppData.Sentences = context.GetSentences();
        }

        public static void LoadSentences(Lesson lesson)
        {
            AppData.Sentences = context.GetSentences(lesson);
            AppData.SelectedLesson = lesson;
        }

        public static void LoadSentences(Lesson[] lessons)
        {
            AppData.Sentences = context.GetSentences(lessons);
        }

        public static void LoadKanjis()
        {
            AppData.Kanjis = context.GetKanjis();
        }
        
        public static void LoadKanjis(Lesson lesson)
        {
            AppData.Kanjis = context.GetKanjis(lesson);
            AppData.SelectedLesson = lesson;
        }
        
        public static void LoadKanjis(Lesson[] lessons)
        {
            AppData.Kanjis = context.GetKanjis(lessons);
        }
        
        /// <summary>
        /// searches for words that contains the given substring
        /// the string can be part of the kana, kanji and translation members of Word
        /// </summary>
        public static void FindWords(String substring)
        {
            Word[] words = context.GetWords(substring);

            if (words.Length > 0)
            {
                AppData.Words = words;
            }
        }

        #endregion

        #region Update Tables

        public static void UpdateProgress()
        {
            Debug.WriteLine("Update Database!");

            context.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
            
            foreach(ObjectChangeConflict occ in context.ChangeConflicts)
            {
                Debug.WriteLine(occ.ToString());
            }
        }

        /// <summary>
        /// Resets correct and wrong values from the words
        /// </summary>
        /// <param name="resetAll">if true it resets all words from all lessons otherwise only the words in data.Words (that should be the current selected lesson in the statistics view)</param>
        public static void ResetWords(bool resetAll)
        {
            if (resetAll)
            {
                Word[] words = context.GetWords();

                foreach (Word w in words)
                {
                    ResetWord(w);
                }
            }
            else
            {
                //reset current loaded lesson
                foreach (Word w in AppData.Words)
                {
                    ResetWord(w);
                }
            }

            context.SubmitChanges();
        }

        private static void ResetWord(Word w)
        {
            w.correctTranslation = 0;
            w.wrongTranslation = 0;

            w.correctJapanese = 0;
            w.wrongJapanese = 0;
        }

        public static void ResetKanji(bool resetAll)
        {
            if (resetAll)
            {
                Kanji[] kanjis = context.GetKanjis();

                foreach (Kanji k in kanjis)
                {
                    ResetKanji(k);
                }
            }
            else
            {
                //reset current loaded lesson
                foreach (Kanji k in AppData.Kanjis)
                {
                    ResetKanji(k);
                }
            }

            context.SubmitChanges();
        }

        private static void ResetKanji(Kanji k)
        {
            //k.learnProgress = 0;
            //k.correctAnswered = 0;
            k.eFactor = 2.5f;
            k.repetition = 0;
            k.nextInterval = 0;
        }

        /// <summary>
        /// Updates Database by the given .txt file.
        /// the Database will only be updated if the versionnumber in the txt file is higher than 
        /// the actually db version
        /// </summary>
        /// <param name="updateStream">stream with the given txt updatefile</param>
        /// <returns>result string with edited/added lessons/words</returns>
        public static String UpdateDatabase(IRandomAccessStream updateStream)
        {
            StringBuilder updateResults = new StringBuilder();

            using (StreamReader sr = new StreamReader(updateStream.AsStream()))
            {
                String line = sr.ReadLine();
                int dbVersion = Convert.ToInt32(line);

                if (dbVersion <= AppSettings.DatabaseVersion)
                {
                    updateResults.AppendLine("Update fehlgeschlagen!");
                    updateResults.AppendLine("Datenbankversion der Updatedatei ist zu niedrig!\n");
                    updateResults.AppendLine("Aktuelle Datenbankversion: " + AppSettings.DatabaseVersion);
                    updateResults.AppendLine("Update-Dateiversion: " + dbVersion);
                }
                else
                {
                    int count = 0;

                    //update lessons
                    try
                    {
                        line = sr.ReadLine();
                        count = Convert.ToInt32(line);

                        for (int i = 0; i < count; ++i)
                        {
                            line = sr.ReadLine();
                            String idStr = line.Substring(0, line.IndexOf('|'));
                            Lesson l = context.GetLesson(Convert.ToInt32(idStr));
                            l.Update(line);
                        }

                        updateResults.AppendLine("Geänderte Lektionen: " + count);
                    }
                    catch (Exception e)
                    {
                        updateResults.Clear();
                        updateResults.AppendLine("Update fehlgeschlagen beim Lektionen Updaten!");
                        updateResults.AppendLine("System: " + e.Message);

                        return updateResults.ToString();
                    }

                    //update words
                    try
                    {
                        line = sr.ReadLine();
                        count = Convert.ToInt32(line);

                        for (int i = 0; i < count; ++i)
                        {
                            line = sr.ReadLine();
                            String idStr = line.Substring(0, line.IndexOf('|'));
                            Word w = context.GetWord(Convert.ToInt32(idStr));

                            AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, w.TimeStampTransl);
                            AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, w.TimeStampJapanese);

                            w.Update(line);
                        }

                        updateResults.AppendLine("Geänderte Wörter: " + count);

                    }
                    catch (Exception e)
                    {
                        updateResults.Clear();
                        updateResults.AppendLine("Update fehlgeschlagen beim Wörter Updaten!");
                        updateResults.AppendLine("System: " + e.Message);

                        return updateResults.ToString();
                    }

                    //add new Lessons

                    line = sr.ReadLine();
                    count = Convert.ToInt32(line);

                    Lesson[] newLessons = new Lesson[count];

                    try
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            ++AppSettings.LastLessonID;

                            line = sr.ReadLine();

                            newLessons[i] = new Lesson(line);
                            newLessons[i].id = AppSettings.LastLessonID;
                        }

                        updateResults.AppendLine("Hinzugefügte Lektionen: " + count);
                    }
                    catch (Exception e)
                    {
                        updateResults.Clear();
                        updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Lektionen!");
                        updateResults.AppendLine("System: " + e.Message);

                        --AppSettings.LastLessonID;

                        return updateResults.ToString();
                    }

                    //add lesson items
                    try
                    {
                        int wordsCount = 0;
                        int sentenceCount = 0;
                        int kanjiCount = 0;

                        foreach (Lesson newLesson in newLessons)
                        {
                            switch ((Lesson.EType)newLesson.type)
                            {
                                case Lesson.EType.vocabulary:

                                    Word[] newWords = new Word[newLesson.size];

                                    for (int i = 0; i < newLesson.size; ++i)
                                    {
                                        line = sr.ReadLine();
                                        newWords[i] = new Word(line, newLesson.id);

                                        AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, newWords[i].TimeStampTransl);
                                        AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, newWords[i].TimeStampJapanese);
                                    }
                                    wordsCount += newLesson.size;

                                    context.words.InsertAllOnSubmit(newWords);

                                    break;

                                case Lesson.EType.conjugation:

                                    break;

                                case Lesson.EType.insert:

                                    Sentence[] newSentences = new Sentence[newLesson.size];

                                    for (int i = 0; i < newLesson.size; ++i)
                                    {
                                        line = sr.ReadLine();
                                        newSentences[i] = new Sentence(line, newLesson.id);
                                    }
                                    sentenceCount += newLesson.size;

                                    context.sentences.InsertAllOnSubmit(newSentences);

                                    break;

                                case Lesson.EType.kanji:

                                    Kanji[] newKanji = new Kanji[newLesson.size];

                                    for (int i = 0; i < newLesson.size; ++i)
                                    {
                                        line = sr.ReadLine();
                                        newKanji[i] = new Kanji(line, newLesson.id);
                                    }
                                    kanjiCount += newLesson.size;

                                    context.kanjis.InsertAllOnSubmit(newKanji);

                                    break;
                            }
                        }

                        updateResults.AppendLine("Hinzugefügte Wörter: " + wordsCount);
                        updateResults.AppendLine("Hinzugefügte Lückentexte: " + sentenceCount);
                        updateResults.AppendLine("Hinzugefügte Kanji: " + kanjiCount);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        updateResults.Clear();
                        updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Items!");
                        updateResults.AppendLine("System: " + e.Message);
                        updateResults.AppendLine("vermutlich weil Trennstrich '|' vergessen.");

                        return updateResults.ToString();
                    }
                    catch (FormatException e)
                    {
                        updateResults.Clear();
                        updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Items!");
                        updateResults.AppendLine("System: " + e.Message);
                        updateResults.AppendLine("vermutlich weil Zahlen Syntax nich stimmt.");

                        return updateResults.ToString();
                    }
                    catch (Exception e)
                    {
                        updateResults.Clear();
                        updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Items!");
                        updateResults.AppendLine("System: " + e.Message);

                        return updateResults.ToString();
                    }

                    context.lessons.InsertAllOnSubmit(newLessons);
                    context.SubmitChanges();
                    
                    AppSettings.DatabaseVersion = dbVersion;
                    AppSettings.SaveSettings();
                }
            }

            return updateResults.ToString();
        }

        public static String ExportDatabase(IRandomAccessStream exportStream)
        {
            String updateResults = "";

            using (StreamWriter sw = new StreamWriter(exportStream.AsStream()))
            {
                //Write Down Databaseversion First
                sw.WriteLine(AppSettings.DatabaseVersion);
                //Zero Lessons to Update
                sw.WriteLine("0");
                //Zero Words to Update
                sw.WriteLine("0");

                LoadLessons();

                //Write Length of all Lessons
                sw.WriteLine(AppData.Lessons.Length);

                //Write Down each Lesson
                foreach (Lesson lesson in AppData.Lessons)
                {
                    sw.WriteLine(lesson.ToExportString());
                }

                int exportedWords = 0;
                int exportedSentences = 0;
                int exportedKanjis = 0;

                //Write Down Each Word/Kanji/Sentence
                foreach (Lesson lesson in AppData.Lessons)
                {
                    switch ((Lesson.EType)lesson.type)
                    {
                        case Lesson.EType.vocabulary:

                            AppData.Words = context.GetWords(lesson);

                            foreach (Word word in AppData.Words)
                            {
                                sw.WriteLine(word.ToExportString());
                            }

                            exportedWords += AppData.Words.Length;

                            break;

                        case Lesson.EType.insert:
                            
                            AppData.Sentences = context.GetSentences(lesson);

                            foreach (Sentence s in AppData.Sentences)
                            {
                                sw.WriteLine(s.ToExportString());
                            }

                            exportedSentences += AppData.Sentences.Length;

                            break;

                        case Lesson.EType.conjugation:
                            
                            break;

                        case Lesson.EType.kanji:

                            AppData.Kanjis = context.GetKanjis(lesson);
                            exportedKanjis += AppData.Kanjis.Length;

                            foreach (Kanji kanji in AppData.Kanjis)
                            {
                                sw.WriteLine(kanji.ToExportString());
                            }

                            break;
                    }

                }

                updateResults += "Export Erfolgreich!";
                updateResults += "\n" + AppData.Lessons.Length + " Lektionen,";
                updateResults += "\n" + exportedWords     + " Wörter,";
                updateResults += "\n" + exportedSentences + " Lückentexte und";
                updateResults += "\n" + exportedKanjis    + " Kanji exportiert.";
            }

            return updateResults;
        }

        #endregion

        #region Other

        public static void SortWords(bool accending, bool sortByTranslation)
        {
            Comparer comparer = new Comparer();

            if(accending)
            {
                if(sortByTranslation)
                {
                    comparer.compFunction = comparer.CompareTranslationAccending;
                }
                else
                {
                    comparer.compFunction = comparer.CompareJapaneseAccending;
                }
            }
            else
            {
                if (sortByTranslation)
                {
                    comparer.compFunction = comparer.CompareTranslationDeccending;
                }
                else
                {
                    comparer.compFunction = comparer.CompareJapaneseDeccending;
                }
            }

            Array.Sort(AppData.Words, comparer);
        }

        #region Class Comparer

        private class Comparer : IComparer
        {
            public delegate int Comp(Word x, Word y);

            public Comp compFunction;

            public int CompareTranslationAccending(Word x, Word y)
            {
                return (int)((y.CorrectWrongRelationTranslation * 100) - (x.CorrectWrongRelationTranslation * 100));
            }

            public int CompareJapaneseAccending(Word x, Word y)
            {
                return (int)((y.CorrectWrongRelationJapanese * 100) - (x.CorrectWrongRelationJapanese * 100));
            }

            public int CompareTranslationDeccending(Word x, Word y)
            {
                return (int)((x.CorrectWrongRelationTranslation * 100) - (y.CorrectWrongRelationTranslation * 100));
            }

            public int CompareJapaneseDeccending(Word x, Word y)
            {
                return (int)((x.CorrectWrongRelationJapanese * 100) - (y.CorrectWrongRelationJapanese * 100));
            }

            public int Compare(object x, object y)
            {
                return compFunction(x as Word, y as Word);
            }
        }
        
        #endregion

        #endregion

        #endregion
    }

}
