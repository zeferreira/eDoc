using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class QueryParser
    {
        public static string GetCleanQuery(string sentence)
        {
            string replaced = Useful.RemoveForbbidenSymbols(sentence);

            return replaced.ToLower();
        }
    }
}
