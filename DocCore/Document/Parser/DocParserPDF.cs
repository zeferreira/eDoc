using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;  

namespace DocCore
{
    class DocParserPDF : IDocParser
    {
        public string GetText(string docFilePath)
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
    }
}
