using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryRepositoryDocument
    {
        public static IRepositoryDocument GetRepositoryDocument()
        {
            string path = (EngineConfiguration.Instance).PathFolderRepository;
            string type = (EngineConfiguration.Instance).RepositoryDocumentType;

            switch (type)
            {
                case "folder":
                    return new RepositoryDocumentFolder(path);
                    
                case "btree":
                    return RepositoryDocumentBplusTree.Instance;
                    
                default:
                    throw new NotImplementedException(Messages.RepositoryDocumentNotImplemented);
            }
        }

    }
}
