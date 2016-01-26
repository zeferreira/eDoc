using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class Log
    {
        char separator;
        char separatorParameters;
        EngineConfiguration engConf;

        string taskDescription;

        public string TaskDescription
        {
            get { return taskDescription; }
            set { taskDescription = value; }
        }

        List<string> logParameters;

        public List<string> LogParameters
        {
            get { return logParameters; }
            set { logParameters = value; }
        }

        TimeSpan executionTime;

        public TimeSpan ExecutionTime
        {
            get 
            { 
                return executionTime; 
            }
            set { executionTime = value; }
        }

        DateTime startDateTime;

        public DateTime StartDateTime
        {
            get { return startDateTime; }
            set { startDateTime = value; }
        }

        public Log()
        {
            this.engConf = new EngineConfiguration();
            this.separator = engConf.LogSeparator;
            this.separatorParameters = '#';
        }

        public override string ToString()
        {
            string logParameters = "";

            foreach (string item in this.LogParameters)
            {
                logParameters += item + separatorParameters.ToString();
            }

            string result = 
                this.TaskDescription + separator.ToString() +
                this.StartDateTime + separator.ToString() +
                
                this.ExecutionTime.Days + separator.ToString() +
                this.ExecutionTime.Hours + separator.ToString() +
                this.ExecutionTime.Minutes + separator.ToString() +
                this.ExecutionTime.Seconds + separator.ToString() +
                this.ExecutionTime.Milliseconds + separator.ToString() +

                logParameters +
                 Environment.NewLine;

            return result;
        }

    }
}
