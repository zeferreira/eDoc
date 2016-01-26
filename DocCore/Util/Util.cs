using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class Util
    {
        public static string RemoveForbbidenSymbols(string text)
        {
            string replaced = text.Replace('.', ' ');
            replaced = text.Replace('\n', ' ');
            replaced = text.Replace('\t', ' ');
            replaced = text.Replace(',', ' ');
            replaced = text.Replace('(', ' ');
            replaced = text.Replace(')', ' ');
            replaced = text.Replace('[', ' ');
            replaced = text.Replace(']', ' ');

            replaced = text.Replace('{', ' ');
            replaced = text.Replace('}', ' ');
            replaced = text.Replace(';', ' ');

            return replaced;
        }
    }
}
