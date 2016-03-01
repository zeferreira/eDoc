using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryRepositoryLog
    {


        public static IRepositoryLog GetRepositoryLog()
        {
            EngineConfiguration engConf = EngineConfiguration.Instance;
            string path = engConf.LogFilePath;

            string type = engConf.LogType;

            switch (type)
            {
                case "txt":
                    return new RepositoryLogTXT(path, engConf.LogSeparator,'#',engConf.LogIsActive);

                case "sql":
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException(Messages.RepositoryLogNotImplemented);

            }
 
        }
    }
}
