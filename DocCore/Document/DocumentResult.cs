using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class DocumentResult : Document
    {
        private long queryRank;

        public long QueryRank
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

        public void CalculateRank(Query query)
        {
            //to do.
        }

    }
}
