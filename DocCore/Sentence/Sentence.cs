using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class Sentence
    {
        string sentenceParameters;

        public string SentenceParameters
        {
            get { return sentenceParameters; }
            set { sentenceParameters = value; }
        }
        TimeSpan timeToSearch;

        public TimeSpan TimeToSearch
        {
            get { return timeToSearch; }
            set { timeToSearch = value; }
        }
        long resultQuantity;

        public long ResultQuantity
        {
            get { return resultQuantity; }
            set { resultQuantity = value; }
        }

        DateTime searchDate;

        public DateTime SearchDate
        {
            get { return searchDate; }
            set { searchDate = value; }
        }
    }
}
