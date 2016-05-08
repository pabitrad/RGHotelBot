using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using RG.Core.Entities;

namespace BOTManager.Entities
{
    public class RGBot
    {
        public RGBot()
        {
            
        }

        public int SourceId { get; set; }
        public string Source { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public string MainMethod { get; set; }
        public string DLLName { get; set; }
        public RGBotConfig Config { get; set; }
        public string UserAgent { get; set; }
        public List<RGProxy> Proxylist { get; set; }
    }

    


}
