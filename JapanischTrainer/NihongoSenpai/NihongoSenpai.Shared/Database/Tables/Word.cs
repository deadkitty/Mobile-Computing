using SQLite;
using System;
using System.Text;

namespace NihongoSenpai.Database
{
    [Table("Words")]
    public class Word
    {
        #region EType

        public enum EType
        {
            noun,
            verb1,
            verb2,
            verb3,
            iAdjective,
            naAdjective,
            adverb,
            particle,
            other,
            suffix,
            prefix,
            phrase,
            count,
            undefined = -1,
        }

        #endregion

        #region EAnswerState

        public enum EAnswerState
        {
            none,
            japanese,
            translation,
            both,
            count,
            undefined = -1,
        }

        #endregion

        #region Fields

        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        
        [NotNull]
        public String kana { get; set; }
        
        [NotNull]
        public String kanji { get; set; }
        
        [NotNull]
        public String translation { get; set; }
        
        [NotNull]
        public String description { get; set; }
                
        [NotNull]
        public short correctTranslation { get; set; }
        
        [NotNull]
        public short wrongTranslation { get; set; }
        
        [NotNull]
        public short correctJapanese { get; set; }
        
        [NotNull]
        public short wrongJapanese { get; set; }
        
        /// <summary>
        /// ATTENTION!!!!
        /// if you create new types, make sure to place them under the others, and do not change
        /// the order in any of the type lists otherwise all types will be propably inconsistent =(
        /// </summary>
        [NotNull]
        public int type { get; set; }

        /// <summary>
        /// <para>0 - Dont show Description</para>
        /// <para>1 - just show Description for German Word</para>
        /// <para>2 - just show Description for Japanese Word</para>
        /// <para>3 - Always show Description</para>
        /// </summary>
        [NotNull]
        public int showFlags { get; set; }
        
        [NotNull]
        public int timeStampJapanese { get; set; }
        
        [NotNull]
        public int timeStampTransl { get; set; }
        
        [NotNull]
        public int lessonID { get; set; }
        
        public EAnswerState answerState = EAnswerState.none;

        public bool showJWord = false;

        #endregion

        #region Properties
        
        [Ignore]
        public EType Type
        {
            get { return (EType)type; }
            set { type = (int)value; }
        }

        [Ignore]
        public int CorrectWrongCountTranslation
        {
            get { return correctTranslation + wrongTranslation; }
        }
        
        [Ignore]
        public int CorrectWrongCountJapanese
        {
            get { return correctJapanese + wrongJapanese; }
        }
        
        [Ignore]
        public float CorrectWrongRelationTranslation
        {
            get { return correctTranslation / (float)(CorrectWrongCountTranslation == 0 ? 1 : CorrectWrongCountTranslation); }
        }
        
        [Ignore]
        public float CorrectWrongRelationJapanese
        {
            get { return correctJapanese / (float)(CorrectWrongCountJapanese == 0 ? 1 : CorrectWrongCountJapanese); }
        }
        
        [Ignore]
        public int TimeStampJapanese
        {
            get { return timeStampJapanese; }
            set { timeStampJapanese = value; }
        }

        /// <summary>
        /// Attention!!! just use if kanji is learned!!!
        /// </summary>
        [Ignore]
        public int TimeStampTransl
        {
            get { return timeStampTransl; }
            set { timeStampTransl = value; }
        }

        #endregion

        #region Constructor

        public Word()
        {
            
        }

        public Word(Word source)
        {
            id                  = source.id;

            kana                = source.kana;
            kanji               = source.kanji;
            translation         = source.translation;
            description         = source.description;

            correctTranslation  = source.correctTranslation;
            wrongTranslation    = source.wrongTranslation;
            correctJapanese     = source.correctJapanese;
            wrongJapanese       = source.wrongJapanese;

            type                = source.type;
            showFlags           = source.showFlags;
            timeStampJapanese   = source.timeStampJapanese;
            timeStampTransl     = source.timeStampTransl;
            lessonID            = source.lessonID;
        }

        public Word(String text, int lessonID)
        {
            String[] fragments = text.Split('|');

            switch(fragments.Length)
            {
                case 14: //import string from earlier export of database
                
                    this.id                  = Convert.ToInt32(fragments[0]);
                    this.lessonID            = Convert.ToInt32(fragments[1]);
                    this.kana                = fragments[2];
                    this.kanji               = fragments[3];
                    this.translation         = fragments[4];
                    this.description         = fragments[5];
                    this.type                = ExportStringToType(fragments[6]);
                    this.correctTranslation  = Convert.ToInt16(fragments[7]);
                    this.wrongTranslation    = Convert.ToInt16(fragments[8]);
                    this.correctJapanese     = Convert.ToInt16(fragments[9]);
                    this.wrongJapanese       = Convert.ToInt16(fragments[10]);
                    this.showFlags           = Convert.ToInt32(fragments[11]);
                    this.timeStampJapanese   = Convert.ToInt32(fragments[12]);
                    this.timeStampTransl     = Convert.ToInt32(fragments[13]);

                    break;

                case 6: //add new word for existing lesson
                
                    this.lessonID    = Convert.ToInt32(fragments[0]);
                    this.kana        = fragments[1];
                    this.kanji       = fragments[2];
                    this.translation = fragments[3];
                    this.description = fragments[4];
                    this.type        = ExportStringToType(fragments[5].ToLower());

                    this.showFlags = 3;

                    break;

                default: //add new word for new lesson
                    
                    this.kana        = fragments[0];
                    this.kanji       = fragments[1];
                    this.translation = fragments[2];
                    this.description = fragments[3];
                    this.type        = ExportStringToType(fragments[4].ToLower());
                    
                    this.showFlags = 3;

                    this.lessonID = lessonID;

                    break;
            }
        }

