using System;
using System.Collections.Generic;
using System.Text;
using BOTManager.Entities;
using System.Text.RegularExpressions;
using BOTManager.Entities.Crawl;
using BOTManager.Entities.Crawl.Interfaces;
using System.IO;
using System.Data;
using System.Collections;
using BOTManager.Entities.Utility;
using System.Reflection;
using RG.Utility;


namespace Hotelopia
{

    public class HotelopiaRequestObject : BotRequestParameterObject
    {
        public string PostData { get; set; }
        public string URL { get; set; }
        public string WebPageResponse { get; set; }
        public List<string> RateCodeCollection { get; set; }        
        public string BaseCookie { get; set; }
        public string referer { get; set; }
        public bool isInternalRetryFailed { get; set; }
        public int customLevel { get; set; }
        public int baseRetry { get; set; }
    }

    public class RGParser : IRGParser
    {

        public RGRateRequest _rateRequest;      
        public static string baseCookie = string.Empty;        
        private string finalResponse = string.Empty;
        private string referer = string.Empty;
         public int customLevel = 1;
        public int baseRetry = 0;
        public bool isInternalRetryFailed = false; 
        private static Hashtable currencies = new Hashtable();
        public ParseResult ParseResponse(CrawlObject crawlObject)
        {
           
            if (crawlObject.Level == 1)
                return getParseLevel1(crawlObject.RGWebRequests[0].Response);
            else
            {
                HotelopiaRequestObject oRequest = crawlObject.RequestParameterObject as HotelopiaRequestObject;
                if (oRequest.baseRetry > 6)
                    return null;
                if (oRequest.isInternalRetryFailed == true)
                {
                    return getParseLevel1(crawlObject.RGWebRequests[0].Response);
                }
                if (oRequest.customLevel + 1 == 2)
                {
                    return getParseLevel2(crawlObject.RGWebRequests[0].Response);
                }
                if (oRequest.customLevel + 1 == 3)
                {
                    return getParseLevel3(crawlObject.RGWebRequests[0].Response);
                }
                if (oRequest.customLevel + 1 == 4)
                {
                    return getParseLevel4(crawlObject);
                }
                else
                    return null;
            }
        }

        public ParseResult getParseLevel1(CrawlResponse crawlResponse)
        {
            ParseResult parseResult = new ParseResult(crawlResponse);

            string strResponse = crawlResponse.ResponseString;          
            string cookie = GetCookieFromHeaderCollection(crawlResponse.ResponseHeaders);           
            string completeUrl = string.Empty;
            string postData = string.Empty;
            customLevel = 1;
            isInternalRetryFailed = false;
            baseRetry += 1;
            if (baseRetry > 4)
            {                
                ParseForData("Bad Request", crawlResponse.RateDetail, crawlResponse.RateRequest, "");
                parseResult.NextLevelRequestParameterObject = null;
                return parseResult;
            }
            HotelopiaRequestObject requestParamObject = new HotelopiaRequestObject();
            if (!string.IsNullOrEmpty(cookie) && !string.IsNullOrWhiteSpace(strResponse))
            {
                baseCookie = fGetCookie(cookie);
                referer = "http://www.hotelopia.com/";
                completeUrl = "http://www.hotelopia.com/TaskManager/resultHandler.ashx";                
                postData = GetPostData(crawlResponse.RateRequest, crawlResponse.RateRequest.CheckInDate, crawlResponse.RateRequest.CheckOutDate, strResponse);
                Logger.LogInfo("Segment:" + crawlResponse.RateRequest.RequestSegmentID + "  Postdata :  " + Environment.NewLine + postData);
                requestParamObject.PostData = postData;
                requestParamObject.WebPageResponse = crawlResponse.ResponseString;
                requestParamObject.URL = completeUrl;
                requestParamObject.referer = referer;
                requestParamObject.BaseCookie = baseCookie;
            }
            else
            {
                customLevel = 1;
                isInternalRetryFailed = true;               
            }
            requestParamObject.customLevel = customLevel;
            requestParamObject.isInternalRetryFailed = isInternalRetryFailed;
            requestParamObject.baseRetry = baseRetry;            
            parseResult.NextLevelRequestParameterObject = requestParamObject;
            return parseResult;
        }


        public ParseResult getParseLevel2(CrawlResponse crawlResponse)
        {
            ParseResult parseResult = new ParseResult(crawlResponse);
            HotelopiaRequestObject requestParamObject = new HotelopiaRequestObject();
            string completeUrl = string.Empty;
            string _Url = crawlResponse.ResponseString.ToString().Trim();
            customLevel = 2;
            isInternalRetryFailed = false;
            if (!string.IsNullOrEmpty(_Url))
            {
                if (_Url.Contains("results.aspx"))
                {
                    completeUrl = "http://www.hotelopia.com/results.aspx";
                }
                else
                {
                    completeUrl = "http://hotels.hotelopia.com/infohotel.aspx?codeHotel=" + crawlResponse.RateRequest.PropertyID + "";                    
                }
                requestParamObject.Reference = "SingleDayCrawling";
                string cookie = GetCookieFromHeaderCollection(crawlResponse.ResponseHeaders);
                requestParamObject.URL = completeUrl;
                requestParamObject.BaseCookie = baseCookie;
            }
            else
            {
                customLevel = 1;
                isInternalRetryFailed = true;               
            }
            requestParamObject.customLevel = customLevel;
            requestParamObject.isInternalRetryFailed = isInternalRetryFailed;
            requestParamObject.baseRetry = baseRetry;           
            parseResult.NextLevelRequestParameterObject = requestParamObject;

            return parseResult;
        }


