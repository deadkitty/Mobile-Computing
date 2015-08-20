using NihongoSenpai.Data;
using NihongoSenpai.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihongoSenpai.Controller
{
    public static class FlashcardsController
    {
        #region Fields

        private static int itemsLearned = 0;
        private const int itemsProInterval = 20;

        #endregion

        #region Initialize

        public static void LoadLessons(Lesson[] selectedLessons)
        {
            FlashcardsData.Lessons = selectedLessons;

            DataManager.LoadKanjis(selectedLessons);
            FlashcardsData.Kanjis.AddRange(AppData.Kanjis);

            AppData.Kanjis = null;
            
            GetNextKanji();

            FlashcardsData.ItemsCorrect = 0;
            FlashcardsData.ItemsWrong = 0;
            FlashcardsData.ItemsLeft = FlashcardsData.Kanjis.Count;
        }

        public static void Deinitialize()
        {
            FlashcardsData.Kanjis.Clear();
            FlashcardsData.Lessons = null;
        }

        #endregion

        #region Practice

        public static void EvaluateKanji(int grade)
        {
            int interval = FlashcardsData.ActiveKanji.nextInterval;
            int repetition = FlashcardsData.ActiveKanji.repetition;

            if(grade >= 3)
            {
                if(repetition == 0)
                {
                    interval = 1;
                    repetition = 1;
                }
                else if(repetition == 1)
                {
                    //on supermemo the new intervall here is 6 but with 20 items per interval the next time i see this item again would be 80 items away o.0 too much i think^^
                    //but maby later i'll give the user an option to adjust the interval size, like with partlessons count
                    interval = 4;
                    repetition = 2;
                }
                else
                {
                    interval = (int)Math.Round(interval * FlashcardsData.ActiveKanji.eFactor);
                    ++repetition;
                }
            }
            else
            {
                interval = 0;
                repetition = 0;
            }

            FlashcardsData.ActiveKanji.eFactor += -0.8f + 0.28f * grade - 0.02f * grade * grade;
            FlashcardsData.ActiveKanji.eFactor = Math.Max(1.3f, FlashcardsData.ActiveKanji.eFactor);
            FlashcardsData.ActiveKanji.nextInterval = interval;
            FlashcardsData.ActiveKanji.repetition = repetition;

            ++FlashcardsData.ItemIndex;
            ++itemsLearned;

            if (itemsLearned == itemsProInterval || FlashcardsData.ItemIndex == FlashcardsData.Kanjis.Count)
            {
                DataManager.UpdateProgress();

                itemsLearned = 0;
                FlashcardsData.ItemIndex = 0;
            }

            GetNextKanji();
        }

        private static void GetNextKanji()
        {
            while (FlashcardsData.ActiveKanji.repetition < FlashcardsData.ActiveKanji.nextInterval)
            {
                Debug.WriteLine("Skiped Kanji: " + FlashcardsData.ActiveKanji.ToDebugString());
                
                ++FlashcardsData.ActiveKanji.repetition;
                ++FlashcardsData.ItemIndex;
                FlashcardsData.ItemIndex %= FlashcardsData.Kanjis.Count;
            }
            
            Debug.WriteLine("Next Kanji: " + FlashcardsData.ActiveKanji.ToDebugString());
        }

        #endregion
    }
}