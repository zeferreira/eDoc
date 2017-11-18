using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CSharpTest.Net.Serialization;
using CSharpTest.Net.Collections;
using System.Threading;
using System.Collections;


namespace DocCore
{
    public class RepositoryDocumentBplusTree : IRepositoryDocument
    {
        private static RepositoryDocumentBplusTree instance = null;
        private static readonly object padlock = new object();

        private BPlusTree<int, string> map;
        private CollectionStatistics statistics;

        private IEnumerator<KeyValuePair<int, string>> iteratorIndexer; 

        RepositoryDocumentBplusTree()
        {
            string documentDatabase = EngineConfiguration.Instance.BplusTreeFileRep;
            
            this.statistics = CollectionStatistics.LoadCollectionStatistics();

            BPlusTree<int, string>.Options options = new BPlusTree<int, string>.Options(
            PrimitiveSerializer.Int32, PrimitiveSerializer.String, Comparer<int>.Default)
            {
                CreateFile = CreatePolicy.IfNeeded,
                FileName = documentDatabase
            };

            this.map = new BPlusTree<int, string>(options);
            this.map.EnableCount();

            //this.iteratorIndexer = map.GetEnumerator(); 
        }

        public static RepositoryDocumentBplusTree Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RepositoryDocumentBplusTree();
                    }
                    return instance;
                }
            }
        }

        public void Insert(Document e)
        {
            lock(padlock)
            {
                Hashtable t = e.GetPostingListClass(); //get word quantity

                if(!map.ContainsKey(e.DocID))
                    map.Add(e.DocID, Useful.Serialize<Document>(e));
                
                this.statistics.TotalLenghtAccumulator += e.WordQuantity;
                
                this.statistics.TotalDocumentQuantity = map.Count;
                this.statistics.WriteCollectionStatistics();
                Thread.Sleep(30);
            }
        }

        public Document Read(int id)
        {
            return Useful.Deserialize<Document>(map[id]);
        }

        public bool Exist(int id)
        {
            lock (padlock) {
                return map.ContainsKey(id); 
            }
        }

        public int GetTotalQuantity()
        {
            lock (padlock)
            {
                return this.map.Count;
            }
        }

        public List<Document> List()
        {
            lock (padlock)
            {
                List<Document> list = new List<Document>();

                IEnumerator<KeyValuePair<int, string>> iterator = map.GetEnumerator();

                while(iterator.MoveNext()) 
                {
                    Document doc = Useful.Deserialize<Document>(iterator.Current.Value);

                    list.Add(doc);
                }

                return list;
            }
        }
        

        public double GetAverageDocumentLenght()
        {
            return this.statistics.AverageDocumentLenght;
        }
        public CollectionStatistics GetStatistitcs()
        {
            return this.statistics;
        }

        public void WriteStatistics()
        {
            this.statistics.WriteCollectionStatistics();
        }

        public bool MoveNext()
        {
            lock (padlock)
            {
                return this.iteratorIndexer.MoveNext();
            }
        }

        public Document GetCurrent()
        {
            lock (padlock)
            {
                return Useful.Deserialize<Document>(this.iteratorIndexer.Current.Value);
            }
        }
    }
}
