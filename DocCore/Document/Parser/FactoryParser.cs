using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DocCore
{
    public class FactoryParser
    {
        public static IDocParser GetParser(string file)
        {
            string extension = Path.GetExtension(file);

            switch (extension.ToLower())
            {
                case ".txt":
                    return DocParserTxt.Instance;

                case ".pdf":
                    return DocParserPdf.Instance;
                
                case ".docx":
                    return DocParserDocxOpenXML.Instance;

                case ".doc":
                    return DocParserDocxInterop.Instance;

                default:
                    {
                        string message = Messages.DocParserNotSupportedFile + @" (File: " + file + ")";

                        Log l = new Log();
                        l.TaskDescription = "Parsing document - Not Supported";
                        l.StartDateTime = DateTime.Now;
                        l.LogParameters.Add(file);

                        IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();
                        repLog.Write(l);

                        throw new NotSupportedException();
                    }
            }
        }
    }
}
