using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace FilesReport
{
    public class FactoryFileSystemHelper
    {
        //Warning for longfileNameSupportType: If you have problems with files that has names with more than 255 chars, change this for true.
        //Supported type"s:
        // - NET: for .net default support
        // - LONG: for too long file names

        public static IFileSystemHelper GetFileSystemHelper()
        {
            string type = ConfigurationManager.AppSettings["longfileNameSupportType"].ToString().ToLower();

            switch (type)
            {
                case "net":
                    return new FileSystemHelperDotNet();

                case "long":
                    return new FileSystemHelperZetaLongPath();

                default:
                    throw new NotImplementedException("Support for long file name type not supported.");

            }

        }
        

    }
}
