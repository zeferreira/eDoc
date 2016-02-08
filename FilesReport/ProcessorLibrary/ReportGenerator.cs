using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using DocCore;
using System.Configuration;
using System.Diagnostics;

namespace FilesReport
{
    public class ReportGenerator
    {
        public delegate void SignalReportProgress(long totalQuant);
        public delegate void SignalReportDone(IRepositorySummaryDocType fileReport);

        private string rootFolder;
        long quant;
        long totalSize;
        DateTime lastIndexTime;
        IRepositorySummaryDocType repSum;
        IRepositoryLog repLog;
        long skippedFiles;
        

        Hashtable ht;
        
        SignalReportProgress signalEvent;
        SignalReportDone signalWorkDone;
        string pathReportsFolder;

        DateTime startTime;
        TimeSpan timeDif;

        Stopwatch sw;

        public ReportGenerator(string rootFolderPath, SignalReportProgress signalEvent, SignalReportDone workDone)
        {
            this.rootFolder = rootFolderPath;
            this.ht = new Hashtable();
            this.signalEvent = signalEvent;
            this.signalWorkDone = workDone;

            this.pathReportsFolder = ConfigurationManager.AppSettings["reportFolder"].ToString();
            this.repLog = FactoryRepositoryLog.GetRepositoryLog();
        }

        public void AddOccurrence(SummaryDocType d)
        {
            if (ht.ContainsKey(d.FileExtension.GetHashCode()))
            {
                SummaryDocType olderSummary = ht[d.FileExtension.GetHashCode()] as SummaryDocType;

                if (olderSummary.MaxSizeFile < d.MaxSizeFile)
                {
                    olderSummary.MaxSizeFile = d.MaxSizeFile;
                }

                if (olderSummary.RecentFile < d.RecentFile)
                {
                    olderSummary.RecentFile = d.RecentFile;
                }

                if (olderSummary.OlderFile > d.OlderFile)
                {
                    olderSummary.OlderFile = d.OlderFile;
                }

                olderSummary.TotalSize += d.TotalSize;
                olderSummary.TotalQuantity++;
            }
            else 
            {
                this.ht.Add(d.FileExtension.GetHashCode(), d);
            }

            signalEvent(this.quant);
        }

        public long GetTotalQuantity()
        {
            return this.quant;
        }

        public void ProcessFilesReport()
        {
            startTime = DateTime.Now;
            sw = Stopwatch.StartNew();

            this.repSum = FactoryRepositorySummaryDocType.GetRepositorySummaryDocType();

            if (Directory.Exists(rootFolder))
            {
                // This path is a directory and his path is valid
                if(IsValidPath(rootFolder))
                {
                    ProcessDirectory(rootFolder);
                }
            }
            else
            {
                throw new Exception("Diretorio do repositorio não encontrado.");
            }

            this.lastIndexTime = DateTime.Now;
            sw.Stop();
            timeDif = sw.Elapsed;

            Log entryLog = new Log();
            entryLog.TaskDescription = "Report File Created";
            entryLog.ExecutionTime = timeDif;
            entryLog.StartDateTime = startTime;
            entryLog.LogParameters = new List<string>();

            entryLog.LogParameters.Add("Quantity: " + this.quant.ToString());
            entryLog.LogParameters.Add("Size: " + GetFormatedSizeString(this.totalSize).ToString());

            this.repLog.Write(entryLog);

            this.CreateReport();
            this.signalWorkDone(repSum);
        }

        public void ProcessDirectory(string targetDirectory)
        {
            try
            {
                //there is a PathTooLongException - Here!!!!!
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);

                foreach (string fileName in fileEntries)
                    ProcessFile(fileName);

            }
            catch (PathTooLongException e)
            {
                Log entryLog = new Log();
                entryLog.TaskDescription = "File Name is Too Long (GetFiles)";
                entryLog.ExecutionTime = timeDif;
                entryLog.StartDateTime = startTime;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryName: " + targetDirectory);
                entryLog.LogParameters.Add("Track: " + e.Message);

                this.repLog.Write(entryLog);
                this.skippedFiles++;
            }

