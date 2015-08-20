using NihongoSenpai.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihongoSenpai.Data
{
    public static class CombineWordsData
    {
        #region Fields

        private static Word[] words;
        private static int itemIndex;

        private static Lesson[] lessons;

        private static char[] sourceSigns;
        private static char[] currentSigns;

        private static bool[] answers;
        
        #endregion

        #region Properties

        public static Word[] Words
        {
            get { return words; }
            set { words = value; }
        }

        public static Word ActiveWord
        {
            get { return words[itemIndex]; }
        }

        public static int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = value; }
        }
        
        public static Lesson[] Lessons
        {
            get { return lessons; }
            set { lessons = value; }
        }

        public static char[] SourceSigns
        {
            get { return sourceSigns; }
            set { sourceSigns = value; }
        }

        public static char[] CurrentSigns
        {
            get { return currentSigns; }
            set { currentSigns = value; }
        }

        public static bool[] Answers
        {
            get { return answers; }
            set { answers = value; }
        }

        #endregion
    }
}
