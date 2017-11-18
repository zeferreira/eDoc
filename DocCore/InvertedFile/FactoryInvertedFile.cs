using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryInvertedFile
    {
        public static IInvertedFile GetInvertedFile()
        {
            EngineConfiguration engConf = EngineConfiguration.Instance;
            string type = engConf.EngineType;

            switch (type)
            {
                case "memory":
                    return InvertedFileMemory.Instance;

                case "disk":
                    return InvertedFileDisk.Instance;

                default:
                    throw new NotImplementedException(Messages.RepositoryLogNotImplemented);
            }
        }
    }
}
