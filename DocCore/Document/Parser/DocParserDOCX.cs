using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Word; 

namespace DocCore
{
    class DocParserDOCX : IDocParser
    {
        Application word;

        public DocParserDOCX()
        {
            this.word = new Application();
        }

        public string GetText(string docFilePath)
        {
            StringBuilder text = new StringBuilder();
            object miss = System.Reflection.Missing.Value;
            object path = docFilePath;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
            }

            //turn off word app
            word.Quit(ref miss,ref miss,ref miss);

            return text.ToString();
        }
    }
}
