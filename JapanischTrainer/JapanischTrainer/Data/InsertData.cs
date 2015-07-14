using JapanischTrainer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer.Data
{
    public static class InsertData
    {
        #region Fields

        private static int itemIndex;

        private static Lesson[] lessons;

        private static Sentence[] sentences;

        private static String sentenceText;
        private static String sentenceAnswer;
        
        #endregion

        #region Properties

        public static int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = value; }
        }

        public static Lesson[] Lessons
        {
            get { return InsertData.lessons; }
            set { InsertData.lessons = value; }
        }

        public static Sentence[] Sentences
        {
            get { return sentences; }
            set { sentences = value; }
        }

        public static Sentence ActiveSentence
        {
            get { return sentences[itemIndex]; }
        }

        public static String SentenceText
        {
            get { return sentenceText; }
            set { sentenceText = value; }
        }

        public static String SentenceAnswer
        {
            get { return sentenceAnswer; }
            set { sentenceAnswer = value; }
        }
        
        #endregion
    }
}
