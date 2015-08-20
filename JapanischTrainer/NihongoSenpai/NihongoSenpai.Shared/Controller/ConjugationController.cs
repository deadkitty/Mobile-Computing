using NihongoSenpai.Data;
using NihongoSenpai.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihongoSenpai.Controller
{
    public static class ConjugationController
    {
        #region Fields

        private static Random rand;

        private static int itemIndex = 0;

        #endregion

        #region Properties

        public static int ItemIndex
        {
            get { return itemIndex; }
        }

        #endregion

        #region Public Methods

        #region Initialize
        
        public static void LoadLessons(Lesson[] selectedLessons)
        {
            ConjugationData.Lessons = selectedLessons;

            Word.EType[] types = 
            {
                Word.EType.verb1,
                Word.EType.verb2,
                Word.EType.verb3,
            };

            DataManager.LoadWords(ConjugationData.Lessons, types);

            ConjugationData.Words = AppData.Words;

            AppData.Words = null;

            rand = new Random();

            Util.SortByRandom(ConjugationData.Words);

            itemIndex = 0;

            ConjugationData.ItemsCorrect = 0;
            ConjugationData.ItemsWrong = 0;
            ConjugationData.ItemsLeft = ConjugationData.Words.Length;

            ConjugationData.ActiveWords = new Word                       [ConjugationData.maxActiveConjugationWordsCount];
            ConjugationData.TargetWords = new String                     [ConjugationData.maxActiveConjugationWordsCount];
            ConjugationData.TargetForms = new ConjugationData.ETargetForm[ConjugationData.maxActiveConjugationWordsCount];
        }

        public static void Deinitialize()
        {
            ConjugationData.Words       = null;
            ConjugationData.ActiveWords = null;
            ConjugationData.TargetWords = null;
            ConjugationData.TargetForms = null;
            ConjugationData.Lessons     = null;
        }
        
        #endregion

        #region Practice

        public static bool[] CheckWords(String[] answers)
        {
            bool[] correctAnswered = new bool[answers.Length];

            for (int i = 0; i < answers.Length; ++i)
            {
                String[] words;

                if (ConjugationData.ActiveWords[i].kanji != null)
                {
                    words = ConjugationData.ActiveWords[i].kanji.Split('、');
                }
                else
                {
                    words = ConjugationData.ActiveWords[i].kana.Split('、');
                }

                foreach (String word in words)
                {
                    ConjugationData.TargetWords[i] = GetTargetWord(word, (Word.EType)ConjugationData.ActiveWords[i].type, ConjugationData.TargetForms[i]);

                    if ((correctAnswered[i] = answers[i] == ConjugationData.TargetWords[i]))
                    {
                        break;
                    }
                }

                if(correctAnswered[i])
                {
                    ConjugationData.ItemsCorrect++;
                }
                else
                {
                    ConjugationData.ItemsWrong++;
                }

                ConjugationData.ItemsLeft -= ConjugationData.maxActiveConjugationWordsCount;
            }

            return correctAnswered;
        }

        public static void GetNextWords()
        {
            if (itemIndex >= ConjugationData.Words.Length)
            {
                Util.SortByRandom(ConjugationData.Words);

                itemIndex = 0;

                ConjugationData.ItemsCorrect = 0;
                ConjugationData.ItemsWrong = 0;
                ConjugationData.ItemsLeft = ConjugationData.Words.Length;
            }

            InitTargetForms();

            for (int i = 0; i < ConjugationData.maxActiveConjugationWordsCount; ++i)
            {
                if (itemIndex < ConjugationData.Words.Length)
                {
                    ConjugationData.ActiveWords[i] = ConjugationData.Words[itemIndex];
                }
                else
                {
                    ConjugationData.ActiveWords[i] = null;
                }

                ++itemIndex;
            }
        }

        public static void InitTargetForms()
        {
            for (int i = 0; i < ConjugationData.TargetForms.Length; ++i)
            {
                switch (rand.Next(19))
                {
                    case  0: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.naiForm             ; break;
                    case  1: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.naiPastForm         ; break;
                    case  2: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.masuForm            ; break;
                    case  3: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.masuNegativeForm    ; break;
                    case  4: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.masuPastForm        ; break;
                    case  5: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.masuPastNegativeForm; break;
                    case  6: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.teForm              ; break;
                    case  7: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.taForm              ; break;
                    case  8: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.imperativeForm      ; break;
                    case  9: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.prohibitiveForm     ; break;
                    case 10: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.volitionalForm      ; break;
                    case 11: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.conditionalForm     ; break;
                    case 12: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.tai                 ; break;
                    case 13: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.sugi                ; break;
                    case 14: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.yasui               ; break;
                    case 15: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.nikui               ; break;
                    case 16: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.potentialVerb       ; break;
                    case 17: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.passiveVerb         ; break;
                    case 18: ConjugationData.TargetForms[i] = ConjugationData.ETargetForm.causativeVerb       ; break;
                }
            }
        }

        private static String GetTargetWord(String sourceWord, Word.EType sourceWordType, ConjugationData.ETargetForm targetForm)
        {
            //before i conjugate the wort into the target form i first bring the word from the masu into the ru form
            //the thing is, normaly the word whould already be in ru form but i saved all verbs in masu form
            //so i just convert them at the begining, later i will convert all verbs into the ru form before so
            //i don't have do convert it here
            sourceWord = GetRuVerb(sourceWord, sourceWordType);

            switch (targetForm)
            {
                case ConjugationData.ETargetForm.ruForm              : return GetRuVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.naiForm             : return GetNaiVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.naiPastForm         : return GetNaiVerbPast(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.masuForm            : return GetMasuVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.masuNegativeForm    : return GetMasuVerbNegative(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.masuPastForm        : return GetMasuVerbPast(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.masuPastNegativeForm: return GetMasuVerbPastNegative(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.teForm              : return GetTeVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.taForm              : return GetTaVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.imperativeForm      : return GetImperativeVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.prohibitiveForm     : return GetProhibitiveVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.volitionalForm      : return GetVolitionalVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.conditionalForm     : return GetConditionalVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.tai                 : return GetTaiVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.sugi                : return GetSugiVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.yasui               : return GetYasuiVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.nikui               : return GetNikuiVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.potentialVerb       : return GetPotentialVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.passiveVerb         : return GetPassiveVerb(sourceWord, sourceWordType);
                case ConjugationData.ETargetForm.causativeVerb       : return GetCausativeVerb(sourceWord, sourceWordType);
            }

            return "";
        }

        #region Get Ru Form

        //at the moment i only have verbs saved in masu form so this funktion takes the masu form and
        //converts it in the ru form, all other convert funktions can take this ru form and 
        //convert it to its target form, later i want to save all verbs already in ru form so i don't need this step anymore
        private static String GetRuVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetRuVerb1(sourceWord);
                case Word.EType.verb2: return GetRuVerb2(sourceWord);
                case Word.EType.verb3: return GetRuVerb3(sourceWord);
            }

            return "";
        }

        private static String GetRuVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 3);

            switch (lastSign)
            {
                case 'い': lastSign = 'う'; break;
                case 'き': lastSign = 'く'; break;
                case 'し': lastSign = 'す'; break;
                case 'ち': lastSign = 'つ'; break;
                case 'に': lastSign = 'ぬ'; break;
                case 'ひ': lastSign = 'ふ'; break;
                case 'み': lastSign = 'む'; break;
                case 'り': lastSign = 'る'; break;
                case 'ぎ': lastSign = 'ぐ'; break;
                case 'じ': lastSign = 'ず'; break;
                case 'ぢ': lastSign = 'づ'; break;
                case 'び': lastSign = 'ぶ'; break;
                case 'ぴ': lastSign = 'ぷ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 3) + lastSign;
        }

        private static String GetRuVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 2) + "る";
        }

        private static String GetRuVerb3(String sourceWord)
        {
            if (sourceWord == "来ます")
            {
                return "来る";
            }

            return sourceWord.Substring(0, sourceWord.Length - 3) + "する";
        }

        #endregion

        #region Get Nai Form

        private static String GetNaiVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetNaiVerb1(sourceWord);
                case Word.EType.verb2: return GetNaiVerb2(sourceWord);
                case Word.EType.verb3: return GetNaiVerb3(sourceWord);
            }

            return "";
        }

        private static String GetNaiVerbPast(String sourceWord, Word.EType sourceWordType)
        {
            String naiForm = GetNaiVerb(sourceWord, sourceWordType);

            return naiForm.Substring(0, naiForm.Length - 1) + "かった";
        }

        private static String GetNaiVerb1(String sourceWord)
        {
            if (sourceWord == "在る" || sourceWord == "有る" || sourceWord == "ある")
            {
                return "ない";
            }

            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'わ'; break;
                case 'く': lastSign = 'か'; break;
                case 'す': lastSign = 'さ'; break;
                case 'つ': lastSign = 'た'; break;
                case 'ぬ': lastSign = 'な'; break;
                case 'ふ': lastSign = 'は'; break;
                case 'む': lastSign = 'ま'; break;
                case 'る': lastSign = 'ら'; break;
                case 'ぐ': lastSign = 'が'; break;
                case 'ず': lastSign = 'ざ'; break;
                case 'づ': lastSign = 'だ'; break;
                case 'ぶ': lastSign = 'ば'; break;
                case 'ぷ': lastSign = 'ぱ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "ない";
        }

        private static String GetNaiVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "ない";
        }

        private static String GetNaiVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来ない";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "しない";
        }

        #endregion

        #region Get Masu Form

        private static String GetMasuVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetMasuVerb1(sourceWord);
                case Word.EType.verb2: return GetMasuVerb2(sourceWord);
                case Word.EType.verb3: return GetMasuVerb3(sourceWord);
            }

            return "";
        }

        private static String GetMasuVerbNegative(String sourceWord, Word.EType sourceWordType)
        {
            String masuForm = GetMasuVerb(sourceWord, sourceWordType);

            return masuForm.Substring(0, masuForm.Length - 1) + "せん";
        }

        private static String GetMasuVerbPast(String sourceWord, Word.EType sourceWordType)
        {
            String masuForm = GetMasuVerb(sourceWord, sourceWordType);

            return masuForm.Substring(0, masuForm.Length - 1) + "した";
        }

        private static String GetMasuVerbPastNegative(String sourceWord, Word.EType sourceWordType)
        {
            String masuForm = GetMasuVerb(sourceWord, sourceWordType);

            return masuForm.Substring(0, masuForm.Length - 1) + "せんでした";
        }

        private static String GetMasuVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'い'; break;
                case 'く': lastSign = 'き'; break;
                case 'す': lastSign = 'し'; break;
                case 'つ': lastSign = 'ち'; break;
                case 'ぬ': lastSign = 'に'; break;
                case 'ふ': lastSign = 'ひ'; break;
                case 'む': lastSign = 'み'; break;
                case 'る': lastSign = 'り'; break;
                case 'ぐ': lastSign = 'ぎ'; break;
                case 'ず': lastSign = 'じ'; break;
                case 'づ': lastSign = 'ぢ'; break;
                case 'ぶ': lastSign = 'び'; break;
                case 'ぷ': lastSign = 'ぴ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "ます";
        }

        private static String GetMasuVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "ます";
        }

        private static String GetMasuVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来ます";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "します";
        }

        #endregion

        #region Get Te/Ta Form

        private static String GetTeVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetTeVerb1(sourceWord);
                case Word.EType.verb2: return GetTeVerb2(sourceWord);
                case Word.EType.verb3: return GetTeVerb3(sourceWord);
            }

            return "";
        }

        private static String GetTaVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetTaVerb1(sourceWord);
                case Word.EType.verb2: return GetTaVerb2(sourceWord);
                case Word.EType.verb3: return GetTaVerb3(sourceWord);
            }

            return "";
        }

        private static String GetTeVerb1(String sourceWord)
        {
            if (sourceWord == "行く")
            {
                return "行って";
            }

            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': return sourceWord.Substring(0, sourceWord.Length - 1) + "って";
                case 'く': return sourceWord.Substring(0, sourceWord.Length - 1) + "いて";
                case 'す': return sourceWord.Substring(0, sourceWord.Length - 1) + "して";
                case 'つ': return sourceWord.Substring(0, sourceWord.Length - 1) + "って";
                case 'ぬ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んで";
                case 'ふ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んで";
                case 'む': return sourceWord.Substring(0, sourceWord.Length - 1) + "んで";
                case 'る': return sourceWord.Substring(0, sourceWord.Length - 1) + "って";
                case 'ぐ': return sourceWord.Substring(0, sourceWord.Length - 1) + "いで";
                case 'ず': return sourceWord.Substring(0, sourceWord.Length - 1) + "して";
                case 'づ': return sourceWord.Substring(0, sourceWord.Length - 1) + "って";
                case 'ぶ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んで";
                case 'ぷ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んで";
            }

            return "";
        }

        private static String GetTeVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "て";
        }

        private static String GetTeVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来て";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "して";
        }

        private static String GetTaVerb1(String sourceWord)
        {
            if (sourceWord == "行く")
            {
                return "行った";
            }

            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': return sourceWord.Substring(0, sourceWord.Length - 1) + "った";
                case 'く': return sourceWord.Substring(0, sourceWord.Length - 1) + "いた";
                case 'す': return sourceWord.Substring(0, sourceWord.Length - 1) + "した";
                case 'つ': return sourceWord.Substring(0, sourceWord.Length - 1) + "った";
                case 'ぬ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んだ";
                case 'ふ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んだ";
                case 'む': return sourceWord.Substring(0, sourceWord.Length - 1) + "んだ";
                case 'る': return sourceWord.Substring(0, sourceWord.Length - 1) + "った";
                case 'ぐ': return sourceWord.Substring(0, sourceWord.Length - 1) + "いだ";
                case 'ず': return sourceWord.Substring(0, sourceWord.Length - 1) + "した";
                case 'づ': return sourceWord.Substring(0, sourceWord.Length - 1) + "った";
                case 'ぶ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んだ";
                case 'ぷ': return sourceWord.Substring(0, sourceWord.Length - 1) + "んだ";
            }

            return "";
        }

        private static String GetTaVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "た";
        }

        private static String GetTaVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来た";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "した";
        }

        #endregion

        #region Get Imperative Form

        private static String GetImperativeVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetImperativeVerb1(sourceWord);
                case Word.EType.verb2: return GetImperativeVerb2(sourceWord);
                case Word.EType.verb3: return GetImperativeVerb3(sourceWord);
            }

            return "";
        }

        private static String GetImperativeVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'え'; break;
                case 'く': lastSign = 'け'; break;
                case 'す': lastSign = 'せ'; break;
                case 'つ': lastSign = 'て'; break;
                case 'ぬ': lastSign = 'ね'; break;
                case 'ふ': lastSign = 'へ'; break;
                case 'む': lastSign = 'め'; break;
                case 'る': lastSign = 'れ'; break;
                case 'ぐ': lastSign = 'げ'; break;
                case 'ず': lastSign = 'ぜ'; break;
                case 'づ': lastSign = 'で'; break;
                case 'ぶ': lastSign = 'べ'; break;
                case 'ぷ': lastSign = 'ぺ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign;
        }

        private static String GetImperativeVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "ろ";
        }

        private static String GetImperativeVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来い";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "しろ";
        }

        #endregion

        #region Get Prohibitive Form

        private static String GetProhibitiveVerb(String sourceWord, Word.EType sourceWordType)
        {
            return sourceWord + "な";
        }

        #endregion

        #region Get Volitional Form

        private static String GetVolitionalVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetVolitionalVerb1(sourceWord);
                case Word.EType.verb2: return GetVolitionalVerb2(sourceWord);
                case Word.EType.verb3: return GetVolitionalVerb3(sourceWord);
            }

            return "";
        }

        private static String GetVolitionalVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'お'; break;
                case 'く': lastSign = 'こ'; break;
                case 'す': lastSign = 'そ'; break;
                case 'つ': lastSign = 'と'; break;
                case 'ぬ': lastSign = 'の'; break;
                case 'ふ': lastSign = 'ほ'; break;
                case 'む': lastSign = 'も'; break;
                case 'る': lastSign = 'ろ'; break;
                case 'ぐ': lastSign = 'ご'; break;
                case 'ず': lastSign = 'ぞ'; break;
                case 'づ': lastSign = 'ど'; break;
                case 'ぶ': lastSign = 'ぼ'; break;
                case 'ぷ': lastSign = 'ぽ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "う";
        }

        private static String GetVolitionalVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "よう";
        }

        private static String GetVolitionalVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来よう";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "しよう";
        }

        #endregion

        #region Get Conditional Form

        private static String GetConditionalVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetConditionalVerb1(sourceWord);
                case Word.EType.verb2: return GetConditionalVerb2(sourceWord);
                case Word.EType.verb3: return GetConditionalVerb3(sourceWord);
            }

            return "";
        }

        private static String GetConditionalVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'え'; break;
                case 'く': lastSign = 'け'; break;
                case 'す': lastSign = 'せ'; break;
                case 'つ': lastSign = 'て'; break;
                case 'ぬ': lastSign = 'ね'; break;
                case 'ふ': lastSign = 'へ'; break;
                case 'む': lastSign = 'め'; break;
                case 'る': lastSign = 'れ'; break;
                case 'ぐ': lastSign = 'げ'; break;
                case 'ず': lastSign = 'ぜ'; break;
                case 'づ': lastSign = 'で'; break;
                case 'ぶ': lastSign = 'べ'; break;
                case 'ぷ': lastSign = 'ぺ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "ば";
        }

        private static String GetConditionalVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "れば";
        }

        private static String GetConditionalVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来れば";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "すれば";
        }

        #endregion

        #region Get Tai, Sugi, Yasui, Nikui Form

        private static String GetTaiVerb(String sourceWord, Word.EType sourceWordType)
        {
            String masuVerb = GetMasuVerb(sourceWord, sourceWordType);

            return masuVerb.Substring(0, masuVerb.Length - 2) + "たい";
        }

        private static String GetSugiVerb(String sourceWord, Word.EType sourceWordType)
        {
            String masuVerb = GetMasuVerb(sourceWord, sourceWordType);

            return masuVerb.Substring(0, masuVerb.Length - 2) + "すぎる";
        }

        private static String GetYasuiVerb(String sourceWord, Word.EType sourceWordType)
        {
            String masuVerb = GetMasuVerb(sourceWord, sourceWordType);

            return masuVerb.Substring(0, masuVerb.Length - 2) + "やすい";
        }

        private static String GetNikuiVerb(String sourceWord, Word.EType sourceWordType)
        {
            String masuVerb = GetMasuVerb(sourceWord, sourceWordType);

            return masuVerb.Substring(0, masuVerb.Length - 2) + "にくい";
        }

        #endregion

        #region Get Potential Verb

        private static String GetPotentialVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetPotentialVerb1(sourceWord);
                case Word.EType.verb2: return GetPotentialVerb2(sourceWord);
                case Word.EType.verb3: return GetPotentialVerb3(sourceWord);
            }

            return "";
        }

        private static String GetPotentialVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'え'; break;
                case 'く': lastSign = 'け'; break;
                case 'す': lastSign = 'せ'; break;
                case 'つ': lastSign = 'て'; break;
                case 'ぬ': lastSign = 'ね'; break;
                case 'ふ': lastSign = 'へ'; break;
                case 'む': lastSign = 'め'; break;
                case 'る': lastSign = 'れ'; break;
                case 'ぐ': lastSign = 'げ'; break;
                case 'ず': lastSign = 'ぜ'; break;
                case 'づ': lastSign = 'で'; break;
                case 'ぶ': lastSign = 'べ'; break;
                case 'ぷ': lastSign = 'ぺ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "る";
        }

        private static String GetPotentialVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "られる";
        }

        private static String GetPotentialVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来られる";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "できる";
        }
      
        #endregion

        #region Get Passive Verb

        private static String GetPassiveVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetPassiveVerb1(sourceWord);
                case Word.EType.verb2: return GetPassiveVerb2(sourceWord);
                case Word.EType.verb3: return GetPassiveVerb3(sourceWord);
            }

            return "";
        }

        private static String GetPassiveVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'わ'; break;
                case 'く': lastSign = 'か'; break;
                case 'す': lastSign = 'さ'; break;
                case 'つ': lastSign = 'た'; break;
                case 'ぬ': lastSign = 'な'; break;
                case 'ふ': lastSign = 'は'; break;
                case 'む': lastSign = 'ま'; break;
                case 'る': lastSign = 'ら'; break;
                case 'ぐ': lastSign = 'が'; break;
                case 'ず': lastSign = 'ざ'; break;
                case 'づ': lastSign = 'だ'; break;
                case 'ぶ': lastSign = 'ば'; break;
                case 'ぷ': lastSign = 'ぱ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "れる";
        }

        private static String GetPassiveVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "られる";
        }

        private static String GetPassiveVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来られる";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "される";
        }
           
        #endregion

        #region Get Causative Verb

        private static String GetCausativeVerb(String sourceWord, Word.EType sourceWordType)
        {
            switch (sourceWordType)
            {
                case Word.EType.verb1: return GetCausativeVerb1(sourceWord);
                case Word.EType.verb2: return GetCausativeVerb2(sourceWord);
                case Word.EType.verb3: return GetCausativeVerb3(sourceWord);
            }

            return "";
        }

        private static String GetCausativeVerb1(String sourceWord)
        {
            char lastSign = sourceWord.ElementAt(sourceWord.Length - 1);

            switch (lastSign)
            {
                case 'う': lastSign = 'わ'; break;
                case 'く': lastSign = 'か'; break;
                case 'す': lastSign = 'さ'; break;
                case 'つ': lastSign = 'た'; break;
                case 'ぬ': lastSign = 'な'; break;
                case 'ふ': lastSign = 'は'; break;
                case 'む': lastSign = 'ま'; break;
                case 'る': lastSign = 'ら'; break;
                case 'ぐ': lastSign = 'が'; break;
                case 'ず': lastSign = 'ざ'; break;
                case 'づ': lastSign = 'だ'; break;
                case 'ぶ': lastSign = 'ば'; break;
                case 'ぷ': lastSign = 'ぱ'; break;
            }

            return sourceWord.Substring(0, sourceWord.Length - 1) + lastSign + "せる";
        }

        private static String GetCausativeVerb2(String sourceWord)
        {
            return sourceWord.Substring(0, sourceWord.Length - 1) + "させる";
        }

        private static String GetCausativeVerb3(String sourceWord)
        {
            if (sourceWord == "来る")
            {
                return "来させる";
            }

            return sourceWord.Substring(0, sourceWord.Length - 2) + "させる";
        }
                 
        #endregion

        #region 
        
        #endregion
        
        #endregion

        #endregion
    }
}
