using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DocCore
{
    class Indexer
    {
        private long totalWordQuantity;

        public long TotalWordQuantity
        {
            get { return totalWordQuantity; }
        }

        private long totalDocumentQuantity;

        public long TotalDocumentQuantity
        {
            get { return totalDocumentQuantity; }
        }

        private Lexicon lexicon;
        private IRepositoryDocument repDoc;

        public Indexer()
        {
            this.lexicon = new Lexicon();
            this.repDoc = FactoryRepositoryDocument.GetRepositoryDocument(EnumRepositoryType.TXT);
        }

        public void ReIndexing()
        {
            List<Document> listOfDocs = repDoc.Search(false);
            this.totalDocumentQuantity += listOfDocs.Count;

            Index(listOfDocs);
        }

        public void Load()
        {
            List<Document> listOfDocs = repDoc.Search(true);
            this.totalDocumentQuantity += listOfDocs.Count;
            Index(listOfDocs);
        }

        public Word Search(string word)
        {
            return this.lexicon.GetWord(word);
        }

        void Index(List<Document> listOfDocs)
        {
            foreach (Document docItem in listOfDocs)
            {
                Hashtable postingList = docItem.GetPostingList();

                foreach (DictionaryEntry dicEntry in postingList)
                {
                    //get posting list and add hits
                    //never reindex the same document 2 times.

                    WordOccurrenceNode occurrence = dicEntry.Value as WordOccurrenceNode;

                    lexicon.AddWordOccurrence(occurrence);
                    this.totalWordQuantity += occurrence.Hits.Count;
                }
            }
        }
    }
}
