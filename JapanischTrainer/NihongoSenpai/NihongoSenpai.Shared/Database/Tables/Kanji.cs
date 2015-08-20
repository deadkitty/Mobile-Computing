using SQLite;
using System;
using System.Text;
using System.Globalization;

namespace NihongoSenpai.Database
{
    [Table("Kanjis")]
    public class Kanji
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        
        [NotNull]
        public String kanji { get; set; }
        
        [NotNull]
        public String meaning { get; set; }
        
        [NotNull]
        public String onyomi { get; set; }
        
        [NotNull]
        public String kunyomi { get; set; }
        
        [NotNull]
        public String example { get; set; }

        /// <summary>
        /// not used yet
        /// </summary>
        [NotNull]
        public String strokeOrder { get; set; }
        
        [NotNull]
        public float eFactor { get; set; }
        
        [NotNull]
        public int repetition { get; set; }
        
        [NotNull]
        public int nextInterval { get; set; }
        
        [NotNull]
        public int lessonID { get; set; }

        public Kanji()
        {

        }

        public Kanji(String text, int setID)
        {
            String[] fragments = text.Split('|');
            if(fragments.Length == 11)
            {
                id = Convert.ToInt32(fragments[0]);
                lessonID = Convert.ToInt32(fragments[1]);
                kanji = fragments[2];
                meaning = fragments[3];
                onyomi = fragments[4];
                kunyomi = fragments[5];
                example = fragments[6];
                strokeOrder = fragments[7];
                eFactor = Convert.ToSingle(fragments[8], CultureInfo.InvariantCulture);
                repetition = Convert.ToInt32(fragments[9]);
                nextInterval = Convert.ToInt32(fragments[10]);
            }
            else
            {
                kanji = fragments[0];
                meaning = fragments[1];
                onyomi = fragments[2];
                kunyomi = fragments[3];
                example = fragments[4];
                strokeOrder = fragments[5];
                eFactor = 2.5f;
                repetition = 0;
                nextInterval = 0;
            }

            this.lessonID = setID;
        }

        public override string ToString()
        {
            return kanji + " - " + meaning + " - " + onyomi + " - " + kunyomi;
        }

        public String ToExportString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append("|");
            sb.Append(lessonID);
            sb.Append("|");
            sb.Append(kanji);
            sb.Append("|");
            sb.Append(meaning);
            sb.Append("|");
            sb.Append(onyomi);
            sb.Append("|");
            sb.Append(kunyomi);
            sb.Append("|");
            sb.Append(example);
            sb.Append("|");
            sb.Append(strokeOrder);
            sb.Append("|");
            sb.Append(eFactor.ToString(CultureInfo.InvariantCulture));
            sb.Append("|");
            sb.Append(repetition);
            sb.Append("|");
            sb.Append(nextInterval);

            return sb.ToString();
        }

        public String ToDebugString()
        {
            return id + "|" + kanji + "|" + meaning + "|" + eFactor +  "|" + repetition + "|" + nextInterval;
        }

        /// <summary>
        /// <para>Updates the kanji</para>
        /// <para>updateString Pattern should be:</para>
        /// <para>id|kanji|meaning|onyomi|kunyomi|example</para>
        /// </summary>
        /// <param name="updateString"></param>
        public void Update(String updateString)
        {
            String[] parts = updateString.Split('|');
            
            kanji   = parts[1];
            meaning = parts[2];
            onyomi  = parts[3];
            kunyomi = parts[4];            
            example = parts[5];
        }
    }
}