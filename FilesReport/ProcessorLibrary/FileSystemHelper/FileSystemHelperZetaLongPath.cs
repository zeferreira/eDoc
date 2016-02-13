using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.IO;
using ZetaLongPaths.Native;
using ZetaLongPaths;

namespace FilesReport
{
    //using https://zetalongpaths.codeplex.com/ - Release 2013-03-20
    public class FileSystemHelperZetaLongPath : IFileSystemHelper
    {
        IRepositoryLog repLog;

        public FileSystemHelperZetaLongPath()
        {
            this.repLog = FactoryRepositoryLog.GetRepositoryLog();
        }

        public long GetFileSize(string fileName)
        {
            ZlpFileInfo fileInfo = new ZlpFileInfo(fileName);

            return (long)fileInfo.Length;
        }

        public string GetExtension(string fileName)
        {
            ZlpFileInfo fileInfo = new ZlpFileInfo(fileName);

            return fileInfo.Extension;
        }

        public DateTime GetCreationDate(string fileName)
        {
            ZlpFileInfo fileInfo = new ZlpFileInfo(fileName);
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

            //Zeta doesn't have File.GetAttributes method :(
            if ((File.GetAttributes(directory) & System.IO.FileAttributes.ReparsePoint) == System.IO.FileAttributes.ReparsePoint)
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
