using JapanischTrainer.Settings;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JapanischTrainer.Database
{
    public class DBContext : DataContext
    {
        #region Fields

        public const String installFolderDBConnectionString = "Data Source = 'appdata:/Resources/trainerDatabase.sdf'; File Mode = read only;";
        public const String isoStorageDBConnectionString = "Data Source=isostore:/trainerDatabase.sdf";
        public const String updateDBConnectionString = "Data Source = 'appdata:/Resources/updateDatabase.sdf'; File Mode = read only;";

        public Table<Lesson> lessons;
        public Table<Word> words;
        public Table<Sentence> sentences;
        public Table<Kanji> kanjis;

        #endregion

        #region Constructor

        public DBContext(String connectionString)
            : base(connectionString)
        {

        }

        #endregion

        #region Public Mehtods
        
        #region Get Lessons

        public Lesson GetLesson(int id)
        {
            return (from c in lessons where c.id == id select c).First();
        }

        public Lesson[] GetLessons()
        {
            return (from c in lessons orderby c.type select c).ToArray();
        }

        public Lesson[] GetLessons(Lesson.EType setType)
        {
            return (from c in lessons where c.type == (int)setType select c).ToArray();
        }

        #endregion

        #region Get Words

        public Word GetWord(int id)
        {
            return (from c in words where c.id == id select c).First();
        }

        public Word[] GetWords()
        {
            return (from c in words select c).ToArray();
        }

        public Word[] GetWords(Lesson lesson)
        {
            return (from c in words where c.lessonID == lesson.id select c).ToArray();
        }

        public Word[] GetWords(Lesson[] lessons)
        {
            int[] ids = ExtractIDs(lessons);

            return (from c in words where ids.Contains(c.lessonID) select c).ToArray();
        }

        public Word[] GetWords(Lesson[] lessons, Word.EType[] types)
        {
            int[] ids = ExtractIDs(lessons);
            
            return (from c in words where ids.Contains(c.lessonID) && types.Contains((Word.EType)c.type) select c).ToArray();
        }

        public Word[] GetWords(String substring)
        {
            return (from c in words where c.kana.Contains(substring) || c.kanji.Contains(substring) || c.translation.Contains(substring) orderby c.kanji select c).ToArray();
        }

        //public Word[] GetWordsForEdit(int[] ids)
        //{
        //    return (from c in words where ids.Contains(c.id) select c).ToArray();
        //}
        
        #endregion

        #region Get Sentences

        public Sentence GetSentence(int id)
        {
            return (from c in sentences where c.id == id select c).Single();
        }

        public Sentence[] GetSentences()
        {
            return (from c in sentences select c).ToArray();
        }

        public Sentence[] GetSentences(Lesson lesson)
        {
            return (from c in sentences where c.lessonID == lesson.id select c).ToArray();
        }

        public Sentence[] GetSentences(Lesson[] lessons)
        {
            int[] ids = ExtractIDs(lessons);

            return (from c in sentences where ids.Contains(c.lessonID) select c).ToArray();
        }

        #endregion

        #region Get Kanjis

        public Kanji GetKanji(int id)
        {
            return (from c in kanjis where c.id == id select c).Single();
        }

        public Kanji[] GetKanjis()
        {
            return (from c in kanjis select c).ToArray();
        }

        public Kanji[] GetKanjis(Lesson lesson)
        {
            return (from c in kanjis where c.lessonID == lesson.id select c).ToArray();
        }

        public Kanji[] GetKanjis(Lesson[] lessons)
        {
            int[] ids = ExtractIDs(lessons);

            return (from c in kanjis where ids.Contains(c.lessonID) select c).ToArray();
        }

        #endregion

        #region Util

        private int[] ExtractIDs(Lesson[] lessons)
        {
            int[] ids = new int[lessons.Length];

            for (int i = 0; i < ids.Length; ++i)
            {
                ids[i] = lessons[i].id;
            }

            return ids;
        }

        /// <summary>
        /// Copies database from install storage to isolated storage
        /// because the install storage is readonly
        /// </summary>
        public static void MoveReferenceDatabase()
        {
            IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            using (Stream inputStream = Application.GetResourceStream(new Uri("Resources/trainerDatabase.sdf", UriKind.Relative)).Stream)
            {
                using (IsolatedStorageFileStream outputStream = isolatedStorage.CreateFile("trainerDatabase.sdf"))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = -1;

                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
