using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace DocCore
{
    public class Useful
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
            replaced = replaced.Replace(':', ' ');
            replaced = replaced.Replace('#', ' ');
            replaced = replaced.Replace('|', ' ');
            replaced = replaced.Replace('_', ' ');

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

        public static string GetFormatedSizeString(long bytesSize)
        {
            if (bytesSize > 1024)
            {
                long kbSize = bytesSize / 1024;

                if (kbSize > 1024)
                {
                    long mbSize = kbSize / 1024;

                    if (mbSize > 1024)
                    {
                        long gbSize = mbSize / 1024;

                        if (gbSize > 1024)
                        {
                            long tbSize = gbSize / 1024;
                            return tbSize.ToString() + "(TB's)";
                        }
                        else
                        {
                            return gbSize.ToString() + "(GB's)";
                        }
                    }
                    else
                    {
                        return mbSize.ToString() + "(MB's)";
                    }
                }
                else
                {
                    return kbSize.ToString() + "(KB's)";
                }

            }
            else
            {
                return bytesSize.ToString() + "(B's)";
            }
        }

    }
}
