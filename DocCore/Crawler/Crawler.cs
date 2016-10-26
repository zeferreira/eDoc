using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Configuration;

namespace DocCore
{
    public class Crawler
    {
        string folderToDownload;

        public Crawler(string folderToDownload)
        {
            this.folderToDownload = folderToDownload;
        }

        public Crawler()
        { }

        public string GetWebPageContent(string url)
        {
            try
            {
                string content = string.Empty;

                HttpWebResponse response;

                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;

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

        public List<string> GetNextPagesListFromPage(string contentPage)
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();
            Regex hrefs = new Regex("<a href.*?>");
            Regex http = new Regex("http:.*?>");

            foreach (Match m in hrefs.Matches(contentPage))
            {
                //result.Add(m.ToString());
                //sb.Append("\n");
                if (http.IsMatch(m.ToString()))
                {
                    result.Add(http.Match(m.ToString()).ToString());
                    //sb.Append(http.Match(m.ToString()));
                    //sb.Append("\n");
                }
                else
                {
                    //sb.Append(m.ToString().Substring(0, m.ToString().Length) + "\n");
                    result.Add(m.ToString().Substring(0, m.ToString().Length));
                }
            }


            return result;
        }

        public List<string> GetNextPagesListFromPage2(string contentPage)
        {
            List<string> result = new List<string>();

            Regex r = new Regex(@"<a.*?href=(""|')(?<href>.*?)(""|').*?>(?<value>.*?)</a>");

            foreach (Match match in r.Matches(contentPage))
            {
                if (!(match.Groups["href"].Value.Contains(".pdf")))
                    result.Add(match.Groups["href"].Value);
                //result.Add(match.Groups["value"].Value);
            }

            return result;
        }

        public List<string> GetDocumentListFromPage(string contentPage)
        {
            List<string> result = new List<string>();

            Regex r = new Regex(@"<a.*?href=(""|')(?<href>.*?)(""|').*?>(?<value>.*?)</a>");

            foreach (Match match in r.Matches(contentPage))
            {
                string s = match.Groups["href"].Value;

                if ((s.Contains(".pdf")) && (!result.Contains(s)))
                {
                    result.Add(match.Groups["href"].Value);
                }
            }

            return result;
        }

        public void DownloadFile(string urlFile)
        {
            using (System.Net.WebClient wc = new WebClient())
            {
                string folder = this.folderToDownload + urlFile.Split('/').Last();

                wc.DownloadFile(urlFile, folder);
            }
        }


    }
}
