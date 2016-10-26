using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace DocCore
{
    public sealed class DocumentIndexHashTable : IDocumentIndex
    {
        Hashtable ht;

        public long Quantity
        {
            get { return ht.Count; }
        }

        private long totalLenghtAccumulator;
        private double averageDocLenght;


        private static DocumentIndexHashTable instance = null;
        private static readonly object padlock = new object();

        public static DocumentIndexHashTable Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DocumentIndexHashTable();
                    }
                    return instance;
                }
            }
        }

        DocumentIndexHashTable()
        {
            this.ht = new Hashtable();
        }

        public void Insert(Document doc)
        {
            this.ht[doc.DocID] = doc;
            this.totalLenghtAccumulator += doc.WordQuantity;
        }

        public Document Search(int docID)
        {
            return this.ht[docID] as Document;
        }

        public void Delete(int docID)
        {
            this.ht.Remove(docID);
        }

        public long GetQuantity()
        {
            return this.Quantity;
        }

        /// <summary>
        /// Used to document normalization
        /// </summary>
        public double GetAverageDocumentLenght()
        {
            this.averageDocLenght = this.totalLenghtAccumulator / this.GetQuantity();

            return averageDocLenght;
        }


    }
}
