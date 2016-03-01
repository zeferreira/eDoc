using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DocCore;
using System.Diagnostics;
using System.Collections;

namespace FilesReport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RunTests();

        }

        private void ShowLogResults()
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            List<Log> listEntry = repLog.List();

            foreach (Log item in listEntry)
            {
                txtReport.Text += item.ToString();
            }
        }

        private void btnRunTest_Click(object sender, EventArgs e)
        {
            RunTests();
        }

        void RunTests()
        {
            IRepositoryDocument repDoc = FactoryRepositoryDocument.GetRepositoryDocument(EnumRepositoryType.Folder);
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();
            Indexer indexer = Indexer.Instance;
            ILexicon lexicon = FactoryLexicon.GetLexicon();

            DateTime start;
            TimeSpan timeDif;
            Stopwatch sw;

            string smsRepDocSeach = "RepDocSearchAll".PadRight(50);
            string smsReadFileTXT = "Document.GetTextTXT".PadRight(50);
            string smsReadFilePDF = "Document.GetTextPDF".PadRight(50);
            string smsGetPostingList = "Document.GetPostingList".PadRight(50);
            string smsLexiconAddOccurrence = "Lexicon.AddOccurrence".PadRight(50);
            string smsIndexerIndex = "Indexer.Index".PadRight(50);

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            List<Document> listOfDocs = repDoc.Search(true);
            sw.Stop();
            timeDif = sw.Elapsed;

            Log entry = new Log();
            entry.TaskDescription = smsRepDocSeach;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalReadDocs: " + listOfDocs.Count.ToString());

            repLog.Write(entry);

            //pdfDocument
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            listOfDocs[0].GetText();
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsReadFilePDF;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + listOfDocs.Count.ToString());
            entry.LogParameters.Add("File: " + listOfDocs[0].File);

            repLog.Write(entry);

            //repeat pdf get text

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            listOfDocs[0].GetText();
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsReadFilePDF + "repeat";
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + listOfDocs.Count.ToString());
            entry.LogParameters.Add("File: " + listOfDocs[0].File);

            repLog.Write(entry);

            //again
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            listOfDocs[0].GetText();
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsReadFilePDF + "repeat again";
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + listOfDocs.Count.ToString());
            entry.LogParameters.Add("File: " + listOfDocs[0].File);

            repLog.Write(entry);

            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            listOfDocs[1].GetText();
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsReadFileTXT;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + listOfDocs[1].WordQuantity);
            entry.LogParameters.Add("File: " + listOfDocs[1].File);

            repLog.Write(entry);

            //getting posting list
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            Hashtable ht = listOfDocs[1].GetPostingList();
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsGetPostingList;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + ht.Count.ToString());
            entry.LogParameters.Add("File: " + listOfDocs[1].File);

            repLog.Write(entry);


            //Index
            start = DateTime.Now;
            sw = Stopwatch.StartNew();
            indexer.Index(listOfDocs);
            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsIndexerIndex;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + indexer.TotalWordQuantity.ToString());
            entry.LogParameters.Add("totalFiles: " + indexer.TotalDocumentQuantity.ToString());

            repLog.Write(entry);

            //Add Word Occurrence
            int totalWordQuantity = 0;
            start = DateTime.Now;
            sw = Stopwatch.StartNew();

            Hashtable postingList = listOfDocs[1].GetPostingList();
            totalWordQuantity = postingList.Count;

            //foreach (DictionaryEntry dicEntry in postingList)
            //{
            //    //get posting list and add hits
            //    //never reindex the same document 2 times.

            //    WordOccurrenceNode occurrence = dicEntry.Value as WordOccurrenceNode;

            //    lexicon.AddWordOccurrence(occurrence);
            //    totalWordQuantity += occurrence.Hits.Count;
            //}

            IDictionaryEnumerator iDicE = postingList.GetEnumerator();

            while (iDicE.MoveNext())
            {
                DictionaryEntry dicEntry = (DictionaryEntry)iDicE.Current;
                WordOccurrenceNode occurrence = dicEntry.Value as WordOccurrenceNode;

                lexicon.AddWordOccurrence(occurrence);
                totalWordQuantity += occurrence.Hits.Count;
            }

            sw.Stop();
            timeDif = sw.Elapsed;

            entry = new Log();
            entry.TaskDescription = smsLexiconAddOccurrence;
            entry.StartDateTime = start;
            entry.ExecutionTime = timeDif;
            entry.LogParameters = new List<string>();
            entry.LogParameters.Add("totalWords: " + indexer.TotalWordQuantity.ToString());
            entry.LogParameters.Add("totalFiles: " + indexer.TotalDocumentQuantity.ToString());

            repLog.Write(entry);

            ShowLogResults();
        }
    }
}
