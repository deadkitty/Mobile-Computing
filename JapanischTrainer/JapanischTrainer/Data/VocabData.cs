using JapanischTrainer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer.Data
{
    public static class VocabData
    {
        #region Fields

        private static int itemsCorrect = 0;
        private static int itemsLeft = 0;
        private static int itemsWrong = 0;

        private static int itemIndex = 0;

        private static Word[] words;
        private static Word[] wordsTemp;

        private static Lesson[] lessons;

        private static List<Word> wrongAnsweredWords = new List<Word>();

        #endregion

        #region Properties

        public static Word ActiveWord
        {
            get { return words[itemIndex]; }
        }

        public static Word[] Words
        {
            get { return words; }
            set { words = value; }
        }

        public static Word[] WordsTemp
        {
            get { return wordsTemp; }
            set { wordsTemp = value; }
        }

        public static Lesson[] Lessons
        {
            get { return lessons; }
            set { lessons = value; }
        }

        public static List<Word> WrongAnsweredWords
        {
            get { return wrongAnsweredWords; }
            set { wrongAnsweredWords = value; }
        }

        public static int ItemsLeft
        {
            get { return itemsLeft; }
            set { itemsLeft = value; }
        }

        public static int ItemsCorrect
        {
            get { return itemsCorrect; }
            set { itemsCorrect = value; }
        }

        public static int ItemsWrong
        {
            get { return itemsWrong; }
            set { itemsWrong = value; }
        }

        public static int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = value; }
        }

        #endregion
    }
}
