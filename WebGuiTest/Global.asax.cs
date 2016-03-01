using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using DocCore;
using System.Diagnostics;

namespace WebGuiTest
{
    public class Global : System.Web.HttpApplication
    {
        private Engine eng;
        IRepositoryLog repLog;

        protected void Application_Start(object sender, EventArgs e)
        {
            this.eng = Engine.Instance;

            this.repLog = FactoryRepositoryLog.GetRepositoryLog();

            Engine eng = Engine.Instance;
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string smsTimeToLoad = "Load Engine".PadRight(15);
            string smsSearch = "Search".PadRight(15);
            string smsSearchTwoWords = "Search Two Words".PadRight(15);
            string smsMemoryUsage = "Memory".PadRight(15);


            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            eng.Load();
            sw.Stop();
            timeDif = sw.Elapsed;

            Log entry = new Log();
            entry.TaskDescription = smsTimeToLoad;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalIndexedDocs: " + eng.TotalDocumentQuantity.ToString());
            entry.LogParameters.Add("totalIndexedWords: " + eng.TotalWordQuantity.ToString());

            repLog.Write(entry);

            //memory monitor
            Process currentProc = Process.GetCurrentProcess();

            long memoryUsed = currentProc.PrivateMemorySize64;

            entry = new Log();
            entry.TaskDescription = smsMemoryUsage;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("TotalMemory: " + Useful.GetFormatedSizeString(memoryUsed));
            repLog.Write(entry);

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}