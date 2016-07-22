using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    /// <summary>
    /// Word is any thing between spaces
    /// </summary>
    public class Word
    {
        public int WordID;

        private int quant;

        public int Quantity
        {
            get { return quant; }
            set { quant = value; }
        }

        public static int GetHashValue(string text)
        {
            return text.GetHashCode();
        }

        string invertedFile;



        private WordOccurrenceNode firstOccurrence;

        public WordOccurrenceNode FirstOccurrence
        {
            get { return firstOccurrence; }
            set { firstOccurrence = value; }
        }

        private WordOccurrenceNode lastOccurrence;

        public WordOccurrenceNode LastOccurrence
        {
            get { return lastOccurrence; }
            set { lastOccurrence = value; }
        }


    }
}

/// quanto mais uma palavra aparece em documentos distintos, menos relevante ela é para a pesquisa.
/// Exemplo: artigos: a, o, etc....