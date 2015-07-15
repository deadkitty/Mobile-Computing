using JapanischTrainer.Data;
using JapanischTrainer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer
{
    public static class Util
    {
        #region Fields

        private static Random rand = new Random();
        
        #endregion

        #region Public Methods

        #region Sort Arrays

        public static void SortByCorrectWrongRelation(Word[] words)
        {
            //get relations for both, japanese word and translation and mix them in one array
            float[] correctWrongRelations = new float[words.Length];

            for (int i = 0; i < correctWrongRelations.Length; ++i)
            {
                if (words[i].showJWord)
                {
                    correctWrongRelations[i] = words[i].CorrectWrongRelationJapanese;
                }
                else
                {
                    correctWrongRelations[i] = words[i].CorrectWrongRelationTranslation;
                }
            }

            //after this, sort these relations array and make every step for both
            //the relations array and the Words array
            for (int i = 0; i < words.Length - 1; ++i)
            {
                for (int j = i + 1; j < words.Length; ++j)
                {
                    if (correctWrongRelations[i] > correctWrongRelations[j])
                    {
                        Swap(ref correctWrongRelations[i], ref correctWrongRelations[j]);
                        Swap(ref words[i], ref words[j]);
                    }
                }
            }
        }

        public static void SortByTimeStamp(Word[] words)
        {
            //get timestamps for both, japanese word and translation and mix them in one array
            int[] timeStamps = new int[words.Length];

            for (int i = 0; i < timeStamps.Length; ++i)
            {
                if (words[i].showJWord)
                {
                    timeStamps[i] = words[i].timeStampJapanese;
                }
                else
                {
                    timeStamps[i] = words[i].timeStampTransl;
                }
            }

            //after this, sort these timestamp array and make every step for both
            //the timestamp array and the Words array
            for (int i = 0; i < words.Length - 1; ++i )
            {
                for(int j = i + 1; j < words.Length; ++j)
                {
                    if(timeStamps[i] > timeStamps[j])
                    {
                        Swap(ref timeStamps[i], ref timeStamps[j]);
                        Swap(ref words[i], ref words[j]);
                    }
                }
            }
        }

        public static void SortByRandom<Type>(Type[] values)
        {
            Random rand = new Random();

            for (int i = 0; i < values.Length; ++i)
            {
                int newIndex = rand.Next(values.Length);

                Swap(ref values[i], ref values[newIndex]);
            }
        }

        public static void SortByRandom(char[] values)
        {
            SortByRandom(values, values.Length);
        }

        /// <summary>
        /// Swaps items in the array randomly 
        /// </summary>
        /// <param name="values">items to randomize</param>
        /// <param name="lastIndex">last index until the method should swap the items</param>
        public static void SortByRandom(char[] values, int lastIndex)
        {
            for (int i = 0; i < lastIndex; ++i)
            {
                int newIndex = GetRandomNumber(values.Length);

                Swap(ref values[i], ref values[newIndex]);
            }
        }
        
        #endregion

        #region Swap Operations

        public static void Swap<Type>(Type a, Type b)
        {
            Type hv = a;
            a = b;
            b = hv;
        }

        public static void Swap<Type>(ref Type a, ref Type b)
        {
            Type hv = a;
            a = b;
            b = hv;
        }
        
        #endregion

        #region Generate Random Values

        public static int GetRandomNumber()
        {
            return rand.Next();
        }

        public static int GetRandomNumber(int max)
        {
            return rand.Next(max);
        }

        public static int GetRandomNumber(int min, int max)
        {
            return rand.Next(min, max);
        }
        
        #endregion

        #region Others

        public static Word.EType[] ExtractTypes(int loadOptions)
        {
            List<Word.EType> types = new List<Word.EType>();
            
            if (loadOptions % 2 == 1) types.Add(Word.EType.other);
            loadOptions >>= 1; 
            if (loadOptions % 2 == 1) types.Add(Word.EType.phrase);
            loadOptions >>= 1;            
            if (loadOptions % 2 == 1) types.Add(Word.EType.suffix);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.prefix);
            loadOptions >>= 1;            
            if (loadOptions % 2 == 1) types.Add(Word.EType.particle);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.noun);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.adverb);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.naAdjective);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.iAdjective);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.verb3);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.verb2);
            loadOptions >>= 1;
            if (loadOptions % 2 == 1) types.Add(Word.EType.verb1);

            return types.ToArray();
        }

        public static String TargetFormToString(ConjugationData.ETargetForm targetForm)
        {
            switch (targetForm)
            {
                case ConjugationData.ETargetForm.ruForm: return "る - Form";
                case ConjugationData.ETargetForm.naiForm: return "ない - Form";
                case ConjugationData.ETargetForm.naiPastForm: return "ない - Form Past";
                case ConjugationData.ETargetForm.masuForm: return "ます - Form";
                case ConjugationData.ETargetForm.masuNegativeForm: return "ます - Form Negative";
                case ConjugationData.ETargetForm.masuPastForm: return "ます - Form Past";
                case ConjugationData.ETargetForm.masuPastNegativeForm: return "ます - Form Past Negative";
                case ConjugationData.ETargetForm.teForm: return "て - Form";
                case ConjugationData.ETargetForm.taForm: return "た - Form";
                case ConjugationData.ETargetForm.imperativeForm: return "Imperative Form";
                case ConjugationData.ETargetForm.prohibitiveForm: return "Prohibitive Form";
                case ConjugationData.ETargetForm.volitionalForm: return "Volitional Form";
                case ConjugationData.ETargetForm.conditionalForm: return "Conditional Form";
                case ConjugationData.ETargetForm.tai: return "たい - Form";
                case ConjugationData.ETargetForm.sugi: return "すぎ - Form";
                case ConjugationData.ETargetForm.yasui: return "やすい - Form";
                case ConjugationData.ETargetForm.nikui: return "にくい - Form";
                case ConjugationData.ETargetForm.potentialVerb: return "Potential Verb";
                case ConjugationData.ETargetForm.passiveVerb: return "Passive Verb";
                case ConjugationData.ETargetForm.causativeVerb: return "Causasitive Verb";
            }

            return "";
        }
        
        #endregion

        #endregion
    }
}
