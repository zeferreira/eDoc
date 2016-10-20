using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class DocumentResult : Document
    {
        IDocumentIndex docIndex;
        ILexicon lexicon;

        private double queryRank;

        public double QueryRank
        {
            get { return queryRank; }
        }
        //it is used for calculate the a temporary solution for a rank where I merge some documents.
        private int docQuantitityResults;

        public int DocQuantitityResults
        {
            get { return docQuantitityResults; }
            set { docQuantitityResults = value; }
        }

        public DocumentResult(Document doc)
        {
            this.docIndex = FactoryDocumentIndex.GetDocumentIndex();
            this.lexicon = FactoryLexicon.GetLexicon();

            this.DocID = doc.DocID;
            this.File = doc.File;
            this.Title = doc.Title;
            this.WordQuantity = doc.WordQuantity;
        }

        public DocumentResult(int docID)
        {
            Document docTmp;

            this.docIndex = FactoryDocumentIndex.GetDocumentIndex();
            this.lexicon = FactoryLexicon.GetLexicon();

            docTmp = docIndex.Search(docID);

            this.DocID = DocID;
            this.File = docTmp.File;
            this.Title = docTmp.Title;
            this.WordQuantity = docTmp.WordQuantity;
        }

        public void CalculateRank(WordOccurrenceNode occ, Query query)
        {
            int countTermQuery = 0;
            foreach (QueryItem item in query.QueryItens)
            {
                if(item.WordID == occ.Word.WordID)
                {
                    countTermQuery++;
                }
            }

            int countTermDoc = occ.Hits.Count;

            double tf = occ.Frequency;
            double idf = Math.Log((((double)docIndex.GetQuantity()) + 1) / ((double)occ.Word.QuantityDocFrequency));

            double tf_idf = ((double)countTermQuery) * ((double)countTermDoc) * idf;

            this.queryRank += tf_idf;
        }
    }
}
