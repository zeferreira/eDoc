﻿using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Globalization;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();
            EngineConfiguration engConf = EngineConfiguration.Instance;

            IEngine eng = FactoryEngine.GetEngine();
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;
            DateTime startTest = DateTime.Now;

            #region loadEngine
            Console.WriteLine("=============================================================================");
            string smsTimeToLoad = "Load Engine".PadRight(15);
            string smsSearch = "Search".PadRight(15);
            string smsSearchTwoWords = "Search Two Words".PadRight(15);
            string smsMemoryUsage = "Memory".PadRight(15);
            //IndexerSPIMI indexer = new IndexerSPIMI();
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            Console.WriteLine(startTest.ToString() + " - Loading...");
            
            eng.Load();

            //eng.Reindex();

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
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);

            repLog.Write(entry);
            Console.WriteLine("final: " + timeDif.ToString());
            #endregion

            #region basicTestsLogicAndPerformance
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
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
            repLog.Write(entry);

            ShowResultsToScreen(docList);
            
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
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
            repLog.Write(entry);

            ShowResultsToScreen(docList);


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
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
            repLog.Write(entry);

            ShowResultsToScreen(docList);

            //memory monitor
            Process currentProc = Process.GetCurrentProcess();

            long memoryUsed = currentProc.PrivateMemorySize64;

            entry = new Log();
            entry.TaskDescription = smsMemoryUsage;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("TotalMemory: " + Useful.GetFormatedSizeString(memoryUsed));
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
            repLog.Write(entry);
            
            Console.WriteLine("=============================================================================");
            #endregion

            #region RankTests
            Console.WriteLine("I'm running ranked queries for tests");
            Console.WriteLine("Wait ...");

            //long term search
            parameters = "DESENVOLVIMENTO DE SISTEMA DE INFORMAÇÃO WEB PARA O CONTROLE INTERNO DE PROTOCOLOS DA ESCOLA POLITÉCNICA DE PERNAMBUCO";
            Console.WriteLine("||||| long term test  ||||| ( {0} )", parameters);

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
            entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
            repLog.Write(entry);


            //SearchRankedThemes();
            Console.WriteLine(startTest.ToString() + " I have done.");
            Console.WriteLine("=============================================================================");
            Console.WriteLine("Start: " + startTest.ToString() + " I have done. (" + DateTime.Now.ToString() + ")");
            #endregion

            #region cultureTest
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            CultureInfo cui = Thread.CurrentThread.CurrentUICulture;

            Console.WriteLine(ci.DisplayName);
            Console.WriteLine(cui.DisplayName);

            #endregion 


            Console.ReadLine();
        }

        static void ShowResultsToScreen(List<DocumentResult> list)
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

            File.AppendAllText(fileNameResult, resultRank);
        }

        static void SearchRankedThemes()
        {
            string smsSearch = "Search".PadRight(15);
            EngineConfiguration engConf = EngineConfiguration.Instance;
            List<string> themes = GetThemes(@"D:\projetos\edoc\CodeRepository\eDoc\CrawlerGUI\DataSearch\Themes\computer_en_us.txt");
            List<string> tcc_poli = GetThemes(@"D:\projetos\edoc\CodeRepository\eDoc\CrawlerGUI\DataSearch\Themes\UPE_poli_tcc_20132.txt");
            IEngine eng = FactoryEngine.GetEngine();

            //foreach (string theme in themes)
            //{
            //    List<DocumentResult> resultList;
            //    IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            //    Console.WriteLine("||||| Ranked Theme  ||||| ( {0} )" + Environment.NewLine, theme);
                
            //    DateTime start;
            //    TimeSpan timeDif;
            //    Stopwatch sw;

            //    start = DateTime.Now;
            //    sw = Stopwatch.StartNew();
            //    resultList = eng.Search(theme);
            //    sw.Stop();
            //    timeDif = sw.Elapsed;

            //    Log entry = new Log();
            //    entry.TaskDescription = smsSearch;
            //    entry.StartDateTime = start;
            //    entry.ExecutionTime = timeDif;
            //    entry.LogParameters = new List<string>();
            //    entry.LogParameters.Add("sentence: " + theme);
            //    entry.LogParameters.Add("totalDocFound: " + resultList.Count.ToString());
            //    entry.LogParameters.Add("totalDocIndexed: " + eng.TotalDocumentQuantity.ToString());
            //    entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
            //    repLog.Write(entry);

            //    WriteResultsToDisk(resultList, theme);
            //}

            foreach (string theme in tcc_poli)
            {
                List<DocumentResult> resultList;
                IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

                Console.WriteLine("||||| Ranked Query Poli ||||| ( {0} )" + Environment.NewLine, theme);

                DateTime start;
                TimeSpan timeDif;
                Stopwatch sw;

                start = DateTime.Now;
                sw = Stopwatch.StartNew();
                resultList = eng.Search(theme);
                sw.Stop();
                timeDif = sw.Elapsed;

                Log entry = new Log();
                entry.TaskDescription = smsSearch;
                entry.StartDateTime = start;
                entry.ExecutionTime = timeDif;
                entry.LogParameters = new List<string>();
                entry.LogParameters.Add("sentence: " + theme);
                entry.LogParameters.Add("totalDocFound: " + resultList.Count.ToString());
                entry.LogParameters.Add("totalDocIndexed: " + eng.TotalDocumentQuantity.ToString());
                entry.LogParameters.Add("RankTypeFunction: " + engConf.RankTypeFunction);
                repLog.Write(entry);

                string file = "20132_TCC_POLI_" + theme;
                WriteResultsToDisk(resultList, file);
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
            var strInstFile = File.ReadAllLines(file, Encoding.GetEncoding("iso-8859-1"));
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
