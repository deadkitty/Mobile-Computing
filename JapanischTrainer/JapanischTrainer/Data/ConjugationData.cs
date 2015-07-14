using JapanischTrainer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer.Data
{
    public static class ConjugationData
    {
        #region ETargetForm

        public enum ETargetForm
        {
            ruForm,

            naiForm,
            naiPastForm,

            masuForm,
            masuNegativeForm,
            masuPastForm,
            masuPastNegativeForm,

            teForm,
            taForm,

            imperativeForm,
            prohibitiveForm,
            volitionalForm,
            conditionalForm,

            tai,
            sugi,
            yasui,
            nikui,

            potentialVerb,
            passiveVerb,
            causativeVerb,
        }

        #endregion

        #region Fields

        public const int maxActiveConjugationWordsCount = 5;
        
        private static Lesson[] lessons;
        
        private static Word[] words;
        private static Word[] activeWords;

        private static ETargetForm[] targetForms;
        private static String     [] targetWords;

        private static int itemsCorrect = 0;
        private static int itemsLeft = 0;
        private static int itemsWrong = 0;

        #endregion

        #region Properties

        public static Word[] Words
        {
            get { return words; }
            set { words = value; }
        }

        public static Word[] ActiveWords
        {
            get { return activeWords; }
            set { activeWords = value; }
        }

        public static Lesson[] Lessons
        {
            get { return lessons; }
            set { lessons = value; }
        }

        public static ETargetForm[] TargetForms
        {
            get { return targetForms; }
            set { targetForms = value; }
        }

        public static String[] TargetWords
        {
            get { return targetWords; }
            set { targetWords = value; }
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
