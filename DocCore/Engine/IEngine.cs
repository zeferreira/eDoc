using System;
using System.Collections.Generic;

namespace DocCore
{
    public interface IEngine
    {
        void Load();
        void Reindex();
        
        List<DocumentResult> Search(string query);
        //List<DocumentResult> SearchFromMemory(string query);
        
        long TotalDocumentQuantity { get; }
        long TotalWordQuantity { get; }
    }
}
