using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    class DocParserTXT : IDocParser
    {
        public string GetText(string docFilePath)
        {
            string text = System.IO.File.ReadAllText(docFilePath);

            return text;
        }
    }
}
