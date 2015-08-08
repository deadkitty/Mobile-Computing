using JapanischTrainer.Settings;
using Microsoft.Phone.Maps.Controls;
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

        public static void LoadLessonsUnsorted()
        {
            AppData.Lessons = context.GetLessonsUnsorted();
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
            context.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
            
            foreach(ObjectChangeConflict occ in context.ChangeConflicts)
            {
                Debug.WriteLine(occ.ToString());
            }
        }

        #region Reset Items

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
        
        #endregion

        /// <summary>
        /// Updates Database by the given .txt file.
        /// the Database will only be updated if the versionnumber in the txt file is higher than 
        /// the actually db version
        /// </summary>
        /// <param name="updateStream">stream with the given txt updatefile</param>
        /// <returns>result string with edited/added lessons/words</returns>
		//public static String UpdateDatabase(IRandomAccessStream updateStream)
		//{
		//	StringBuilder updateResults = new StringBuilder();

		//	using (AppStreamReader sr = new AppStreamReader(updateStream.AsStream()))
		//	{
		//		String line = sr.ReadLine();
		//		int dbVersion = Convert.ToInt32(line);

		//		if (dbVersion <= AppSettings.DatabaseVersion)
		//		{
		//			updateResults.AppendLine("Update fehlgeschlagen!");
		//			updateResults.AppendLine("Datenbankversion der Updatedatei ist zu niedrig!\n");
		//			updateResults.AppendLine("Aktuelle Datenbankversion: " + AppSettings.DatabaseVersion);
		//			updateResults.AppendLine("Update-Dateiversion: " + dbVersion);
		//		}
		//		else
		//		{
		//			int count = 0;

		//			//update lessons
		//			try
		//			{
		//				line = sr.ReadLine();
		//				count = Convert.ToInt32(line);

		//				for (int i = 0; i < count; ++i)
		//				{
		//					line = sr.ReadLine();
		//					String idStr = line.Substring(0, line.IndexOf('|'));
		//					Lesson l = context.GetLesson(Convert.ToInt32(idStr));
		//					l.Update(line);
		//				}

		//				updateResults.AppendLine("Geänderte Lektionen: " + count);
		//			}
		//			catch (Exception e)
		//			{
		//				updateResults.Clear();
		//				updateResults.AppendLine("Update fehlgeschlagen beim Lektionen Updaten!");
		//				updateResults.AppendLine("System: " + e.Message);

		//				return updateResults.ToString();
		//			}

		//			//update words
		//			try
		//			{
		//				line = sr.ReadLine();
		//				count = Convert.ToInt32(line);

		//				for (int i = 0; i < count; ++i)
		//				{
		//					line = sr.ReadLine();
		//					String idStr = line.Substring(0, line.IndexOf('|'));
		//					Word w = context.GetWord(Convert.ToInt32(idStr));

		//					AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, w.TimeStampTransl);
		//					AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, w.TimeStampJapanese);

		//					w.Update(line);
		//				}

		//				updateResults.AppendLine("Geänderte Wörter: " + count);

		//			}
		//			catch (Exception e)
		//			{
		//				updateResults.Clear();
		//				updateResults.AppendLine("Update fehlgeschlagen beim Wörter Updaten!");
		//				updateResults.AppendLine("System: " + e.Message);

		//				return updateResults.ToString();
		//			}

		//			//add new Lessons

		//			line = sr.ReadLine();
		//			count = Convert.ToInt32(line);

		//			Lesson[] newLessons = new Lesson[count];

		//			try
		//			{
		//				for (int i = 0; i < count; ++i)
		//				{
		//					++AppSettings.LastLessonID;

		//					line = sr.ReadLine();

		//					newLessons[i] = new Lesson(line);
		//					newLessons[i].id = AppSettings.LastLessonID;
		//				}

		//				updateResults.AppendLine("Hinzugefügte Lektionen: " + count);
		//			}
		//			catch (Exception e)
		//			{
		//				updateResults.Clear();
		//				updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Lektionen!");
		//				updateResults.AppendLine("System: " + e.Message);

		//				--AppSettings.LastLessonID;

		//				return updateResults.ToString();
		//			}

		//			//add lesson items
		//			try
		//			{
		//				int wordsCount = 0;
		//				int sentenceCount = 0;
		//				int kanjiCount = 0;

		//				foreach (Lesson newLesson in newLessons)
		//				{
		//					switch ((Lesson.EType)newLesson.type)
		//					{
		//						case Lesson.EType.vocabulary:

		//							Word[] newWords = new Word[newLesson.size];

		//							for (int i = 0; i < newLesson.size; ++i)
		//							{
		//								line = sr.ReadLine();
		//								newWords[i] = new Word(line, newLesson.id);

		//								AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, newWords[i].TimeStampTransl);
		//								AppSettings.TimeStamp = Math.Max(AppSettings.TimeStamp, newWords[i].TimeStampJapanese);
		//							}
		//							wordsCount += newLesson.size;

		//							context.words.InsertAllOnSubmit(newWords);

		//							break;

		//						case Lesson.EType.conjugation:

		//							break;

		//						case Lesson.EType.insert:

		//							Sentence[] newSentences = new Sentence[newLesson.size];

		//							for (int i = 0; i < newLesson.size; ++i)
		//							{
		//								line = sr.ReadLine();
		//								newSentences[i] = new Sentence(line, newLesson.id);
		//							}
		//							sentenceCount += newLesson.size;

		//							context.sentences.InsertAllOnSubmit(newSentences);

		//							break;

		//						case Lesson.EType.kanji:

		//							Kanji[] newKanji = new Kanji[newLesson.size];

		//							for (int i = 0; i < newLesson.size; ++i)
		//							{
		//								line = sr.ReadLine();
		//								newKanji[i] = new Kanji(line, newLesson.id);
		//							}
		//							kanjiCount += newLesson.size;

		//							context.kanjis.InsertAllOnSubmit(newKanji);

		//							break;
		//					}
		//				}

		//				updateResults.AppendLine("Hinzugefügte Wörter: " + wordsCount);
		//				updateResults.AppendLine("Hinzugefügte Lückentexte: " + sentenceCount);
		//				updateResults.AppendLine("Hinzugefügte Kanji: " + kanjiCount);
		//			}
		//			catch (IndexOutOfRangeException e)
		//			{
		//				updateResults.Clear();
		//				updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Items!");
		//				updateResults.AppendLine("System: " + e.Message);
		//				updateResults.AppendLine("vermutlich weil Trennstrich '|' vergessen.");

		//				return updateResults.ToString();
		//			}
		//			catch (FormatException e)
		//			{
		//				updateResults.Clear();
		//				updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Items!");
		//				updateResults.AppendLine("System: " + e.Message);
		//				updateResults.AppendLine("vermutlich weil Zahlen Syntax nich stimmt.");

		//				return updateResults.ToString();
		//			}
		//			catch (Exception e)
		//			{
		//				updateResults.Clear();
		//				updateResults.AppendLine("Update fehlgeschlagen beim einfügen neuer Items!");
		//				updateResults.AppendLine("System: " + e.Message);

		//				return updateResults.ToString();
		//			}

		//			context.lessons.InsertAllOnSubmit(newLessons);
		//			context.SubmitChanges();
                    
		//			//AppSettings.DatabaseVersion = dbVersion;
		//			AppSettings.SaveSettings();
		//		}
		//	}

		//	return updateResults.ToString();
		//}

		//public static String ExportDatabase(IRandomAccessStream exportStream)
		//{
		//	String updateResults = "";

		//	using (StreamWriter sw = new StreamWriter(exportStream.AsStream()))
		//	{
		//		//Write Down Databaseversion First
		//		sw.WriteLine(AppSettings.DatabaseVersion);
		//		//Zero Lessons to Update
		//		sw.WriteLine("0");
		//		//Zero Words to Update
		//		sw.WriteLine("0");

		//		LoadLessons();

		//		//Write Length of all Lessons
		//		sw.WriteLine(AppData.Lessons.Length);

		//		//Write Down each Lesson
		//		foreach (Lesson lesson in AppData.Lessons)
		//		{
		//			sw.WriteLine(lesson.ToExportString());
		//		}

		//		int exportedWords = 0;
		//		int exportedSentences = 0;
		//		int exportedKanjis = 0;

		//		//Write Down Each Word/Kanji/Sentence
		//		foreach (Lesson lesson in AppData.Lessons)
		//		{
		//			switch ((Lesson.EType)lesson.type)
		//			{
		//				case Lesson.EType.vocabulary:

		//					AppData.Words = context.GetWords(lesson);

		//					foreach (Word word in AppData.Words)
		//					{
		//						sw.WriteLine(word.ToExportString());
		//					}

		//					exportedWords += AppData.Words.Length;

		//					break;

		//				case Lesson.EType.insert:
                            
		//					AppData.Sentences = context.GetSentences(lesson);

		//					foreach (Sentence s in AppData.Sentences)
		//					{
		//						sw.WriteLine(s.ToExportString());
		//					}

		//					exportedSentences += AppData.Sentences.Length;

		//					break;

		//				case Lesson.EType.conjugation:
                            
		//					break;

		//				case Lesson.EType.kanji:

		//					AppData.Kanjis = context.GetKanjis(lesson);
		//					exportedKanjis += AppData.Kanjis.Length;

		//					foreach (Kanji kanji in AppData.Kanjis)
		//					{
		//						sw.WriteLine(kanji.ToExportString());
		//					}

		//					break;
		//			}

		//		}

		//		updateResults += "Export Erfolgreich!";
		//		updateResults += "\n" + AppData.Lessons.Length + " Lektionen,";
		//		updateResults += "\n" + exportedWords     + " Wörter,";
		//		updateResults += "\n" + exportedSentences + " Lückentexte und";
		//		updateResults += "\n" + exportedKanjis    + " Kanji exportiert.";
		//	}

		//	return updateResults;
		//}

        #region Import

        public static String ImportFromFile(IRandomAccessStream importStream)
        {
            StringBuilder importResults = new StringBuilder();

            using (AppStreamReader sr = new AppStreamReader(importStream.AsStream()))
            {
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
                            case 0: results = AddLessons(sr, itemCount); break;
                            case 1: results = UpdateLessons(sr, itemCount); break;
                            case 2: results = UpdateWords(sr, itemCount); break;
                            case 3: results = UpdateKanjis(sr, itemCount); break;
                            case 4: results = AddWords(sr, itemCount); break;
                            case 5: results = AddKanjis(sr, itemCount); break;
                        }

                        importResults.Append(results);

                        line = sr.ReadLine();
                    }
                    while (line != null);

                    context.SubmitChanges();
                }
                catch (Exception e)
                {
                    importResults.Clear();
                    importResults.AppendLine("Import Fehlgeschlagen in Zeile: " + sr.CurrentLine);
                    importResults.AppendLine("System: " + e.Message);
                }
            }

            return importResults.ToString();
        }

        private static String AddLessons(AppStreamReader sr, int itemCount)
        {
            int lessonID = context.GetLastLessonID();

            int importedLessons = 0;
            int importedWords = 0;
            int importedSentences = 0;
            int importedKanjis = 0;

            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();

                Lesson lesson = new Lesson(line);
                lesson.id = ++lessonID;

                switch (lesson.type)
                {
                    case 0: importedWords += ReadVocabLesson(sr, lesson); break;
                    case 1: importedSentences += ReadInsertLesson(sr, lesson); break;
                    case 2: break;
                    case 3: importedKanjis += ReadKanjiLesson(sr, lesson); break;
                    case 4: break;
                }
                context.lessons.InsertOnSubmit(lesson);

                ++importedLessons;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Hinzugefügte Lektionen\t: " + importedLessons);
            sb.AppendLine("Hinzugefügte Wörter\t: " + importedWords);
            sb.AppendLine("Hinzugefügte Lückentexte\t: " + importedSentences);
            sb.AppendLine("Hinzugefügte Kanjis\t: " + importedKanjis);

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

            context.words.InsertAllOnSubmit(words);

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

            context.sentences.InsertAllOnSubmit(sentences);

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

            context.kanjis.InsertAllOnSubmit(kanjis);

            return kanjis.Length;
        }

        private static String UpdateLessons(AppStreamReader sr, int itemCount)
        {
            for (int i = 0; i < itemCount; ++i)
            {
                String line = sr.ReadLine();

                Lesson newLesson = new Lesson(line);
                Lesson oldLesson = context.GetLesson(newLesson.id);

                oldLesson.Update(newLesson);
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
                Word w = context.GetWord(Convert.ToInt32(idStr));

                w.Update(line);
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
                Kanji k = context.GetKanji(Convert.ToInt32(idStr));

                k.Update(line);
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

                Word w = new Word(line, lessonID);
                context.words.InsertOnSubmit(w);
            }

            foreach (KeyValuePair<int, int> pair in lessonsDict)
            {
                Lesson l = context.GetLesson(pair.Key);
                l.size += pair.Value;
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

                Kanji k = new Kanji(line, lessonID);
                context.kanjis.InsertOnSubmit(k);
            }

            foreach (KeyValuePair<int, int> pair in lessonsDict)
            {
                Lesson l = context.GetLesson(pair.Key);
                l.size += pair.Value;
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
                LoadLessonsUnsorted();

                //write Number of Lessons
                sw.WriteLine(AppData.Lessons.Length.ToString());

                int exportedLessons = 0;
                int exportedWords = 0;
                int exportedSentences = 0;
                int exportedKanjis = 0;

                foreach (Lesson lesson in AppData.Lessons)
                {
                    sw.WriteLine(lesson.ToExportString());

                    switch (lesson.type)
                    {
                        case 0: exportedWords += WriteVocabLesson(sw, lesson); break;
                        case 1: exportedSentences += WriteInsertLesson(sw, lesson); break;
                        case 2: break;
                        case 3: exportedKanjis += WriteKanjiLesson(sw, lesson); break;
                        case 4: break;
                    }

                    ++exportedLessons;
                }

                exportResults.AppendLine("Datenbank erfolgreich exportiert!");
                exportResults.AppendLine("Exportierte Lektionen\t: " + exportedLessons);
                exportResults.AppendLine("Exportierte Wörter\t: " + exportedWords);
                exportResults.AppendLine("Exportierte Lückentexte\t: " + exportedSentences);
                exportResults.AppendLine("Exportierte Kanjis\t\t: " + exportedKanjis);
            }

            AppData.Lessons = null;
            AppData.Words = null;
            AppData.Sentences = null;
            AppData.Kanjis = null;

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
