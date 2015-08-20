using NihongoSenpai.Settings;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NihongoSenpai.Database
{
    public static class DataManager
    {
        #region Fields

        private static String dbConnectionString = "NihongoSenpaiDatabase.db";

        private static SQLiteConnection database = null;
        
        #endregion

        #region Database Connect/Disconnect/Create
        
        public static void ConnectToDatabase()
        {
            if(database != null)
            {
                return;
            }

            database = new SQLiteConnection(dbConnectionString, true);
        }

        public static void CloseConnection()
        {
            if(database == null)
            {
                return;
            }

            database.Dispose();
            database = null;
        }

        public static void CreateDatabase()
        {
            ConnectToDatabase();

            database.CreateTable<Lesson>();
            database.CreateTable<Word>();
            database.CreateTable<Kanji>();
            database.CreateTable<Sentence>();
            database.CreateTable<Sign>();

            CloseConnection();
        }

        #endregion

        #region Insert
        
        public static void Insert<Table>(Table item)
        {
            database.Insert(item);
        }

        public static void Insert<Table>(IEnumerable<Table> items)
        {
            database.InsertAll(items);
        }

        #endregion
        
        #region Get Lessons

        public static void LoadLessons()
        {
            AppData.Lessons = database.Query<Lesson>("select * from Lessons").ToArray();
        }

        public static void LoadLessons(Lesson.EType type)
        {
            AppData.Lessons = database.Query<Lesson>("select * from Lessons where type = " + (int)type).ToArray();
        }

        public static void LoadLesson(int lessonID)
        {
            List<Lesson> lessons = database.Query<Lesson>("select * from Lessons where id = " + lessonID);

            if(lessons.Count > 0)
            {
                AppData.SelectedLesson = lessons[0];
            }
        }

        private static Lesson GetLesson(int lessonID)
        {
            return database.Table<Lesson>().Where(x => x.id == lessonID).FirstOrDefault();
        }

        #endregion

        #region Get Words
   
        public static void LoadWords(int lessonID)
        {
            LoadLesson(lessonID);
            
            AppData.Words = database.Query<Word>("select * from Words where lessonID = " + lessonID).ToArray();
        }

        public static void LoadWords(Lesson lesson)
        {
            AppData.SelectedLesson = lesson;

            AppData.Words = database.Query<Word>("select * from Words where lessonID = " + lesson.id).ToArray();
        }

        public static void LoadWords(Lesson[] lessons)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("select * from Words where lessonID in (");

            for(int i = 0; i < lessons.Length - 1; ++i)
            {
                sb.Append(lessons[i].id);
                sb.Append(", ");
            }

            sb.Append(lessons[lessons.Length - 1].id);
            sb.Append(")");
            
            AppData.Words = database.Query<Word>(sb.ToString()).ToArray();

            //AppData.Words = database.Query<Word>("select * from Words where lessonID = ?", ExtractIDs(lessons)).ToArray();
        }

        public static void LoadWords(Lesson[] lessons, Word.EType[] types)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("select * from Words where lessonID in (");

            for(int i = 0; i < lessons.Length - 1; ++i)
            {
                sb.Append(lessons[i].id);
                sb.Append(", ");
            }

            sb.Append(lessons[lessons.Length - 1].id);
            
            sb.Append(") and type in (");
                        
            for(int i = 0; i < types.Length - 1; ++i)
            {
                sb.Append(types[i]);
                sb.Append(", ");
            }

            sb.Append(types[types.Length - 1]);
            
            sb.Append(")");
            
            AppData.Words = database.Query<Word>(sb.ToString()).ToArray();
        }
        
        private static Word GetWord(int id)
        {
            return database.Table<Word>().Where(x => x.id == id).FirstOrDefault();
        }

        /// <summary>
        /// searches for words that contains the given substring
        /// the string can be part of the kana, kanji and translation members of Word
        /// </summary>
        public static void FindWords(String substring)
        {
            TableQuery<Word> qWords = database.Table<Word>().Where(x => x.kana.Contains       (substring) || 
                                                                        x.kanji.Contains      (substring) || 
                                                                        x.translation.Contains(substring));
            
            if (qWords.Count() > 0)
            {
                List<Word> words = new List<Word>();
                words.AddRange(qWords);
                AppData.Words = words.ToArray();
            }
        }

        #endregion

        #region Get Kanji
        
        public static void LoadKanjis()
        {
            AppData.Kanjis = database.Query<Kanji>("select * from Kanjis").ToArray();
        }

        public static void LoadKanjis(Lesson lesson)
        {
            AppData.SelectedLesson = lesson;
            AppData.Kanjis = database.Query<Kanji>("select * from Kanjis where lessonID = " + lesson.id).ToArray();
        }

        public static void LoadKanjis(Lesson[] lessons)
        {
            AppData.Kanjis = database.Query<Kanji>("select * from Kanjis where lessonID = ?", ExtractIDs(lessons)).ToArray();
        }

        private static Kanji GetKanji(int id)
        {
            return database.Table<Kanji>().Where(x => x.id == id).FirstOrDefault();
        }

        #endregion

        #region Get Sentences
        
        public static void LoadSentences()
        {
            AppData.Sentences = database.Query<Sentence>("select * from Sentences").ToArray();
        }

        public static void LoadSentences(Lesson lesson)
        {
            AppData.SelectedLesson = lesson;
            AppData.Sentences = database.Query<Sentence>("select * from Sentences where lessonID = " + lesson.id).ToArray();
        }

        public static void LoadSentences(Lesson[] lessons)
        {
            AppData.Sentences = database.Query<Sentence>("select * from Sentences where lessonID = ?", ExtractIDs(lessons)).ToArray();
        }

        #endregion

        #region Get Signs
        
        public static void LoadSigns()
        {
            AppData.Signs = database.Query<Sign>("select * from Signs").ToArray();
        }

        #endregion

        #region Import/Export Database
        
        #region Import

        public static String ImportFromFile(IRandomAccessStream importStream)
        {
            StringBuilder importResults = new StringBuilder();

            using (AppStreamReader sr = new AppStreamReader(importStream.AsStream()))
            {
                String savepoint = database.SaveTransactionPoint();

                try
                {
                    String line = sr.ReadLine();

                    do
                    {
                        String[] parts = line.Split('|');
                        int operation = Convert.ToInt32(parts[0]);
                        int itemCount = Convert.ToInt32(parts[1]);

                        String results = "";

                        switch (operation)
                        {
                            case 0: results = AddLessons   (sr, itemCount); break;
                            case 1: results = UpdateLessons(sr, itemCount); break;
                            case 2: results = UpdateWords  (sr, itemCount); break;
                            case 3: results = UpdateKanjis (sr, itemCount); break;
                            case 4: results = AddWords     (sr, itemCount); break;
                            case 5: results = AddKanjis    (sr, itemCount); break;
                        }

                        importResults.Append(results);

                        line = sr.ReadLine();
                    }
                    while (line != null);

                    database.Commit();
                }
                catch (Exception e)
                {
                    database.RollbackTo(savepoint);

                    importResults.Clear();
                    importResults.AppendLine("Import Fehlgeschlagen in Zeile: " + sr.CurrentLine);
                    importResults.AppendLine("System: " + e.Message);
                }
            }

            return importResults.ToString();
        }

        private static String AddLessons(AppStreamReader sr, int itemCount)
        {
            int lessonID = GetLastLessonID();

            int importedLessons   = 0;
            int importedWords     = 0;
            int importedSentences = 0;
            int importedKanjis    = 0;

            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();

                Lesson lesson = new Lesson(line);
                lesson.id = ++lessonID;

                switch (lesson.type)
                {
                    case 0: importedWords     += ReadVocabLesson (sr, lesson); break;
                    case 1: importedSentences += ReadInsertLesson(sr, lesson); break;
                    case 2: break;
                    case 3: importedKanjis    += ReadKanjiLesson (sr, lesson); break;
                    case 4: break;
                }
                Insert<Lesson>(lesson);
                
                ++importedLessons;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Hinzugefügte Lektionen\t: "   + importedLessons);
            sb.AppendLine("Hinzugefügte Wörter\t: "      + importedWords);
            sb.AppendLine("Hinzugefügte Lückentexte\t: " + importedSentences);
            sb.AppendLine("Hinzugefügte Kanjis\t: "      + importedKanjis);

            return sb.ToString();
        }

        private static int ReadVocabLesson(AppStreamReader sr, Lesson lesson)
        {
            Word[] words = new Word[lesson.size];

            for (int i = 0; i < words.Length; ++i)
            {
                String line = sr.ReadLine();

                words[i] = new Word(line, lesson.id);

                AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, words[i].TimeStampTransl);
                AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, words[i].TimeStampJapanese);
            }

            Insert<Word>(words);

            return words.Length;
        }

        private static int ReadInsertLesson(AppStreamReader sr, Lesson lesson)
        {
            Sentence[] sentences = new Sentence[lesson.size];

            for (int i = 0; i < sentences.Length; ++i)
            {
                String line = sr.ReadLine();

                sentences[i] = new Sentence(line, lesson.id);
            }

            Insert<Sentence>(sentences);

            return sentences.Length;
        }

        private static int ReadKanjiLesson(AppStreamReader sr, Lesson lesson)
        {
            Kanji[] kanjis = new Kanji[lesson.size];

            for (int i = 0; i < kanjis.Length; ++i)
            {
                String line = sr.ReadLine();

                kanjis[i] = new Kanji(line, lesson.id);
            }

            Insert<Kanji>(kanjis);

            return kanjis.Length;
        }

        private static String UpdateLessons(AppStreamReader sr, int itemCount)
        {
            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();
                
                Lesson newLesson = new Lesson(line);                
                Lesson oldLesson = GetLesson(newLesson.id);

                oldLesson.Update(newLesson);
                database.Update(oldLesson);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Aktualisierte Lektionen\t: " + itemCount);
            return sb.ToString();
        }

        private static String UpdateWords(AppStreamReader sr, int itemCount)
        {
            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();
                String idStr = line.Substring(0, line.IndexOf('|'));

                Word word = GetWord(Convert.ToInt32(idStr));

                word.Update(line);
                database.Update(word);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Aktualisierte Wörter\t: " + itemCount);
            return sb.ToString();
        }

        private static String UpdateKanjis(AppStreamReader sr, int itemCount)
        {
            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();

                String idStr = line.Substring(0, line.IndexOf('|'));
                Kanji kanji = GetKanji(Convert.ToInt32(idStr));

                kanji.Update(line);
                database.Update(kanji);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Aktualisierte Kanjis\t: " + itemCount);
            return sb.ToString();
        }

        private static String AddWords(AppStreamReader sr, int itemCount)
        {
            Dictionary<int, int> lessonsDict = new Dictionary<int, int>();

            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();
                String[] parts = line.Split('|');

                int lessonID = Convert.ToInt32(parts[0]);

                if (lessonsDict.ContainsKey(lessonID))
                {
                    lessonsDict[lessonID] += 1;
                }
                else
                {
                    lessonsDict.Add(lessonID, 1);
                }

                Word word = new Word(line, lessonID);
                Insert<Word>(word);
            }

            foreach (KeyValuePair<int, int> pair in lessonsDict)
            {
                Lesson lesson = GetLesson(pair.Key);
                lesson.size += pair.Value;

                database.Update(lesson);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Wörter zu bestehenden Lektionen hinzugefügt: " + itemCount);
            return sb.ToString();
        }

        private static String AddKanjis(AppStreamReader sr, int itemCount)
        {
            Dictionary<int, int> lessonsDict = new Dictionary<int, int>();

            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();
                String[] parts = line.Split('|');

                int lessonID = Convert.ToInt32(parts[1]);

                if (lessonsDict.ContainsKey(lessonID))
                {
                    lessonsDict[lessonID] += 1;
                }
                else
                {
                    lessonsDict.Add(lessonID, 1);
                }

                Kanji kanji = new Kanji(line, lessonID);
                Insert<Kanji>(kanji);
            }

            foreach (KeyValuePair<int, int> pair in lessonsDict)
            {
                Lesson lesson = GetLesson(pair.Key);
                lesson.size += pair.Value;

                database.Update(lesson);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Kanjis zu bestehenden Lektionen hinzugefügt: " + itemCount);
            return sb.ToString();
        }
        
        #endregion

        #region Export

        public static String ExportToFile(IRandomAccessStream exportStream)
        {
            StringBuilder exportResults = new StringBuilder();

            using (AppStreamWriter sw = new AppStreamWriter(exportStream.AsStream()))
            {
                LoadLessons();

                //write Number of Lessons
                sw.WriteLine(AppData.Lessons.Length.ToString());

                int exportedLessons   = 0;
                int exportedWords     = 0;
                int exportedSentences = 0;
                int exportedKanjis    = 0;

                foreach (Lesson lesson in AppData.Lessons)
                {
                    sw.WriteLine(lesson.ToExportString());

                    switch (lesson.type)
                    {
                        case 0: exportedWords     += WriteVocabLesson (sw, lesson); break;
                        case 1: exportedSentences += WriteInsertLesson(sw, lesson); break;
                        case 2: break;
                        case 3: exportedKanjis    += WriteKanjiLesson (sw, lesson); break;
                        case 4: break;
                    }

                    ++exportedLessons;
                }

                exportResults.AppendLine("Datenbank erfolgreich exportiert!");
                exportResults.AppendLine("Exportierte Lektionen\t: "   + exportedLessons);
                exportResults.AppendLine("Exportierte Wörter\t: "      + exportedWords);
                exportResults.AppendLine("Exportierte Lückentexte\t: " + exportedSentences);
                exportResults.AppendLine("Exportierte Kanjis\t\t: "    + exportedKanjis);
            }

            AppData.Lessons   = null;
            AppData.Words     = null;
            AppData.Sentences = null;
            AppData.Kanjis    = null;

            return exportResults.ToString();
        }

        private static int WriteVocabLesson(AppStreamWriter sw, Lesson lesson)
        {
            LoadWords(lesson);

            foreach (Word word in AppData.Words)
            {
                sw.WriteLine(word.ToExportString());
            }

            return AppData.Words.Length;
        }

        private static int WriteInsertLesson(AppStreamWriter sw, Lesson lesson)
        {
            LoadSentences(lesson);

            foreach (Sentence sentence in AppData.Sentences)
            {
                sw.WriteLine(sentence.ToExportString());
            }

            return AppData.Sentences.Length;
        }

        private static int WriteKanjiLesson(AppStreamWriter sw, Lesson lesson)
        {
            LoadKanjis(lesson);

            foreach (Kanji kanji in AppData.Kanjis)
            {
                sw.WriteLine(kanji.ToExportString());
            }

            return AppData.Kanjis.Length;
        }
        
        #endregion

        #endregion
        
        #region Util
        
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
        
		public static int GetLastLessonID()
		{
            List<int> ids = database.Query<int>("select MAX(id) from Lessons");

			return ids.Count > 0 ? ids[0] : 0;
		}

        private static int[] ExtractIDs(Lesson[] lessons)
        {
            int[] ids = new int[lessons.Length];

            for (int i = 0; i < ids.Length; ++i)
            {
                ids[i] = lessons[i].id;
            }

            return ids;
        }

        public static void UpdateProgress()
        {
            if(AppData.Words != null)
            {
                database.UpdateAll(AppData.Words);
            }
            if(AppData.Kanjis != null)
            {
                database.UpdateAll(AppData.Kanjis);
            }

            database.Commit();
        }

        #endregion
    }
    
    //    #region Reset Items

    //    public static void ResetWords(bool resetAll)
    //    {
    //        if (resetAll)
    //        {
    //            Word[] words = context.GetWords();

    //            foreach (Word w in words)
    //            {
    //                ResetWord(w);
    //            }
    //        }
    //        else
    //        {
    //            //reset current loaded lesson
    //            foreach (Word w in AppData.Words)
    //            {
    //                ResetWord(w);
    //            }
    //        }

    //        context.SubmitChanges();
    //    }

    //    private static void ResetWord(Word w)
    //    {
    //        w.correctTranslation = 0;
    //        w.wrongTranslation = 0;

    //        w.correctJapanese = 0;
    //        w.wrongJapanese = 0;
    //    }

    //    public static void ResetKanji(bool resetAll)
    //    {
    //        if (resetAll)
    //        {
    //            Kanji[] kanjis = context.GetKanjis();

    //            foreach (Kanji k in kanjis)
    //            {
    //                ResetKanji(k);
    //            }
    //        }
    //        else
    //        {
    //            //reset current loaded lesson
    //            foreach (Kanji k in AppData.Kanjis)
    //            {
    //                ResetKanji(k);
    //            }
    //        }

    //        context.SubmitChanges();
    //    }

    //    private static void ResetKanji(Kanji k)
    //    {
    //        //k.learnProgress = 0;
    //        //k.correctAnswered = 0;
    //        k.eFactor = 2.5f;
    //        k.repetition = 0;
    //        k.nextInterval = 0;
    //    }
}
