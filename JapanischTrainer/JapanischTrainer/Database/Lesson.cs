using System;
using System.Data.Linq.Mapping;
using System.Text;

namespace NihongoSenpai.Database
{
    [Table]
    public class Lesson
    {
        public enum EType
        {
            vocabulary  =  0,
            insert      =  1,
            conjugation =  2,
            kanji       =  3,
            grammar     =  4,
            count       =  5,
            undefined   = -1,
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int id;

        [Column(CanBeNull = false)]
        public String name;

        /// <summary>
        /// <para>Type of the Lesson</para>
        /// <para>0 - Vocab</para>
        /// <para>1 - Insert</para>
        /// <para>2 - conjugation Lesson (unused yet)</para>
        /// <para>3 - Kanji Flashcards</para>
        /// <para>4 - Grammar (unused yet)</para>
        /// </summary>
        [Column(CanBeNull = false)]
        public int type;

        [Column(CanBeNull = false)]
        public int size;

        public Lesson()
        {

        }

        public Lesson(String text)
        {
            String[] textFragments = text.Split('|');
            if (textFragments.Length == 4)
            {
                id   = Convert.ToInt32(textFragments[0]);
                name = textFragments[1];
                type = Convert.ToInt32(textFragments[2]);
                size = Convert.ToInt32(textFragments[3]);
            }
            else
            {
                name = textFragments[0];
                type = Convert.ToInt32(textFragments[1]);
                size = Convert.ToInt32(textFragments[2]);
            }
        }

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// <para>Creates a String to use for export as txt file.</para>
        /// <para>Export Pattern:</para>
        /// <para>id|name|type|size</para>
        /// </summary>
        public String ToExportString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append("|");
            sb.Append(name);
            sb.Append("|");
            sb.Append(type);
            sb.Append("|");
            sb.Append(size);

            return sb.ToString();
        }

        /// <summary>
        /// <para>Updates the Lesson</para>
        /// <para>updateString Pattern Should Be:</para>
        /// <para>id|name|size</para>
        /// <para>type can not be changed yet</para>
        /// </summary>
		//public void Update(String updateString)
		//{
		//	String[] textFragments = updateString.Split('|');

		//	name = textFragments[1];
		//	size = Convert.ToInt32(textFragments[2]);
		//}

		/// <summary>
		/// Updates the Lesson name with the name in the newer Lesson parameter
		/// </summary>
		/// <param name="newer"></param>
		public void Update(Lesson newer)
		{
			name = newer.name;
		}
    }
}
