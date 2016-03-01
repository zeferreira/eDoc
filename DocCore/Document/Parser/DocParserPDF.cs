using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;  

namespace DocCore
{
    class DocParserPdf : IDocParser
    {
        //implements singleton pattern
        private static DocParserPdf instance = null;
        private static readonly object padlock = new object();

        public static DocParserPdf Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DocParserPdf();
                    }
                    return instance;
                }
            }
        }

        DocParserPdf()
        { }

        public string GetText(string docFilePath)
        {
            try
            {
                StringBuilder text = new StringBuilder();
                using (PdfReader reader = new PdfReader(docFilePath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }

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
