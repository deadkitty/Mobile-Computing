using NihongoSenpai.Data;
using NihongoSenpai.Database;
using NihongoSenpai.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihongoSenpai.Controller
{
    public static class VocabController
    {
        #region Public Methods

        #region Initialize

        public static void LoadLessons(Lesson[] lessons)
        {
            DataManager.LoadWords(lessons, Util.ExtractTypes(AppSettings.LoadOptions));

            LoadLessons();
        }

        private static void LoadLessons()
        {
            VocabData.         Words.Clear();
            VocabData.  CorrectWords.Clear();
            VocabData.IncorrectWords.Clear();
            
            VocabData.ItemsWrong = 0;
            
            switch (AppSettings.WordPracticeMethod)
            {
                case 0: FilterWords(); break;
                case 1: FilterGermanWords(); break;
                case 2: FilterJapaneseWords(); break;
            }
                        
            PrepareWords();

            switch (AppSettings.SortOrder)
            {
                case 0: Util.SortByRandom              (VocabData.Words);        //sort by random first because words with the same CorrectWrongRelation aren't mixed and it would always be the
                        Util.SortByCorrectWrongRelation(VocabData.Words); break; //same pattern then, ger word1, jap word1, ger word2, jap word2, ... so i mix them once at the beginning ;D
                case 1: Util.SortByTimeStamp           (VocabData.Words); break;
                case 2: /*          Allready Sorted By Lesson          */ break;
                case 3: Util.SortByRandom              (VocabData.Words); break;
            }
            
            GetNext();
        }

        private static void PrepareWords()
        {
            foreach(Word w in VocabData.Words)
            {
                //if i don't set these here in the next round, answerstate will still be set to "both"
                switch (AppSettings.WordPracticeMethod)
                {
                    case 0:
                        
                        //words that are just one time in the list were set by filter words to japanese or translation, so i just have to 
                        //look for those words that are still 2 times in the list, where the answerstate is still set to both
                        if(w.answerState == Word.EAnswerState.both)
                        {
                            w.showJWord = Convert.ToBoolean(Util.GetRandomNumber(2));
                            w.answerState = Word.EAnswerState.none;
                        }
                        
                        break;

                    case 1:

                        //i don't set the states back in filter german/japanese words because of the load all option and 
                        //because i use add range then, i can't set them there either, so i make this here
                        w.showJWord = false;
                        w.answerState = Word.EAnswerState.japanese;
                        
                        break;

                    case 2:

                        w.showJWord = true;
                        w.answerState = Word.EAnswerState.translation;
                        
                        break;
                }
            }
        }

        private static void FilterWords()
        {
            if(AppSettings.LoadAllWords)
            {
                VocabData.Words.AddRange(AppData.Words);
                VocabData.Words.AddRange(AppData.Words);
            }
            else
            {
                foreach(Word w in AppData.Words)
                {
                    if (w.CorrectWrongCountTranslation < AppSettings.MinimumWordCount || w.CorrectWrongRelationTranslation < AppSettings.CorrectWrongRelation)
                    {
                        VocabData.Words.Add(w);
                    }
                    else //if just the word is just added one time i have to make sure that the correct answerstate is already set
                    {
                        w.answerState = Word.EAnswerState.translation;
                    }

                    if (w.CorrectWrongCountJapanese < AppSettings.MinimumWordCount || w.CorrectWrongRelationJapanese < AppSettings.CorrectWrongRelation)
                    {
                        VocabData.Words.Add(w);
                    }
                    else
                    {
                        w.answerState = Word.EAnswerState.japanese;
                    }
                }
            }
        }

        private static void FilterGermanWords()
        {
            if (AppSettings.LoadAllWords)
            {
                VocabData.Words.AddRange(AppData.Words);
            }
            else
            {
                foreach (Word w in AppData.Words)
                {
                    if (w.CorrectWrongCountTranslation < AppSettings.MinimumWordCount || w.CorrectWrongRelationTranslation < AppSettings.CorrectWrongRelation)
                    {
                        VocabData.Words.Add(w);
                    }
                }
            }
        }

        private static void FilterJapaneseWords()
        {
            if (AppSettings.LoadAllWords)
            {
                VocabData.Words.AddRange(AppData.Words);
            }
            else
            {
                foreach (Word w in AppData.Words)
                {
                    if (w.CorrectWrongCountJapanese < AppSettings.MinimumWordCount || w.CorrectWrongRelationJapanese < AppSettings.CorrectWrongRelation)
                    {
                        VocabData.Words.Add(w);
                    }
                }
            }
        }

        public static void Deinitialize()
        {
            VocabData.Words.Clear();
            VocabData.CorrectWords.Clear();
            VocabData.IncorrectWords.Clear();

            AppData.Words = null;
        }
           
        #endregion

        #region Practice

        private static void StartNewRound()
        {
            if (VocabData.IncorrectWords.Count > 0)
            {
                VocabData.Words.AddRange(VocabData.IncorrectWords);
                VocabData.IncorrectWords.Clear();

                Util.SortByRandom(VocabData.Words);

                FillTextProperties();
            }
            else
            {
                LoadLessons();
            }
        }

        /// <summary>
        /// Takes the Wrong Words by now and inserts them at the begining of the Words List again
        /// </summary>
        public static void LearnWrongWords()
        {
            Util.SortByRandom(VocabData.IncorrectWords);

            foreach(Word w in VocabData.IncorrectWords)
            {
                if(w.answerState == Word.EAnswerState.none)
                {
                    w.showJWord = !w.showJWord;
                }
            }

            VocabData.Words.InsertRange(0, VocabData.IncorrectWords);

            VocabData.IncorrectWords.Clear();

            FillTextProperties();
        }
                
        public static void WordCorrect()
        {
            if (VocabData.ActiveWord.showJWord)
            {
                VocabData.ActiveWord.correctJapanese++;
                VocabData.ActiveWord.TimeStampJapanese = ++AppSettings.TimeStamp;
            }
            else
            {
                VocabData.ActiveWord.correctTranslation++;
                VocabData.ActiveWord.timeStampTransl = ++AppSettings.TimeStamp;
            }

            if(VocabData.ActiveWord.answerState == Word.EAnswerState.none)
            {
                if (VocabData.ActiveWord.showJWord)
                {
                    VocabData.ActiveWord.answerState = Word.EAnswerState.japanese;
                }
                else
                {
                    VocabData.ActiveWord.answerState = Word.EAnswerState.translation;
                }

                VocabData.ActiveWord.showJWord = !VocabData.ActiveWord.showJWord;
            }
            else
            {
                VocabData.ActiveWord.answerState = Word.EAnswerState.both;
            }
            
            VocabData.CorrectWords.Add(VocabData.ActiveWord);
            VocabData.Words.Remove(VocabData.ActiveWord);
        }

        public static void WordWrong()
        {
            if (VocabData.ActiveWord.showJWord)
            {
                VocabData.ActiveWord.wrongJapanese++;
                VocabData.ActiveWord.TimeStampJapanese = ++AppSettings.TimeStamp;
            }
            else
            {
                VocabData.ActiveWord.wrongTranslation++;
                VocabData.ActiveWord.timeStampTransl = ++AppSettings.TimeStamp;
            }

            if(VocabData.ActiveWord.answerState == Word.EAnswerState.none)
            {
                VocabData.ActiveWord.showJWord = !VocabData.ActiveWord.showJWord;
            }

            VocabData.ItemsWrong++;

            VocabData.IncorrectWords.Add(VocabData.ActiveWord);
            VocabData.Words.Remove(VocabData.ActiveWord);

            if(AppSettings.LearnWrongWords)
            {
                if(VocabData.IncorrectWords.Count == AppSettings.LearnWrongWordsCount)
                {
                    LearnWrongWords();
                }
            }
        }

        public static void GetNext()
        {
            if(VocabData.Words.Count == 0)
            {
                DataManager.UpdateProgress();

                StartNewRound();
            }
            else
            {
                FillTextProperties();
            }
        }
        
        private static void FillTextProperties()
        {
            if (VocabData.ActiveWord.showJWord)
            {
                if (VocabData.ActiveWord.kanji == null)
                {
                    VocabData.ShownText = VocabData.ActiveWord.kana;
                    VocabData.AnswerText = VocabData.ActiveWord.translation;
                }
                else
                {
                    VocabData.ShownText = VocabData.ActiveWord.kanji;
                    VocabData.AnswerText = VocabData.ActiveWord.kana + ",\n" + VocabData.ActiveWord.translation;
                }
            }
            else
            {
                VocabData.ShownText = VocabData.ActiveWord.translation;

                if (VocabData.ActiveWord.kanji == null)
                {
                    VocabData.AnswerText = VocabData.ActiveWord.kana;
                }
                else
                {
                    VocabData.AnswerText = VocabData.ActiveWord.kanji + ",\n" + VocabData.ActiveWord.kana;
                }
            }

            if (AppSettings.ShowDescription)
            {
                switch (VocabData.ActiveWord.showFlags)
                {
                    case 0:                                      VocabData.DescriptionText = ""                                        ; break;
                    case 1: if (!VocabData.ActiveWord.showJWord) VocabData.DescriptionText = VocabData.ActiveWord.ToDescriptionString(); break;
                    case 2: if ( VocabData.ActiveWord.showJWord) VocabData.DescriptionText = VocabData.ActiveWord.ToDescriptionString(); break;
                    case 3:                                      VocabData.DescriptionText = VocabData.ActiveWord.ToDescriptionString(); break;
                }
            }
        }

        #endregion

        #endregion
    }
}
