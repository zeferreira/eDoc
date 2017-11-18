using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerCore
{
    public interface IRepositoryDocumentCandidate
    {
        List<DocumentCandidate> List();
        void Insert(DocumentCandidate doc);

        bool Exist(int id);

        void ClearRepository();
    }
}
