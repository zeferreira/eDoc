using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerCore
{
    public class RepositoryDocumentCandidateTXT : IRepositoryDocumentCandidate
    {
        string repFileName;

        public RepositoryDocumentCandidateTXT(string repTxTFile)
        {
            this.repFileName = repTxTFile;
        }

        public void Insert(DocumentCandidate doc)
        {
            string[] lines = new string[1];
            lines[0] = doc.OriginalUrl;
            File.AppendAllLines(this.repFileName, lines);
        }

        public List<DocumentCandidate> List()
        {
            List<DocumentCandidate> result = new List<DocumentCandidate>();

            string[] repList = File.ReadAllLines(this.repFileName);

            foreach (string item in repList)
            {
                DocumentCandidate dcn = new DocumentCandidate(item);
                result.Add(dcn);
            }

            return result;
        }

        public void ClearRepository()
        {
            File.WriteAllText(this.repFileName, String.Empty);
        }


        public bool Exist(int id)
        {
            throw new NotImplementedException();
        }
    }
}
