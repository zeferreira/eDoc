using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    /// <summary>
    /// Class with Pivoted Length Normalization VSM
    /// http://singhal.info/pivoted-dln.pdf - 25/10/2016
    /// http://singhal.info/ieee2001.pdf - - 25/10/2016
    /// 
    /// </summary>
    public class RankFunctionPivotedLengthNormVSM : IRankFunction
    {
        private EngineConfiguration engConf;
        private IDocumentIndex docIndex;
        private double avdl;

        private static RankFunctionPivotedLengthNormVSM instance = null;
        private static readonly object padlock = new object();

        long totalDocQuantity;

        double s;

        public static RankFunctionPivotedLengthNormVSM Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RankFunctionPivotedLengthNormVSM();
                    }
                    return instance;
                }
            }
        }

        RankFunctionPivotedLengthNormVSM()
        {
            this.engConf = EngineConfiguration.Instance;
            this.docIndex = FactoryDocumentIndex.GetDocumentIndex();
            
            this.s = engConf.SNormalizationfactor;

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
            double termQueryFactor = qtf;

            double df = (double)occ.Word.QuantityDocFrequency;
            //double termLogFactor = Math.Log( ((totalDocQuantity - df + 0.5D)/(df + 0.5D)),Math.E);
            double termLogFactor = Math.Log( ((double)totalDocQuantity + 1) / ((double)df));

            double tf = ((double)occ.Hits.Count / (double)occ.Doc.WordQuantity);
            double normalizer = ((1 - s) + (s * ((double)occ.Doc.WordQuantity / avdl)));
            double normalizationTermFactor = Math.Log(1 + Math.Log(1 + tf)) / normalizer;

            queryRank = termQueryFactor * normalizationTermFactor * termLogFactor;

            return queryRank;
        }
    }
}
