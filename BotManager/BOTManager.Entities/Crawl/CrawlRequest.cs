using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Crawl
{
    [Serializable]
    public class CrawlRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsTaxSettingEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentLevel { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int NextLevel { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public BotRequestParameterObject RequestParameterObject { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public CrawlObject PreviousCrawlObject { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public RGRateRequest RateRequest
        {
            get
            {
                if (this.RequestParameterObject != null && this.RequestParameterObject is RGRateRequest)
                    return (RGRateRequest)this.RequestParameterObject;
                
                if (this.PreviousCrawlObject != null)
                    return this.PreviousCrawlObject.RateRequest;
                
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RGRateDetail RateDetails
        {
            get
            {
                if (this.PreviousCrawlObject != null)
                    return this.PreviousCrawlObject.RateDetails;
                return null;
            }
        }
    }
}
