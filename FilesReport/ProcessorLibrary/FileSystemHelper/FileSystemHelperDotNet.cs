using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.IO;

namespace FilesReport
{
    public class FileSystemHelperDotNet : IFileSystemHelper
    {
        IRepositoryLog repLog;

        public FileSystemHelperDotNet()
        {
            this.repLog = FactoryRepositoryLog.GetRepositoryLog();
        }

        public long GetFileSize(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            return fileInfo.Length;
        }

        public string GetExtension(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            return fileInfo.Extension;
        }

        public DateTime GetCreationDate(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            DateTime result;

            if (fileInfo.LastWriteTime < fileInfo.CreationTime)
            {
                result = fileInfo.LastWriteTime;
            }
            else
            {
                result = fileInfo.CreationTime;
            }

            return result;
        }

        public string[] GetFiles(string directoryName)
        {
            return Directory.GetFiles(directoryName);
        }

        public string[] GetDirectories(string directoryRootName)
        {
           return Directory.GetDirectories(directoryRootName); 
        }

        public bool IsReadable(string directory)
        {
            try
            {
                string[] s = Directory.GetDirectories(directory);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            return true;
        }

        public bool IsValidPath(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Log entryLog = new Log();

                entryLog.TaskDescription = "Diretorio do repositorio não encontrado.";
                entryLog.StartDateTime = DateTime.Now;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryEntry Name: " + directory);

                this.repLog.Write(entryLog);

                return false;
            }

            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                Log entryLog = new Log();

                entryLog.TaskDescription = "'" + directory + "' is a reparse point. Skipped";
                entryLog.StartDateTime = DateTime.Now;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryEntry Name: " + directory);

                this.repLog.Write(entryLog);
                
                return false;
            }
            if (!IsReadable(directory))
            {
                Log entryLog = new Log();

                entryLog.TaskDescription = "'" + directory + "' *ACCESS DENIED*. Skipped";
                entryLog.StartDateTime = DateTime.Now;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryEntry Name: " + directory);

                this.repLog.Write(entryLog);

                return false;
            }
            return true;
        }
    }
}
