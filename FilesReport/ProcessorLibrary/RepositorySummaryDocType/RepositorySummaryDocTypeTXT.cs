using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FilesReport
{
    public class RepositorySummaryDocTypeTXT : IRepositorySummaryDocType
    {
        string reportFilePath;

        public string ReportFilePath
        {
            get { return reportFilePath; }
        }

        char separator;

        public RepositorySummaryDocTypeTXT(string reportFolder, char separator)
        {
            this.reportFilePath = reportFolder;
            this.separator = separator;

            File.AppendAllText(this.reportFilePath,
                        "FileExtension" + "|" +
                        "TotalQuantity" + "|" +
                        "TotalSize" + "|" +
                        "MaxSizeFile" + "|" +
                        "OlderFile" + "|" +
                        "RecentFile" + Environment.NewLine
                        );
        }

        public void Write(SummaryDocType entry)
        {
            File.AppendAllText(reportFilePath, entry.ToString());
        }

        public List<SummaryDocType> List()
        {
            List<SummaryDocType> result = new List<SummaryDocType>();
            
            string text = System.IO.File.ReadAllText(reportFilePath);

            string[] logEntrysTxtLines = text.Split(Environment.NewLine.ToCharArray());

            foreach (string item in logEntrysTxtLines)
            {
                if ((!string.IsNullOrEmpty(item)) & (!item.Contains(logEntrysTxtLines[0])))
                {
                    string[] properties = item.Split(separator);

                    SummaryDocType entry = new SummaryDocType(properties[0].ToString());
                    
                    entry.TotalQuantity = Convert.ToInt64(properties[1].ToString());
                    entry.TotalSize = Convert.ToInt64(properties[2].ToString());
                    entry.MaxSizeFile = Convert.ToInt64(properties[3].ToString());
                    entry.OlderFile = Convert.ToDateTime(properties[4].ToString());
                    entry.RecentFile = Convert.ToDateTime(properties[5].ToString());
                
                    result.Add(entry);
                }
            }

            return result;
        }
    }
}
