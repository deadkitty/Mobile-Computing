using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace NihongoSenpai.Database
{
    [Table("Signs")]
    public class Sign
    {
        //[PrimaryKey, AutoIncrement]
        //public int id { get; set; }

        [PrimaryKey]
        public String value { get; set; }
    }
}
