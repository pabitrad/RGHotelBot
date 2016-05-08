using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOTManager.Entities.Crawl;
using BOTManager.Entities;
using BOTManager.BL;


namespace TestBotConsole
{
    class Program
    {
        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomain)]
        static void Main(string[] args)
        {
            string NameSpace = "RateGain.Hoteloasia ";
            string classname = "Crawler";
            string botAssemblyPath = @"E:\Projects\Direct\Rate Gain\Source Code\Hoteloasia\bin\Debug\Hoteloasia.dll";

            CrawlRequest crawlRequest = new CrawlRequest();
            RGRateRequest rateRequest = new RGRateRequest();
            crawlRequest.RequestParameterObject = rateRequest;
            rateRequest.RequestID = 598;
            rateRequest.RateRequestID = 180498;
            rateRequest.RequestSegmentID = 45;
            rateRequest.RequestType = "Agentware Rate and Availability Request v1.0";
            rateRequest.RequestPriority = RequestPriority.Batch;
            rateRequest.CheckInDate = DateTime.Now.AddDays(8);
            rateRequest.DaysOfData = 1;
            rateRequest.Guests = 2;
            rateRequest.MinLengthOfStay = 2;
            rateRequest.SpecialRatePlan = "Unrestricted";
            rateRequest.Source = "Hoteloasia";
            rateRequest.AirportCityCode = "";
            rateRequest.City = "AAR";
            rateRequest.State = "";
            rateRequest.MailCode = "";
            rateRequest.Country = "";
            rateRequest.PropertyName = "";
            rateRequest.PropertyID = "76832"; //Hotel Oasis
            rateRequest.PropertyChainCode = "";
            rateRequest.CurrencyCode = "USD";
            var exitcode = (new RGCrawlManager()).InvokeBotCrawl(botAssemblyPath, string.Format("{0}.{1}", NameSpace, classname), new object[] { crawlRequest });

        }
    }
}


//            string NameSpace = "RateGain.Loews";
//            string classname = "Crawler";
//            string botAssemblyPath = @"D:\RG.PriceGainHotel.Bots\V21\Bots\Loews\bin\Release\Loews.dll";
//            CrawlRequest crawlRequest = new CrawlRequest();
//            RGRateRequest rateRequest = new RGRateRequest();
//            crawlRequest.RequestParameterObject = rateRequest;
//            rateRequest.RequestID = 598;
//            rateRequest.RateRequestID = 180498;
//            rateRequest.RequestSegmentID = 45;
//            rateRequest.RequestType = "Agentware Rate and Availability Request v1.0";
//            rateRequest.RequestPriority = RequestPriority.Batch;
//            rateRequest.CheckInDate = DateTime.Now.AddDays(1);
//            rateRequest.DaysOfData = 1;
//            rateRequest.Guests = 1;
//            rateRequest.MinLengthOfStay = 1;
//            rateRequest.SpecialRatePlan = "Unrestricted";
//            rateRequest.Source = "Loews ";
//            rateRequest.AirportCityCode = "LBBB";
//            rateRequest.City = "";
//            rateRequest.State = "";
//            rateRequest.MailCode = "";
//            rateRequest.Country = "";
//            rateRequest.PropertyName = "";
//            rateRequest.PropertyID = "9";
//            rateRequest.PropertyChainCode = "LZ";
//            rateRequest.CurrencyCode = "USD";
//            var exitcode = (new RGCrawlManager()).InvokeBotCrawl(botAssemblyPath, string.Format("{0}.{1}", NameSpace, classname), new object[] { crawlRequest });

//        }
//    }
//}



//            string NameSpace = "RateGain.PremierInn";
//            string classname = "Crawler";
//            string botAssemblyPath = @"D:\RG.PriceGainHotel.Bots\V21\Bots\PremierInn\bin\Release\PremierInn.dll";
//            CrawlRequest crawlRequest = new CrawlRequest();
//            RGRateRequest rateRequest = new RGRateRequest();
//            crawlRequest.RequestParameterObject = rateRequest;
//            rateRequest.RequestID = 598;
//            rateRequest.RateRequestID = 180498;
//            rateRequest.RequestSegmentID = 45;
//            rateRequest.RequestType = "Agentware Rate and Availability Request v1.0";
//            rateRequest.RequestPriority = RequestPriority.Batch;
//            rateRequest.CheckInDate = DateTime.Now.AddDays(8);
//            rateRequest.DaysOfData = 1;
//            rateRequest.Guests = 1;
//            rateRequest.MinLengthOfStay = 1;
//            rateRequest.SpecialRatePlan = "Unrestricted";
//            rateRequest.Source = "PremierInn";
//            rateRequest.AirportCityCode = "";
//            rateRequest.City = "";
//            rateRequest.State = "";
//            rateRequest.MailCode = "";
//            rateRequest.Country = "";
//            rateRequest.PropertyName = "Wigan Town Centre";
//            rateRequest.PropertyID = "WIGHAR";
//            rateRequest.PropertyChainCode = "P3";
//            rateRequest.CurrencyCode = "GBP";
//            var exitcode = (new RGCrawlManager()).InvokeBotCrawl(botAssemblyPath, string.Format("{0}.{1}", NameSpace, classname), new object[] { crawlRequest });

//        }
//    }
//}