            try
            {
                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                foreach (string subdirectory in subdirectoryEntries)
                {
                    if (IsValidPath(subdirectory))
                    {
                        ProcessDirectory(subdirectory);
                    }
                }
            }
            catch (PathTooLongException e)
            {
                Log entryLog = new Log();
                entryLog.TaskDescription = "Directory Name is Too Long (GetSubDirectories)";
                entryLog.ExecutionTime = timeDif;
                entryLog.StartDateTime = startTime;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryName: " + targetDirectory);
                entryLog.LogParameters.Add("Track: " + e.Message);

                this.repLog.Write(entryLog);
                this.skippedFiles++;
            }
        }

        private void ProcessFile(string fileName)
        {
            try
            {
                //there is a PathTooLongException - Here!!!!!
                FileInfo fileInfo = new FileInfo(fileName);

                SummaryDocType newDoc = new SummaryDocType(fileInfo.Extension);
                newDoc.MaxSizeFile = fileInfo.Length;

                if (fileInfo.LastWriteTime < fileInfo.CreationTime)
                {
                    newDoc.OlderFile = fileInfo.LastWriteTime;
                }
                else
                {
                    newDoc.OlderFile = fileInfo.CreationTime;
                }

                newDoc.RecentFile = fileInfo.CreationTime;
                newDoc.TotalSize = fileInfo.Length;
                newDoc.TotalQuantity = 1;
                this.quant += 1;
                this.totalSize += fileInfo.Length;

                AddOccurrence(newDoc);
            }
            catch (PathTooLongException e)
            {
                Log entryLog = new Log();
                entryLog.TaskDescription = "File Name is Too Long";
                entryLog.ExecutionTime = timeDif;
                entryLog.StartDateTime = startTime;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("FileName: " + fileName);
                entryLog.LogParameters.Add("Track: " + e.Message);

                this.repLog.Write(entryLog);
                this.skippedFiles++;
            }
        }

        private void CreateReport()
        {
            foreach (DictionaryEntry item in ht)
            {
                SummaryDocType s = item.Value as SummaryDocType;

                repSum.Write(s);
            }
        }

        public static string GetFormatedSizeString(long bytesSize)
        {
            if (bytesSize > 1024)
            {
                long kbSize = bytesSize / 1024;

                if (kbSize > 1024)
                {
                    long mbSize = kbSize / 1024;

                    if (mbSize > 1024)
                    {
                        long gbSize = mbSize / 1024;

                        if (gbSize > 1024)
                        {
                            long tbSize = gbSize / 1024;
                            return tbSize.ToString() + "(TB's)";
                        }
                        else
                        {
                            return gbSize.ToString() + "(GB's)";
                        }
                    }
                    else
                    {
                        return mbSize.ToString() + "(MB's)";
                    }
                }
                else
                {
                    return kbSize.ToString() + "(KB's)";
                }
                
            }
            else
            {
                return bytesSize.ToString() + "(B's)";
            }
        }

        private bool IsValidPath(string p)
        {
            if ((File.GetAttributes(p) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                Log entryLog = new Log();

                entryLog.TaskDescription = "'" + p + "' is a reparse point. Skipped";
                entryLog.StartDateTime = DateTime.Now;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryEntry Name: " + p);

                this.repLog.Write(entryLog);
                
                return false;
            }
            if (!IsReadable(p))
            {
                Log entryLog = new Log();

                entryLog.TaskDescription = "'" + p + "' *ACCESS DENIED*. Skipped";
                entryLog.StartDateTime = DateTime.Now;
                entryLog.LogParameters = new List<string>();

                entryLog.LogParameters.Add("DirectoryEntry Name: " + p);

                this.repLog.Write(entryLog);

                return false;
            }
            return true;
        }

        private bool IsReadable(string p)
        {
            try
            {
                string[] s = Directory.GetDirectories(p);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            return true;
        }

    }
}
