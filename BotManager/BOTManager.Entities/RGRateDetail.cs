using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BOTManager.Entities
{

    /// <summary>
    /// RateDetail is a struct for storing informating regarding per room type
    /// and is a part of the TVC_RateDetailClass
    /// </summary>
    [Serializable]
    public class RoomTypeDetail
    {
        string _RoomDesc;
        string _RateDesc;
        string _Currency;
        string _RateCode;
        decimal _StayAmount;
        decimal _AvgDailyAmount;
        char _RateChangeIndicator;
        char _MerchantRateIndicator;
        decimal _OriginalRate;
        decimal _DiscountedRate;
        int _MaxPersons;
        string _Price;
        string _TotalPrice;
        string _ExtraCharge;
        string _RatePlan;
        string _CancelPolicy;

        string _RateCategory = string.Empty;


        public string RateCategory
        {
            get;
            set;
        }

        
        /// <summary>
        ///Provides Room Description  
        /// </summary>
        /// 
        public string RoomDesc
        {
            get
            {
                return _RoomDesc;
            }
            set
            {
                _RoomDesc = value;
            }
        }
        /// <summary>
        /// Provides Rate Change Indicator
        /// </summary>
        public char RateChangeIndicator
        {
            get
            {
                return _RateChangeIndicator;
            }
            set
            {
                _RateChangeIndicator = value;
            }
        }
        /// <summary>
        /// Provides Merchant Rate Indicator
        /// </summary>
        public char MerchantRateIndicator
        {
            get
            {
                return _MerchantRateIndicator;
            }
            set
            {
                _MerchantRateIndicator = value;
            }
        }
        /// <summary>
        /// Provides Rate Description
        /// </summary>
        public string RateDesc
        {
            get
            {
                return _RateDesc;
            }
            set
            {
                _RateDesc = value;
            }
        }

        /// <summary>
        /// Currency Type(String)
        /// for e.g. "Eur" ....
        /// </summary>
        public string Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                _Currency = value;
            }
        }

        /// <summary>
        /// Rate Code(String)
        /// 
        /// </summary>
        public string RateCode
        {
            get
            {
                return _RateCode;
            }
            set
            {
                _RateCode = value;
            }
        }

        /// <summary>
        /// Stay Amount for the room
        /// </summary>
        public decimal StayAmount
        {
            get
            {
                return _StayAmount;
            }
            set
            {
                _StayAmount = value;
            }
        }

        /// <summary>
        /// Average Daily Amount for the day
        /// </summary>
        public decimal AvgDailyAmount
        {
            get
            {
                return _AvgDailyAmount;
            }
            set
            {
                _AvgDailyAmount = value;
            }
        }
        //		
        /// <summary>
        /// Original Rate for the room
        /// </summary>
        public decimal OriginalRate
        {
            get
            {
                return _OriginalRate;
            }
            set
            {
                _OriginalRate = value;
            }
        }


        /// <summary>
        /// Discounted Rate for the room
        /// </summary>
        public decimal DiscountedRate
        {
            get
            {
                return _DiscountedRate;
            }
            set
            {
                _DiscountedRate = value;
            }
        }

        /// <summary>
        /// Maximum Persons allowed in the room
        /// </summary>
        public int MaxPersons
        {
            get
            {
                return _MaxPersons;
            }
            set
            {
                _MaxPersons = value;
            }
        }

        /// <summary>
        /// Hold raw price
        /// </summary>
        public string Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
            }
        }

        /// <summary>
        /// Hold raw price
        /// </summary>
        public string TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                _TotalPrice = value;
            }
        }

        public string ExtraCharge
        {
            get
            {
                return _ExtraCharge;
            }
            set
            {
                _ExtraCharge = value;
            }
        }
        public string RatePlan
        {
            get
            {
                return _RatePlan;
            }
            set
            {
                _RatePlan = value;
            }
        }
        public string CancelPolicy
        {
            get
            {
                return _CancelPolicy;
            }
            set {
                _CancelPolicy = value;
            }
        }

    }

    /// <summary>
    /// TVC_RateDetail class is a class which mimics the raterequest which 
    /// comes form the client end;
    /// 
    /// It contains information regarding requests (requestid , checkindate , checkoutdate )and 
    /// information about room types by maintaing a list of ratedetail struct (for each room type)
    /// in the TVC_RateDetail class 
    /// </summary>
    [Serializable]
    public class RGRateDetail
    {
        DateTime _CheckInDate;
        DateTime _CheckOutDate;
        List<RoomTypeDetail> _RoomTypeDetails;
        int _WebsiteID;
        long _URLID;
        long _RateDetailID;
        long _RateRequestID;
        string _RequestStatus;
        string _AvailStatus;
        string _RateCode;
        string _RateCategory;
        float _DailyRateAmount;
        char _RateChangeIndicator;
        char _MerchantRate;
        int _Status = 1;
        int _Deleted = 0;
        DateTime _CreationTime = DateTime.MinValue;
        string _ErrorCode;
        string _ErrorDesc;
        string _Url;
        string _fileName;
        string _Extracharge;
        string _RatePlan;
        string _CancelPolicy;
        public RGRateDetail()
        {
            _RoomTypeDetails = new List<RoomTypeDetail>();
        }


        public string CachePageContent { get; private set; }

        public void SetRateStatus(string availStatus, string errorCode, string errorDesc)
        {
            AvailStatus = availStatus;
            ErrorCode = errorCode;
            ErrorDesc = errorDesc;
        }

        public void SetCachePageContent(string content)
        {
            this.CachePageContent = content;
        }

        public long RateDetailID
        {
            get
            {
                return _RateDetailID;
            }
            set
            {
                _RateDetailID = value;
            }
        }
        public long RateRequestID
        {
            get
            {
                return _RateRequestID;
            }
            set
            {
                _RateRequestID = value;
            }
        }
        public int WebsiteID
        {
            get
            {
                return _WebsiteID;
            }
            set
            {
                _WebsiteID = value;
            }
        }
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        public long URLID
        {
            get
            {
                return _URLID;
            }
            set
            {
                _URLID = value;
            }
        }
        public string RequestStatus
        {
            get
            {
                return _RequestStatus;
            }
            set
            {
                _RequestStatus = value;
            }
        }
        public DateTime CheckInDate
        {
            get
            {
                return _CheckInDate;
            }
            set
            {
                _CheckInDate = value;
            }
        }
        public DateTime CheckOutDate
        {
            get
            {
                return _CheckOutDate;
            }
            set
            {
                _CheckOutDate = value;
            }
        }
        public string AvailStatus
        {
            get
            {
                return _AvailStatus;
            }
            set
            {
                _AvailStatus = value;
            }
        }
        public string RateCode
        {
            get
            {
                return _RateCode;
            }
            set
            {
                _RateCode = value;
            }
        }
        public string RateCategory
        {
            get
            {
                return _RateCategory;
            }
            set
            {
                _RateCategory = value;
            }
        }
        public float DailyRateAmount
        {
            get
            {
                return _DailyRateAmount;
            }
            set
            {
                _DailyRateAmount = value;
            }
        }
        public char RateChangeIndicator
        {
            get
            {
                return _RateChangeIndicator;
            }
            set
            {
                _RateChangeIndicator = value;
            }
        }
        public char MerchantRate
        {
            get
            {
                return _MerchantRate;
            }
            set
            {
                _MerchantRate = value;
            }
        }
        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }
        public int Deleted
        {
            get
            {
                return _Deleted;
            }
            set
            {
                _Deleted = value;
            }
        }
        public DateTime CreationTime
        {
            get
            {
                return _CreationTime;
            }
            set
            {
                _CreationTime = value;
            }
        }
        public string ErrorDesc
        {
            get
            {
                return _ErrorDesc;
            }
            set
            {
                _ErrorDesc = value;
            }
        }
        public string ErrorCode
        {
            get
            {
                return _ErrorCode;
            }
            set
            {
                _ErrorCode = value;
            }
        }
        public List<RoomTypeDetail> RoomTypeDetails
        {
            get
            {
                return _RoomTypeDetails;
            }
        }
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
            }
        }
        public string ExtraCharge
        {
            get
            {
                return _Extracharge;
            }
            set
            {
                _Extracharge = value;
            }
        }
        public string RatePlan
        {
            get
            {
                return _RatePlan;
            }
            set
            {
                _RatePlan = value;
            }
        }
        public string CancelPolicy
        {
            get
            {
                return _CancelPolicy;
            }
            set
            {
                _CancelPolicy = value;
            }
        }
        //public void AddRoomTypeRateDetail(string roomDesc,	string rateDesc, string currency , decimal stayAmount,decimal avgDailyAmount,	decimal discountedRate , int maxPersons)
        public void AddRoomTypeRateDetail(string roomDesc, string rateDesc, string Extracharge,string RatePlan,string CancelPolicy, string currency, decimal stayAmount, decimal avgDailyAmount, decimal discountedRate, int maxPersons, char _MarchentRate, char _RateChangeIndicator)
        {
            RoomTypeDetail rateDetail = new RoomTypeDetail();
            rateDetail.RoomDesc = roomDesc;
            rateDetail.RateDesc = rateDesc;
            rateDetail.Currency = currency;
            rateDetail.StayAmount = stayAmount;
            rateDetail.MerchantRateIndicator = _MarchentRate;
            rateDetail.RateChangeIndicator = _RateChangeIndicator;
            rateDetail.DiscountedRate = discountedRate;
            rateDetail.MaxPersons = maxPersons;
            rateDetail.ExtraCharge = Extracharge;
            rateDetail.RatePlan = RatePlan;
            rateDetail.CancelPolicy = CancelPolicy;
            _RoomTypeDetails.Add(rateDetail);
        }
    }
}
