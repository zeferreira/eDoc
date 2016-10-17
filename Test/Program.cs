using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.Diagnostics;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            IEngine eng = FactoryEngine.GetEngine();
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;
            DateTime startTest = DateTime.Now;

            #region basicTestsLogicAndPerformance
            Console.WriteLine("=============================================================================");
            string smsTimeToLoad = "Load Engine".PadRight(15);
            string smsSearch = "Search".PadRight(15);
            string smsSearchTwoWords = "Search Two Words".PadRight(15);
            string smsMemoryUsage = "Memory".PadRight(15);

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            Console.WriteLine(startTest.ToString() + " - Loading...");
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
            entry.LogParameters.Add("TypeGUI: CONSOLE");

            repLog.Write(entry);
            Console.WriteLine("I'm running some basic tests");
            //search one word
            string parameters = "search";
            Console.WriteLine("||||| positive test - one word ||||| ( {0} )", parameters);
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<DocumentResult> docList = eng.Search(parameters);
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsSearch;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("sentence: " + parameters);
            entry.LogParameters.Add("totalDocFound: " + docList.Count.ToString());
            repLog.Write(entry);

            TestResult(docList);
            
            //search 2 words 
            //search one word
            parameters = "web search";
            Console.WriteLine("||||| positive test - two words ||||| ( {0} )",   parameters);
            
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            docList = eng.Search(parameters);
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsSearchTwoWords;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("sentence: " + parameters);
            entry.LogParameters.Add("totalDocFound: " + docList.Count.ToString());
            repLog.Write(entry);

            TestResult(docList);


            //search word that not exist
            parameters = "gimgolbel123#321456654987qqqwweqweq";
            Console.WriteLine("||||| negative test  ||||| ( {0} )", parameters);
            
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            docList = eng.Search(parameters);
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsSearch;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("sentence: " + parameters);
            entry.LogParameters.Add("totalDocFound: " + docList.Count.ToString());
            entry.LogParameters.Add("totalDocIndexed: " + eng.TotalDocumentQuantity.ToString());
            repLog.Write(entry);

            TestResult(docList);

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
            #endregion
            Console.WriteLine("=============================================================================");

            #region RankTests
            Console.WriteLine("I'm running ranked queries for tests");
            Console.WriteLine("Wait ...");
            
            SearchRankedThemes();
            Console.WriteLine(startTest.ToString() + " I have done.");
            Console.WriteLine("=============================================================================");
            Console.WriteLine("Start: " + startTest.ToString() + " I have done. (" + DateTime.Now.ToString() + ")");
            #endregion
            Console.ReadLine();
        }

        static void TestResult(List<DocumentResult> list)
        {
            int qtd = 1;

            foreach (DocumentResult item in list)
            {
                Console.WriteLine(
                    
                    qtd + " - " + item.Title +
                    " | QueryRank: " + item.QueryRank + 
                    " | WordsQtd:" + item.WordQuantity +
                    //"\n" + " | File: " + item.File +
                    "\n");
                
                qtd++;
            }

            if (list.Count == 0)
            {
                Console.WriteLine(Messages.DocumentNotFound + "\n");
            }
            else
                Console.WriteLine("{0} "+ Messages.DocumentsFound +" \n", list.Count);
        }

        /// <summary>
        /// Method for write results to disk. It store the order, ID and rank of documents. This metrics are used to compare modifcations.
        /// </summary>
        /// <param name="list">Lista de documentos</param>
        /// <param name="query">Query que foi retornada</param>
        static void WriteResultsToDisk(List<DocumentResult> list, string query)
        {
            int qtd = 1;

            string fileNameResult = RemoveForFileName( EngineConfiguration.Instance.PathEvaluationLog + query + ".txt");
            string resultRank = "#Date" + DateTime.Now.ToString() + "#" + Environment.NewLine;

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

            File.AppendAllText(fileNameResult, resultRank);
        }

        static void SearchRankedThemes()
        {
            List<string> themes = GetThemes(@"D:\projetos\edoc\CodeRepository\eDoc\CrawlerGUI\DataSearch\Themes\computer_en_us.txt");
            List<string> tcc_poli = GetThemes(@"D:\projetos\edoc\CodeRepository\eDoc\CrawlerGUI\DataSearch\Themes\UPE_poli_tcc_20132.txt");
            IEngine eng = FactoryEngine.GetEngine();

            foreach (string theme in themes)
            {
                List<DocumentResult> resultList = eng.Search(theme);

                WriteResultsToDisk(resultList, theme);
            }

            foreach (string theme in tcc_poli)
            {
                List<DocumentResult> resultList = eng.Search(theme);

                WriteResultsToDisk(resultList, theme);
            }
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

        private static List<string> GetThemes(string file)
        {
            var strInstFile = File.ReadAllLines(file);
            List<string> instList = new List<string>(strInstFile);

            return instList;
        }

        static void ShowLogEntrys()
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            List<Log> logList = repLog.List();

            foreach (Log item in logList)
            {
                Console.WriteLine(item.ToString());
            }
        }


    }
}
