using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Crawl
{
    public class BotRequestParameterObject
    {

        /// <summary>
        /// 
        /// </summary>
        public BotRequestParameterObject()
        {
            this.Identifier = Guid.NewGuid().ToString();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Name you would like to set and it will be available in responses as well.
        /// </summary>
        public string Reference { get; set; }

    }
}
