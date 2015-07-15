using JapanischTrainer.Data;
using JapanischTrainer.Database;
using JapanischTrainer.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer.Controller
{
    public static class VocabController
    {
        #region Fields

        private static int partLessonStartIndex = 0;

        private static int lastItemIndex = 0;
        private static int lastItemsCorrect = 0;
        private static int lastItemsWrong = 0;

        private static bool wrongAnswersRound = false;
        
        #endregion

        #region Public Methods

        #region Prepare Lessons

        public static void LoadLessons(Lesson[] lessons)
        {
            VocabData.Lessons = lessons;

            LoadLessons();

            if (AppSettings.PartLessons)
            {
                VocabData.WordsTemp = VocabData.Words;
                LoadNextPart();
            }
            else
            {
                VocabData.ItemsCorrect = 0;
                VocabData.ItemsWrong = 0;
                VocabData.ItemsLeft = VocabData.Words.Length;

                VocabData.ItemIndex = 0;
            }
        }

        private static void LoadLessons()
        {
            DataManager.LoadWords(VocabData.Lessons, Util.ExtractTypes(AppSettings.LoadOptions));

            switch (AppSettings.WordPracticeMethod)
            {
                case 0: FilterWords(); break;
                case 1: FilterGermanWords(); break;
                case 2: FilterJapaneseWords(); break;
            }

            //From now i only work on VocabData.Words, so i don't need AppData.Words any longer
            AppData.Words = null;

            switch (AppSettings.SortOrder)
            {
                case 0: Util.SortByRandom              (VocabData.Words);        //sort by random first because after all wrong words there would be only words left with no wrong answers and it would 
                        Util.SortByCorrectWrongRelation(VocabData.Words); break; //always be the same pattern then, ger word1, jap word1, ger word2, jap word2, ... so i mix them once at the beginning ;D
                case 1: Util.SortByTimeStamp           (VocabData.Words); break;
                case 2: /*Allready Sorted By Lesson*/                     break;
                case 3: Util.SortByRandom              (VocabData.Words); break;
            }
        }

        private static void FilterWords()
        {
            if (AppSettings.LoadAllWords)
            {
                VocabData.Words = new Word[AppData.Words.Length * 2];

                //i have to add each word manually, otherwise the original pointer and showJWord members are not set
                int j = 0;
                for (int i = 0; i < AppData.Words.Length; ++i)
                {
                    VocabData.Words[j++] = AppData.Words[i];
                    Word cpy = new Word(AppData.Words[i]);
                    cpy.showJWord = true;
                    VocabData.Words[j++] = cpy;
                }
            }
            else
            {
                List<Word> words = new List<Word>();

                foreach (Word w in AppData.Words)
                {
                    if (w.CorrectWrongCountTranslation < AppSettings.MinimumWordCount || w.CorrectWrongRelationTranslation < AppSettings.CorrectWrongRelation)
                    {
                        words.Add(w);
                    }

                    if (w.CorrectWrongCountJapanese < AppSettings.MinimumWordCount || w.CorrectWrongRelationJapanese < AppSettings.CorrectWrongRelation)
                    {
                        Word cpy = new Word(w);
                        cpy.showJWord = true;
                        words.Add(cpy);
                    }
                }

                VocabData.Words = words.ToArray();
            }
        }

        private static void FilterGermanWords()
        {
            if (AppSettings.LoadAllWords)
            {
                VocabData.Words = AppData.Words;
            }
            else
            {
                List<Word> words = new List<Word>();

                foreach (Word w in AppData.Words)
                {
                    if (w.CorrectWrongCountTranslation < AppSettings.MinimumWordCount || w.CorrectWrongRelationTranslation < AppSettings.CorrectWrongRelation)
                    {
                        words.Add(w);
                    }
                }

                VocabData.Words = words.ToArray();
            }
        }

        private static void FilterJapaneseWords()
        {
            if (AppSettings.LoadAllWords)
            {
                VocabData.Words = new Word[AppData.Words.Length];

                //i have to add each word manually, otherwise the original pointer and showJWord members are not set
                for (int i = 0; i < AppData.Words.Length; ++i)
                {
                    VocabData.Words[i] = AppData.Words[i];
                    VocabData.Words[i].showJWord = true;
                }
            }
            else
            {
                List<Word> words = new List<Word>();

                foreach (Word w in AppData.Words)
                {
                    if (w.CorrectWrongCountJapanese < AppSettings.MinimumWordCount || w.CorrectWrongRelationJapanese < AppSettings.CorrectWrongRelation)
                    {
                        w.showJWord = true;
                        words.Add(w);
                    }
                }

                VocabData.Words = words.ToArray();
            }
        }

        public static void Deinitialize()
        {
            VocabData.Words      = null;
            VocabData.WordsTemp  = null;
            VocabData.Lessons    = null;

            VocabData.WrongAnsweredWords.Clear();
        }
        
        #endregion

        #region Practice

        private static void StartNewRound()
        {
            if(VocabData.WrongAnsweredWords.Count > 0)
            {
                VocabData.WordsTemp = VocabData.Words;
                VocabData.Words = VocabData.WrongAnsweredWords.ToArray();
                VocabData.WrongAnsweredWords.Clear();
            }
            else
            {
                LoadLessons();
            }

            VocabData.ItemsCorrect = 0;
            VocabData.ItemsWrong = 0;
            VocabData.ItemsLeft = VocabData.Words.Length;

            VocabData.ItemIndex = 0;
        }

        /// <summary>
        /// saves the current practice state where the user
        /// pressed the learnWrongAnsweresButton and 
        /// starts a new Round with all wrong answered words so far
        /// </summary>
        public static void LearnWrongWords()
        {
            lastItemIndex = VocabData.ItemIndex;
            VocabData.ItemIndex = 0;

            lastItemsCorrect = VocabData.ItemsCorrect;
            lastItemsWrong = VocabData.ItemsWrong;

            VocabData.ItemsCorrect = 0;
            VocabData.ItemsWrong = 0;

            VocabData.WordsTemp = VocabData.Words;

            VocabData.Words = VocabData.WrongAnsweredWords.ToArray();

            VocabData.WrongAnsweredWords.Clear();

            VocabData.ItemsLeft = VocabData.Words.Length;

            wrongAnswersRound = true;
        }

        /// <summary>
        /// recoves the last practice state after the user
        /// has pressed the learnWrongAnsweresButton and ended all words in the 
        /// wrongAnsweredWords List
        /// </summary>
        private static void RecoverOldPracticeState()
        {
            wrongAnswersRound = false;

            VocabData.Words = VocabData.WordsTemp;
            VocabData.ItemIndex = lastItemIndex - 1;

            VocabData.ItemsCorrect += lastItemsCorrect;

            VocabData.ItemsLeft = VocabData.Words.Length - VocabData.ItemIndex;
        }

        /// <summary>
        /// loads the next couple words
        /// </summary>
        private static void LoadNextPart()
        {
            if (VocabData.WrongAnsweredWords.Count > 0)
            {
                VocabData.Words = VocabData.WrongAnsweredWords.ToArray();
                VocabData.WrongAnsweredWords.Clear();
            }
            else
            {
                if (partLessonStartIndex >= VocabData.WordsTemp.Length)
                {
                    partLessonStartIndex = 0;
                    LoadLessons();
                }

                VocabData.Words = new Word[Math.Min(AppSettings.PartLessonWordsCount, VocabData.WordsTemp.Length - partLessonStartIndex)];

                for (int i = 0; i < VocabData.Words.Length; ++i)
                {
                    VocabData.Words[i] = VocabData.WordsTemp[partLessonStartIndex + i];
                }

                partLessonStartIndex += VocabData.Words.Length;
            }
            
            VocabData.ItemsCorrect = 0;
            VocabData.ItemsWrong = 0;
            VocabData.ItemsLeft = VocabData.Words.Length;

            VocabData.ItemIndex = 0;
        }
        
        public static void WordCorrect()
        {
            VocabData.ItemsLeft--;
            VocabData.ItemsCorrect++;

            if (VocabData.ActiveWord.showJWord)
            {
                if(VocabData.ActiveWord.original != null)
                {
                    VocabData.ActiveWord.original.correctJapanese++;
                    VocabData.ActiveWord.original.TimeStampJapanese = ++AppSettings.TimeStamp;
                }
                else
                {
                    VocabData.ActiveWord.correctJapanese++;
                    VocabData.ActiveWord.TimeStampJapanese = ++AppSettings.TimeStamp;
                }
            }
            else
            {
                VocabData.ActiveWord.correctTranslation++;
                VocabData.ActiveWord.timeStampTransl = ++AppSettings.TimeStamp;
            }
        }

        public static void WordWrong()
        {
            VocabData.ItemsLeft--;
            VocabData.ItemsWrong++;

            if (VocabData.ActiveWord.showJWord)
            {
                if (VocabData.ActiveWord.original != null)
                {
                    VocabData.ActiveWord.original.wrongJapanese++;
                    VocabData.ActiveWord.original.TimeStampJapanese = ++AppSettings.TimeStamp;
                }
                else
                {
                    VocabData.ActiveWord.wrongJapanese++;
                    VocabData.ActiveWord.TimeStampJapanese = ++AppSettings.TimeStamp;
                }
            }
            else
            {
                VocabData.ActiveWord.wrongTranslation++;
                VocabData.ActiveWord.timeStampTransl = ++AppSettings.TimeStamp;
            }

            VocabData.WrongAnsweredWords.Add(VocabData.ActiveWord);
        }

        public static void GetNext()
        {
            if(VocabData.ItemsLeft == 0)
            {
                DataManager.UpdateProgress();
                
                if(AppSettings.PartLessons)
                {
                    LoadNextPart();
                }
                else
                {
                    if (wrongAnswersRound)
                    {
                        RecoverOldPracticeState();
                    }
                    else
                    {
                        StartNewRound();
                    }    
                }
            }
            else
            {
                ++VocabData.ItemIndex;
                
                if (VocabData.ItemIndex % 50 == 49)
                {
                    DataManager.UpdateProgress();
                }
            }
        }
        
        #endregion

        #endregion
    }
}
