using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public interface IDocumentIndex
    {
        void Insert(Document doc);
        Document Search(int docID);
        void Delete(int docID);
    }
}
