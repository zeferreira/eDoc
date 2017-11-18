using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections;
using CrawlerCore;

namespace DocCore
{
    public class URLFrontier
    {
        IRepositoryDocumentCandidate repDocCandidate;
        IRepositoryDocument repDocumentFinal;
        IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();
        Queue<Uri> uriQueue;
        RobotsFilter filter;

        string strRepDocURLFRontier = ""; 

        //To do: include DNS Cache (get ip and add to a hash for avoid dns resolution for all links in one domain)

        public URLFrontier()
        {
            this.repDocCandidate = FactoryRepositoryDocumentCandidate.GetRepositoryDocumentCandidate();
            this.repDocumentFinal = FactoryRepositoryDocument.GetRepositoryDocument();
            this.strRepDocURLFRontier = EngineConfiguration.Instance.PathRepositoryUrlFrontierFile;

            this.uriQueue = new Queue<Uri>();

            List<Document> list = this.repDocumentFinal.List();

            foreach (Document item in list)
            {
                Uri url = new Uri(item.Url);
                this.uriQueue.Enqueue(url);
            }

        }

        public bool HasNextUri()
        {
            if (uriQueue.Count > 0)
                return true;
            else
                return false;
        }

        public Uri GetNext()
        {
            Uri next = this.uriQueue.Dequeue();
            
            filter = new RobotsFilter(next, "asktume.Robot");

            if (filter.Allowed(next))
                return next;
            else
            {
                return GetNext();
            }
        }

        public void SubmitDocumentCandidate(Uri uri)
        {
            if(!repDocCandidate.Exist(uri.OriginalString.GetHashCode()))
            this.repDocCandidate.Insert(new DocumentCandidate(uri.OriginalString));
        }

        //DocCandidate roles
        #region DocCandidate
        public void ValidateDocCandidateList()
        {
            List<DocumentCandidate> initialList = this.repDocCandidate.List();

            List<DocumentCandidate> resultListApproved = new List<DocumentCandidate>();
            List<DocumentCandidate> resultListDisapproved = new List<DocumentCandidate>();

            foreach (DocumentCandidate dcn in initialList)
            {
                int hashCandidate = dcn.OriginalUrl.GetHashCode();

                if (!this.repDocumentFinal.Exist(hashCandidate))
                {
                    if (dcn.HasValidUrl())
                    {
                        string contentType = "application/pdf";

                        if (GetHeaderContentType(dcn.OriginalUrl).ToLower().Contains(contentType.ToLower()))
                        {
                            resultListApproved.Add(dcn);
                        }
                        else
                        {
                            dcn.ReasonDisapproved = "HasNoValidHEADER";
                            dcn.DateDisapproved = DateTime.Now;
                            resultListDisapproved.Add(dcn);
                        }
                    }
                    else
                    {
                        dcn.ReasonDisapproved = "HasNoValidURL";
                        dcn.DateDisapproved = DateTime.Now;
                        resultListDisapproved.Add(dcn);
                    }
                }
                else
                {
                    dcn.ReasonDisapproved = "DuplicatedURL";
                    dcn.DateDisapproved = DateTime.Now;
                    resultListDisapproved.Add(dcn);
                }
            }

            foreach (DocumentCandidate item in resultListApproved)
            {
                Uri url = new Uri(item.OriginalUrl);
                this.uriQueue.Enqueue(url);
            }

            WriteLogDisapprovedDocCandidate(resultListDisapproved);
        }

        public void ClearDocumentCandidate()
        {
            this.repDocCandidate.ClearRepository();
        }

        private void WriteLogDisapprovedDocCandidate(List<DocumentCandidate> result)
        {
            for (int i = 0; i < result.Count; i++)
            {
                string urlLog = result[i].ReasonDisapproved + "#" + result[i].OriginalUrl;

                Log entry = new Log();
                entry.TaskDescription = "URL Crawler Disapproved";
                entry.StartDateTime = result[i].DateDisapproved;
                entry.LogParameters = new List<string>();
                entry.LogParameters.Add("URL Address: " + result[i].OriginalUrl);
                entry.LogParameters.Add("Reason: " + result[i].ReasonDisapproved);

                repLog.Write(entry);
            }
        }

        #endregion

        public static string GetHeaderContentType(string url)
        {
            try
            {
                string content = string.Empty;

                HttpWebResponse response;

                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Asktume.bot";

                //validateUrl(url);              
                request.Method = "HEAD";
                response = (HttpWebResponse)request.GetResponse();

                string contentType = response.ContentType;

                response.Close();

                return contentType;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao tentar pegar o HEADER: Url -> " + url +  " - Erro: " + e.Message );
                return "";
            }
        }

    }
}
