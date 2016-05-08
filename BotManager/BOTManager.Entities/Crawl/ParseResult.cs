using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Crawl
{
    [Serializable]
    public class ParseResult
    {
        public ParseResult(CrawlResponse response)
        {
            this.Response = response;
        }

        public CrawlResponse Response { get; private set; }

        public RGRateRequest RateRequest
        {
            get
            {
                if (this.Response != null)
                    return Response.RateRequest;
                return null;
            }
        }

        public RGRateDetail RateDetail
        {
            get
            {
                if (this.Response != null)
                    return Response.RateDetail;
                return null;
            }
        }

        private List<RGRateDetail> _rateDetails = new List<RGRateDetail>();

        /// <summary>
        /// 
        /// </summary>
        public BotRequestParameterObject PreviousRequestParameterObject
        {
            get
            {
                return Response.RequestParameterObject;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BotRequestParameterObject NextLevelRequestParameterObject { get; set; }
    }
}
