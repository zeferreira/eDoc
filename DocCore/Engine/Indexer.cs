using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DocCore
{
    public class Indexer
    {
        private Lexicon lexicon;
        private IRepositoryDocument repDoc;

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

        public Indexer()
        {
            this.lexicon = new Lexicon();
            this.repDoc = FactoryRepositoryDocument.GetRepositoryDocument(EnumRepositoryType.Folder);
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

        public void Index(List<Document> listOfDocs)
        {
            foreach (Document docItem in listOfDocs)
            {
                Hashtable postingList = docItem.GetPostingList();
                IDictionaryEnumerator iDicE = postingList.GetEnumerator();

                while (iDicE.MoveNext())
                {
                    //get posting list and add hits
                    //never reindex the same document 2 times.

                    DictionaryEntry dicEntry = (DictionaryEntry)iDicE.Current;
                    WordOccurrenceNode occurrence = dicEntry.Value as WordOccurrenceNode;

                    lexicon.AddWordOccurrence(occurrence);
                    totalWordQuantity += occurrence.Hits.Count;
                }
            }
        }

        private void IndexOneFile(Document doc)
        {
            
        }
    }
}
