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

        public void CalculateRank(Query query)
        {
            //this.queryRank += DocQuantitityResults;
            //to do.
            if (HasPhrase(query))
            {
                this.queryRank += 50;
            }
        }

        public void CalculateRank(Word word)
        {
            this.queryRank += ((double)lexicon.Quantity) / ((double)word.Quantity);
        }

        public bool HasPhrase(Query query)
        {
            return false;
        }

    }
}
