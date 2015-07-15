using System;
using System.Data.Linq.Mapping;
using System.Text;

namespace JapanischTrainer.Database
{
    [Table]
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

        #region Fields

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int id;


        [Column(CanBeNull = false)]
        public String kana;

        [Column(CanBeNull = true)]
        public String kanji;

        [Column(CanBeNull = false)]
        public String translation;

        [Column(CanBeNull = true)]
        public String description;


        [Column(CanBeNull = false)]
        public short correctTranslation;

        [Column(CanBeNull = false)]
        public short wrongTranslation;

        [Column(CanBeNull = false)]
        public short correctJapanese;

        [Column(CanBeNull = false)]
        public short wrongJapanese;


        /// <summary>
        /// ATTENTION!!!!
        /// if you create new types, make sure to place them under the others, and do not change
        /// the order in any of the type lists otherwise all types will be propably inconsistent =(
        /// </summary>
        [Column(CanBeNull = false)]
        public int type = -1;

        /// <summary>
        /// <para>0 - Dont show Description</para>
        /// <para>1 - just show Description for German Word</para>
        /// <para>2 - just show Description for Japanese Word</para>
        /// <para>3 - Always show Description</para>
        /// </summary>
        [Column(CanBeNull = false)]
        public int showFlags = 3;

        [Column(CanBeNull = false)]
        public int timeStampJapanese;

        [Column(CanBeNull = false)]
        public int timeStampTransl;

        [Column(CanBeNull = false)]
        public int lessonID;


        public bool showJWord;

        /// <summary>
        /// because i make a copy of a word every time the kanji will be asked i need a reference to the original
        /// to make sure to increase the counter of the original as well. otherwise the counter for kanjis will
        /// be ignored when i update the database!
        /// </summary>
        public Word original = null;

        #endregion

        #region Properties

        public int CorrectWrongCountTranslation
        {
            get { return correctTranslation + wrongTranslation; }
        }

        public int CorrectWrongCountJapanese
        {
            get { return correctJapanese + wrongJapanese; }
        }

        public float CorrectWrongRelationTranslation
        {
            get { return correctTranslation / (float)(CorrectWrongCountTranslation == 0 ? 1 : CorrectWrongCountTranslation); }
        }

        public float CorrectWrongRelationJapanese
        {
            get { return correctJapanese / (float)(CorrectWrongCountJapanese == 0 ? 1 : CorrectWrongCountJapanese); }
        }

        public int TimeStampJapanese
        {
            get { return timeStampJapanese; }
            set { timeStampJapanese = value; }
        }

        /// <summary>
        /// Attention!!! just use if kanji is learned!!!
        /// </summary>
        public int TimeStampTransl
        {
            get { return original.timeStampTransl; }
            set { original.timeStampTransl = value; }
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
            correctJapanese        = source.correctJapanese;
            wrongJapanese          = source.wrongJapanese;

            type                = source.type;
            showFlags           = source.showFlags;
            timeStampJapanese   = source.timeStampJapanese;
            timeStampTransl     = source.timeStampTransl;
            lessonID            = source.lessonID;

            original            = source;
        }

        public Word(String text, int lessonID)
        {
            String[] fragments = text.Split('|');

            if (fragments.Length == 14)
            {
                this.id                  = Convert.ToInt32(fragments[0]);
                this.lessonID            = Convert.ToInt32(fragments[1]);
                this.kana                = fragments[2];
                this.kanji               = fragments[3];
                this.translation         = fragments[4];
                this.description         = fragments[5];
                this.type                = StringToType(fragments[6]);
                this.correctTranslation  = Convert.ToInt16(fragments[7]);
                this.wrongTranslation    = Convert.ToInt16(fragments[8]);
                this.correctJapanese     = Convert.ToInt16(fragments[9]);
                this.wrongJapanese       = Convert.ToInt16(fragments[10]);
                this.showFlags           = Convert.ToInt32(fragments[11]);
                this.timeStampJapanese   = Convert.ToInt32(fragments[12]);
                this.timeStampTransl     = Convert.ToInt32(fragments[13]);
            }
            else
            {
                this.kana        = fragments[0];
                this.kanji       = fragments[1];
                this.translation = fragments[2];
                this.description = fragments[3];

                if (fragments.Length == 5)
                {
                    this.type = StringToType(fragments[4].ToLower());
                }

                this.lessonID = lessonID;
            }

            if (kanji == "")
            {
                kanji = null;
            }
            if (description == "")
            {
                description = null;
            }
        }

        #endregion

        #region ToString

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(kana);

            sb.Append("|");
            sb.Append(kanji != null ? kanji : "");
            sb.Append("|");
            sb.Append(translation);
            sb.Append("|");
            sb.Append(description != null ? description : "");

            return CorrectWrongRelationJapanese + " " + CorrectWrongRelationTranslation + "  " + sb.ToString();
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
            sb.Append(kanji != null ? kanji : "");
            sb.Append("|");
            sb.Append(translation);
            sb.Append("|");
            sb.Append(description != null ? description : "");
            sb.Append("|");
            sb.Append(TypeToString());
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

            if (kanji != null)
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
            String typeStr = GetTypeString();

            if (description != null)
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
            return kanji == null ? kana : kanji;
        }

        #endregion

        #region Util

        /// <summary>
        /// <para>Updates the Word</para>
        /// <para>updateString Pattern Should Be:</para>
        /// <para>id|kana|kanji|translation|description</para>
        /// <para>kanji and description can be empty strings</para>
        /// </summary>
        public void Update(String updateString)
        {
            String[] parts = updateString.Split('|');
            
            kana        = parts[1];
            kanji       = parts[2] == "" ? null : parts[2];
            translation = parts[3];
            description = parts[4] == "" ? null : parts[4]; ;
        }

        /// <summary>
        /// Returns Typestring for Export String
        /// </summary>
        private String TypeToString()
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
        private int StringToType(String typeStr)
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
        private String GetTypeString()
        {
            switch ((EType)type)
            {
                case EType.noun       : return "Nomen";
                case EType.verb1      : return "う-Verb";
                case EType.verb2      : return "る-Verb";
                case EType.verb3      : return "irregular Verb";
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
