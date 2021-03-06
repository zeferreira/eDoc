﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;


namespace DocCore
{
    public class EngineDisk : IEngine
    {
        private IIndexer indexer;
        private IInvertedFile invertedFile;
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
        private static EngineDisk instance = null;
        private static readonly object padlock = new object();

        public static EngineDisk Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EngineDisk();
                    }
                    return instance;
                }
            }
        }

        EngineDisk()
        {
            this.engineConfiguration = EngineConfiguration.Instance;
            this.logRep = FactoryRepositoryLog.GetRepositoryLog();
            maxSentence = engineConfiguration.MaxSentence;
            maxResultList = engineConfiguration.MaxResultList;
            this.indexer = FactoryIndexer.GetIndexer();
            this.invertedFile = FactoryInvertedFile.GetInvertedFile();
        }

        public void Load()
        {
            this.indexer.Load();
        }

        public void Reindex()
        {
            this.indexer.ReIndexing();
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

        public List<DocumentResult> Search(string query)
        {
            Hashtable resultHash = new Hashtable();

            List<DocumentResult> resultList = new List<DocumentResult>();

            Query parsedQuery = new Query(query);

            List<Word> wordFound = FindWords(parsedQuery);

            //merging the list.
            foreach (Word item in wordFound)
            {
                List<WordOccurrenceNode> tempDocList = invertedFile.GetWordOccurrencies(item);

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
    }
}
