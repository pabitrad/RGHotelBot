using System;
using BOTManager.Entities.Crawl;
using BOTManager.Entities.Crawl.Interfaces;
using BOTManager.Entities;
using System.Net;
using RG.Utility;
using RG.Core.Entities;
using System.Text;

namespace Hoteloasia
{
    public class RGCrawler : IRGCrawler
    {
        public CrawlObject GetCrawlObject(CrawlRequest rateRequest, int level)
        {   
            try
            {
                if (level == 1)
                    return getCrawlLevel1(rateRequest);
                else
                    return null;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in GetCrawlObject : " + Environment.NewLine + ex.Message);
            }

            return null;
        }

        private static CrawlObject getCrawlLevel1(CrawlRequest crawlRequest)
        {
            try
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
                string completeUrl = getCompleteUrl(crawlRequest);

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
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in getCrawlLevel1 : " + Environment.NewLine + ex.Message);
            }

            return null;
        }

        private static string getCompleteUrl(CrawlRequest crawlRequest)
        {
            try
            {
                StringBuilder completeUrl = new StringBuilder("http://booking.hoteloasia.com/domino.aspx?from=");
                RGRateRequest rateRequest = crawlRequest.RequestParameterObject as RGRateRequest;

                completeUrl.Append(rateRequest.CheckInDate.ToString("dd-MM-yyyy"));
                completeUrl.Append("&to=");
                completeUrl.Append(rateRequest.CheckOutDate.ToString("dd-MM-yyyy"));
                completeUrl.Append("&adults=");
                completeUrl.Append(rateRequest.Guests.ToString());
                completeUrl.Append("&infants_2=0&p_cmc=&infants_1=0&lang=en&");
                completeUrl.Append("hotelid=" + rateRequest.PropertyID);
                completeUrl.Append("&p_arr=");
                completeUrl.Append(rateRequest.CheckInDate.ToString("yyyyMMdd"));
                completeUrl.Append("_0000&p_dep=");
                completeUrl.Append(rateRequest.CheckOutDate.ToString("yyyyMMdd"));
                completeUrl.Append("_0000&p_pax=");
                completeUrl.Append(rateRequest.Guests.ToString());
                completeUrl.Append("_0");

                return completeUrl.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in getCompleteUrl : " + Environment.NewLine + ex.Message);
            }

            return null;
        }
    }
}
