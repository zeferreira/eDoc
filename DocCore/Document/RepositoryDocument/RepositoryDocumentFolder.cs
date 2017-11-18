using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DocCore
{
    public class RepositoryDocumentFolder : IRepositoryDocument
    {
        private string rootFolder;
        int quant;
        DateTime lastIndexTime;
        private CollectionStatistics statistics;

        public RepositoryDocumentFolder(string folder)
        {
            this.rootFolder = folder;
            this.statistics = CollectionStatistics.LoadCollectionStatistics();
        }

        public void Insert(Document e)
        {
            throw new NotImplementedException();
        }

        public Document Read(int id)
        {
            throw new NotImplementedException();
        }

        public int GetTotalQuantity()
        {
            return this.quant;
        }

        public List<Document> List()
        {
            List<Document> list = new List<Document>();

            if (Directory.Exists(rootFolder))
            {
                // This path is a directory
                ProcessDirectory(rootFolder, list);
            }
            else
            {
                throw new Exception("Diretorio do repositorio não encontrado.");
            }

            this.quant = list.Count;
            this.lastIndexTime = DateTime.Now;
            return list;
        }

        public static void ProcessDirectory(string targetDirectory, List<Document> list)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, list);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, list);
        }

        private static void ProcessFile(string fileName, List<Document> list)
        {
            Document newDoc = new Document();
            
            string tmpFileName = Useful.FormatFileNameToUrl(Path.GetFileName(fileName));

            newDoc.Title = tmpFileName;
            newDoc.File = fileName;
            newDoc.DocID = newDoc.File.GetHashCode();
            newDoc.Url = tmpFileName;
            list.Add(newDoc);
        }


        public bool Exist(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Document GetCurrent()
        {
            throw new NotImplementedException();
        }
    }
}
