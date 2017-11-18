using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;


namespace DocCore
{
    public class EngineSPIMI : IEngine
    {
        private IIndexer indexer;
        private EngineConfiguration engineConfiguration;
        private IRepositoryLog logRep;
        int maxSentence;
        long maxResultList;

        public long TotalDocumentQuantity 
        { 
            get{
                return this.indexer.TotalDocumentQuantity;
            }
        }

        public long TotalWordQuantity 
        { 
            get{
                return this.indexer.TotalWordQuantity;
            }
        }

        //implements singleton pattern
        private static EngineSPIMI instance = null;
        private static readonly object padlock = new object();

        public static EngineSPIMI Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EngineSPIMI();
                    }
                    return instance;
                }
            }
        }

        EngineSPIMI()
        {
            this.engineConfiguration = EngineConfiguration.Instance;
            this.logRep = FactoryRepositoryLog.GetRepositoryLog();
            maxSentence = engineConfiguration.MaxSentence;
            maxResultList = engineConfiguration.MaxResultList;
            this.indexer = FactoryIndexer.GetIndexer();

            //for debug
            //TextWriterTraceListener traceListener = new TextWriterTraceListener(System.IO.File.CreateText(EngineConfiguration.Instance.TraceDebugFile));
            //Debug.Listeners.Add(traceListener);
        }

        public void Load()
        {
            this.indexer.Load();
        }

        public void Reindex()
        {
            ((IndexerSPIMI)this.indexer).Index();
        }

        public List<DocumentResult> Search(string query)
        {
            Hashtable resultHash = new Hashtable();

            List<DocumentResult> resultList = new List<DocumentResult>();

            Query parsedQuery = new Query(query);

            List<Word> wordFound = FindWords(parsedQuery);
            //for debug
            //foreach (Word item in wordFound)
            //{
            //    Debug.WriteLine("WordID: " + item.WordID + " Start: " + item.StartPositionInvertedFile + " End: " + item.EndPositionInvertedFile);
            //}

            //merging the list.
            foreach (Word item in wordFound)
            {
                List<WordOccurrenceNode> tempDocList = ((IndexerSPIMI)indexer).GetWordOccurrencies(item);

                foreach (WordOccurrenceNode wordOccur in tempDocList)
                {
                    if (!resultHash.ContainsKey(wordOccur.Doc.DocID))
                    {
                        DocumentResult newDoc = new DocumentResult(wordOccur.Doc);
                        newDoc.CalculateRank(wordOccur, parsedQuery);
                        resultHash.Add(newDoc.DocID, newDoc);
                    }
                    else
                    {
                        DocumentResult newDoc = resultHash[wordOccur.Doc.DocID] as DocumentResult;
                        newDoc.CalculateRank(wordOccur, parsedQuery);
                    }
                }
            }

            //convert hasthtable to list
            foreach (DictionaryEntry entry in resultHash)
            {
                DocumentResult doc = entry.Value as DocumentResult;
                resultList.Add(doc);
            }

            //sort result list by QueryRank and return
            resultList.Sort((y, x) => x.QueryRank.CompareTo(y.QueryRank));

            return resultList;
        }

        private List<Word> FindWords(Query parsedQuery)
        {
            List<Word> wordFound = new List<Word>();

            for (int i = 0; ((i < parsedQuery.QueryItens.Count) && (i < maxSentence)); i++)
            {
                Word wf = indexer.Search(parsedQuery.QueryItens[i].WordID);

                if (wf != null)
                {
                    wordFound.Add(wf);
                }

            }

            return wordFound;
        }

    }
}
