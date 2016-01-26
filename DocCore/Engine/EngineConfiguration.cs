using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DocCore
{
    public class EngineConfiguration
    {
        int maxSentence;

        public int MaxSentence
        {
            get 
            {
                this.maxSentence = Convert.ToInt32(ConfigurationManager.AppSettings["maxSentence"].ToString());
                return maxSentence;
            }
        }
        
        string pathFolderRepository;

        public string PathFolderRepository
        {
            get { 
                    this.pathFolderRepository = ConfigurationManager.AppSettings["pathFolderRepository"] as string;
                    return this.pathFolderRepository;
                }
        }

        char logSeparator;

        public char LogSeparator
        {
            get 
            {
                logSeparator = Convert.ToChar(ConfigurationManager.AppSettings["logSeparator"].ToString());
                return logSeparator; 
            }

        }

        bool logIsActive;

        public bool LogIsActive
        {
            get 
            {
                logIsActive = Convert.ToBoolean(ConfigurationManager.AppSettings["logIsActive"].ToString().ToLower());
                return logIsActive; 
            }
        }

        string logFilePath;

        public string LogFilePath
        {
            get 
            {
                this.logFilePath = ConfigurationManager.AppSettings["logFilePath"] as string;
                return logFilePath; 
            }
        }

        string logType;

        public string LogType
        {
            get
            {
                this.logType = ConfigurationManager.AppSettings["logType"] as string;
                return logType.ToLower();
            }
        }



    }
}
