using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using CrawlerCore;
using System.Threading;
using System.Collections;


namespace CrawlerGUI
{
    class Program
    {
        static void Main(string[] args)
        {
            ////spider = create candidates 
            string strUrl = @"http://tcc.ecomp.poli.br/";
            //string strUrl = @"http://singhal.info/home";

            Spider sp = new Spider(strUrl);

            sp.GetCandidates(strUrl);

            PrintList(sp.docCandidates);

            string strFolder = @"G:\CrawlerTest\";

            Crawler c = new Crawler(strFolder);
            URLFrontier urlServer = new URLFrontier();

            foreach (DocumentCandidate item in sp.docCandidates)
            {
                urlServer.SubmitDocumentCandidate(item.Url);
            }

            c.Start();

            IRepositoryDocument repDoc = FactoryRepositoryDocument.GetRepositoryDocument();

            List<Document> list = repDoc.List();

            foreach (Document item in list)
            {
                Console.WriteLine("Url: " + item.Url);
                Console.WriteLine("WordQuantity: " + item.WordQuantity);

                if (item.WordQuantity == 0)
                    Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ " + item.File);
            }
            
            Console.ReadLine();
        }

        public static void PrintList(List<DocumentCandidate> list)
        {
            foreach (DocumentCandidate item in list)
            {
                Console.WriteLine(item.OriginalUrl);
            }
        }
    }
}
