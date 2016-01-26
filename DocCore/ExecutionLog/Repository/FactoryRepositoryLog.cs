using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryRepositoryLog
    {


        public static IRepositoryLog GetRepositoryLog()
        {
            EngineConfiguration engConf = new EngineConfiguration();
            string path = (new EngineConfiguration()).LogFilePath;

            string type = engConf.LogType;

            switch (type)
            {
                case "txt":
                    return new RepositoryLogTXT("ExecutionLog.txt");

                case "sql":
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException(Messages.RepositoryLogNotImplemented);

            }
 
        }
    }
}
