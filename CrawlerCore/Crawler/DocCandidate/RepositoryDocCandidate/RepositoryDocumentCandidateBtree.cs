using DocCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Serialization;
using CSharpTest.Net.Collections;
using System.Threading;

namespace CrawlerCore
{
    public class RepositoryDocumentCandidateBtree : IRepositoryDocumentCandidate
    {
        string repFileName;
        BPlusTree<int, string> map;

        private static readonly object padlock = new object();

        public RepositoryDocumentCandidateBtree(string repBtreeFile)
        {
            this.repFileName = repBtreeFile;

            BPlusTree<int, string>.Options options = new BPlusTree<int, string>.Options(
            PrimitiveSerializer.Int32, PrimitiveSerializer.String, Comparer<int>.Default)
            {
                CreateFile = CreatePolicy.IfNeeded,
                FileName = repFileName
            };

            this.map = new BPlusTree<int, string>(options);
        }

        public void Insert(DocumentCandidate doc)
        {
            lock (padlock)
            {
                string value = doc.OriginalUrl;
                int key = value.GetHashCode();

                map.Add(key, value);
                Thread.Sleep(30);
            }
        }

        public List<DocumentCandidate> List()
        {
            List<DocumentCandidate> result = new List<DocumentCandidate>();

            IEnumerator<KeyValuePair<int, string>> iterator = map.GetEnumerator();

            while (iterator.MoveNext())
            {
                DocumentCandidate dcn = new DocumentCandidate(iterator.Current.Value);
                result.Add(dcn);

            }       

            return result;
        }

        public void ClearRepository()
        {
            map.Clear();
        }


        public bool Exist(int id)
        {
            lock (padlock)
            {
                return map.ContainsKey(id);
            }
        }
    }
}
