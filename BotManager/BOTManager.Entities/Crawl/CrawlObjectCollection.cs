using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Crawl
{
    public class CrawlObjectCollection : List<CrawlObject>
    {

        /// <summary>
        /// 
        /// </summary>
        public CrawlObject Parent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public CrawlObjectCollection(CrawlObject parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childObject"></param>
        public new void Add(CrawlObject childObject)
        {
            childObject.Level = Parent.Level;
            childObject.Parent = Parent;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="childObjects"></param>
        public new void AddRange(IEnumerable<CrawlObject> childObjects)
        {
            foreach (var child in childObjects)
            {
                child.Level = Parent.Level;
                child.Parent = Parent;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public RGRateRequest RateRequest
        {
            get
            {
                return Parent.RateRequest;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RGRateDetail RateDetails
        {
            get
            {
                return Parent.RateDetails;
            }
        }
    }
}
