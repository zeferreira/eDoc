using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryIndexer
    {
        public static IIndexer GetIndexer()
        {
            EngineConfiguration engConf = EngineConfiguration.Instance;
            string path = engConf.LogFilePath;

            string type = engConf.EngineType;

            switch (type)
            {
                case "memory":
                    return IndexerMemory.Instance;

                case "disk":
                    return IndexerDisk.Instance;

                default:
                    throw new NotImplementedException(Messages.IndexTypeNotImplemented);

            }
 
        }
    }
}
