using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BOTManager.Entities.Crawl
{
    [Serializable]
    public class CrawlResponse
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crawlObject"></param>
        public CrawlResponse(CrawlObject crawlObject,RGWebRequest rgWebRequest)
        {
            this.CrawlObject = crawlObject;
            this.RGWebRequest = rgWebRequest;
        }


        public Uri ResponseUri { get; set; }


        public RGWebRequest RGWebRequest { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BotRequestParameterObject RequestParameterObject
        {
            get
            {
                return CrawlObject.RequestParameterObject;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public WebResponse Response { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private CookieCollection _cookies = new CookieCollection();

        /// <summary>
        /// 
        /// </summary>
        public CookieCollection Cookies
        {
            get
            {
                return _cookies;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CrawlObject CrawlObject { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        //public CrawlObject GetPreviousCrawlObject(int level)
        //{
        //    return GetPrevCrawlObject(this.CrawlObject, level);
        //}

        //private CrawlObject GetPrevCrawlObject(CrawlObject obj, int Level)
        //{
        //    if (obj != null && obj.Level == Level)
        //        return obj;
        //    if (obj.CrawlResponse != null)
        //        return GetPrevCrawlObject(obj.CrawlResponse.CrawlObject, --Level);
        //    return null;
        //}

        /// <summary>
        /// 
        /// </summary>
        public RGRateDetail RateDetail
        {
            get
            {
                if (CrawlObject != null)
                    return CrawlObject.RateDetails;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RGRateRequest RateRequest
        {
            get
            {
                if (this.CrawlObject != null)
                    return CrawlObject.RateRequest;
                return null;
            }
        }

        /// <summary>
        /// In case response is JSON, or any specific string 
        /// </summary>
        public string ResponseString { get; set; }
    }
}
