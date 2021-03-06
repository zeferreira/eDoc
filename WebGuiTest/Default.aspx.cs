﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocCore;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.IO;

namespace WebGuiTest
{
    public partial class _Default : System.Web.UI.Page
    {
        IEngine eng;
        string host;

        protected void Page_Load(object sender, EventArgs e)
        {
            eng = FactoryEngine.GetEngine();
            host = Request.Url.Host.ToLower();

            string query = Request.QueryString["qr"];

            if (!string.IsNullOrEmpty(query))
            {
                this.divFeedback.Visible = true;
                Response.Write("You are searching about: ");
                Response.Write("<b>" + query + "</b>"+ "<br>");
                //this.txtQuerySearch.Text = query;
                Search(query);
            }
            else
            {
                this.divFeedback.Visible = false;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx?qr=" + this.txtQuerySearch.Text);
        }

        void Search(string query)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            IEngine eng = FactoryEngine.GetEngine();
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string smsSearch = "Search".PadRight(15);

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<DocumentResult> result = eng.Search(query);
            sw.Stop();
            timeDif = sw.Elapsed;

            Log entry = new Log();
            entry.TaskDescription = smsSearch;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("sentence: " + query);
            entry.LogParameters.Add("totalDocFound: " + result.Count.ToString());
            repLog.Write(entry);

            TestResult(result);

            if (result.Count > 0)
            {
                WriteResultsToDisk(result, query);
            }
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
                    " | DocID: " + item.DocID.ToString() +
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
                 this.divFeedback.Visible = false;
            }
            else
            {
                Response.Write(list.Count.ToString() + " " + Messages.DocumentsFound + "<br>");
            }
              
        }
        /// <summary>
        /// Method for write results to disk. It store the order, ID and rank of documents. This metrics are used to compare modifcations.
        /// </summary>
        /// <param name="list">Lista de documentos</param>
        /// <param name="query">Query que foi retornada</param>
        private void WriteResultsToDisk(List<DocumentResult> list, string query)
        {
            int qtd = 1;

            string fileNameResult = EngineConfiguration.Instance.PathEvaluationLog + query + ".txt";
            string resultRank = "#Date" + DateTime.Now.ToString() + "#" + Environment.NewLine;

            foreach (DocumentResult item in list)
            {
                resultRank +=

                    "Order: " + qtd +
                    " | DocID: " + item.DocID.ToString() +
                    " | QueryRank: " + item.QueryRank +
                    " | WordsQtd:" + item.WordQuantity +
                    " | FileName: "+ item.File +

                    //"\n" + " | File: " + item.File +
                    Environment.NewLine;

                qtd++;
            }

            File.AppendAllText(fileNameResult, resultRank);
        }


        private string GetEncodedString(string text)
        {
            string resolveTemp = Page.ResolveUrl(text);

            return Page.ResolveUrl(resolveTemp);
        }

        protected void btnGravarFeedback_Click(object sender, EventArgs e)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            IEngine eng = FactoryEngine.GetEngine();
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string query = Request.QueryString["qr"];
            string parameters = "";

            if (!string.IsNullOrEmpty(query))
            {
                parameters = query;
            }

            string smsFeedback = "Feedback".PadRight(15);

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<DocumentResult> result = eng.Search(query);
            sw.Stop();
            timeDif = sw.Elapsed;

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            sw.Stop();
            timeDif = sw.Elapsed;

            Log entry = new Log();
            entry.TaskDescription = smsFeedback;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("sentence: " + parameters);
            entry.LogParameters.Add(txtFeedback.Text);

            if (RadioButtonFeedback.SelectedIndex > -1)
            {
                entry.LogParameters.Add(RadioButtonFeedback.SelectedItem.Text);
                entry.LogParameters.Add(("FeedbackValue: " + RadioButtonFeedback.SelectedItem.Value.ToString()));
            }

            repLog.Write(entry);
            this.divFeedback.Visible = false;
            Response.Write("Feedback Sent!!" + "<br>");
        }
    }
}