using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data.Linq.Mapping;
using System.Text;
using System.Globalization;

namespace JapanischTrainer.Database
{
    [Table]
    public class Kanji
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int id;

        [Column(CanBeNull = false)]
        public String kanji;

        [Column(CanBeNull = false)]
        public String meaning;

        [Column(CanBeNull = false)]
        public String onyomi;

        [Column(CanBeNull = false)]
        public String kunyomi;

        [Column(CanBeNull = false)]
        public String example;

        /// <summary>
        /// not used yet
        /// </summary>
        [Column(CanBeNull = false)]
        public String strokeOrder;

        [Column(CanBeNull = false)]
        public float eFactor;

        [Column(CanBeNull = false)]
        public int repetition;

        [Column(CanBeNull = false)]
        public int nextInterval;
        
        [Column(CanBeNull = false)]
        public int lessonID;

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
            //eFactor = 2.5f;
            //repetition = 0;
            //nextInterval = 0;
            //correctAnswered = Convert.ToInt32(fragments[6]);
            //learnProgress   = Convert.ToInt32(fragments[7]);

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
            //sb.Append(correctAnswered);
            //sb.Append("|");
            //sb.Append(learnProgress);

            return sb.ToString();
        }

        public String ToDebugString()
        {
            return id + "|" + kanji + "|" + meaning + "|" + eFactor +  "|" + repetition + "|" + nextInterval;
        }
    }
}