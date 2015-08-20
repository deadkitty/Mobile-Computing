using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihongoSenpai
{
    public class AppStreamReader : StreamReader
    {
        private int currentLine = 0;

        public int CurrentLine
        {
            get { return currentLine; }
        }

        public AppStreamReader(Stream stream)
            : base(stream)
        {

        }

        public override string ReadLine()
        {
            ++currentLine;
            return base.ReadLine();
        }
    }
}
