using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    /// <summary>
    /// Class with default BM25 TF transformation (with document lenght normalization)
    /// http://singhal.info/ieee2001.pdf
    /// </summary>
    public class RankFunctionBM25_Okapi : IRankFunction
    {
        private EngineConfiguration engConf;
        private IDocumentIndex docIndex;
        private double avdl;

        private static RankFunctionBM25_Okapi instance = null;
        private static readonly object padlock = new object();

        long totalDocQuantity;

        double b;
        double k1;
        double k3;
        public static RankFunctionBM25_Okapi Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RankFunctionBM25_Okapi();
                    }
                    return instance;
                }
            }
        }

        RankFunctionBM25_Okapi()
        {
            this.engConf = EngineConfiguration.Instance;
            this.docIndex = FactoryDocumentIndex.GetDocumentIndex();
            
            this.b = engConf.BNormalizationfactor;
            this.k1 = engConf.BM25OkapiK1factor;
            this.k3 = engConf.BM25OkapiK3factor;
            this.totalDocQuantity = docIndex.GetQuantity();

            this.avdl = docIndex.GetAverageDocumentLenght();
        }

        public double CalcRankFactor(WordOccurrenceNode occ, Query query)
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

            double qtf = ((double)countTermQuery / (double)query.QueryItens.Count);
            double termQueryFactor = ((k3 + 1) * qtf) / (k3 + qtf);

            double df = (double)occ.Word.QuantityDocFrequency;
            //double termLogFactor = Math.Log( ((totalDocQuantity - df + 0.5D)/(df + 0.5D)),Math.E);
            double termLogFactor = Math.Log( ((double)totalDocQuantity) / ((double)df));

            double tf = ((double)occ.Hits.Count / (double)occ.Doc.WordQuantity);
            double normalizer = ((k1*(1 - b)) + (b * (occ.Doc.WordQuantity / avdl))) + tf;
            double normalizationTermFactor = ((k1 + 1) * tf) / normalizer;

            queryRank = termLogFactor * normalizationTermFactor * termQueryFactor;

            return queryRank;
        }
    }
}
