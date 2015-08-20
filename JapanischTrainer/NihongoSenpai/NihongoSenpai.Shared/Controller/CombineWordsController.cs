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
    public static class CombineWordsController
    {
        public static void LoadLessons(Lesson[] lessons)
        {
            CombineWordsData.Lessons = lessons;

            DataManager.LoadWords(CombineWordsData.Lessons, Util.ExtractTypes(AppSettings.LoadOptions));
            DataManager.LoadKanjis();

            List<char> sourceSigns = new List<char>();

            //TODO: for now i take my signs for the buttons from the kanjis, later i could use an extra sign table with all 
            //japanese signs in it, so that i don't have to check if a sign is multiple times in the sourceSigns array
            for (int i = 0; i < AppData.Kanjis.Length; ++i)
            {
                sourceSigns.Add(AppData.Kanjis[i].kanji[0]);
            }

            CombineWordsData.Words = AppData.Words;
            CombineWordsData.SourceSigns = sourceSigns.Distinct().ToArray();

            AppData.Words = null;

            Util.SortByRandom(CombineWordsData.SourceSigns);
            Util.SortByRandom(CombineWordsData.Words);

            CombineWordsData.ItemIndex = -1;
        }

        //public static void LoadLessons(Lesson[] lessons)
        //{
        //    CombineWordsData.Lessons = lessons;

        //    DataManager.LoadWords(CombineWordsData.Lessons, Util.ExtractTypes(AppSettings.LoadOptions));
        //    DataManager.LoadKanjis();
                        
        //    List<char> sourceSigns = new List<char>();

        //    //TODO: for now i take my signs for the buttons from the kanjis, later i could use an extra sign table with all 
        //    //japanese signs in it, so that i don't have to check if a sign is multiple times in the sourceSigns array
        //    for (int i = 0; i < AppData.Kanjis.Length; ++i )
        //    {
        //        bool addSign = true;

        //        for(int j = 0; j < sourceSigns.Count; ++j)
        //        {
        //            if(sourceSigns[j] == AppData.Kanjis[i].kanji[0])
        //            {
        //                addSign = false;
        //                break;
        //            }
        //        }

        //        if(addSign)
        //        {
        //            sourceSigns.Add(AppData.Kanjis[i].kanji[0]);
        //        }
        //    }
            
        //    CombineWordsData.Words = AppData.Words;
        //    CombineWordsData.SourceSigns = sourceSigns.ToArray();

        //    AppData.Words = null;
            
        //    Util.SortByRandom(CombineWordsData.SourceSigns);
        //    Util.SortByRandom(CombineWordsData.Words);

        //    CombineWordsData.ItemIndex = -1;            
        //}

        public static void GetNext(int sourceSignButtonsCount)
        {
            ++CombineWordsData.ItemIndex;
            CombineWordsData.ItemIndex %= CombineWordsData.Words.Length;

            //first i randomize the signs for the buttons
            Util.SortByRandom(CombineWordsData.SourceSigns, sourceSignButtonsCount);

            //i have to copy the first 30 signs to an extra array 
            //if i would copy the activeWord signs to the sourceSign Array, the array would be
            //more and more get mixed up with always the same signs, (especialy with ま、す、い、etc.) 
            //so to prevent this i'll just make a copy that i can mess up ;D
            CombineWordsData.CurrentSigns = new char[sourceSignButtonsCount];

            for(int i = 0; i < sourceSignButtonsCount; ++i)
            {
                CombineWordsData.CurrentSigns[i] = CombineWordsData.SourceSigns[i];
            }
            
            //than i get the signs for the next word
            String activeWord  = CombineWordsData.ActiveWord.ToJString();            
            char[] sourceSigns = CombineWordsData.CurrentSigns;

            Random rand = new Random();
            
            //at least i have to place the answer signs in the sourceSigns array
            foreach (char sign in activeWord)
            {
                //some words have more than one kanji reading and are seperated throug a '、' sign
                //for this excersice i just check against the first reading
                //so i skip the rest of the signs if this sign shows up
                if(sign == '、')
                {
                    break;
                }

                bool signContained = false;
                
                //first i check if the current sign is already in the sourceSigns array
                for(int i = 0; i < sourceSignButtonsCount; ++i)
                {
                    if(sign == sourceSigns[i])
                    {
                        signContained = true;
                        break;
                    }
                }

                //if not i choose a new index to place the sign there
                if(!signContained)
                {
                    //i do this in a loop because i don't want to accidently replace another sign 
                    //that is requiered for the activWord, so i search for a new index until
                    //i get one with a sign not in the active word
                    int newIndex = 0;
                    do
                    {
                        newIndex = rand.Next(sourceSignButtonsCount);
                    }
                    while(activeWord.Contains(sourceSigns[newIndex]));

                    sourceSigns[newIndex] = sign;
                }
            }
        }

        public static void CheckAnswer(String answer)
        {
            String correctAnswer = CombineWordsData.ActiveWord.ToJString();

            int minLength = Math.Min(answer.Length, correctAnswer.Length);
            int maxLength = Math.Max(answer.Length, correctAnswer.Length);

            List<bool> answers = new List<bool>();

            for (int i = 0; i < minLength; ++i)
            {
                //just check until the first kanji reading
                if (correctAnswer[i] == '、')
                {
                    break;
                }

                answers.Add(answer[i] == correctAnswer[i]);
            }
            
            for (int i = minLength; i < maxLength; ++i)
            {
                answers.Add(false);
            }

            CombineWordsData.Answers = answers.ToArray();
        }
    }
}
