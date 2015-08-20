using System;
using Windows.Storage;


namespace NihongoSenpai.Settings
{
    public static class AppSettings
    {
        #region Fields

        private static ApplicationDataContainer settings;
        
        private static bool firstStart = true;
        private static int wordPracticeMethod = 0;

        private static bool loadAllWords = false;
        private static int minimumWordCount = 5;
        private static float correctWrongRelation = 0.9f;

        private static bool learnWrongWords = false;
        private static int learnWrongWordsCount = 15;

        private static bool showDescription = true;
        private static String backgroundImage = null;
        
        private static int timeStamp   = 0;
        private static int sortOrder   = 3;
        private static int loadOptions = 4095;  //binary : 1111 1111 1111 (at first start load all types)
        
        private static int flashCardsInterval = 1;

        private static String firstStartKey = "firstStart";
        private static String wordPracticeMethodKey = "wordPracticeMethod";

        private static String loadAllWordsKey = "loadAllWords";
        private static String minimumWordCountKey = "minimumWordCount";
        private static String correctWrongRelationKey = "correctWrongRelation";

        private static String learnWrongWordsKey = "learnWrongWords";
        private static String learnWrongWordsCountKey = "learnWrongWordsCount";

        private static String showDescriptionKey = "showDescription";
        private static String backgroundImageKey = "backgroundImage";

        private static String timeStampKey   = "timeStamp";
        private static String sortOrderKey   = "sortOrder";
        private static String loadOptionsKey = "loadOptions";

        private static String flashCardsIntervalKey = "flashCardsInterval";

        #endregion

        #region Properties

        /// <summary>
        /// Checks if the app starts the first time
        /// </summary>
        public static bool FirstStart
        {
            get { return firstStart; }
            private set
            {
                firstStart = value;
                settings.Values[firstStartKey] = value;
            }
        }

        /// <summary>
        /// if true, all words will be loaded on WordsPracticePage and ignores MinimumWordCount and CorrectWrongRelation
        /// </summary>
        public static bool LoadAllWords
        {
            get { return loadAllWords; }
            set
            {
                loadAllWords = value;
                settings.Values[loadAllWordsKey] = value;
            }
        }

        /// <summary>
        /// Value between 0 and 1 is used as a Percentage value between Correct and Wrong answered Words.
        /// If a word is answered Correct, more often than wrong, the word will not be loaded in WordsPracticePage.
        /// </summary>
        public static float CorrectWrongRelation
        {
            get { return correctWrongRelation; }
            set
            {
                correctWrongRelation = value;
                settings.Values[correctWrongRelationKey] = value;
            }
        }

        /// <summary>
        /// because we can not say after one correct answered word we have 100% correct answered words and don´t need to learn
        /// this word again. We need a minimum wordCount so the word has to be minimum answered (correct and wrong) 
        /// so that CorrectWrongRelation can Trigger
        /// </summary>
        public static int MinimumWordCount
        {
            get { return minimumWordCount; }
            set
            {
                minimumWordCount = value;
                settings.Values[minimumWordCountKey] = value;
            }
        }
        
        /// <summary>
        /// if true, Description in wordsPracticePage will be shown
        /// </summary>
        public static bool ShowDescription
        {
            get { return showDescription; }
            set
            {
                showDescription = value;
                settings.Values[showDescriptionKey] = value;
            }
        }

        /// <summary>
        /// if PartLessons is true, the user will learn just one part after another from the selected Lessons.
        /// like the first 10 words of a lesson and just when the user has all words answered true. he gets the
        /// next 10 words.
        /// Why doing this?
        /// because Lessons can have hundreds of new words in it, the user doesn´t know.
        /// if the user has 90 of these 100 words wrong. he can´t possibly remember all these words
        /// for the next round and answers them again wrong. but he can remember 10 words
        /// and answer them correct in the next round ;D
        /// </summary>
        public static bool LearnWrongWords
        {
            get { return learnWrongWords; }
            set
            {
                learnWrongWords = value;
                settings.Values[learnWrongWordsKey] = value;
            }
        }

