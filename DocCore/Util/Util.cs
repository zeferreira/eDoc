using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace DocCore
{
    public class Util
    {
        public static string RemoveForbbidenSymbols(string text)
        {
            //sometimes you must or not index the stop words and symbols. Think about it.
            //A heuristic query analisys maybe it is a good solution.
            //By now, index all terms is a better solution for me.

            return RemoveDiacritics( RemoveVisibleSymbols(RemoveInvisibleSymbols(text)));
        }

        private static string RemoveVisibleSymbols(string text)
        {
            string replaced = text.Replace('.', ' ');
            replaced = replaced.Replace(',', ' ');
            replaced = replaced.Replace('(', ' ');
            replaced = replaced.Replace(')', ' ');
            replaced = replaced.Replace('[', ' ');
            replaced = replaced.Replace(']', ' ');

            replaced = replaced.Replace('{', ' ');
            replaced = replaced.Replace('}', ' ');
            replaced = replaced.Replace(';', ' ');
            replaced = replaced.Replace('#', ' ');
            replaced = replaced.Replace('|', ' ');

            return replaced;
        }

        private static string RemoveInvisibleSymbols(string text)
        {
            string replaced = text.Replace('\n', ' ');
            replaced = replaced.Replace('\t', ' ');

            return replaced;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
