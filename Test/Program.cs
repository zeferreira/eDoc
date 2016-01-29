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

            int qtd = 0;
            Engine eng = new Engine();
            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string smsTimeToLoad = "Load Engine".PadRight(15);
            string smsSearch = "Search".PadRight(15);

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
            Console.WriteLine("||||| positive test - one word |||||");
            string parameters = "search";
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<WordOccurrenceNode> docList = eng.Search(parameters);
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

            foreach (WordOccurrenceNode item in docList)
            {
                qtd++;
                Console.WriteLine(qtd + " - " + item.Doc.Title + " | Occurrences: " + item.Hits.Count + "\n");
            }
             
            TestResult(docList);
            
            //search word that not exist
            Console.WriteLine("||||| negative test  |||||");

            parameters = "gimgolbel123#321456654987qqqwweqweq";
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

            qtd = 0;

            foreach (WordOccurrenceNode item in docList)
            {
                Console.WriteLine(qtd + " - " + item.Doc.Title + " | WordsQtd:" + item.Doc.WordQuantity + "\n");
            }

            TestResult(docList);

            //ShowLogEntrys();



            Console.ReadLine();
        }

        static void TestResult(List<WordOccurrenceNode> list)
        {
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