        public ParseResult getParseLevel3(CrawlResponse crawlResponse)
        {
          
            ParseResult parseResult = new ParseResult(crawlResponse);
            HotelopiaRequestObject requestParamObject = new HotelopiaRequestObject();
            string completeUrl = string.Empty;
            finalResponse = crawlResponse.ResponseString;
            customLevel = 3;
            isInternalRetryFailed = false;

            if (string.IsNullOrWhiteSpace(finalResponse)
                                || (finalResponse.IndexOf("Para continuar, debemos comprobar que no eres un robot") > -1)
                                || (finalResponse.IndexOf("We have implemented this step to prevent unauthorized usage of our site.") > -1)
                                || (finalResponse.IndexOf("Please Login to access the internet") > -1
                                || finalResponse.IndexOf("Welcome to the CoDeeN HTTP CDN Service") > -1 || finalResponse.IndexOf("Please enter Verification code") >= 0
                                || finalResponse.IndexOf("please type the characters as you see in the box") > -1 || finalResponse.IndexOf("<input type=\"hidden\" name=\"errorPage\"") > -1))
            {
                
                customLevel = 1;
                isInternalRetryFailed = true;
                requestParamObject.customLevel = customLevel;
                requestParamObject.isInternalRetryFailed = isInternalRetryFailed;
                requestParamObject.baseRetry = baseRetry;
                parseResult.NextLevelRequestParameterObject = requestParamObject;
                return parseResult;
            }
            Regex rgxBlk = new Regex(@"Your hotel selected</li><li class=""hotel bloque box"" id=""result-1"">(?<Block>[\s\S]*?)</li>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            List<string> RateCodeCollection = new List<string>();
            string SelectedRateBlk = rgxBlk.Match(crawlResponse.ResponseString).Value;

            if (!string.IsNullOrWhiteSpace(SelectedRateBlk))
            {
                rgxBlk = new Regex(@"<tr id=""SYS_hotel_room_table""><td[\s\S]*?class=""morePrices"">\s*<a rel=""viewMorePrices""[\s\S]*?>See more</a></td></tr>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                MatchCollection _MoreratesButtonCount = rgxBlk.Matches(SelectedRateBlk);

                if (_MoreratesButtonCount.Count > 0)
                {
                    // for code for multiple hit...
                    HotelopiaRequestObject nextParameterObject = new HotelopiaRequestObject();

                    nextParameterObject.Reference = "MultipleRateCodes";

                    for (int j = 0; j < _MoreratesButtonCount.Count; j++)
                    {
                        string DirectValue = string.Empty;
                        if (_MoreratesButtonCount[j].Value.Contains("false"))
                            DirectValue = "false";
                        else
                            DirectValue = "true";
                        RateCodeCollection.Add("http://www.hotelopia.com/TaskManager/hotels/HotelsHandler.ashx?fnc=getMorePrices&hotelcode="
                            + crawlResponse.RateRequest.PropertyID + "&direct=" + DirectValue + "&noCache=" + GetMilliseconds(System.DateTime.Now) + "");

                    }

                    requestParamObject.Reference = "MultipleRateCodes";// "SingleDayCrawling";
                    requestParamObject.URL = completeUrl;
                    requestParamObject.RateCodeCollection = RateCodeCollection;
                    requestParamObject.BaseCookie = baseCookie;
                    requestParamObject.customLevel = customLevel;
                    requestParamObject.isInternalRetryFailed = isInternalRetryFailed;
                    requestParamObject.baseRetry = baseRetry; 
                    parseResult.NextLevelRequestParameterObject = requestParamObject;

                }
                else
                {
                    ParseDataandcachePase(crawlResponse, parseResult);
                }
            }
            else
            {
                ParseDataandcachePase(crawlResponse, parseResult);
            }
            
            return parseResult;
        }

        private void ParseDataandcachePase(CrawlResponse crawlResponse, ParseResult parseResult)
        {
            ParseForData(crawlResponse.ResponseString, crawlResponse.RateDetail, crawlResponse.RateRequest, "");
            finalResponse = cachePageCreation(crawlResponse.ResponseString, crawlResponse.RateDetail, crawlResponse.RateRequest);
            finalResponse = finalResponse.Replace("<head>", @"<head> <base href=""http://www.hotelopia.com/"" />");
            crawlResponse.RateDetail.SetCachePageContent(finalResponse);
            parseResult.NextLevelRequestParameterObject = null;
        }
        public ParseResult getParseLevel4(CrawlObject crawlObject)
        {
            CrawlResponse crawlResponse = crawlObject.RGWebRequests[0].Response;
            ParseResult parseResult = new ParseResult(crawlResponse);
            string MoreRate = string.Empty;
            customLevel = 4;
            isInternalRetryFailed = false;
            if (crawlObject.RGWebRequests.Count > 0)
            {
                
                    for (int i = 0; i < crawlObject.RGWebRequests.Count; i++)
                    {
                        MoreRate += crawlObject.RGWebRequests[i].Response.ResponseString;
                    }

                    
            }
            ParseForData(finalResponse, crawlResponse.RateDetail, crawlResponse.RateRequest, MoreRate);
            finalResponse = cachePageCreation(finalResponse, crawlResponse.RateDetail, crawlResponse.RateRequest);
            finalResponse = finalResponse.Replace("<head>", @"<head> <base href=""http://www.hotelopia.com/"" />");
            crawlResponse.RateDetail.SetCachePageContent(finalResponse);
            parseResult.NextLevelRequestParameterObject = null; //set null at final parsing 
            return parseResult;
        }


        public static string cachePageCreation(string strResponse,RGRateDetail rd,RGRateRequest objRateRequest)
        {
            string strRateBLKResp = Regex.Match(strResponse, @"<table cellspacing=""0""\s*cellpadding=""0""\s*class=""dispo-table single single-with-offers""\s*id=""dispo-" + objRateRequest.PropertyID + @""">[\s\S]*?</table>|id=""dispo-" + objRateRequest.PropertyID + @"""[\s\S]*?</table>", RegexOptions.IgnoreCase).Value;
            if (!string.IsNullOrEmpty(strRateBLKResp))
            {
                StringBuilder sbRateBlk = new StringBuilder();
                string strRateBlkNew = string.Empty;

                Assembly _assembly = Assembly.GetExecutingAssembly();
                string[] _strPageTemplate = new StreamReader(_assembly.GetManifestResourceStream("Hotelopia.Template.htm")).ReadToEnd().Split('^');
                sbRateBlk.Length = 0;
                try
                {
                    for (int Rt = 0; Rt < rd.RoomTypeDetails.Count; Rt++)
                    {
                        if (((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RateDesc.ToLower().IndexOf("when") > 0)
                        {
                            sbRateBlk.Append(_strPageTemplate[1].Replace("---RoomType---",
                                ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RoomDesc)
                                .Replace("---BordType---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RateDesc.Substring(0, ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RateDesc.ToLower().IndexOf("when")))
                                .Replace("---When---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RateDesc)
                                .Replace("---PerNightPrice---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).AvgDailyAmount + " " + ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).Currency)
                                 .Replace("---TotalPrice---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).StayAmount + " " + ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).Currency)
                                );
                        }
                        else
                        {
                            sbRateBlk.Append(_strPageTemplate[1].Replace("---RoomType---",
                                     ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RoomDesc)
                                     .Replace("---BordType---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RateDesc)
                                     .Replace("---When---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).RateDesc)
                                     .Replace("---PerNightPrice---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).AvgDailyAmount + " " + ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).Currency)
                                      .Replace("---TotalPrice---", ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).StayAmount + " " + ((RoomTypeDetail)(rd.RoomTypeDetails[Rt])).Currency)
                                     );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("Exception occure in cachePageCreation : " + Environment.NewLine + ex.Message);
                }
                strRateBlkNew = _strPageTemplate[0] + sbRateBlk.ToString() + _strPageTemplate[2];
                strResponse = strResponse.Replace(strRateBLKResp, strRateBlkNew);
            }
            return strResponse;
        }
      



        private string fGetCookie(string _Page)
        {
            try
            {
                string str = string.Empty;
                Regex regex = new Regex(@"[\s\S]*?ASP.NET_SessionId=(?<Session>([\s\S]*?));");
                if (!string.IsNullOrEmpty(regex.Match(_Page).Groups["Session"].Value))
                    str = "ASP.NET_SessionId=" + regex.Match(_Page).Groups["Session"].Value;
                regex = new Regex(@"[\s\S]*?AlteonP=(?<AlteonP>([\s\S]*?));");
                if (!string.IsNullOrEmpty(regex.Match(_Page).Groups["AlteonP"].Value))
                    str += "; AlteonP=" + regex.Match(_Page).Groups["AlteonP"].Value + ";";
                regex = new Regex(@"avr_(?<Avr>[\s\S]*?)=(?<Avr1>[\s\S]*?);");
                if (!string.IsNullOrEmpty(regex.Match(_Page).Groups["Avr"].Value))
                    str += "avr_" + regex.Match(_Page).Groups["Avr"].Value + "=" + regex.Match(_Page).Groups["Avr1"].Value + ";";
                return str;
            }
            catch(Exception ex)
            {
                Logger.LogWarning("Exception occure in fGetCookie : " + Environment.NewLine + ex.Message);
                return null;
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
        long GetMilliseconds(System.DateTime dt)
        {
            long daysFromEpoch = _d(dt.Year, dt.Month, dt.Day) - _d(1970, 1, 1);
            long millis = 0;
            millis = daysFromEpoch * 86400000;
            millis += dt.Hour * 3600000; 
            millis += dt.Minute * 60000;  
            millis += dt.Second * 1000;
            millis += dt.Millisecond;
            return millis;
        }
        long _d(int y, int m, int d)
        {
            int[] days = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            return (((y - 1901) * 1461) / 4) + days[m - 1] + d
                + (m > 2 && (y % 4 == 0 && y % 100 != 0 || y % 400 == 0) ? 1 : 0);
        }
        public enum BotCurrency
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



        private static string SetFirstURL(string strPropertyID, string strCountry)
        {
            string completeUrl = string.Empty;
            if (string.IsNullOrEmpty(strCountry))
            {
                completeUrl = "https://reservations.synxis.com/XBE/rez.aspx?Hotel=" + strPropertyID;
            }
            else
            {
                completeUrl = "https://reservations.synxis.com/XBE/rez.aspx?Hotel=" + strPropertyID + "&Chain=" + strCountry + "&locale=en-US";

            }
            return completeUrl;
        }

        public string PostDataCurrencyText(string strGetResponseValue)
        {
            DataTable dtInput = StringUtil.FGetDataTable(strGetResponseValue, @"((?:<input\s*type|<input\s*name|<select\s*name)[\s\S]*?>)", 1);
            string strInputName = "";
            foreach (DataRow drInput in dtInput.Rows)
            {
                strInputName = StringUtil.GetStringResult(drInput[0].ToString(), @"name=""(?<name>[\s\S]*?)""[\s\S]*?(?:value=""(?<value>[\s\S]*?)""|\s*)|name='(?<name>[\s\S]*?)'[\s\S]*?(?:value='(?<value>[\s\S]*?)'|\s*)", 1);

                if (Regex.IsMatch(strInputName, "ddlCurrencies", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    strInputName = strInputName.Trim().Replace("$", "%24");
                    break;
                }
            }
            return strInputName;
        }

        public string GetCurrencyIntegerValue(string _Currency)
        {
            string strCurIntVal = string.Empty;
            switch (_Currency)
            {
                case "AFN":
                    strCurIntVal = "40";
                    break;
                case "ALL":
                    strCurIntVal = "39";
                    break;
                case "AOA":
                    strCurIntVal = "43";
                    break;
                case "ARS":
                    strCurIntVal = "44";
                    break;
                case "AMD":
                    strCurIntVal = "41";
                    break;
                case "AWG":
                    strCurIntVal = "56";
                    break;
                case "AUD":
                    strCurIntVal = "9";
                    break;
                case "AZN":
                    strCurIntVal = "91";
                    break;
                case "BSD":
                    strCurIntVal = "55";
                    break;
                case "BHD":
                    strCurIntVal = "78";
                    break;
                case "BMD":
                    strCurIntVal = "2";
                    break;

                case "BWP":
                    strCurIntVal = "97";
                    break;
                case "BRL":
                    strCurIntVal = "10";
                    break;
                case "GBP":
                    strCurIntVal = "11";
                    break;
                case "BGN":
                    strCurIntVal = "56";
                    break;
                case "CAD":
                    strCurIntVal = "3";
                    break;
                case "CLP":
                    strCurIntVal = "48";
                    break;
                case "CNY":
                    strCurIntVal = "29";
                    break;
                case "COP":
                    strCurIntVal = "37";
                    break;
                case "XOF":
                    strCurIntVal = "75";
                    break;
                case "CRC":
                    strCurIntVal = "47";
                    break;
                case "HRK":
                    strCurIntVal = "83";
                    break;
                case "CUC":
                    strCurIntVal = "66";
                    break;
                case "CYP":
                    strCurIntVal = "27";
                    break;
                case "CZK":
                    strCurIntVal = "61";
                    break;
                case "DKK":
                    strCurIntVal = "30";
                    break;
                case "DOP":
                    strCurIntVal = "46";
                    break;
                case "XCD":
                    strCurIntVal = "53";
                    break;
                case "EGP":
                    strCurIntVal = "60";
                    break;
                case "EEK":
                    strCurIntVal = "84";
                    break;
                case "EUR":
                    strCurIntVal = "5";
                    break;
                case "FJD":
                    strCurIntVal = "59";
                    break;
                case "GEL":
                    strCurIntVal = "93";
                    break;
                case "GHS":
                    strCurIntVal = "86";
                    break;
                case "GNF":
                    strCurIntVal = "67";
                    break;
                case "GYD":
                    strCurIntVal = "54";
                    break;
                case "HTG":
                    strCurIntVal = "51";
                    break;
                case "HKD":
                    strCurIntVal = "12";
                    break;
                case "HUF":
                    strCurIntVal = "88";
                    break;
                case "ISK":
                    strCurIntVal = "31";
                    break;
                case "INR":
                    strCurIntVal = "14";
                    break;

                case "IDR":
                    strCurIntVal = "13";
                    break;
                case "IQD":
                    strCurIntVal = "103";
                    break;
                case "ISL":
                    strCurIntVal = "90";
                    break;
                case "JPY":
                    strCurIntVal = "15";
                    break;
                case "JOD":
                    strCurIntVal = "79";
                    break;
                case "KZT":
                    strCurIntVal = "92";
                    break;
                case "KES":
                    strCurIntVal = "77";
                    break;
                case "KWD":
                    strCurIntVal = "32";
                    break;
                case "MOP":
                    strCurIntVal = "64";
                    break;
                case "MGA":
                    strCurIntVal = "68";
                    break;
                case "MYR":
                    strCurIntVal = "18";
                    break;
                case "MVR":
                    strCurIntVal = "98";
                    break;
                case "MTL":
                    strCurIntVal = "28";
                    break;
                case "MRU":
                    strCurIntVal = "102";
                    break;
                case "MXN":
                    strCurIntVal = "17";
                    break;
                case "MAD":
                    strCurIntVal = "45";
                    break;
                case "MZN":
                    strCurIntVal = "62";
                    break;
                case "NAD":
                    strCurIntVal = "99";
                    break;
                case "NPR":
                    strCurIntVal = "87";
                    break;
                case "ANG":
                    strCurIntVal = "42";
                    break;
                case "NZD":
                    strCurIntVal = "19";
                    break;
                case "NIO":
                    strCurIntVal = "69";
                    break;
                case "NOK":
                    strCurIntVal = "33";
                    break;
                case "OMR":
                    strCurIntVal = "34";
                    break;
                case "XPF":
                    strCurIntVal = "58";
                    break;
                case "PGK":
                    strCurIntVal = "94";
                    break;
                case "PYG":
                    strCurIntVal = "50";
                    break;

                case "PEN":
                    strCurIntVal = "20";
                    break;
                case "PHP":
                    strCurIntVal = "6";
                    break;
                case "PLN":
                    strCurIntVal = "63";
                    break;
                case "QAR":
                    strCurIntVal = "35";
                    break;
                case "RON":
                    strCurIntVal = "70";
                    break;
                case "RUB":
                    strCurIntVal = "21";
                    break;
                case "SAR":
                    strCurIntVal = "22";
                    break;
                case "SCR":
                    strCurIntVal = "96";
                    break;
                case "SGD":
                    strCurIntVal = "23";
                    break;
                case "ZAR":
                    strCurIntVal = "26";
                    break;
                case "KRW":
                    strCurIntVal = "16";
                    break;
                case "SSP":
                    strCurIntVal = "95";
                    break;
                case "LKR":
                    strCurIntVal = "100";
                    break;
                case "SRD":
                    strCurIntVal = "71";
                    break;
                case "SEK":
                    strCurIntVal = "36";
                    break;
                case "CHF":
                    strCurIntVal = "4";
                    break;
                case "TWD":
                    strCurIntVal = "24";
                    break;
                case "TJS":
                    strCurIntVal = "72";
                    break;
                case "TZS":
                    strCurIntVal = "101";
                    break;
                case "THB":
                    strCurIntVal = "7";
                    break;
                case "TTD":
                    strCurIntVal = "52";
                    break;
                case "TND":
                    strCurIntVal = "38";
                    break;
                case "TRY":
                    strCurIntVal = "73";
                    break;
                case "TMT":
                    strCurIntVal = "81";
                    break;
                case "UGX":
                    strCurIntVal = "74";
                    break;
                case "UAH":
                    strCurIntVal = "82";
                    break;
                case "USD":
                    strCurIntVal = "1";
                    break;
                case "AED":
                    strCurIntVal = "8";
                    break;
                case "VUV":
                    strCurIntVal = "89";
                    break;
                case "VEF":
                    strCurIntVal = "80";
                    break;
                case "VEB":
                    strCurIntVal = "25";
                    break;
                case "VND":
                    strCurIntVal = "85";
                    break;

                default:
                    strCurIntVal = "1";
                    break;
            }
            return strCurIntVal;
        }

        public string GetCurrencyValue(RGRateRequest objRateRequest, string response)
        {
            string defaultCurrency = string.Empty;
            string Curr = string.Empty;
            if (!string.IsNullOrEmpty(objRateRequest.CurrencyCode))
            {
                response = Regex.Match(response, @"CurrencySelectionDiv[\s\S]*?</ul>", RegexOptions.IgnoreCase | RegexOptions.Multiline).Value;
                MatchCollection matchCollection = Regex.Matches(response, @"<li\s*class[\s\S]*?CurrencyCodeHF&#39;,\s*&#39;\s*(?<Value>[\s\S]*?)\s*&[\s\S]*?>\s*(?<Currency>[\s\S]*?)\s*</a>|<li\s*class[\s\S]*?CurCode[\s\S]*?"">(?<Currency>[\s\S]*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (Match mtch in matchCollection)
                {
                    Curr = mtch.Groups["Currency"].Value;
                    if (mtch.Groups["Currency"].Value.Trim().ToUpper().Equals(objRateRequest.CurrencyCode.ToUpper()))
                    {
                        defaultCurrency = mtch.Groups["Value"].Value.Trim();
                        if (string.IsNullOrEmpty(defaultCurrency))
                        {
                            defaultCurrency = Regex.Match(response, @"" + Curr + @"[\s\S]*?onclick=""SelCur\((?<Value>[\s\S]*?)\)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups["Value"].Value;
                        }
                        break;
                    }
                }
            }
            return defaultCurrency;
        }


        private static string GetCookieFromHeaderCollection(Dictionary<string, string> responseHeader)
        {
            string strCookie = string.Empty;
            if (responseHeader != null)
            {
                foreach (var item in responseHeader)
                {
                    if (item.Key == "Set-Cookie")
                    {
                        strCookie = item.Value;
                        break;
                    }
                }
            }
            return strCookie;
        }


        private string GetPostData(RGRateRequest objRateRequest, DateTime tempCheckInDate, DateTime tempCheckOutDate, string strFinalResponse)
        {
            Regex objData = new Regex(@"zoneid[\s\S]*?name=""(?<Data1>[\s\S]*?)""[\s\S]*?value=""(?<Data2>[\s\S]*?)""", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            long timespan = GetTime(tempCheckInDate);

            if (string.IsNullOrWhiteSpace(objData.Match(strFinalResponse).Groups["Data1"].Value) 
                || string.IsNullOrWhiteSpace(objData.Match(strFinalResponse).Groups["Data2"].Value))
            {
                customLevel = 1;
                isInternalRetryFailed = true;
            }
            StringBuilder strBuidPostdata = new StringBuilder("_dest=%2Fresults.aspx"
                + "&fts=HN%3A" + objRateRequest.PropertyID
                + "&textDestination=Amari+Watergate"
                + "&DayIn=" + tempCheckInDate.ToString("dd")
                + "&MonthIn=" + tempCheckInDate.ToString("MM")
                + "&YearIn=" + tempCheckInDate.Year
                + "&DayOut=" + tempCheckOutDate.ToString("dd")
                + "&MonthOut=" + tempCheckOutDate.ToString("MM")
                + "&YearOut=" + tempCheckOutDate.Year
                + "&RO=1"
                + "&advanced=false"
                + "&typeProductSearch=hotels"
                + "&countryID=-1"
                + "&destinationID=-1"
                + "&zoneID=-1"
                + "&" + objData.Match(strFinalResponse).Groups["Data1"].Value + "=" + objData.Match(strFinalResponse).Groups["Data2"].Value
                + "&geoPoint="
                + "&HA1=" + objRateRequest.Guests + "&HN1=0");

            return strBuidPostdata.ToString();
        }

        private Int64 GetTime(DateTime nDate)
        {
            Int64 retval = 0;
            DateTime st = new DateTime(1970, 1, 1).ToLocalTime();
            TimeSpan t = (nDate - st);
            retval = (Int64)(t.TotalMilliseconds);

            return retval;
        }

        void AddRoomDetails(RGRateDetail rd, string roomDesc, string rateDesc, string currency, decimal stayAmount, decimal avgDailyAmount, decimal discountedRate, int maxPersons, char _MarchentRate, char _RateChangeIndicator)
        {
            rd.AddRoomTypeRateDetail(roomDesc, rateDesc, "", "", "", currency, stayAmount, avgDailyAmount, discountedRate, maxPersons, _MarchentRate, _RateChangeIndicator);
        }
        private static void FillCurrencies()
        {
            currencies.Add("$", "USD");
            currencies.Add("&pound;", "GBP");
            currencies.Add("€", "EUR");
            currencies.Add("&euro;", "EUR");
            currencies.Add("\x00a3", "GBP");
            currencies.Add("SGD", "SGD");
            currencies.Add("HKD", "HKD");
            currencies.Add("AUD", "AUD");
            currencies.Add("CAD$", "CAD");
            currencies.Add("CZK", "CZK");
            currencies.Add("NZD$", "NZD");
            currencies.Add("SEK", "SEK");
            currencies.Add("THB", "THB");
            currencies.Add("ARS", "ARS");
            currencies.Add("BGN", "BGN");
            currencies.Add("CNY", "CNY");
            currencies.Add("DKK", "DKK");
            currencies.Add("AED", "AED");
            currencies.Add("INR", "INR");
            currencies.Add("IDR", "IDR");
            currencies.Add("ILS", "ILS");
            currencies.Add("KWD", "KWD");
            currencies.Add("RON", "RON");
            currencies.Add("HUF", "HUF");
            currencies.Add("MYR", "MYR");
            currencies.Add("MAD", "MAD");
            currencies.Add("NOK", "NOK");
            currencies.Add("MXN", "MXN");
            currencies.Add("PHP", "PHP");
            currencies.Add("PLN", "PLN");
            currencies.Add("RUB", "RUB");
            currencies.Add("SAR", "SAR");
            currencies.Add("CHF", "CHF");
            currencies.Add("ZAR", "ZAR");
            currencies.Add("KRW", "KRW");
            currencies.Add("TRY", "TRY");
            
        }

        public string GetCurrency(string input)
        {
            string str = string.Empty;
            if (currencies.Count == 0)
            {
                FillCurrencies();
            }
            if (currencies.ContainsKey(input))
            {
                str = currencies[input].ToString();
            }
            return str;
        }

        public void ParseForData(string strResponse, RGRateDetail rd, RGRateRequest objRateRequest, string Morerates)
        {

            string roomDesc = string.Empty;
            string rateDesc = string.Empty;
            string input = string.Empty;
            string str4 = string.Empty;
            string _Currency = string.Empty;
            string _Price = string.Empty;
            decimal stayAmount = 0M;
            decimal avgDailyAmount = 0M;
            char rChangeInd = 'N';
            char merchantR = 'N';
            if (strResponse.IndexOf("City Not Found") >= 0)
            {
                rd.ErrorCode = "SYS01";
                rd.ErrorDesc = "City Not Found";
                return;
            }
            if (strResponse.IndexOf("Invalid check in date") >= 0)
            {
                rd.ErrorCode = "SYS01";
                rd.ErrorDesc = "Invalid check in date";
                rd.AvailStatus = "RF";
                return;
            }
            if (strResponse.IndexOf("Bad Request") >= 0)
            {
                rd.ErrorCode = "SYS01";
                rd.ErrorDesc = "Bad Request";
                rd.AvailStatus = "RF";
                return;
            }
            
            if (strResponse.IndexOf("REQUIRED HOTEL IS NOT AVAILABLE") >= 0
                || strResponse.IndexOf("The destination you are looking for does not have availability for the dates and number of rooms selected.") >= 0

                )
            {
                rd.AvailStatus = "C";
                rd.ErrorCode = "AVL01";
                rd.ErrorDesc = "Hotel Not Availble";
                return;
            }

            if (strResponse.IndexOf("No hotels found") >= 0
                || strResponse.IndexOf("Site_Error_MinimumStay") >= 0
                || strResponse.IndexOf("<li class=\"titmorpheo\">Your search criteria did not return any results, please select other options and search again.</li>") >= 0
                )
            {
                rd.AvailStatus = "C";
                rd.ErrorCode = "AVL01";
                rd.ErrorDesc = "Room Not Availble";
                return;
            }
            DataTable _dtRoomType = fGetRoomType(strResponse, rd, objRateRequest, Morerates);
            if (_dtRoomType.Rows.Count > 0)
            {
                for (int _dtRows = 0; _dtRows < _dtRoomType.Rows.Count; _dtRows++)
                {
                    roomDesc = _dtRoomType.Rows[_dtRows]["RoomType"].ToString().Trim();
                    roomDesc = StringUtil.FilterString(roomDesc, true, true);                  
                    roomDesc = Regex.Replace(roomDesc, @"\d+ available", "", RegexOptions.IgnoreCase);

                    rateDesc = _dtRoomType.Rows[_dtRows]["RateType"].ToString().Trim();
                    rateDesc = StringUtil.FilterString(rateDesc, true, true);                   

                    string _RoomPrice = _dtRoomType.Rows[_dtRows]["Price"].ToString().Trim();
                    _Currency = _dtRoomType.Rows[_dtRows]["Currency"].ToString().Trim();
                    _Currency =GetCurrency(_Currency);

                    if (objRateRequest.CurrencyCode.ToUpper() == "NZD")
                        _Currency = "NZD";
                    if (objRateRequest.CurrencyCode.ToUpper() == "USD")
                        _Currency = "USD";
                    if (objRateRequest.CurrencyCode.ToUpper() == "AUD")
                        _Currency = "AUD";
                    if (objRateRequest.CurrencyCode.ToUpper() == "ARS")
                        _Currency = "ARS";
                    if (objRateRequest.CurrencyCode.ToUpper() == "SEK")
                        _Currency = "SEK";
                    if (objRateRequest.CurrencyCode.ToUpper() == "DKK")
                        _Currency = "DKK";
                    if (objRateRequest.CurrencyCode.ToUpper() == "NOK")
                        _Currency = "NOK";
                    
                    stayAmount = Convert.ToDecimal(_RoomPrice);
                    if (_Currency == "THB")
                    {
                        avgDailyAmount = Math.Round((decimal)(stayAmount / objRateRequest.MinLengthOfStay));

                    }
                    else
                    {
                        avgDailyAmount = Math.Round((decimal)(stayAmount / objRateRequest.MinLengthOfStay), 2);
                    }
                    rd.AvailStatus = "O";
                    rd.ErrorCode = "OK";
                    rd.ErrorDesc = "SUCCESS";
                    this.AddRoomDetails(rd, roomDesc, rateDesc, _Currency, stayAmount, avgDailyAmount, avgDailyAmount, objRateRequest.Guests, merchantR, rChangeInd);
                }
            }
        }
        private DataTable fGetRoomType(string _Page, RGRateDetail rd, RGRateRequest objRateRequest, string MoreRates)
        {
            DataTable _RoomTypetype = new DataTable();
            try
            {
                _RoomTypetype.Columns.Add("RoomType");
                _RoomTypetype.Columns.Add("RateType");
                _RoomTypetype.Columns.Add("Price");
                _RoomTypetype.Columns.Add("Currency");

                string roomDesc = string.Empty;
                string rateDesc = string.Empty;
                string _Price = string.Empty;
                string _Currency = string.Empty;

                Regex regex = new Regex(@"<tr.*?>\s*<td.*?>(?<RoomDesc>[\s\S]*?)</td>\s*<td.*?>(?<RateDesc>[\s\S]*?)</td>[\s\S]*?<td\s*class="".*?price.*?"">(?<Price>[\s\S]*?)</td>[\s\S]*?</tr>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Regex rgxBlk = new Regex(@"Your hotel selected</li><li class=""hotel bloque box"" id=""result-1"">(?<Block>[\s\S]*?)</li>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                string SelectedRateBlk = rgxBlk.Match(_Page).Value;
                SelectedRateBlk = SelectedRateBlk + MoreRates;
                foreach (Match match in regex.Matches(SelectedRateBlk))
                {
                    string mm = match.Value;
                    roomDesc = string.Empty;
                    rateDesc = string.Empty;
                    _Price = string.Empty;
                    _Currency = string.Empty;

                    roomDesc = StringUtil.FilterString(match.Groups["RoomDesc"].Value, true, true);
                    roomDesc = roomDesc.Replace("See more", "").Replace("See less", "");
                    roomDesc = Regex.Replace(roomDesc, @"\s*[\s\S]*?pay\s*on-line", "", RegexOptions.IgnoreCase);
 
                    roomDesc = Regex.Replace(roomDesc, "Room TypeBoardOffersPer NightTotal PricePay On-Line", "", RegexOptions.IgnoreCase);
                    roomDesc = Regex.Replace(roomDesc, "Note: Some Hotels May Require Additional Fees To Be Paid Upon Arrival.", "", RegexOptions.IgnoreCase);

                    rateDesc = StringUtil.FilterString(match.Groups["RateDesc"].Value, true, true);
                    Regex rgx;
                    if (match.Value.Contains("crossed_out_price"))
                    {
                        rgx = new Regex("<td\\s*class\\s*=\\s*\"c[\\d]\\s*price\\s*\"\\s*id\\s*=\\s*\"SYS_c[\\d]\\s*\">\\s*<span\\s*class\\s*=\\s*\"\\s*crossed_out_price\\s*\"[\\s\\S]*?</span>(?<Price1>[\\s\\S]*?)<small\\s*class\\s*=\\s*\"\\s*decimal\\s*\">(?<Price2>[\\s\\S]*?)</small></td>"
                            + "|<td\\s*class\\s*=\\s*\"c[\\d]\\s*price\\s*\"\\s*id\\s*=\\s*\"SYS_c[\\d]\\s*\">(?<Price1>[\\s\\S]*?)<small\\s*class\\s*=\\s*\"\\s*decimal\\s*\">(?<Price2>[\\s\\S]*?)</small></td>"
                            + "|<td\\s*class\\s*=\\s*\"c[\\d]\\s*price\\s*c[\\d]special\"\\s*id\\s*=\\s*\"SYS_c[\\d]\\s*\">(?<Price1>[\\s\\S]*?)<small\\s*class\\s*=\\s*\"\\s*decimal\\s*\">(?<Price2>[\\s\\S]*?)</small></td>"
                            , RegexOptions.IgnoreCase | RegexOptions.Multiline);

                        string tempPrice = _Price = rgx.Match(match.Value).Groups["Price1"].Value.ToString() +
                                 rgx.Match(match.Value).Groups["Price2"].Value.ToString();
                        _Currency = string.Join(null, System.Text.RegularExpressions.Regex.Split(StringUtil.FilterString(_Price,true ,true), "[\\d.,]")).Trim();
                        if (tempPrice.Trim().Length == 0)
                        {
                            rgx = new Regex(@"<td class=""c4[\s\S]*?</span>(?<Avg>[\s\S]*?)</td><td class=""c5[\s\S]*?</span>(?<TotalPrice>[\s\S]*?)</td>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            //tempPrice = _Price = rgx.Match(match.Value).Groups["TotalPrice"].Value.ToString();
                            tempPrice  = rgx.Match(match.Value).Groups["TotalPrice"].Value.ToString();
                            string _tmpPrice = string.Empty;
                            if(!string.IsNullOrWhiteSpace(tempPrice))
                            {
                                rgx = new Regex(@"[\s\S]*?</small>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                _tmpPrice = rgx.Match(tempPrice).Value.ToString();
                                if (!string.IsNullOrWhiteSpace(_tmpPrice))
                                {
                                    tempPrice = _Price = _tmpPrice;
                                }
                                else
                                    _Price = tempPrice;
                            }
                            _Currency = string.Join(null, System.Text.RegularExpressions.Regex.Split(StringUtil.FilterString(_Price, true, true), "[\\d.,]")).Trim();
                        }
                        _Price = GetPrice(tempPrice.Replace(_Currency, ""), objRateRequest.CurrencyCode);
                    }
                    else
                    {
                       
                        rgx = new Regex(@"<td\s*class\s*=\s*""c[\d]\s*price\s*[\s\S]*?id\s*=\s*""SYS_c[\d]\s*"">\s*(?<PriceCUR>[\s\S]*?)\s*</", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string tempPrice = _Price = rgx.Match(match.Value).Groups["PriceCUR"].Value.ToString();
                        _Currency = string.Join(null, System.Text.RegularExpressions.Regex.Split(StringUtil.FilterString(_Price, true, true), "[\\d.,]")).Trim();
                        _Price = GetPrice(tempPrice.Replace(_Currency, ""), objRateRequest.CurrencyCode);
                    }
 
                    rgx = new Regex(@"class=""tip-title"">(?<Offer>[\s\S]*?)</[\s\S]*?class=""tip-text"">(?<Offer1>[\s\S]*?)</", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    StringBuilder strOffer = new StringBuilder();
                    foreach (Match mratedesc in rgx.Matches(match.Value))
                    {
                        strOffer.Append(" " + mratedesc.Groups["Offer"].Value);
                    }
                    rateDesc = strOffer.ToString();
                    if (!string.IsNullOrWhiteSpace(_Price))
                    {
                        _RoomTypetype.NewRow();
                        _RoomTypetype.Rows.Add(new object[] { roomDesc, rateDesc, _Price, _Currency });
                    }
                }
            }
          
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in fGetCookie : " + Environment.NewLine + ex.Message);
                return null;
            }
            return _RoomTypetype;
        }

        private string GetPrice(string price, string _Currency)
        {
            price = Regex.Replace(price, @"[^\d\.\,]", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            string[] splitedPrice = null;

            if (_Currency.Equals("PHP"))
            {

                price = price.Replace(".", "");
            }
            else
            {
                if (price.Contains(",") && price.Contains("."))
                {
                    splitedPrice = price.Split('.');
                    price = splitedPrice[0].Length >= splitedPrice[1].Length ? price = price.Replace(",", "") : price = price.Replace(".", "").Replace(",", ".");
                }
                else if (price.Contains(","))
                {
                    splitedPrice = price.Split(',');
                    if (_Currency.Equals("THB"))
                    {
                        price = splitedPrice[0].Length >= splitedPrice[1].Length ? price = price.Replace(",", "") : price = price.Replace(",", "");

                    }
                    else
                    {
                        price = splitedPrice[0].Length >= splitedPrice[1].Length ? price = price.Replace(",", ".") : price = price.Replace(",", "");
                    }
                }
                else if (price.Contains("."))
                {
                    splitedPrice = price.Split('.');
                    price = splitedPrice[0].Length >= splitedPrice[1].Length ? price : price = price.Replace(".", "");
                }
            }
            return price;
        }
        private string fFilterRate(string _MixRate)
        {
            try
            {
                 
                if (_MixRate.Contains(","))
                {
                    if (_MixRate.IndexOf(".") == 1)
                        _MixRate = _MixRate.Replace(".", "");
                    int temp = _MixRate.IndexOf(',');
                    temp = (_MixRate.Length) - temp;
                    if (temp == 3)
                        _MixRate = _MixRate.Replace(',', '.');

                }
                string _Price = String.Empty;
                char[] _PriceChar = _MixRate.ToCharArray();
                for (int Row = 0; Row < _PriceChar.Length; Row++)
                {
                    if (Char.IsSymbol(_PriceChar[Row]))
                        continue;
                    if (Char.IsWhiteSpace(_PriceChar[Row]))
                        continue;                   
                    if (_PriceChar[Row] == '.')
                    {
                        _Price = _Price.Replace(".", "");
                        _Price = _Price + _PriceChar[Row].ToString();
                    }
                    if (Char.IsDigit(_PriceChar[Row]))
                        _Price = _Price + _PriceChar[Row].ToString();
                }
                return _Price;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in fFilterRate : " + Environment.NewLine + ex.Message);
                return null;
            }
        }
   

    
    }
}
