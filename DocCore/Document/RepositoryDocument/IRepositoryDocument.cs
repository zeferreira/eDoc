using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public interface IRepositoryDocument
    {
        void Insert(Document e);
        Document Read(long id);
        void MarkIndexed(long id);
        long GetTotalQuantity();
        List<Document> Search(bool onlyIndexed);
    }
}
