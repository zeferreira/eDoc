using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public interface IDocParser
    {
        string GetText(string docFilePath);
    }
}