        /// <summary>
        /// the number of words that will be learned in one part lesson
        /// </summary>
        public static int LearnWrongWordsCount
        {
            get { return learnWrongWordsCount; }
            set
            {
                learnWrongWordsCount = value;
                settings.Values[learnWrongWordsCountKey] = value;
            }
        }

        /// <summary>
        /// is used to lessons for word practice in differend ways
        /// <para>0 - shows translation and jWord</para>
        /// <para>1 - shows just translation</para>
        /// <para>2 - shows just jWord</para>
        /// </summary>
        public static int WordPracticeMethod
        {
            get { return wordPracticeMethod; }
            set
            {
                wordPracticeMethod = value;
                settings.Values[wordPracticeMethodKey] = value;
            }
        }

        /// <summary>
        /// decides the order in wich the loaded words are displayed
        /// <para>0 - worst correctWrongRelation first</para>
        /// <para>1 - longest time not learned first</para>
        /// <para>2 - by Lesson</para>
        /// <para>3 - Random</para>
        /// </summary>
        public static int SortOrder
        {
            get { return sortOrder; }
            set
            {
                sortOrder = value;
                settings.Values[sortOrderKey] = value;
            }
        }

        /// <summary>
        /// stores word types that should be loaded bitwise in an integer value
        /// </summary>
        public static int LoadOptions
        {
            get { return loadOptions; }
            set 
            { 
                loadOptions = value;
                settings.Values[loadOptionsKey] = value;
            }
        }
        
        public static int FlashCardsInterval
        {
            get { return flashCardsInterval; }
            set 
            { 
                flashCardsInterval = value;
                settings.Values[flashCardsIntervalKey] = value;
            }
        }
        
        public static int TimeStamp
        {
            get { return timeStamp; }
            set
            {
                timeStamp = value;
                settings.Values[timeStampKey] = value;
            }
        }

        /// <summary>
        /// Path to background Image on MainPage
        /// </summary>
        public static String BackgroundImage
        {
            get { return backgroundImage; }
            set
            {
                backgroundImage = value;
                settings.Values[backgroundImageKey] = value;
            }
        }

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            settings = ApplicationData.Current.LocalSettings;
            
            if (!settings.Values.ContainsKey(firstStartKey))
            {
                settings.Values[firstStartKey]           = firstStart;

                settings.Values[wordPracticeMethodKey]   = wordPracticeMethod;

                settings.Values[loadAllWordsKey]         = loadAllWords;
                settings.Values[minimumWordCountKey]     = minimumWordCount;
                settings.Values[correctWrongRelationKey] = correctWrongRelation;

                settings.Values[learnWrongWordsKey]      = learnWrongWords;
                settings.Values[learnWrongWordsCountKey] = learnWrongWordsCount;

                settings.Values[showDescriptionKey]      = showDescription;
                settings.Values[backgroundImageKey]      = backgroundImage;

                settings.Values[timeStampKey]            = timeStamp;
                settings.Values[sortOrderKey]            = sortOrder;
                settings.Values[loadOptionsKey]          = loadOptions;

                settings.Values[flashCardsIntervalKey]   = flashCardsInterval;
            }
            else
            {
                settings.Values[firstStartKey] = false;
                firstStart = false;

                wordPracticeMethod   = (int)    settings.Values[wordPracticeMethodKey];

                loadAllWords         = (bool)   settings.Values[loadAllWordsKey];
                minimumWordCount     = (int)    settings.Values[minimumWordCountKey];
                correctWrongRelation = (float)  settings.Values[correctWrongRelationKey];

                learnWrongWords      = (bool)   settings.Values[learnWrongWordsKey];
                learnWrongWordsCount = (int)    settings.Values[learnWrongWordsCountKey];

                showDescription      = (bool)   settings.Values[showDescriptionKey];
                backgroundImage      = (String) settings.Values[backgroundImageKey];

                timeStamp            = (int)    settings.Values[timeStampKey];
                sortOrder            = (int)    settings.Values[sortOrderKey];
                loadOptions          = (int)    settings.Values[loadOptionsKey];

                flashCardsInterval   = (int)    settings.Values[flashCardsIntervalKey];
            }
        }
        
        #endregion
    }
}