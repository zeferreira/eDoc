using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.Diagnostics;

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

            //ShowLogEntrys();



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
