using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DocCore
{
    public class RepositoryDocumentFolder : IRepositoryDocument
    {
        private string rootFolder;
        long quant;
        DateTime lastIndexTime;

        public RepositoryDocumentFolder(string folder)
        {
            this.rootFolder = folder;
        }

        public void Insert(Document e)
        {
            throw new NotImplementedException();
        }

        public Document Read(long id)
        {
            throw new NotImplementedException();
        }

        public void MarkIndexed(long id)
        {
            throw new NotImplementedException();
        }

        public long GetTotalQuantity()
        {
            return this.quant;
        }

        public List<Document> Search(bool onlyIndexed)
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
            newDoc.Title = Path.GetFileName(fileName);
            newDoc.File = fileName;

            list.Add(newDoc);
        }
    }
}
