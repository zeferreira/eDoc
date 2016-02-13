using System;
using System.Collections.Generic;
using System.Text;

namespace FilesReport
{
    public interface IFileSystemHelper
    {
        //problems 
        // - FileInfo constructor throws PathTooLongException.
        // - Directory.GetFiles throws PathTooLongException.
        // - I don't find any mention to 'PathTooLongException' for Directory.GetDirectories

        long GetFileSize(string fileName);
        DateTime GetCreationDate(string fileName);
        string[] GetFiles(string directoryName);
        string[] GetDirectories(string directoryRootName);
        bool IsReadable(string directory);
        bool IsValidPath(string directory);
        string GetExtension(string fileName);
    }
}
