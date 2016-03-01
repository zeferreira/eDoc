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
        //private string text;

        //public string Text
        //{
        //    get { return text; }
        //    set { text = value; }
        //}

        private int wordID;

        public int WordID
        {
            get { return wordID; }
            set { wordID = value; }
        }

        private int quant;

        public int Quantity
        {
            get { return quant; }
            set { quant = value; }
        }

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

        //public override int GetHashCode()
        //{
        //    return GetHashValue(Text);
        //}

        public static int GetHashValue(string text)
        {
            return text.GetHashCode();
        }
    }
}

/// quanto mais uma palavra aparece em documentos distintos, menos relevante ela é para a pesquisa.
/// Exemplo: artigos: a, o, etc....