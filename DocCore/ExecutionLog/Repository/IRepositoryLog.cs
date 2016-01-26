using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public interface IRepositoryLog
    {
        void Write(Log entry);
        List<Log> List();
    }
}
