using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace DocCore
{
    public class CollectionStatistics
    {
        DateTime lastIndexTime;

        public DateTime LastIndexTime
        {
            get { return lastIndexTime; }
            set { lastIndexTime = value; }
        }

        int totalDocumentQuantity;

        public int TotalDocumentQuantity
        {
            get { return totalDocumentQuantity; }
            set { totalDocumentQuantity = value; }
        }

        double averageDocumentLenght;

        public double AverageDocumentLenght
        {
            get 
            {
                this.averageDocumentLenght = this.totalLenghtAccumulator / this.TotalDocumentQuantity;

                return averageDocumentLenght;
            }
        }

        private long totalLenghtAccumulator;

        public long TotalLenghtAccumulator
        {
            get { return totalLenghtAccumulator; }
            set { totalLenghtAccumulator = value; }
        }

        public static CollectionStatistics LoadCollectionStatistics()
        {
            string fileName = EngineConfiguration.Instance.CollectionStatisticsFile;

            if (File.Exists(fileName))
            {
                string content = File.ReadAllText(fileName);

                return Useful.Deserialize<CollectionStatistics>(content);
            }
            else
                return new CollectionStatistics();
        }

        public void WriteCollectionStatistics()
        {
            string filePath = EngineConfiguration.Instance.CollectionStatisticsFile;
            string content = Useful.Serialize<CollectionStatistics>(this);
            File.WriteAllText(filePath, content);
        }
    }
}
