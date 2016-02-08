using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryRepositoryDocument
    {
        public static IRepositoryDocument GetRepositoryDocument(EnumRepositoryType type)
        {
            string path = (new EngineConfiguration()).PathFolderRepository;

            switch (type)
            {
                case EnumRepositoryType.Folder:
                    return new RepositoryDocumentFolder(path);
                    
                case EnumRepositoryType.SQL:
                    throw new NotImplementedException();
                    
                default:
                    throw new NotImplementedException(Messages.RepositoryDocumentNotImplemented);
                    
            }
        }

    }
}
