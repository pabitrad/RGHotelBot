using System;
using System.Globalization;
using RG.Core.Entities;
using BOTManager.Entities.Crawl;
using RG.Core.Entities.BOT;
using System.Collections.Generic;

namespace BOTManager.Entities
{

    public enum RequestPriority
    {
        OnDemand = 1,
        Batch = 2
    }


    /// <summary>
    /// Summary description for RateRequest.
    /// </summary>
    [Serializable]
    public class RGRateRequest : BotRequestParameterObject
    {
        public List<KeyValuePair<int, string>> CrawlResponses 
        {
            get;
            private set;
        }

        public List<KeyValuePair<int, string>> LevelUrls  
        {
            get; 
            private set;
        }

        public RGRateRequest()
        {
            RateDetail = new RGRateDetail();
            CrawlResponses = new List<KeyValuePair<int, string>>();
            LevelUrls = new List<KeyValuePair<int, string>>();
        }

        public RGRateRequest(RateAvailabilityRequest coreRequest,string segmentId,string replyLocation)
        {
            CrawlResponses = new List<KeyValuePair<int, string>>();
            LevelUrls = new List<KeyValuePair<int, string>>();
            RateDetail = new RGRateDetail();
            this.ReplyLocation = replyLocation;
            //this.RequestID = -1;
            this.RequestSegmentID = Convert.ToInt32(GetSegmentKey(segmentId));
            var rateSegment = coreRequest.RateAvailabilityRequestSegments[0];
            this.RequestType = rateSegment.RequestType;
            this.RequestPriority = (RequestPriority)rateSegment.RequestPriority.Value;
            //this.RequestTimeStamp = rateSegment.RequestTimeStamp;
            this.RequestTimeStampUTC = DateTime.UtcNow;
            //tvcRateRequest["Request_TimeStamp"] = null;
            this.CheckInDate = DateTime.ParseExact(rateSegment.FirstCheckInDate, "MMddyyyy", CultureInfo.InvariantCulture);
            this.DaysOfData  = rateSegment.DaysOfData.Value;
            this.Guests  = rateSegment.Guests.Value;
            this.MinLengthOfStay = Convert.ToInt16(rateSegment.MinimumLengthOfStay);
            this.SpecialRatePlan = string.IsNullOrWhiteSpace(rateSegment.SpecialRatePlan) ? string.Empty : rateSegment.SpecialRatePlan;
            this.Source  = TravelClickNew.Classes.General.DotTo(rateSegment.Source);
            this.AirportCityCode = string.IsNullOrWhiteSpace(rateSegment.HotelReference.AirportCityCode) ? string.Empty : rateSegment.HotelReference.AirportCityCode;
            this.City  = string.IsNullOrWhiteSpace(rateSegment.HotelReference.City) ? string.Empty : rateSegment.HotelReference.City;
            this.State = string.IsNullOrWhiteSpace(rateSegment.HotelReference.State) ? string.Empty : rateSegment.HotelReference.State;
            this.MailCode = string.IsNullOrWhiteSpace(rateSegment.HotelReference.MailCode) ? string.Empty : rateSegment.HotelReference.MailCode;
            this.Country = string.IsNullOrWhiteSpace(rateSegment.HotelReference.Country) ? string.Empty : rateSegment.HotelReference.Country;
            this.PropertyName = string.IsNullOrWhiteSpace(rateSegment.HotelReference.PropertyName) ? string.Empty : rateSegment.HotelReference.PropertyName;
            this.PropertyID  = string.IsNullOrWhiteSpace(rateSegment.HotelReference.PropertyID) ? string.Empty : rateSegment.HotelReference.PropertyID;
            this.PropertyChainCode = string.IsNullOrWhiteSpace(rateSegment.HotelReference.PropertyChainCode) ?
                string.Empty : rateSegment.HotelReference.PropertyChainCode;
            this.Status = RG.Core.Entities.Status.Optimized; //tvcRateRequest["Status"] = 3;//-1 - Failed; 2- Success ; 3: TBCLD is invoked on these requests. 0: Ready for crawler
            this.CurrencyCode = string.IsNullOrWhiteSpace(rateSegment.HotelReference.CurrencyCode) ? string.Empty : rateSegment.HotelReference.CurrencyCode;
        }



        public RGRateDetail RateDetail { get; private set; }


        public RGBot Bot { get; set; }

        #region PRIVATE VARIABLES
       


        #endregion

        #region PUBLIC VARIABLES
       
        public long RateRequestID { get; set; }

        public string CurrencyCode { get; set; }

        public long RequestID { get; set; }

        public long RequestSegmentID { get; set; }

        public string RequestType { get; set; }

        public string AirportCityCode { get; set; }

        public RequestPriority RequestPriority { get; set; }

        public DateTime RequestTimeStamp { get; set; }

        public DateTime RequestTimeStampUTC
        {
            get;
            set;
        }

        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CheckInDateMMddYYYY
        {
            get
            {
                return this.CheckInDate.ToString("MMddyyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime CheckOutDate
        {
            get { return CheckInDate.AddDays(MinLengthOfStay); }
        }



        public int DaysOfData { get; set; }
        
        public int Guests { get; set; }

        public int MinLengthOfStay { get; set; }

        public string SpecialRatePlan{get;set;}

        public string Source { get; set; }

        public string City { get; set; }

        public string State{get;set;}
        
        public string MailCode{get;set;}
        
        public string Country{get;set;}
        
        public string PropertyName{get;set;}
        
        public string PropertyID{get;set;}
        
        public string PropertyChainCode{get;set;}
        
        public int WebSiteId{get;set;}

        #endregion


        public string GetCurrencyCode()
        {
            if (this.Bot != null && this.Bot.Config != null && this.Bot.Config.SupportedCurrencies.Exists(y => String.Equals(y, this.CurrencyCode, StringComparison.InvariantCultureIgnoreCase)))
                return this.CurrencyCode;

            if (this.Bot != null && this.Bot.Config != null && this.Bot.Config.PropertyBag["DefaultCurrency"] != null)
                return this.Bot.Config.PropertyBag.GetValue<string>("DefaultCurrency");

            return string.Empty;
        }

        public string ReplyLocation { get; set; }


        private static string GetSegmentKey(string input)
        {
            if (input.Contains("#"))
            {
                return input.Substring(0, input.IndexOf("#"));
            }
            return input;
        }

        public Status Status { get; set; }

    }
}
