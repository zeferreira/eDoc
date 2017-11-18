using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerCore
{
    public class DocumentCandidate
    {
        int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        bool isValidUrl;

        public DocumentCandidate(string url)
        {
            this.ID = url.GetHashCode();

            this.originalUrl = url;
            this.isValidUrl = IsValidUrl(url);
            if (this.isValidUrl)
            {
                this.url = new Uri(originalUrl);
            }
        }

        Uri url;
        public Uri Url
        {
            get { return url; }
        }
        string originalUrl;

        public string OriginalUrl
        {
            get { return originalUrl; }
        }

        string mailOwner;
        public string MailOwner
        {
            get { return mailOwner; }
            set { mailOwner = value; }
        }

        public static bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) 
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public bool HasValidUrl()
        {
            return this.isValidUrl;
        }

        string reasonDisapproved;
        public string ReasonDisapproved
        {
            get { return reasonDisapproved; }
            set { reasonDisapproved = value; }
        }

        DateTime dateDisapproved;
        public DateTime DateDisapproved
        {
            get { return dateDisapproved; }
            set { dateDisapproved = value; }
        }
    }
}
