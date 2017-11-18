using System;
using System.Collections.Generic;
using System.Text;
using DocCore;

namespace CrawlerCore
{
    public class FactoryRepositoryDocumentCandidate
    {
        public static IRepositoryDocumentCandidate GetRepositoryDocumentCandidate()
        {
            string type = EngineConfiguration.Instance.RepositoryDocumentCandidateType;
            
            string pathFile = EngineConfiguration.Instance.PathRepositoryDocumentCandidateFile;

            switch (type)
            {
                case "txt":
                    return new RepositoryDocumentCandidateTXT(pathFile);
                    
                case "xml":
                    return new RepositoryDocumentCandidateXML(pathFile);

                case "btree":
                    return new RepositoryDocumentCandidateBtree(pathFile);
                    
                default:
                    throw new NotImplementedException(Messages.RepositoryDocumentCandidateNotImplemented);
                    
            }
        }

    }
}
