using System;
using System.Collections.Generic;
using System.Text;

namespace FilesReport
{
    public interface IRepositorySummaryDocType
    {
        void Write(SummaryDocType entry);
        List<SummaryDocType> List();
    }
}
