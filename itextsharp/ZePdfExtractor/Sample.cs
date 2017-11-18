using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;

namespace PDFzeExtractor
{
    class ProgramSample
    {
        static void MainSample(string[] args)
        {
            string filePath = @"G:\RepositoryFiles\Effect of Inverted Index Partitioning Schemes on Performance of Query Processing in Parallel Text Retrieval Systems.pdf";

            PdfReader reader = new PdfReader(filePath);
            
            ZeFontSizeLocationTextExtractionStrategy S = new ZeFontSizeLocationTextExtractionStrategy();
            List<ZeChunkFontSize> resultList = ZePdfTextExtractor.GetTextFromPage(reader, 1, S);

            StringBuilder resultSb = new StringBuilder();

            for (int i = 0; i < resultList.Count; i++)
            {
                resultSb.AppendFormat(@"<span style=""font-family:{0};font-size:{1}"">", resultList[i].CurFont, resultList[i].CurFontSize);

                while ((i < resultList.Count) && (resultList[i].Text != "\n") && (resultList[i].Text != " ")  )
                {
                    resultSb.Append(resultList[i].Text);
                    i++;
                }

                if ((i < resultList.Count) && (resultList[i].Text == " "))
                {
                    resultSb.Append(resultList[i].Text);
                }

                if ((i < resultList.Count) && (resultList[i].Text == "\n"))
                {
                    resultSb.Append("<br />");

                }

                resultSb.AppendLine("</span>");
            }

            string F = resultSb.ToString();

            Console.WriteLine(F);

            string fileResult = F;
            System.IO.File.WriteAllText(@"G:\htmlextrac.html", fileResult);
            
            Console.ReadLine();
        }
    }
}
