using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocCore;
using System.Diagnostics;
using System.Configuration;
using System.Text;

namespace WebGuiTest
{
    public partial class _Default : System.Web.UI.Page
    {
        Engine eng;

        protected void Page_Load(object sender, EventArgs e)
        {
            eng = Engine.Instance;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Clear();

            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            Engine eng = Engine.Instance;
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string smsSearch = "Search".PadRight(15);

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<DocumentResult> result = eng.Search(txtQuerySearch.Text);
            sw.Stop();
            timeDif = sw.Elapsed;

            string parameters = txtQuerySearch.Text;
            
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<DocumentResult> docList = eng.Search(parameters);
            sw.Stop();
            timeDif = sw.Elapsed;

            Log entry = new Log();
            entry.TaskDescription = smsSearch;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("sentence: " + parameters);
            entry.LogParameters.Add("totalDocFound: " + docList.Count.ToString());
            repLog.Write(entry);

            TestResult(result);
        }

        void TestResult(List<DocumentResult> list)
        {
            int qtd = 1;

            foreach (DocumentResult item in list)
            {
                string physicalRepositoryPath = EngineConfiguration.Instance.PathFolderRepository;
                string virtualRepositoryParh = ConfigurationManager.AppSettings["virtualRepositoryPath"] as string;

                string resultPath = item.File.Remove(0,physicalRepositoryPath.Length);

                resultPath = virtualRepositoryParh + resultPath.Replace("\\", "/");


                //resultPath = Server.MapPath(resultPath);
                //resultPath = HttpContext.Current.Request.Url +"/" + resultPath;
                //to do: needs encoding.
                resultPath = "<a href=\"" + GetEncodedString( resultPath ) + "\">" + item.Title + "</a>" + "<br>";

                Response.Write(

                    qtd + " - " + item.Title +
                    " | QueryRank: " + item.QueryRank +
                    " | WordsQtd:" + item.WordQuantity +
                    //"\n" + " | File: " + item.File +
                    "<br>");

                qtd++;

                Response.Write(resultPath);
            }

            if (list.Count == 0)
            {
                 Response.Write(Messages.DocumentNotFound +"<br>");
            }
            else
                 Response.Write(list.Count.ToString() +" " + Messages.DocumentsFound + "<br>" );
        }

        private string GetEncodedString(string text)
        {
            string[] temp = text.Split('/');
            StringBuilder builder = new StringBuilder();

            foreach (string item in temp)
            {
                builder.Append(HttpContext.Current.Server.UrlEncode(item));
                builder.Append("/");
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}