using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public interface IDocumentIndex
    {
        void Insert(Document doc);
        Document Search(long docID);
        void Delete(long docID);
    }
}
