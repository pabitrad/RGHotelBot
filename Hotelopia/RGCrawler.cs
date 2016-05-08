using System;
using BOTManager.Entities.Crawl;
using BOTManager.Entities.Crawl.Interfaces;
using BOTManager.Entities;
using System.Net;
using RG.Utility;
using RG.Core.Entities;
namespace Hotelopia
{
    public class RGCrawler : IRGCrawler
    {
        public CrawlObject GetCrawlObject(CrawlRequest rateRequest, int level)
        {           
            if (level == 1)
                return getCrawlLevel1(rateRequest);
            else
            {
                HotelopiaRequestObject oRequest = rateRequest.RequestParameterObject as HotelopiaRequestObject;
                if (oRequest.isInternalRetryFailed == true)
                {
                    return getCrawlLevel1(rateRequest);
                }
                if (oRequest.customLevel + 1 == 2)
                {
                    return getCrawlLevel2(rateRequest);
                }
                if (oRequest.customLevel + 1 == 3)
                {
                    return getCrawlLevel3(rateRequest);
                }
                if (oRequest.customLevel + 1 == 4)
                {
                    return getCrawlLevel4(rateRequest);
                }

                else
                    return null;
            }
            throw new NotImplementedException();
        }

        private static CrawlObject getCrawlLevel1(CrawlRequest crawlRequest)
        {
            CrawlObject cObj = new CrawlObject(crawlRequest);

//--------------------------------------------------------------------------from here -------------------
            //int maxMinimumLengthofStay = 30;
            //if (cObj.RateRequest.Bot.Config.PropertyBag["maxMinimumLengthofStay"] != null)
            //{
            //    maxMinimumLengthofStay = Convert.ToInt32(cObj.RateRequest.Bot.Config.PropertyBag["maxMinimumLengthofStay"].ToString());
            //}
            //if (crawlRequest.RateRequest.MinLengthOfStay > maxMinimumLengthofStay)
            //{
            //    Logger.LogInfo("Bookings can only be made for a maximum of " + maxMinimumLengthofStay + " nights. Please enter different dates and try again." + Environment.NewLine +
            //      "RequestSegmentID is :" + crawlRequest.RateRequest.RequestSegmentID);
            //    throw new NRException("Minimum length of stay should by less than or equal to " + maxMinimumLengthofStay);
            //} 
 //----------------------------------------------------------------------------to here --------------comment this code to run through TestBotConsole
          


            var rateRequest = crawlRequest.RequestParameterObject as RGRateRequest;
            string completeUrl = "http://www.hotelopia.com/?language=64&currency=" + GetCurrency(crawlRequest.RateRequest) + "";


            //SetCurrentProxy(cObj);                                                             //comment this also to run through TestBotConsole


            RGWebRequest rgWebRequest = cObj.GetNewRGWebRequest(completeUrl);
            ((HttpWebRequest)rgWebRequest.WebRequest).ContentType = "text/html; charset=utf-8";
            ((HttpWebRequest)rgWebRequest.WebRequest).Headers.Add("x-requested-with", "XMLHttpRequest");
            ((HttpWebRequest)rgWebRequest.WebRequest).Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            ((HttpWebRequest)rgWebRequest.WebRequest).Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            ((HttpWebRequest)rgWebRequest.WebRequest).Method = "GET";
            ((HttpWebRequest)rgWebRequest.WebRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            ((HttpWebRequest)rgWebRequest.WebRequest).Accept = "*/*";
            ((HttpWebRequest)rgWebRequest.WebRequest).KeepAlive = true;            
            ((HttpWebRequest)rgWebRequest.WebRequest).AllowAutoRedirect = false;
            cObj.RGWebRequests.Add(rgWebRequest);

            cObj.Level = 1;
            return cObj;
        }

        private static void SetCurrentProxy(CrawlObject cObj)
        {
            if (cObj.RateRequest.Bot.AllowProxy == true)
            {
                cObj.RateRequest.Bot.CurrentProxy = cObj.RateRequest.Bot.Proxylist.PickRandom<RGProxy>();
                Logger.LogInfo("Segment:" + cObj.RateRequest.RequestSegmentID + "  CurrentProxy :  " + cObj.RateRequest.Bot.CurrentProxy.IpAddress.ToString());
            }
        }

        private static int GetCurrency(RGRateRequest objRateRequest)
        {
            int result = 148;
            foreach (BotCurrency val in Enum.GetValues(typeof(BotCurrency)))
            {
                if (!string.IsNullOrEmpty(objRateRequest.CurrencyCode))
                {
                    if (val.ToString().ToUpper().Equals(objRateRequest.CurrencyCode.ToUpper()))
                    {
                        result = (int)val;

                    }
                }
                else
                {
                    break;
                }
            }
            if (result.Equals(148))
            {
                objRateRequest.CurrencyCode = BotCurrency.USD.ToString();
            }
            return result;
        }

        public  enum BotCurrency
        {
            USD = 148,
            DKK = 39,
            CAD = 25,
            AED = 1,
            EUR = 46,
            HKD = 57,
            ILS = 63,
            MAD = 88,
            NZD = 108,
            NOK = 106,
            MXN = 100,
            PLN = 115,
            GBP = 49,
            BRL = 19,
            RUB = 120,
            SAR = 122,
            CHF = 27,
            SGD = 127,
            ZAR = 162,
            SEK = 126,
            THB = 136,
            INR = 64,
            TRY = 142,
            BGN = 168,
            CNY = 167,
            IDR = 62,
            CZK = 37,
            KWD = 77,
            RON = 165,
            HUF = 61,
            KRW = 76,
            AUD = 8,
            ARS = 7,
            PHP = 113,
            MYR = 101
        }
            
        private static CrawlObject getCrawlLevel2(CrawlRequest crawlRequest)
        {
            CrawlObject cObj = new CrawlObject(crawlRequest);
            var rateRequest = crawlRequest.RateRequest;
            RGWebRequest rgWebRequest = null;
            HotelopiaRequestObject requestObject = cObj.RequestParameterObject as HotelopiaRequestObject;            
            string completeUrl = requestObject.URL;
            rgWebRequest = cObj.GetNewRGWebRequest(completeUrl);                        
            ((HttpWebRequest)rgWebRequest.WebRequest).ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            ((HttpWebRequest)rgWebRequest.WebRequest).Method = WebRequestMethods.Http.Post;
            rgWebRequest.PostDetails = requestObject.PostData.Trim();
            ((HttpWebRequest)rgWebRequest.WebRequest).KeepAlive = true;
            ((HttpWebRequest)rgWebRequest.WebRequest).AllowAutoRedirect=false;
            ((HttpWebRequest)rgWebRequest.WebRequest).Referer = completeUrl;         
            ((HttpWebRequest)rgWebRequest.WebRequest).Headers.Add("x-requested-with", "XMLHttpRequest");            
            ((HttpWebRequest)rgWebRequest.WebRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            ((HttpWebRequest)rgWebRequest.WebRequest).Accept = "*/*";            
            ((HttpWebRequest)rgWebRequest.WebRequest).Headers["Cookie"] = requestObject.BaseCookie;        
            cObj.RGWebRequests.Add(rgWebRequest);
            cObj.Level = 2;
            return cObj;
        }

        private static CrawlObject getCrawlLevel3(CrawlRequest crawlRequest)
        {
            CrawlObject cObj = new CrawlObject(crawlRequest);
            string PostData = string.Empty;
            var rateRequest = crawlRequest.RateRequest;            
            RGWebRequest rgWebRequest = null;
            HotelopiaRequestObject requestObject = crawlRequest.RequestParameterObject as HotelopiaRequestObject;
            string completeUrl = requestObject.URL;
            rgWebRequest = cObj.GetNewRGWebRequest(completeUrl);           
            ((HttpWebRequest)rgWebRequest.WebRequest).ContentType = "text/html; charset=utf-8";            
            ((HttpWebRequest)rgWebRequest.WebRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            ((HttpWebRequest)rgWebRequest.WebRequest).Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*";
            ((HttpWebRequest)rgWebRequest.WebRequest).KeepAlive = false;
            ((HttpWebRequest)rgWebRequest.WebRequest).AllowAutoRedirect = true;           
           ((HttpWebRequest)rgWebRequest.WebRequest).Headers["Cookie"] = requestObject.BaseCookie;
            cObj.RGWebRequests.Add(rgWebRequest);           
            cObj.Level = 3;
            return cObj;
        }

        private static CrawlObject getCrawlLevel4(CrawlRequest crawlRequest)
        {
            CrawlObject cObj = new CrawlObject(crawlRequest);
            var rateRequest = crawlRequest.RateRequest;
            RGWebRequest rgWebRequest = null;
            HotelopiaRequestObject requestObject = cObj.RequestParameterObject as HotelopiaRequestObject;
            string completeUrl = requestObject.URL;          
            if (requestObject.Reference == "MultipleRateCodes")
            {
                if (requestObject.RateCodeCollection != null && requestObject.RateCodeCollection.Count > 0)
                {                   
                    foreach (string rateCode in requestObject.RateCodeCollection)
                    {
                        CrawlObject cRateChild = new CrawlObject(crawlRequest);
                        cObj.ChildCollection.Add(cRateChild);
                        rgWebRequest = cObj.GetNewRGWebRequest(rateCode);                                              
                        ((HttpWebRequest)rgWebRequest.WebRequest).ContentType = "text/html; charset=utf-8";
                        ((HttpWebRequest)rgWebRequest.WebRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                        ((HttpWebRequest)rgWebRequest.WebRequest).Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*";                                              
                        ((HttpWebRequest)rgWebRequest.WebRequest).Headers["Cookie"] = requestObject.BaseCookie;                 
                        cObj.RGWebRequests.Add(rgWebRequest);
                    }
                }
            }

            cObj.Level = 4;
            return cObj;
        }

     

        public static WebProxy SetManualProxyAddress()
        {

            WebProxy wp = new WebProxy();
            wp.Address = new Uri("http://192.241.72.157:3128/");//("http://104.143.91.164:64058");
            wp.BypassProxyOnLocal = true;
            wp.Credentials = new NetworkCredential("", "");
            return wp;

        }
        public static WebProxy _ProxyAddress(string ipAddress, string UserName, string Password)
        {
            WebProxy wp = new WebProxy();
            wp.Address = new Uri("http://" + ipAddress);
            wp.BypassProxyOnLocal = true;
            wp.Credentials = new NetworkCredential(UserName.Trim(), Password.Trim());

            return wp;

        }
    }


}
