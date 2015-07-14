using JapanischTrainer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer.Data
{
    public static class FlashcardsData
    {
        #region Fields

        private static int itemsCorrect = 0;
        private static int itemsLeft = 0;
        private static int itemsWrong = 0;

        private static int itemIndex = 0;

        private static List<Kanji> kanjis = new List<Kanji>();

        private static Lesson[] lessons;


        #endregion

        #region Properties

        public static Kanji ActiveKanji
        {
            get { return kanjis[itemIndex]; }
        }

        public static List<Kanji> Kanjis
        {
            get { return kanjis; }
            set { kanjis = value; }
        }

        public static Lesson[] Lessons
        {
            get { return lessons; }
            set { lessons = value; }
        }
        
        public static int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = value; }
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

        #endregion
    }
}
