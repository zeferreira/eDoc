using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryEngine
    {
        public static IEngine GetEngine()
        {
            EngineConfiguration engConf = EngineConfiguration.Instance;
            string path = engConf.LogFilePath;

            string type = engConf.EngineType;

            switch (type)
            {
                case "memory":
                    return EngineMemory.Instance;

                case "disk":
                    return EngineDisk.Instance;

                default:
                    throw new NotImplementedException(Messages.RepositoryLogNotImplemented);

            }
 
        }
    }
}
