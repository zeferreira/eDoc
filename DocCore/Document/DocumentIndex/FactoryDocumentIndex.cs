using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryDocumentIndex
    {
        public static IDocumentIndex GetDocumentIndex()
        {
            string type = EngineConfiguration.Instance.DocumentIndexType;

            switch (type)
            {
                case "hashtable":
                    return DocumentIndexHashTable.Instance;

                default:
                    throw new NotImplementedException(Messages.DocumentIndexNotImplemented);

            }
        }
    }
}
