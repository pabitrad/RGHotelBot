using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RG.Core.Entities.BOT;

namespace BOTManager.Entities
{

    /// <summary>
    /// 
    /// </summary>
    public class BOTArgs : EventArgs
    {
        private List<RGRateDetail> _responses = new List<RGRateDetail>();
        public RGBot BotInvoked { get; set; }
        public string RequestId { get; set; }
        public long TimeTakenMilliSeconds { get; set; }
        public int exitCode { get; set; }
        public List<RGRateRequest> Request { get; set; }
        public List<RGRateDetail> Responses
        {
            get { return _responses; }
            set { _responses = value; }
        }
    }


}
