using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class RankFunctionBM25 : IRankFunction
    {
        private static RankFunctionBM25 instance = null;
        private static readonly object padlock = new object();

        public static RankFunctionBM25 Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RankFunctionBM25();
                    }
                    return instance;
                }
            }
        }

        RankFunctionBM25()
        { }

        public double CalcRankFactor(WordOccurrenceNode occ, Query query, long totalDocQuantity)
        {
            double queryRank = 0.0;

            int countTermQuery = 0;
            foreach (QueryItem item in query.QueryItens)
            {
                if (item.WordID == occ.Word.WordID)
                {
                    countTermQuery++;
                }
            }
            
            int countTermDoc = occ.Hits.Count;

            double bm25_TF = ((occ.Word.QuantityDocFrequency + 1)*countTermDoc)/(countTermDoc + occ.Word.QuantityDocFrequency);

            double tf = occ.Frequency;
            double idf = Math.Log((((double)totalDocQuantity) + 1) / ((double)occ.Word.QuantityDocFrequency));

            double tf_idf = ((double)countTermQuery) * (bm25_TF) * idf;

            queryRank += tf_idf;

            return queryRank;
        }
    }
}
