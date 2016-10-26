using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DocCore
{
    public class EngineConfiguration
    {
        int maxSentence;
        long maxResultList;

        //implements singleton pattern
        private static EngineConfiguration instance = null;
        private static readonly object padlock = new object();

        public static EngineConfiguration Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EngineConfiguration();
                    }
                    return instance;
                }
            }
        }

        EngineConfiguration()
        { }

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

        public string PathEvaluationLog
        {
            get
            {
                this.pathFolderRepository = ConfigurationManager.AppSettings["pathEvaluationLog"] as string;
                return this.pathFolderRepository;
            }
        }


        string pathFolderIndex;

        public string PathFolderIndex
        {
            get
            {
                this.pathFolderIndex = ConfigurationManager.AppSettings["pathFolderIndex"] as string;
                return this.pathFolderIndex;
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

        public long MaxResultList
        {
            get
            {
                this.maxResultList = Convert.ToInt64(ConfigurationManager.AppSettings["maxResultList"].ToString());
                return maxResultList;
            }
        }


        string documentIndexType;

        public string DocumentIndexType
        {
            get
            {
                this.documentIndexType = ConfigurationManager.AppSettings["documentIndexType"] as string;
                return documentIndexType.ToLower();
            }
        }

        string lexiconType;

        public string LexiconType
        {
            get
            {
                this.lexiconType = ConfigurationManager.AppSettings["lexiconType"] as string;
                return lexiconType.ToLower();
            }
        }

        string invertedFileType;

        public string InvertedFileType
        {
            get
            {
                this.lexiconType = ConfigurationManager.AppSettings["invertedFileType"] as string;
                return lexiconType.ToLower();
            }
        }

        private string engineType;

        public string EngineType
        {
            get
            {
                this.engineType = ConfigurationManager.AppSettings["engineType"] as string;
                return engineType.ToLower();
            }
        }

        
        private string rankTypeFunction;
        public string RankTypeFunction
        {
            get
            {
                this.rankTypeFunction = ConfigurationManager.AppSettings["rankTypeFunction"] as string;
                return rankTypeFunction.ToLower();
            }
        }

        private string bNormalizationfactor;
        public double BNormalizationfactor
        {
            get
            {
                this.bNormalizationfactor = ConfigurationManager.AppSettings["bNormalizationfactor"] as string;
                return Convert.ToDouble(bNormalizationfactor.ToLower());
            }
        }

        private string bm25OkapiK1factor;
        public double BM25OkapiK1factor
        {
            get
            {
                this.bm25OkapiK1factor = ConfigurationManager.AppSettings["bm25OkapiK1factor"] as string;
                return Convert.ToDouble(bm25OkapiK1factor.ToLower());
            }
        }


        private string bm25OkapiK3factor;
        public double BM25OkapiK3factor
        {
            get
            {
                this.bm25OkapiK3factor = ConfigurationManager.AppSettings["bm25OkapiK3factor"] as string;
                return Convert.ToDouble(bm25OkapiK3factor.ToLower());
            }
        }

        private string sNormalizationfactor;
        public double SNormalizationfactor
        {
            get
            {
                this.sNormalizationfactor = ConfigurationManager.AppSettings["sNormalizationfactor"] as string;
                return Convert.ToDouble(sNormalizationfactor.ToLower());
            }
        }
    }
}
