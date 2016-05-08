using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BOTManager.Entities.Crawl
{

    public class RGWebRequest
    {


        internal RGWebRequest(WebRequest webRequest,CrawlObject parentCrawlObject,string url)
        {
            WebRequest = webRequest;
            CrawlObject = parentCrawlObject;
            Url = url;
        }
        
        public string Url { get; private set; }

        public string PostDetails { get; set; }

        public WebRequest WebRequest { get; private set; }

        public string Id { get; set; }

        public CrawlResponse Response { get; set; }

        public CrawlObject CrawlObject
        {
            get;
            private set;
        }
    }


    //[Serializable]
    //internal class WebRequestParameters
    //{

    //    public WebRequestParameters()
    //    {

    //    }

    //    public WebRequestParameters(CrawlObject obj)
    //    {
    //        //obj.WebParameters = this;
    //        Parent = obj;
    //        HeaderCollection = new WebHeaderCollection();
    //        HeaderCollection.Add("Cookie:");
    //    }

    //    public string Url { get; set; }
    //    public string WebMethod { get; set; }
    //    public string Referer { get; set; }
    //    public string Cookie { get; set; }
    //    public string PostDetails { get; set; }
    //    public string Authorization { get; set; }
    //    public IWebProxy Proxy { get; set; }
    //    public bool AutoRedirect { get; set; }
    //    //Location to be in response
    //    public bool KeepAlive { get; set; }
    //    public string UserAgent { get; set; }
    //    public string ContentType { get; set; }
    //    public string Accept { get; set; }
    //    public Version ProtocolVersion { get; set; }
    //    public WebHeaderCollection HeaderCollection { get; set; }
    //    public ICredentials Credentials { get; set; }
    //    public int TimeOut { get; set; }
    //    public CrawlObject Parent { get; set; }
    //}
}
