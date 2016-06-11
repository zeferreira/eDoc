using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class FactoryLexicon
    {
        public static ILexicon GetLexicon()
        {
            string type = (EngineConfiguration.Instance).LexiconType.ToLower();

            switch (type)
            {
                case "hashtable":
                    return LexiconHashtable.Instance;

                default:
                    throw new NotImplementedException(Messages.LexiconTypeNotImplemented);

            }
        }
    }
}
