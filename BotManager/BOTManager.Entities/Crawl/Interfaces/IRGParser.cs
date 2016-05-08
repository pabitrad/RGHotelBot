using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Crawl.Interfaces
{
    public interface IRGParser
    {
        ParseResult ParseResponse(CrawlObject crawlObject);
    }
}
