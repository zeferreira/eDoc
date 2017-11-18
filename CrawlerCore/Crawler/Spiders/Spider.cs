using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;
using CrawlerCore;


namespace DocCore
{

    public class Spider
    {
        string urlSiteBase;
        Uri siteBase;
        public List<DocumentCandidate> docCandidates;
        Hashtable ht;

        public Spider(string urlSite)
        {
            this.urlSiteBase = urlSite;
            this.siteBase = new Uri(urlSiteBase);
            this.ht = new Hashtable();
            this.docCandidates = new List<DocumentCandidate>();
        }

        

        public void GetCandidates(string UrlCandParameter)
        {
            Uri newUrl = new Uri(UrlCandParameter);
            List<DocumentCandidate> tmpDocCandidates = new List<DocumentCandidate>();

            if (URLFrontier.GetHeaderContentType(newUrl.OriginalString).ToLower().Contains("text/html"))
            {
                tmpDocCandidates = Crawler.GetUrlListFromWebPage(newUrl);
            }    

            if (tmpDocCandidates.Count > 0)
            {
                foreach (DocumentCandidate doc in tmpDocCandidates)
                {
                    if ((doc.Url.Host.Contains(siteBase.Host)) && (!ht.Contains(doc.OriginalUrl.GetHashCode())) && (doc.OriginalUrl.GetHashCode() != siteBase.OriginalString.GetHashCode()))
                    {
                        if (URLFrontier.GetHeaderContentType(doc.OriginalUrl).ToLower().Contains("application/pdf"))
                        {
                            docCandidates.Add(doc);
                            ht.Add(doc.OriginalUrl.GetHashCode(), doc);
                        }
                        else if (URLFrontier.GetHeaderContentType(doc.OriginalUrl).ToLower().Contains("text/html"))
                        {
                            ht.Add(doc.OriginalUrl.GetHashCode(), doc);
                            GetCandidates(doc.OriginalUrl);
                        }
                    }
                }
            }
        }
    }

    
}
