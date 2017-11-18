using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocCore;

namespace CrawlerCore
{
    public class RepositoryDocumentCandidateXML : IRepositoryDocumentCandidate
    {
        string repFileName;
        public RepositoryDocumentCandidateXML(string repXMLFile)
        {
            this.repFileName = repXMLFile;
        }

        public void Insert(DocumentCandidate doc)
        {
            string content = Useful.Serialize<DocumentCandidate>(doc);

            File.AppendAllText(repFileName, content + Environment.NewLine);
        }

        public List<DocumentCandidate> List()
        {
            List<DocumentCandidate> result = new List<DocumentCandidate>();

            string[] repList = File.ReadAllLines(this.repFileName);

            foreach (string item in repList)
            {
                DocumentCandidate dcn = Useful.Deserialize<DocumentCandidate>(item);
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
