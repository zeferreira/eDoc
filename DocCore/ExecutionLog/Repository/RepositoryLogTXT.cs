using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DocCore
{
    public class RepositoryLogTXT : IRepositoryLog
    {
        string logFilePath;
        char separator;
        char separatorParameters;
        bool activeStatus;


        public RepositoryLogTXT(string logFileName, char separator, char separatorParm, bool isActive)
        {
            this.logFilePath = logFileName;
            this.separator = separator;
            this.separatorParameters = separatorParm;
            this.activeStatus = isActive;
        }

        public void Write(Log entry)
        {
            if(this.activeStatus)
                File.AppendAllText(logFilePath, entry.ToString());
        }
        
        public List<Log> List()
        {
            List<Log> result = new List<Log>();
            
            string text = System.IO.File.ReadAllText(logFilePath);

            string[] logEntrysTxtLines = text.Split(Environment.NewLine.ToCharArray());

            foreach (string item in logEntrysTxtLines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    Log entry = new Log();
                    string[] properties = item.Split(separator);

                    entry.TaskDescription = properties[0].ToString();
                    entry.StartDateTime = Convert.ToDateTime(properties[1].ToString());

                    int days = Convert.ToInt32(properties[2].ToString());
                    int hours = Convert.ToInt32(properties[3].ToString());
                    int min = Convert.ToInt32(properties[4].ToString());

                    int seconds = Convert.ToInt32(properties[5].ToString());
                    int milliseconds = Convert.ToInt32(properties[6].ToString());

                    entry.ExecutionTime = new TimeSpan(days, hours, min, seconds, milliseconds);

                    string[] logParameters = properties[7].ToString().Split(separatorParameters);

                    List<string> parameters = new List<string>();

                    foreach (string parm in logParameters)
                    {
                        parameters.Add(parm);
                    }

                    entry.LogParameters = parameters;
                    result.Add(entry);
                }
            }

            return result;
        }
    }
}
