using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using DocCore;
using System.Threading;
using HtmlAgilityPack;
using System.Collections;
using System.Net;

namespace CrawlerCore
{
    public class Crawler
    {
        IRepositoryDocument repDocumentFinal;
        string folderToDownload;
        
        List<Thread> queue;
        bool isRunning = false;

        public bool IsRunning
        {
            get {
                return this.isRunning;
            }
        }

        public Crawler(string folderToDownload)
        {
            this.folderToDownload = folderToDownload;
            this.queue = new List<Thread>();
            isRunning = false;

            this.repDocumentFinal = FactoryRepositoryDocument.GetRepositoryDocument();
        }

        public void Start()
        {
            this.isRunning = true;
            DateTime start = DateTime.Now;

            int threadQuantLimit = 7;
                
            //front
            URLFrontier urlFrontier = new URLFrontier();
            urlFrontier.ValidateDocCandidateList();

            int tmpQuantThreadAlive = 0;
            while (urlFrontier.HasNextUri())
            {
                for (; (tmpQuantThreadAlive < threadQuantLimit) && (urlFrontier.HasNextUri()); tmpQuantThreadAlive++)
                {
                    Uri doc = urlFrontier.GetNext();
                    Thread thread = new Thread(() => DownloadFile(doc.AbsoluteUri));
                    queue.Add(thread);
                    thread.Start();
                }

                foreach (Thread th in queue)
                {
                    if (!th.IsAlive)
                    {
                        queue.Remove(th);
                        tmpQuantThreadAlive--;
                        break;
                    }
                }
            }

            while (queue.Count > 0)
            {
                foreach (Thread th in queue)
                {
                    if (!th.IsAlive)
                    {
                        queue.Remove(th);
                        tmpQuantThreadAlive--;
                        break;
                    }
                }
            }
            

            this.isRunning = false;
        }

        public void DownloadFile(string urlFile)
        {
            try
            {
                System.Net.WebClient wc = new WebClient();
                
                string _UserAgent = "Asktume.bot";
                wc.Headers.Add(HttpRequestHeader.UserAgent, _UserAgent);

                string folderFileName = this.folderToDownload + urlFile.Replace('/','#').Replace(':','$');
                Console.WriteLine(folderFileName);
                wc.DownloadFile(urlFile, folderFileName);
                
                Thread.Sleep(30);
                Document doc = new Document();

                doc.Url = urlFile;
                doc.Title = urlFile;
                doc.DocID = urlFile.GetHashCode();
                doc.File = folderFileName;
                
                Hashtable h = doc.GetPostingListClass();//load the size of document (word quantity)
                
                this.repDocumentFinal.Insert(doc);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO Downloading: " + urlFile + " - " + ex.Message);
                //throw;
            }
        }

        public static string GetWebPageContent(string url)
        {
            try
            {
                string content = string.Empty;

                HttpWebResponse response;

                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.Referer = url;
                request.AllowAutoRedirect = true;
                //validateUrl(url);              
                request.Method = "Get";
                response = (HttpWebResponse)request.GetResponse();

                // Display the status.  
                //Console.WriteLine(response.StatusDescription);
                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string _responseFromServer = reader.ReadToEnd();
                // Display the content.  
                //Console.WriteLine(_responseFromServer);
                // Cleanup the streams and the response.  
                reader.Close();
                dataStream.Close();
                response.Close();

                return _responseFromServer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

        public static List<DocumentCandidate> GetUrlListFromWebPage(Uri url)
        {
            List<DocumentCandidate> urlList = new List<DocumentCandidate>();

            string content = GetWebPageContent(url.OriginalString);

            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(content);

            HtmlNodeCollection nodesIframe = doc.DocumentNode.SelectNodes("//iframe[@src]");

            if (nodesIframe != null)
            {
                foreach (var item in nodesIframe)
                {
                    HtmlAttribute attr = item.Attributes["src"];
                    string originalUrl = ResolveUrl(attr.Value, url);

                    DocumentCandidate newDoc = new DocumentCandidate(originalUrl);

                    urlList.Add(newDoc);
                }
            }

            HtmlNodeCollection nodesFrame = doc.DocumentNode.SelectNodes("//frame[@src]");

            if (nodesFrame != null)
            {
                foreach (var item in nodesFrame)
                {
                    HtmlAttribute attr = item.Attributes["src"];
                    string originalUrl = ResolveUrl(attr.Value, url);

                    DocumentCandidate newDoc = new DocumentCandidate(originalUrl);

                    urlList.Add(newDoc);
                }
            }

            HtmlNodeCollection nodesHref = doc.DocumentNode.SelectNodes("//a[@href]");

            if (nodesHref != null)
            {
                foreach (HtmlNode link in nodesHref)
                {
                    HtmlAttribute attr = link.Attributes["href"];
                    string originalUrl = ResolveUrl(attr.Value, url);

                    DocumentCandidate newDoc = new DocumentCandidate(originalUrl);

                    urlList.Add(newDoc);
                }
            }

            return urlList;
        }
        private static string ResolveUrl(string url, Uri urlBase)
        {
            Uri uri;

            if (Uri.TryCreate(urlBase, url, out uri))
            {
                return uri.OriginalString;
            }

            return null;

        }
    }
}
