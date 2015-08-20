using SQLite;
using System;
using System.Text;

namespace NihongoSenpai.Database
{
    [Table("Sentences")]
    public class Sentence
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [NotNull]
        public String text { get; set; }

        [NotNull]
        public String inserts { get; set; }

        [NotNull]
        public String hints { get; set; }

        [NotNull]
        public int lessonID { get; set; }

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
