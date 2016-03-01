using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace DocCore
{
    class DocParserDocxOpenXML : IDocParser
    {
        WordprocessingDocument package;

        //implements singleton pattern
        private static DocParserDocxOpenXML instance = null;
        private static readonly object padlock = new object();

        public static DocParserDocxOpenXML Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DocParserDocxOpenXML();
                    }
                    return instance;
                }
            }
        }

        DocParserDocxOpenXML()
        {
            
        }

        public string GetText(string docFilePath)
        {
            try
            {
                this.package = WordprocessingDocument.Open(docFilePath, true);

                StringBuilder sb = new StringBuilder();
                OpenXmlElement element = package.MainDocumentPart.Document.Body;
                if (element == null)
                {
                    return string.Empty;
                }

                sb.Append(GetPlainText(element));
                return sb.ToString(); 
            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        private string GetPlainText(OpenXmlElement element)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                        break;


                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(Environment.NewLine);
                        break;


                    // Tab 
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;


                    // Paragraph 
                    case "p":
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;


                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }

            return PlainTextInWord.ToString();
        } 
    }
}
