using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class QueryItem
    {
        string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        int wordID;

        public int WordID
        {
            get { return wordID; }
            set { wordID = value; }
        }

        int position;

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

    }
}
