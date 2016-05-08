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

namespace Hoteloasia
{
    public class HoteloasiaRequestObject : BotRequestParameterObject
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
            try
            {
                if (crawlObject.Level == 1)
                    return getParseLevel1(crawlObject);
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in ParseResponse : " + Environment.NewLine + ex.Message);
            }

            return null;
        }

        public ParseResult getParseLevel1(CrawlObject crawlObject)
        {
            try
            {
                CrawlResponse crawlResponse = crawlObject.RGWebRequests[0].Response;
                ParseResult parseResult = new ParseResult(crawlResponse);
                string MoreRate = string.Empty;
                customLevel = 1;
                isInternalRetryFailed = false;

                finalResponse = crawlResponse.ResponseString;
                ParseForData(finalResponse, crawlResponse.RateDetail, crawlResponse.RateRequest, MoreRate);
                finalResponse = cachePageCreation(finalResponse, crawlResponse.RateDetail, crawlResponse.RateRequest);
                finalResponse = finalResponse.Replace("<head>", @"<head> <base href=""http://www.hoteloasia.com/"" />");
                crawlResponse.RateDetail.SetCachePageContent(finalResponse);
                parseResult.NextLevelRequestParameterObject = null; //set null at final parsing 

                return parseResult;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in getParseLevel1 : " + Environment.NewLine + ex.Message);
            }

            return null;
        }


        public static string cachePageCreation(string strResponse,RGRateDetail rd,RGRateRequest objRateRequest)
        {
            return strResponse;
        }

        void AddRoomDetails(RGRateDetail rd, string roomDesc, string rateDesc, string currency, decimal stayAmount, decimal avgDailyAmount, decimal discountedRate, int maxPersons, char _MarchentRate, char _RateChangeIndicator)
        {
            try
            {
                rd.AddRoomTypeRateDetail(roomDesc, rateDesc, "", "", "", currency, stayAmount, avgDailyAmount, discountedRate, maxPersons, _MarchentRate, _RateChangeIndicator);
            }
            catch(Exception ex)
            {
                Logger.LogWarning("Exception occure in AddRoomDetails : " + Environment.NewLine + ex.Message);
            }
        }

        public void ParseForData(string strResponse, RGRateDetail rd, RGRateRequest objRateRequest, string Morerates)
        {
            try
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

                if (string.IsNullOrWhiteSpace(strResponse))
                {
                    rd.ErrorCode = "SYS01";
                    rd.ErrorDesc = "Website not responding";
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

                if (strResponse.IndexOf("Unfortunately we are out of rooms in the choosen period") >= 0)
                {
                    rd.AvailStatus = "C";
                    rd.ErrorCode = "AVL01";
                    rd.ErrorDesc = "Hotel Not Availble";
                    return;
                }

                DataTable _dtRoomType = fGetRoomType(strResponse, rd, objRateRequest, Morerates);
                if (_dtRoomType.Rows.Count > 0)
                {
                    for (int _dtRows = 0; _dtRows < _dtRoomType.Rows.Count; _dtRows++)
                    {
                        roomDesc = _dtRoomType.Rows[_dtRows]["RoomType"].ToString().Trim();
                        roomDesc = StringUtil.FilterString(roomDesc, true, true);

                        rateDesc = _dtRoomType.Rows[_dtRows]["RateType"].ToString().Trim();
                        rateDesc = StringUtil.FilterString(rateDesc, true, true);

                        string _RoomPrice = _dtRoomType.Rows[_dtRows]["Price"].ToString().Trim();
                        _Currency = _dtRoomType.Rows[_dtRows]["Currency"].ToString().Trim();

                        stayAmount = Convert.ToDecimal(_RoomPrice);
                        avgDailyAmount = Math.Round((decimal)(stayAmount / objRateRequest.MinLengthOfStay), 2);
                        
                        rd.AvailStatus = "O";
                        rd.ErrorCode = "OK";
                        rd.ErrorDesc = "SUCCESS";
                        this.AddRoomDetails(rd, roomDesc, rateDesc, _Currency, stayAmount, avgDailyAmount, avgDailyAmount, objRateRequest.Guests, merchantR, rChangeInd);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in ParseForData : " + Environment.NewLine + ex.Message);
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

                string roomType = string.Empty;
                string roomDesc = string.Empty;
                string rateDesc = string.Empty;
                string _Price = string.Empty;
                string _Currency = string.Empty;

                Regex regRoomType = new Regex(@"<div id=""dominoheadtext"">(?<RateDesc>[\s\S]*?)</div>[\s\S]*?<td id=""DominoContentPlaceHolder_DominoAvailability_DominoAdvancedList_DominoAdvancedRoomtypeList_\d_RoomtypeHideBeginTD_\d""[\s\S]style=""vertical-align: top;"">[\s\S]*?""Book stay""");
                Regex regRoomDetails = new Regex(@"dominoroomtype"">(?<RoomType>[\w ]+)[\s\S]*?""dominoroomtypedesc"">[\s\S]*?<span[\s\S]*?>(?<RoomDesc>[\s\S]*?)</span>[\s\S]*?""dominoroomtypeprice"">(?<Currency>\w{3})\s(?<RoomPrice>\d?\,?\d{3}.\d{2})");
                
                MatchCollection roomTypeCollection = regRoomType.Matches(_Page);
                foreach (Match roomTypeMatch in roomTypeCollection)
                {
                    rateDesc = roomTypeMatch.Groups["RateDesc"].Value.Trim();
                    MatchCollection matchRoomDetailsCollection = regRoomDetails.Matches(roomTypeMatch.Value);
                    foreach (Match matchRoomDetails in matchRoomDetailsCollection)
                    {
                        if (matchRoomDetails.Success)
                        {

                            roomType = matchRoomDetails.Groups["RoomType"].Value.Trim();
                            roomDesc = matchRoomDetails.Groups["RoomDesc"].Value.Trim();

                            //If it contains "..." (ellipsis) at end then remove it.
                            if (roomDesc.Contains("..."))
                            {
                                roomDesc = roomDesc.Remove(roomDesc.Length - 3);
                            }

                            roomDesc = roomType + ":" + roomDesc;
                            _Currency = matchRoomDetails.Groups["Currency"].Value.Trim();
                            _Price = matchRoomDetails.Groups["RoomPrice"].Value.Trim();

                            if (!string.IsNullOrWhiteSpace(_Price))
                            {
                                _RoomTypetype.NewRow();
                                _RoomTypetype.Rows.Add(new object[] { roomDesc, rateDesc, _Price, _Currency });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Exception occure in fGetRoomType : " + Environment.NewLine + ex.Message);
                return null;
            }

            return _RoomTypetype;
        }
    }
}