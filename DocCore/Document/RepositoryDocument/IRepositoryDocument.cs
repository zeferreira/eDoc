using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public interface IRepositoryDocument
    {
        double GetAverageDocumentLenght();

        int GetTotalQuantity();

        CollectionStatistics GetStatistitcs();

        void WriteStatistics();

        void Insert(Document e);
        Document Read(int id);

        bool Exist(int id);
        
        List<Document> List();

        bool MoveNext();

        Document GetCurrent();


    }
}
