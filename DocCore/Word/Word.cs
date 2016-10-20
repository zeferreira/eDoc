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
        //public int WordID
        //{
        //    get { return wordID; }
        //    set { wordID = value; }
        //}

        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private int quant;
        //hits that word have
        public int Quantity
        {
            get { return quant; }
            set { quant = value; }
        }

        private int quantDocFreq;
        //quantity of documents that have this word
        public int QuantityDocFrequency
        {
            get { return quantDocFreq; }
            set { quantDocFreq = value; }
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