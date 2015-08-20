//using System;
//using System.Data.Linq;
//using System.IO;
//using System.Linq;
//using Windows.UI.Xaml;

//namespace NihongoSenpai.Database
//{
//    public class AppDatabase : DataContext
//    {
//        #region Fields

//        public const String installFolderDBConnectionString = "Data Source = 'appdata:/Resources/trainerDatabase.sdf'; File Mode = read only;";
//        public const String isoStorageDBConnectionString = "isostore:/NihongoSenpaiDatabase.sdf";
//        public const String updateDBConnectionString = "Data Source = 'appdata:/Resources/updateDatabase.sdf'; File Mode = read only;";

//        public Table<Lesson> lessons;
//        public Table<Word> words;
//        public Table<Sentence> sentences;
//        public Table<Kanji> kanjis;

//        #endregion

//        #region Constructor

//        public AppDatabase(String connectionString)
//            : base(connectionString)
//        {

//        }

//        #endregion

//        #region Public Mehtods
        
//        #region Get Lessons

//        public Lesson GetLesson(int id)
//        {
//            return (from c in lessons where c.id == id select c).First();
//        }

//        public Lesson[] GetLessons()
//        {
//            return (from c in lessons orderby c.type select c).ToArray();
//        }

//        public Lesson[] GetLessons(Lesson.EType setType)
//        {
//            return (from c in lessons where c.type == (int)setType select c).ToArray();
//        }
        
//        public Lesson[] GetLessonsUnsorted()
//        {
//            return (from c in lessons select c).ToArray();
//        }

//        #endregion

//        #region Get Words

//        public Word GetWord(int id)
//        {
//            return (from c in words where c.id == id select c).First();
//        }

//        public Word[] GetWords()
//        {
//            return (from c in words select c).ToArray();
//        }

//        public Word[] GetWords(Lesson lesson)
//        {
//            return (from c in words where c.lessonID == lesson.id select c).ToArray();
//        }

//        public Word[] GetWords(Lesson[] lessons)
//        {
//            int[] ids = ExtractIDs(lessons);

//            return (from c in words where ids.Contains(c.lessonID) select c).ToArray();
//        }

//        public Word[] GetWords(Lesson[] lessons, Word.EType[] types)
//        {
//            int[] ids = ExtractIDs(lessons);
            
//            return (from c in words where ids.Contains(c.lessonID) && types.Contains((Word.EType)c.type) select c).ToArray();
//        }

//        public Word[] GetWords(String substring)
//        {
//            return (from c in words where c.kana.Contains(substring) || c.kanji.Contains(substring) || c.translation.Contains(substring) orderby c.kanji select c).ToArray();
//        }

//        //public Word[] GetWordsForEdit(int[] ids)
//        //{
//        //    return (from c in words where ids.Contains(c.id) select c).ToArray();
//        //}
        
//        #endregion

//        #region Get Sentences

//        public Sentence GetSentence(int id)
//        {
//            return (from c in sentences where c.id == id select c).Single();
//        }

//        public Sentence[] GetSentences()
//        {
//            return (from c in sentences select c).ToArray();
//        }

//        public Sentence[] GetSentences(Lesson lesson)
//        {
//            return (from c in sentences where c.lessonID == lesson.id select c).ToArray();
//        }

//        public Sentence[] GetSentences(Lesson[] lessons)
//        {
//            int[] ids = ExtractIDs(lessons);

//            return (from c in sentences where ids.Contains(c.lessonID) select c).ToArray();
//        }

//        #endregion

//        #region Get Kanjis

//        public Kanji GetKanji(int id)
//        {
//            return (from c in kanjis where c.id == id select c).Single();
//        }

//        public Kanji[] GetKanjis()
//        {
//            return (from c in kanjis select c).ToArray();
//        }

//        public Kanji[] GetKanjis(Lesson lesson)
//        {
//            return (from c in kanjis where c.lessonID == lesson.id select c).ToArray();
//        }

//        public Kanji[] GetKanjis(Lesson[] lessons)
//        {
//            int[] ids = ExtractIDs(lessons);

//            return (from c in kanjis where ids.Contains(c.lessonID) select c).ToArray();
//        }

//        #endregion

//        #region Util
        
//        public int GetLastLessonID()
//        {
//            IQueryable<int> qLessonIDs = from c in lessons orderby c.id descending select c.id;

//            return qLessonIDs.Count() > 0 ? qLessonIDs.First() : 0;
//        }

//        private int[] ExtractIDs(Lesson[] lessons)
//        {
//            int[] ids = new int[lessons.Length];

//            for (int i = 0; i < ids.Length; ++i)
//            {
//                ids[i] = lessons[i].id;
//            }

//            return ids;
//        }

//        #endregion

//        #endregion
//    }
//}
