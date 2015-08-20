using System;
using System.Data.Linq.Mapping;
using System.Text;

namespace NihongoSenpai.Database
{
    [Table]
    public class Sentence
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int id;

        [Column(CanBeNull = false)]
        public String text;

        [Column(CanBeNull = false)]
        public String inserts;

        [Column(CanBeNull = false)]
        public String hints;

        [Column(CanBeNull = false)]
        public int lessonID;

        public Sentence()
        {

        }

        public Sentence(String source)
        {
            String[] textFragments = source.Split('|');
            if (textFragments.Length == 5)
            {
                this.id       = Convert.ToInt32(textFragments[0]);
                this.lessonID = Convert.ToInt32(textFragments[1]);
                this.text     = textFragments[2];
                this.inserts  = textFragments[3];
                this.hints    = textFragments[4];
            }
            else
            {
                this.text    = textFragments[0];
                this.inserts = textFragments[1];
                this.hints   = textFragments[2];
            }
        }

        public Sentence(String source, int lessonID)
        {
            String[] textFragments = source.Split('|');
            if (textFragments.Length == 5)
            {
                this.id       = Convert.ToInt32(textFragments[0]);
                this.lessonID = Convert.ToInt32(textFragments[1]);
                this.text     = textFragments[2];
                this.inserts  = textFragments[3];
                this.hints    = textFragments[4];
            }
            else
            {
                this.lessonID = lessonID;

                this.text    = textFragments[0];
                this.inserts = textFragments[1];
                this.hints   = textFragments[2];
            }
        }

        public override string ToString()
        {
            return text;
        }

        /// <summary>
        /// <para>Creates a String to use for export as txt file.</para>
        /// <para>Export Pattern:</para>
        /// <para>id|lessonID|text|insertText|hintText</para>
        /// </summary>
        public String ToExportString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append("|");
            sb.Append(lessonID);
            sb.Append("|");
            sb.Append(text);
            sb.Append("|");
            sb.Append(inserts);
            sb.Append("|");
            sb.Append(hints);

            return sb.ToString();
        }

        /// <summary>
        /// <para>Updates the Sentence</para>
        /// <para>updateString Pattern Should Be:</para>
        /// <para>id|text|insertText|hintText</para>
        /// </summary>
        public void Update(String updateString)
        {
            String[] textFragments = updateString.Split('|');

            text    = textFragments[1];
            inserts = textFragments[2];
            hints   = textFragments[3];
        }
    }
}
