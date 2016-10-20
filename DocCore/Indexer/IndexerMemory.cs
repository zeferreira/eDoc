using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace DocCore
{
    public class IndexerMemory : IIndexer
    {
        private ILexicon lexicon;
        private IDocumentIndex documentIndex;
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

        private static IndexerMemory instance = null;
        private static readonly object padlock = new object();

        public static IndexerMemory Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new IndexerMemory();
                    }
                    return instance;
                }
            }
        }

        IndexerMemory()
        {
            this.lexicon = FactoryLexicon.GetLexicon();
            this.documentIndex = FactoryDocumentIndex.GetDocumentIndex();
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

        public Word Search(int wordID)
        {
            return this.lexicon.GetWord( ref wordID);
        }

        //parallel linked list (memory)
        public void Index(List<Document> listOfDocs)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            Parallel.ForEach(listOfDocs, (docItem) =>
            {
                try
                {
                    Hashtable postingList = docItem.GetPostingList();
                    IDictionaryEnumerator iDicE = postingList.GetEnumerator();

                    this.documentIndex.Insert(docItem);

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
                                occurrence.Word.FirstOccurrence = occurrence;
                                occurrence.Word.LastOccurrence = occurrence;
                                occurrence.Word.Quantity = occurrence.Hits.Count;
                                lexicon.AddNewWord(occurrence.Word);
                                occurrence.Word.QuantityDocFrequency++;
                            }
                            else
                            {
                                //to do: memory allocation alert!! remove ref!! use newnode.Word.text or something, don't pass WordID as a parameter!! Performance is poor.
                                occurrence.Word = lexicon.GetWord(ref occurrence.Word.WordID);

                                occurrence.PreviousOccurrence = occurrence.Word.LastOccurrence;
                                occurrence.Word.LastOccurrence.NextOccurrence = occurrence;
                                occurrence.Word.LastOccurrence = occurrence;
                                occurrence.Word.QuantityDocFrequency++;
                                occurrence.Word.Quantity += occurrence.Hits.Count;
                            }

                            totalWordQuantity += occurrence.Hits.Count;
                        }
                    }

                    GC.ReRegisterForFinalize(postingList);
                    GC.ReRegisterForFinalize(iDicE);
                    GC.Collect();
                }
                catch (Exception e)
                {
                    Log entry = new Log();
                    entry.TaskDescription = "Read pdf file error";
                    entry.LogParameters = new List<string>();
                    entry.LogParameters.Add("FileName: " + docItem.File);
                    entry.LogParameters.Add("Error: " + e.Message);

                    repLog.Write(entry);
                    
                }

            });
        }

     }
}
