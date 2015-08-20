using NihongoSenpai.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihongoSenpai.Database
{
    public static class AppData
    {
        #region Fields

        private static Lesson[] lessons;
        private static Lesson selectedLesson;

        private static Word[] words;
        private static Sentence[] sentences;
        private static Kanji[] kanjis;
        private static Sign[] signs;

        private static int selectedKanjiIndex;

        #endregion

        #region Properties

        public static Lesson[] Lessons
        {
            get { return lessons; }
            set { lessons = value; }
        }

        public static Lesson SelectedLesson
        {
            get { return selectedLesson; }
            set { selectedLesson = value; }
        }

        public static Word[] Words
        {
            get { return words; }
            set { words = value; }
        }

        public static Sentence[] Sentences
        {
            get { return AppData.sentences; }
            set { AppData.sentences = value; }
        }
        
        public static Kanji[] Kanjis
        {
            get { return kanjis; }
            set { kanjis = value; }
        }
        
        public static Sign[] Signs
        {
            get { return signs; }
            set { signs = value; }
        }

        public static int SelectedKanjiIndex
        {
            get { return selectedKanjiIndex; }
            set { selectedKanjiIndex = value; }
        }

        #endregion
    }
}
