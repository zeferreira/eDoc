using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FilesReport
{
    public class SummaryDocType
    {
        readonly char separator = '|';

        long totalQuantity;

        public long TotalQuantity
        {
            get { return totalQuantity; }
            set { totalQuantity = value; }
        }


        string fileExtension;

        public string FileExtension
        {
            get { return fileExtension; }
        }
        
        long totalSize;

        public long TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }

        DateTime olderFile;

        public DateTime OlderFile
        {
            get { return olderFile; }
            set { olderFile = value; }
        }
        DateTime recentFile;

        public DateTime RecentFile
        {
            get { return recentFile; }
            set { recentFile = value; }
        }

        long maxSizeFile;

        public long MaxSizeFile
        {
            get { return maxSizeFile; }
            set { maxSizeFile = value; }
        }

        public SummaryDocType(string extension)
        {
            this.fileExtension = extension;
        }

        public override string ToString()
        {
            string result =
                this.FileExtension.ToString() + separator.ToString() +
                this.TotalQuantity.ToString() + separator.ToString() +

                this.TotalSize.ToString() + separator.ToString() +
                this.MaxSizeFile.ToString() +separator.ToString() +
                this.OlderFile.ToString() + separator.ToString() +
                this.RecentFile.ToString() + separator.ToString() +
                Environment.NewLine;

            return result;
        }
    }
}
