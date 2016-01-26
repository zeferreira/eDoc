using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class SentenceParser
    {
        public static string GetCleanSentence(string sentence)
        {
            string replaced = Util.RemoveForbbidenSymbols(sentence);

            return replaced.ToLower();
        }
    }
}
