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
                    return new DocParserTXT();

                case ".pdf":
                    return new DocParserPDF();
                
                case ".docx":
                    return new DocParserDOCX();

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