        #endregion

        #region ToString

        /// <summary>
        /// <para>Creates a String with the following pattern:</para>
        /// <para>kana|kanji|translation|description</para>
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(kana);

            sb.Append("|");
            sb.Append(kanji);
            sb.Append("|");
            sb.Append(translation);
            sb.Append("|");
            sb.Append(description);

            return sb.ToString();
        }

        /// <summary>
        /// <para>Creates a String to use for export as txt file.</para>
        /// <para>Export Pattern:</para>
        /// <para>id|lessonId|kana|kanji|translation|description|type|correctTransl|wrongTransl|correctKanji|wrongKanji|showFlags|timeStampJapanese|timeStampTransl</para>
        /// </summary>
        public string ToExportString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append("|");
            sb.Append(lessonID);
            sb.Append("|");
            sb.Append(kana);
            sb.Append("|");
            sb.Append(kanji);
            sb.Append("|");
            sb.Append(translation);
            sb.Append("|");
            sb.Append(description);
            sb.Append("|");
            sb.Append(TypeToExportString());
            sb.Append("|");
            sb.Append(correctTranslation);
            sb.Append("|");
            sb.Append(wrongTranslation);
            sb.Append("|");
            sb.Append(correctJapanese);
            sb.Append("|");
            sb.Append(wrongJapanese);
            sb.Append("|");
            sb.Append(showFlags);
            sb.Append("|");
            sb.Append(timeStampJapanese);
            sb.Append("|");
            sb.Append(timeStampTransl);

            return sb.ToString();
        }

        public string ToDetailString()
        {
            StringBuilder sb = new StringBuilder();

            if (kanji != "")
            {
                sb.Append(kanji);
                sb.Append(" - ");
            }

            sb.Append(kana);
            sb.Append(" - ");
            sb.Append(translation);

            return sb.ToString();
        }

        public string ToDescriptionString()
        {
            String typeStr = TypeToString();

            if (description != "")
            {
                if (typeStr == "")
                {
                    return description;
                }
                return typeStr + " - " + description;
            }
            return typeStr;
        }

        /// <summary>
        /// checks if the kanji part is null or not and returns 
        /// either the kanji or the kana part
        /// </summary>
        public string ToJString()
        {
            return kanji == "" ? kana : kanji;
        }

        #endregion

        #region Util

        /// <summary>
        /// <para>Updates the Word</para>
        /// <para>updateString Pattern Should Be:</para>
        /// <para>id|kana|kanji|translation|description|type</para>
        /// <para>kanji and description can be empty strings</para>
        /// </summary>
        public void Update(String updateString)
        {
            String[] parts = updateString.Split('|');
            
            kana        = parts[1];
            kanji       = parts[2];
            translation = parts[3];
            description = parts[4];
            type        = ExportStringToType(parts[5]);
        }

        /// <summary>
        /// Returns Typestring for Export String
        /// </summary>
        private String TypeToExportString()
        {
            switch ((EType)type)
            {
                case EType.noun       : return "n";
                case EType.verb1      : return "1";
                case EType.verb2      : return "2";
                case EType.verb3      : return "3";
                case EType.iAdjective : return "i";
                case EType.naAdjective: return "na";
                case EType.adverb     : return "adv";
                case EType.particle   : return "pa";
                case EType.other      : return "o";
                case EType.prefix     : return "suf";
                case EType.suffix     : return "pre";
                case EType.phrase     : return "phr";
                default               : return "";
            }
        }

        /// <summary>
        /// Converts Given typeString and returns it as an int
        /// </summary>
        private int ExportStringToType(String typeStr)
        {
            switch (typeStr)
            {
                case "n"  : return (int)EType.noun;
                case "1"  : return (int)EType.verb1;
                case "2"  : return (int)EType.verb2;
                case "3"  : return (int)EType.verb3;
                case "i"  : return (int)EType.iAdjective;
                case "na" : return (int)EType.naAdjective;
                case "adv": return (int)EType.adverb;
                case "pa" : return (int)EType.particle;
                case "o"  : return (int)EType.other;
                case "pre": return (int)EType.prefix;
                case "suf": return (int)EType.suffix;
                case "phr": return (int)EType.phrase;
                default   : return (int)EType.other;
            }
        }

        /// <summary>
        /// Returns a string of the word objects type for description string
        /// </summary>
        /// <returns></returns>
        private String TypeToString()
        {
            switch ((EType)type)
            {
                case EType.noun       : return "Nomen";
                case EType.verb1      : return "う-Verb";
                case EType.verb2      : return "る-Verb";
                case EType.verb3      : return "unregelmäßiges Verb";
                case EType.iAdjective : return "い-Adj";
                case EType.naAdjective: return "な-Adj";
                case EType.adverb     : return "Adverb";
                case EType.particle   : return "Partikel";
                case EType.prefix     : return "Präfix";
                case EType.suffix     : return "Suffix";
                case EType.phrase     : return "Phrase";
                default: return "";
            }
        }

        #endregion
    }
}
