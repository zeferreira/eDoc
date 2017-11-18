using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Timers;
using CrawlerCore;
using DocCore;

namespace CrawlerGUI
{
    public class AsktumeBotMain
    {
            System.Timers.Timer _timer;
            int period = 15000;
            Crawler asktume_bot;
            string pathFolderDownload;

            public AsktumeBotMain()
            {
                pathFolderDownload = @"G:\CrawledFiles\";

                asktume_bot = new Crawler(pathFolderDownload);

                _timer = new System.Timers.Timer(period);

                _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                _timer.Enabled = true;
            }

            public void Start()
            {
                //_timer.Start();
            }

            void _timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                    
            }

            public void ProcessCandidates()
            {
                DateTime start;
                TimeSpan timeDif;
                Stopwatch sw;

                string smsAction = "CralwerStarted".PadRight(15);

                Console.WriteLine("starting Crawler..." + DateTime.Now.ToString());
                start = DateTime.Now;
                sw = Stopwatch.StartNew();
                asktume_bot.Start();
                sw.Stop();
                timeDif = sw.Elapsed;
                Console.WriteLine("ending Crawler..." + DateTime.Now.ToString());

                Log entry = new Log();
                entry.TaskDescription = smsAction;
                entry.StartDateTime = start;
                entry.ExecutionTime = timeDif;
                entry.LogParameters = new List<string>();
                entry.LogParameters.Add("folderRepDestiny: " + pathFolderDownload);
            }
    }
}
