using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocCore;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.IO;
using guiMVC.Models;


namespace guiMVC.Controllers
{
    public class EngineController : Controller
    {
        IEngine eng;
        EngineConfiguration engConf;
        int resultPageSize = 10;
        //
        // GET: /Search/
        public ActionResult Index(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                return this.RedirectToAction("search", new { q = q.ToString() });

            }
            else
                return View();
        }

        //
        // GET: /Search/
        public ActionResult Search(string q, int startPaging = 0)
        {
            ViewData["resultPageSize"] = resultPageSize;
            

            if (string.IsNullOrEmpty(q))
            {
                return this.RedirectToAction("Index");

            }
            else
            {
                this.eng = FactoryEngine.GetEngine();
                ResultSearchModel model = new ResultSearchModel();
                model.query = q;
                model.start = startPaging;
                List<DocumentResult> tmpResult = Search(q, model);
                model.results = tmpResult;

                if ((model.results.Count - startPaging) > resultPageSize)
                {
                    ViewData["offSet"] = startPaging + resultPageSize;
                }
                else
                {
                    ViewData["offSet"] = startPaging + (model.results.Count - startPaging);
                }

                return View(model);
            }
        }

        public ActionResult about()
        {
            IIndexer index = FactoryIndexer.GetIndexer();

            Process currentProc = Process.GetCurrentProcess();

            long memoryUsed = currentProc.PrivateMemorySize64;
            
            ViewData["aboutMemory"] = Useful.GetFormatedSizeString(memoryUsed);
            ViewData["aboutFiles"] = index.TotalDocumentQuantity;
            ViewData["aboutWords"] = index.TotalWordQuantity;

            return View();
        }

        private List<DocumentResult> Search(string query, ResultSearchModel model)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            engConf = EngineConfiguration.Instance;
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

            Query parsedQuery = new Query(query);
            parsedQuery.ResultQuantity = result.Count;
            parsedQuery.SearchDate = DateTime.Now;
            parsedQuery.TimeToSearch = timeDif;
            string strQuerySer = Useful.Serialize<Query>(parsedQuery);

            Log entry = new Log();
            entry.TaskDescription = smsSearch;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("query: " + strQuerySer);
            entry.LogParameters.Add("totalDocFound: " + result.Count.ToString());
            entry.LogParameters.Add("totalDocIndexed: " + eng.TotalDocumentQuantity.ToString());
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);

            if (model.start == 0)
            {
                repLog.Write(entry);
            }

            if (result.Count > 0)
            {
                if (model.start == 0)
                {
                    WriteResultsToDisk(result, query);
                }
            }
            return result;
        }

        private string GetEncodedString(string text)
        {
            string resolveTemp = Url.Content(text);

            return Url.Content(resolveTemp);
        }


        static void WriteResultsToDisk(List<DocumentResult> list, string query)
        {
            EngineConfiguration engConf = EngineConfiguration.Instance;
            IRepositoryDocument docIndex = FactoryRepositoryDocument.GetRepositoryDocument();

            int qtd = 1;
            string fileName = RemoveForFileName(Useful.RemoveForbbidenSymbols(query));

            string fileNameResult = EngineConfiguration.Instance.PathEvaluationLog + fileName + ".txt";
            string resultRank = "#Date: " + DateTime.Now.ToString() + "# " + engConf.RankTypeFunction
                + "# BNormalizationfactor:" + engConf.BNormalizationfactor
                + "# SNormalizationfactor:" + engConf.SNormalizationfactor
                + "# BM25OkapiK1factor:" + engConf.BM25OkapiK1factor
                + "# BM25OkapiK3factor:" + engConf.BM25OkapiK3factor
                + "# avdl:" + docIndex.GetAverageDocumentLenght()
                + Environment.NewLine;

            foreach (DocumentResult item in list)
            {
                resultRank +=

                    "Order: " + qtd +
                    " | DocID: " + item.DocID.ToString() +
                    " | QueryRank: " + item.QueryRank +
                    " | WordsQtd:" + item.WordQuantity +
                    " | FileName: " + item.File +

                    //"\n" + " | File: " + item.File +
                    Environment.NewLine;

                qtd++;
            }

            System.IO.File.AppendAllText(fileNameResult, resultRank);
        }

        private static string RemoveForFileName(string fileName)
        {
            string illegal = fileName;
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                illegal = illegal.Replace(c.ToString(), "");
            }

            return illegal;
        }

    }
}
