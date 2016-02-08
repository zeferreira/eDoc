using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.Configuration;

namespace FilesReport
{
    public class FactoryRepositorySummaryDocType
    {
        public static IRepositorySummaryDocType GetRepositorySummaryDocType()
        {
            string path = ConfigurationManager.AppSettings["reportFolder"].ToString();
            char separator = Convert.ToChar(ConfigurationManager.AppSettings["reportSeparator"].ToString());
            string type = ConfigurationManager.AppSettings["reportType"].ToString().ToLower(); 

            switch (type)
            {
                case "txt":

                    string newReportFileName = path + @"\" + "SummaryReport_" + 
                    DateTime.Now.Year.ToString() +
                    DateTime.Now.Month.ToString()+
                    DateTime.Now.Day.ToString()+ "_" +
                    DateTime.Now.Hour.ToString() +
                    DateTime.Now.Minute.ToString() + ".txt";

                    return new RepositorySummaryDocTypeTXT(newReportFileName, separator);

                case "sql":
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException("Repository for Report files type not implemented");

            }
 
        }
    }
}
