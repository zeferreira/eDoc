using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Word;

namespace DocCore
{
    class DocParserDocxInterop : IDocParser
    {
        Application word;

        //implements singleton pattern
        private static DocParserDocxInterop instance = null;
        private static readonly object padlock = new object();

        public static DocParserDocxInterop Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DocParserDocxInterop();
                    }
                    return instance;
                }
            }
        }

        DocParserDocxInterop()
        {
            this.word = new Application();
        }

        public string GetText(string docFilePath)
        {
            try
            {
                StringBuilder text = new StringBuilder();
                object miss = System.Reflection.Missing.Value;
                object path = docFilePath;
                object readOnly = true;
                Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

                for (int i = 0; i < docs.Paragraphs.Count; i++)
                {
                    //text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
                    text.Append(docs.Paragraphs[i + 1].Range.Text.ToString());
                }

                //turn off word app
                word.Quit(ref miss, ref miss, ref miss);

                return text.ToString();
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
