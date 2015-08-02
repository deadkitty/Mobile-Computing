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

        private static String shownText;
        private static String answerText;
        private static String descriptionText;
        
        private static List<Word> activeWords    = new List<Word>();
        private static List<Word> correctWords   = new List<Word>();
        private static List<Word> incorrectWords = new List<Word>();

        private static int wrongAnswers;

        #endregion

        #region Properties
        
        public static Word ActiveWord
        {
            get { return activeWords[0]; }
        }


        public static List<Word> Words
        {
            get { return activeWords; }
            set { activeWords = value; }
        }

        public static List<Word> CorrectWords
        {
            get { return correctWords; }
            set { correctWords = value; }
        }

        public static List<Word> IncorrectWords
        {
            get { return incorrectWords; }
            set { incorrectWords = value; }
        }


        public static int ItemsLeft
        {
            get { return activeWords.Count + incorrectWords.Count; }
        }

        public static int ItemsCorrect
        {
            get { return correctWords.Count; }
        }

        public static int ItemsWrong
        {
            get { return wrongAnswers; }
            set { wrongAnswers = value; }
        }


        public static String ShownText
        {
            get { return shownText; }
            set { shownText = value; }
        }

        public static String AnswerText
        {
            get { return answerText; }
            set { answerText = value; }
        }

        public static String DescriptionText
        {
            get { return descriptionText; }
            set { descriptionText = value; }
        }

        #endregion
    }
}
