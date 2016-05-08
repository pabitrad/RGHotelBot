using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using RG.Utility;

namespace BOTManager.Entities.Crawl
{

    [Serializable]
    public class CrawlObject
    {
        private CrawlObjectCollection _childObjects;

        /// <summary>
        /// 
        /// </summary>
        private List<RGWebRequest> _webRequests;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public CrawlObject(CrawlRequest request)
        {
            _childObjects = new CrawlObjectCollection(this);
            this.Request = request;
            _webRequests = new List<RGWebRequest>();
        }

        /// <summary>
        /// 
        /// </summary>
        public CrawlObjectCollection ChildCollection
        {
            get
            {
                return _childObjects;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BotRequestParameterObject RequestParameterObject
        {
            get
            {
                return this.Request.RequestParameterObject;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<RGWebRequest> RGWebRequests
        {
            get
            {
                return _webRequests;
            }
        }

        public CrawlObject Parent { get; set; }


        public RGWebRequest GetNewRGWebRequest(string url)
        {

            url = (ConfigMaster.Exists("DummyURL") && !string.IsNullOrWhiteSpace(ConfigMaster.AppSetting<string>("DummyURL"))) ?
                ConfigMaster.AppSetting<string>("DummyURL") : url;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            RGWebRequest rgWebRequest = new RGWebRequest(webRequest, this, url);
            webRequest.Method = "GET";
            webRequest.ContentType = "text/html;charset=UTF-8";
            webRequest.Referer = string.Empty;
            webRequest.Headers["Cookie"] = string.Empty;
            webRequest.UserAgent = string.Empty;
            webRequest.KeepAlive = true;
            webRequest.AllowAutoRedirect = true;
            webRequest.Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*";
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Timeout = 1200000; //Default of 20 mins
            return rgWebRequest;
        }

        /// <summary>
        /// Level 
        /// </summary>
        public int Level { get; set; }

        ///// <summary>
        ///// If you want to invoke your specific request please specify.
        ///// </summary>
        //public WebRequest DefaultRequest { get; set; }

        ///// <summary>
        ///// Specific string to be assigned if Specific Request is used.
        ///// </summary>
        //public string SpecificRequestString { get; set; }

        /// <summary>
        /// In case request to be fired is JSON etc. 
        /// </summary>
        public string RequestString { get; set; }

        ////Request parameters
        //public WebRequestParameters WebParameters { get; set; }

        /// <summary>
        /// In case this is level 2,3 or 4 object.
        /// </summary>
        //public CrawlResponse CrawlResponse { get; set; }

        /// <summary>
        /// Parent Request from which this object is created.
        /// </summary>
        public CrawlRequest Request { get; private set; }


        public bool AllowProxy
        {
            get
            {
                return RateRequest != null
                    && RateRequest.Bot != null
                    && RateRequest.Bot.AllowProxy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RGRateDetail RateDetails
        {
            get
            {
                if (this.Parent != null && this.Parent.RateRequest != null)
                    return this.Parent.RateRequest.RateDetail;

                if (this.Level == 1)
                    return ((RGRateRequest)Request.RequestParameterObject).RateDetail;

                if (this.Request != null && this.Request.RateRequest != null)
                    return this.Request.RateRequest.RateDetail;

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RGRateRequest RateRequest
        {
            get
            {
                if (this.Parent != null && this.Parent.RateRequest != null)
                    return this.Parent.RateRequest;

                if (this.Level == 1)
                    return Request.RequestParameterObject as RGRateRequest;

                if (this.Request != null && this.Request.RateRequest != null)
                    return this.Request.RateRequest;

                return null;
            }
        }

        //private RGRateRequest GetRootRequest(CrawlResponse CrawlResponse)
        //{
        //    if (CrawlResponse.CrawlObject.Level == 1)
        //        return CrawlResponse.CrawlObject.Request.RequestParameterObject as RGRateRequest;
        //    else
        //        if (CrawlResponse.CrawlObject != null)
        //            return GetRootRequest(CrawlResponse.CrawlObject.CrawlResponse);
        //    return null;
        //}

        //public CrawlRequest GetPreviousCrawlRequest(int previousLevel)
        //{
        //    if (previousLevel > this.Level)
        //        throw new NotSupportedException("Only previous level is supported.");
        //    if (previousLevel == this.Level)
        //        return Request;
        //    else
        //        if (this.CrawlResponse != null)
        //            return GetPreviousCrawlRequest(this.CrawlResponse, previousLevel);
        //    return null;
        //}

        //private CrawlRequest GetPreviousCrawlRequest(Crawl.CrawlResponse crawlResponse, int previousLevel)
        //{
        //    if (crawlResponse.CrawlObject.Level == previousLevel)
        //        return crawlResponse.CrawlObject.Request;
        //    else
        //        if (crawlResponse.CrawlObject.CrawlResponse != null)
        //            return GetPreviousCrawlRequest(crawlResponse.CrawlObject.CrawlResponse, previousLevel);
        //    return null;
        //}
    }
}
