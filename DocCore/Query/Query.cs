using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class Query
    {
        private string originalQueryText;

        public string OriginalQueryText
        {
            get { return originalQueryText; }
            set { originalQueryText = value; }
        }

        private string clearQueryText;

        public string ClearQueryText
        {
            get { return clearQueryText; }
            set { clearQueryText = value; }
        }

        private List<QueryItem> queryItens;

        public List<QueryItem> QueryItens
        {
            get { return queryItens; }
            set { queryItens = value; }
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

        public Query(string queryText)
        {
            this.OriginalQueryText = queryText;
            this.QueryItens = ParseQueryItens(queryText);
            this.ClearQueryText = Useful.RemoveForbbidenSymbols(queryText);
            this.QueryItens = ParseQueryItens(queryText);
        }

        public static List<QueryItem> ParseQueryItens(string query)
        {
            List<QueryItem> result = new List<QueryItem>();

            string queryClear = QueryParser.GetCleanQuery(query);

            string[] splitWords = queryClear.Split(' ');

            for (int i = 0; (i < splitWords.Length); i++)
            {
                if (!string.IsNullOrEmpty( splitWords[i] ))
                {
                    QueryItem item = new QueryItem();

                    item.Text = splitWords[i];
                    item.Position = i;
                    item.WordID = Word.GetHashValue(splitWords[i]);
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
