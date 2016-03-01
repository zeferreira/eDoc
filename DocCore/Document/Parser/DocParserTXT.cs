using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    class DocParserTxt : IDocParser
    {
        //implements singleton pattern
        private static DocParserTxt instance = null;
        private static readonly object padlock = new object();

        public static DocParserTxt Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DocParserTxt();
                    }
                    return instance;
                }
            }
        }

        DocParserTxt()
        { }

        public string GetText(string docFilePath)
        {
            try
            {
                string text = System.IO.File.ReadAllText(docFilePath);

                return text;
                
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
