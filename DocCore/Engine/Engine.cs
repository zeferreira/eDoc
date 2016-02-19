using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace DocCore
{
    public class Engine
    {
        private Indexer indexer;
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

        public Engine()
        {
            this.engineConfiguration = new EngineConfiguration();
            this.logRep = FactoryRepositoryLog.GetRepositoryLog();
            maxSentence = engineConfiguration.MaxSentence;
            maxResultList = engineConfiguration.MaxResultList;
        }

        public void Load()
        {
            this.indexer = new Indexer();

            this.indexer.Load();
        }

        public void Reindex()
        {
            this.indexer.ReIndexing();
        }

        public List<WordOccurrenceNode> Search(string sentence)
        {
            Hashtable resultHash = new Hashtable();

            List<WordOccurrenceNode> resultList = new List<WordOccurrenceNode>();
            List<Word> wordFound = new List<Word>();

            Query query = new Query(sentence);
            
            for (int i = 0; ((i < query.QueryItens.Count) && (i < maxSentence)); i++)
            {
                Word wf = indexer.Search(query.QueryItens[i].Text);
                
                if(wf != null)
                    wordFound.Add(wf);
            }

            //merging the list.
            foreach (Word item in wordFound)
            {
                WordOccurrenceNode firstOcc = item.FirstOccurrence;
                //problem: the number of occurrences is wrong! The 'else' case, doesn't exist and becaouse this, 
                //the program don't count the occurrences of the second word. 
                //when he merge, it discards the occurrences. 
                if (!resultHash.ContainsKey(firstOcc.Doc.DocID))
                {
                    resultHash.Add(firstOcc.Doc.DocID, firstOcc);
                }

                WordOccurrenceNode tmp = firstOcc;

                while (tmp.HasNext())
                {
                    tmp = tmp.NextOccurrence;

                    if (!resultHash.ContainsKey(tmp.Doc.DocID))
                    {
                        resultHash.Add(tmp.Doc.DocID, tmp);
                        RankCalc(tmp, query);
                    }
                }
            }

            //convert hasthtable to list
            foreach (DictionaryEntry entry in resultHash)
            {
                resultList.Add(entry.Value as WordOccurrenceNode);
            }

            return resultList;
        }

        public List<DocumentResult> Search(string query)
        {
            Hashtable resultHash = new Hashtable();

            List<DocumentResult> resultList = new List<DocumentResult>();
            List<Word> wordFound = new List<Word>();

            Query parsedQuery = new Query(query);

            for (int i = 0; ((i < parsedQuery.QueryItens.Count) && (i < maxSentence)); i++)
            {
                Word wf = indexer.Search(parsedQuery.QueryItens[i].Text);

                if (wf != null)
                    wordFound.Add(wf);
            }

            //merging the list.
            foreach (Word item in wordFound)
            {
                WordOccurrenceNode firstOcc = item.FirstOccurrence;
                //problem: the number of occurrences is wrong! The 'else' case, doesn't exist and becaouse this, 
                //the program don't count the occurrences of the second word. 
                //when he merge, it discards the occurrences. 
                if (!resultHash.ContainsKey(firstOcc.Doc.DocID))
                {
                    DocumentResult newDoc = (DocumentResult)firstOcc.Doc;

                    resultHash.Add(newDoc.DocID, newDoc);
                }

                WordOccurrenceNode tmp = firstOcc;

                while (tmp.HasNext())
                {
                    tmp = tmp.NextOccurrence;

                    if (!resultHash.ContainsKey(tmp.Doc.DocID))
                    {
                        DocumentResult newDoc = (DocumentResult)tmp.Doc;

                        resultHash.Add(newDoc.DocID, newDoc);
                    }
                    else
                    {
                        DocumentResult newDoc = resultHash[tmp.Doc.DocID] as DocumentResult;
                        newDoc.DocQuantitityResults += tmp.Hits.Count;
                    }
                }
            }

            //convert hasthtable to list
            foreach (DictionaryEntry entry in resultHash)
            {
                resultList.Add(entry.Value as DocumentResult);
            }

            return resultList;
        }

        private void RankCalc(WordOccurrenceNode node, Query query)
        {
            double tempRank = node.Frequency - (node.Word.Quantity / this.indexer.TotalWordQuantity);
            node.Rank = tempRank;

            
        }
    }
}
