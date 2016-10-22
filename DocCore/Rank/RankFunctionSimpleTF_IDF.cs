using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class RankFunctionSimpleTF_IDF : IRankFunction
    {
        private static RankFunctionSimpleTF_IDF instance = null;
        private static readonly object padlock = new object();

        public static RankFunctionSimpleTF_IDF Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RankFunctionSimpleTF_IDF();
                    }
                    return instance;
                }
            }
        }

        RankFunctionSimpleTF_IDF()
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

            double tf = occ.Frequency;
            double idf = Math.Log((((double)totalDocQuantity) + 1) / ((double)occ.Word.QuantityDocFrequency));

            double tf_idf = ((double)countTermQuery) * ((double)countTermDoc) * idf;

            queryRank += tf_idf;

            return queryRank;
        }
    }
}
