using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
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
        
        /// <summary>
        /// Total of hits of that word 
        /// </summary>
        public int QuantityHits
        {
            get { return quant; }
            set { quant = value; }
        }

        private int quantDocFreq;
        /// <summary>
        /// Quantity of documents that have this word in the collection
        /// </summary>
        public int QuantityDocFrequency
        {
            get { return quantDocFreq; }
            set { quantDocFreq = value; }
        }

        public static int GetHashValue(string text)
        {
            return HashStringNet(text);
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

        long startPositionInvertedFile;

        public long StartPositionInvertedFile
        {
            get { return startPositionInvertedFile; }
            set { startPositionInvertedFile = value; }
        }

        long endPositionInvertedFile;

        public long EndPositionInvertedFile
        {
            get { return endPositionInvertedFile; }
            set { endPositionInvertedFile = value; }
        }

        private static int HashString(string text)
        {
            // TODO: Determine nullity policy.

            unchecked
            {
                int hash = 23;
                foreach (char c in text)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecuritySafeCritical]
        public unsafe static int HashStringNet(string strToHash)
        {
            fixed (char* str = strToHash)
            {
                char* chPtr = str;
                int num = 352654597;
                int num2 = num;
                int* numPtr = (int*)chPtr;
                for (int i = strToHash.Length; i > 0; i -= 4)
                {
                    num = (((num << 5) + num) + (num >> 27)) ^ numPtr[0];
                    if (i <= 2)
                    {
                        break;
                    }
                    num2 = (((num2 << 5) + num2) + (num2 >> 27)) ^ numPtr[1];
                    numPtr += 2;
                }
                return (num + (num2 * 1566083941));
            }
        }
    }
}

/// quanto mais uma palavra aparece em documentos distintos, menos relevante ela é para a pesquisa.
/// Exemplo: artigos: a, o, etc....