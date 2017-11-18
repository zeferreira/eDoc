using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class DocumentResult : Document
    {
        IRepositoryDocument docIndex;

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
            this.docIndex = FactoryRepositoryDocument.GetRepositoryDocument();

            this.DocID = doc.DocID;
            this.File = doc.File;
            this.Title = doc.Title;
            this.Description = doc.Description;
            this.Url = doc.Url;
            this.WordQuantity = doc.WordQuantity;
        }

        public DocumentResult(int docID)
        {
            Document docTmp;

            this.docIndex = FactoryRepositoryDocument.GetRepositoryDocument();

            docTmp = docIndex.Read(docID);

            this.DocID = DocID;
            this.File = docTmp.File;
            this.Title = docTmp.Title;
            this.Url = docTmp.Url;
            this.WordQuantity = docTmp.WordQuantity;
        }

        public void CalculateRank(WordOccurrenceNode occ, Query query)
        {
            IRankFunction rankFunc = FactoryRankFunction.GetRankFunction();

            this.queryRank += rankFunc.CalcRankFactor(occ, query);
        }
    }
}
