using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;

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
            replaced = replaced.Replace('-', ' ');

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


        public static float GetTotalMemoryProcessInGB()
        {
            Process currentProc = Process.GetCurrentProcess();
            long bytesInMemory = currentProc.PrivateMemorySize64;

            float resultInGiga = (float)(((float)bytesInMemory) / 1024.0 / 1024.0 / 1024.0);

            return resultInGiga;
        }


        public static string FormatUrlToFileName(string url)
        {
            return url.Replace('/', '#').Replace(':', '_');
        }

        public static string FormatFileNameToUrl(string fileName)
        {
            return fileName.Replace('#', '/').Replace('_', ':');
        }

        public static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                CultureInfo n = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentCulture = n;
                Thread.CurrentThread.CurrentUICulture = n;

                var serializer = new XmlSerializer(typeof(T));
                var stringwriter = new System.IO.StringWriter();

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                var xmlWriter = XmlWriter.Create(stringwriter, new XmlWriterSettings() { OmitXmlDeclaration = true });

                serializer.Serialize(xmlWriter, dataToSerialize, ns);

                return stringwriter.ToString();
            }
            catch
            {
                throw;
            }
        }

        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                CultureInfo n = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentCulture = n;
                Thread.CurrentThread.CurrentUICulture = n;

                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch
            {
                throw;
            }
        }
    }
}
