using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace DocCore
{
    public class EngineMemory : IEngine
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
        private static EngineMemory instance = null;
        private static readonly object padlock = new object();

        public static EngineMemory Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EngineMemory();
                    }
                    return instance;
                }
            }
        }

        EngineMemory()
        {
            this.engineConfiguration = EngineConfiguration.Instance;
            this.logRep = FactoryRepositoryLog.GetRepositoryLog();
            maxSentence = engineConfiguration.MaxSentence;
            maxResultList = engineConfiguration.MaxResultList;
            this.indexer = FactoryIndexer.GetIndexer();
        }

        public void Load()
        {
            this.indexer.Load();
        }

        public void Reindex()
        {
            this.indexer.ReIndexing();
        }


        public List<DocumentResult> Search(string query)
        {
            Hashtable resultHash = new Hashtable();

            List<DocumentResult> resultList = new List<DocumentResult>();

            Query parsedQuery = new Query(query);

            List<Word> wordFound = FindWords(parsedQuery);

            //merging the list.
            foreach (Word item in wordFound)
            {
                WordOccurrenceNode firstOcc = item.FirstOccurrence;
                //problem: the number of occurrences is wrong! The 'else' case, doesn't exist and because this, 
                //the program don't count the occurrences of the second word. 
                //when he merge, it discards the occurrences. 
                if (!resultHash.ContainsKey(firstOcc.Doc.DocID))
                {
                    DocumentResult newDoc = new DocumentResult(firstOcc.Doc);
                    newDoc.CalculateRank(firstOcc, parsedQuery);
                    resultHash.Add(newDoc.DocID, newDoc);
                }
                else
                {
                    DocumentResult newDoc = resultHash[firstOcc.Doc.DocID] as DocumentResult;
                    newDoc.CalculateRank(firstOcc, parsedQuery);
                }

                WordOccurrenceNode tmp = firstOcc;

                while (tmp.HasNext())
                {
                    tmp = tmp.NextOccurrence;

                    if (!resultHash.ContainsKey(tmp.Doc.DocID))
                    {
                        DocumentResult newDoc = new DocumentResult(tmp.Doc);
                        newDoc.CalculateRank(tmp, parsedQuery);
                        resultHash.Add(newDoc.DocID, newDoc);
                    }
                    else
                    {
                        DocumentResult newDoc = resultHash[tmp.Doc.DocID] as DocumentResult;
                        newDoc.CalculateRank(tmp, parsedQuery);
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
                    wordFound.Add(wf);
            }

            return wordFound;
        }
    }
}
