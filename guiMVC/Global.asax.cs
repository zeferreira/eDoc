using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using DocCore;
using System.Threading;
using System.Globalization;

namespace guiMVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private IEngine eng;
        IRepositoryLog repLog;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //// Change current culture
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-br");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            
            this.eng = FactoryEngine.GetEngine();

            this.repLog = FactoryRepositoryLog.GetRepositoryLog();

            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string smsTimeToLoad = "Load Engine".PadRight(15);
            string smsSearch = "Search".PadRight(15);
            string smsMemoryUsage = "Memory".PadRight(15);
            string smsConfFile = "ConfigFile".PadRight(15);

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
            entry.LogParameters.Add("TypeGUI: WEB");
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

            //configuration file
            //string strConfFile = Useful.Serialize<EngineConfiguration>(EngineConfiguration.Instance);

            //entry = new Log();
            //entry.TaskDescription = smsConfFile;
            //entry.StartDateTime = start;
            //entry.ExecutionTime = timeDif;
            //entry.LogParameters = new List<string>();
            //entry.LogParameters.Add(strConfFile);
            //repLog.Write(entry);

        }
    }
}