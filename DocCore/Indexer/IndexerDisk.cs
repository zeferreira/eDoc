using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace DocCore
{
    public class IndexerDisk : IIndexer
    {
        private ILexicon lexicon;
        private IRepositoryDocument repDoc;
        private IInvertedFile invertedFile;

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

        private static IndexerDisk instance = null;
        private static readonly object padlock = new object();

        public static IndexerDisk Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new IndexerDisk();
                    }
                    return instance;
                }
            }
        }

        IndexerDisk()
        {
            this.lexicon = FactoryLexicon.GetLexicon();
            this.repDoc = FactoryRepositoryDocument.GetRepositoryDocument();
            this.invertedFile = FactoryInvertedFile.GetInvertedFile();
        }

        public void ReIndexing()
        {
            List<Document> listOfDocs = repDoc.List();
            this.totalDocumentQuantity += listOfDocs.Count;

            Index(listOfDocs);
        }

        public void Load()
        {
            List<Document> listOfDocs = repDoc.List();
            this.totalDocumentQuantity += listOfDocs.Count;
            //this.lexicon.LoadFromStorage();
            Index(listOfDocs);
        }

        public Word Search(int wordID)
        {
            return this.lexicon.GetWord( ref wordID);
        }

        //index to disk
        public void Index(List<Document> listOfDocs)
        {
            Parallel.ForEach(listOfDocs, (docItem) =>
            {
                Hashtable postingList = docItem.GetPostingListClass();
                IDictionaryEnumerator iDicE = postingList.GetEnumerator();

                while (iDicE.MoveNext())
                {
                    //get posting list and add hits
                    //never reindex the same document 2 times.

                    DictionaryEntry dicEntry = (DictionaryEntry)iDicE.Current;
                    WordOccurrenceNode occurrence = dicEntry.Value as WordOccurrenceNode;
                    lock (this)
                    {
                        //Do not has any word
                        if (!lexicon.HasWord(ref occurrence.Word.WordID))
                        {
                            lexicon.AddNewWord(occurrence.Word);
                            occurrence.Word.QuantityHits = occurrence.Hits.Count;
                            occurrence.Word.QuantityDocFrequency++;
                            invertedFile.AddWordOccurrence(occurrence);
                        }
                        else
                        {
                            //to do: memory allocation alert!! remove ref!! use newnode.Word.text or something, don't pass WordID as a parameter!! Performance is poor.
                            occurrence.Word = lexicon.GetWord(ref occurrence.Word.WordID);
                            occurrence.Word.QuantityHits += occurrence.Hits.Count;
                            occurrence.Word.QuantityDocFrequency++;
                            invertedFile.AddWordOccurrence(occurrence);
                        }

                        totalWordQuantity += occurrence.Hits.Count;
                    }
                }

                GC.ReRegisterForFinalize(postingList);
                GC.ReRegisterForFinalize(iDicE);
                GC.Collect();
            });

            this.invertedFile.WriteToStorage();
            this.lexicon.WriteToStorage();
        }
     }
}
