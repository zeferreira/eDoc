using System;
using System.Collections.Generic;

namespace DocCore
{
    public interface IIndexer
    {
        void Index(List<Document> listOfDocs);
        void Load();
        void ReIndexing();

        Word Search(int wordID);

        long TotalDocumentQuantity { get; }
        long TotalWordQuantity { get; }
    }
}
