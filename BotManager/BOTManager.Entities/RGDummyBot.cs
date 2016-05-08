using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RG.Core.Entities.BOT;
using BOTManager.Entities.Crawl.Interfaces;
using RG.Utility;
using BOTManager.Entities.Crawl;
using System.Threading;

namespace BOTManager.Entities
{
    public class RGDummyBot : IRGCrawler, IRGParser
    {
        public RGRateDetail GetDummyRateDetail(RGRateRequest rateRequest)
        {
            RGRateDetail rateDetails = rateRequest.RateDetail;
            rateDetails.AvailStatus = StatusList.PickRandom<string>();
            if (rateDetails.AvailStatus == "O")
            {
                int totalRates = RateCount.PickRandom();
                int ratecount = 0;
                while (ratecount++ <= totalRates)
                {
                    //Create Rates
                    rateDetails.AddRoomTypeRateDetail("Dummy " + RoomDescription.PickRandom(),
                        "Dummy " + RateDescription.PickRandom(),
                        ExtraCharge.PickRandom(),
                        RatePlan.PickRandom(),
                        CancelPolicy.PickRandom(),
                        Currency.PickRandom(),
                        Amount.PickRandom(),
                        Amount.PickRandom(),
                        Amount.PickRandom(),
                        MaxPersons.PickRandom(),
                        MerchantRateIndicators.PickRandom(),
                        RateChangeIndicators.PickRandom());
                }
            }
            rateDetails.ErrorCode = (rateDetails.AvailStatus == "RF") ? ErrorCodeList.PickRandom() : "OK";
            rateDetails.CheckInDate = rateRequest.CheckInDate;
            rateDetails.CheckOutDate = rateRequest.CheckOutDate;
            rateDetails.CreationTime = DateTime.Now;
            Thread.Sleep(ThreadDelaySeconds.PickRandom() * 1000);
            return rateDetails;
        }

        public Crawl.CrawlObject GetCrawlObject(Crawl.CrawlRequest rateRequest, int level)
        {
            return null;
        }

        public Crawl.ParseResult ParseResponse(Crawl.CrawlObject crawlObject)
        {
            return null;
        }
        public List<string> StatusList = new List<string>() { "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "RF", "C", "C", "C", "NR", "C" };
        public List<string> RoomDescription = new List<string>() { "1 KING BED DELUXE POOL FACING NONSMOKING", "1-bedroom Ocean Front Pool Villa - 1 king bed", "2 Single Beds", "Best available rate ocean view king-1 king-nonsmoking-accessible communication features-265-355 sqft-sleeps 2", "Club Bayview with One King Bed", "Club Double or Twin Room (Crystal Premier) - REFUNDABLE - FREE CANCELLATION", "DOUBLE STANDARD", "Deluxe Balcony Room King-sized or twin beds", "Deluxe King Room", "Deluxe Pool Villa Deluxe Pool Villa(China market)(pre-pay)[with breakfast]", "Deluxe Twin Room - Non-refundable - Breakfast included - Found your room online at a lower price? Weamp;#39;ll match it", "Dorrington Villa", "Double Hilton Guestroom", "Executive Room, 1 King Bed", "Garden Villa with King Sized Bed", "Guest Room Queen or Twin and Sofa Bed", "Ocean View Junior Suite", "Patio with Fireplace", "Premier Double Room with Club Lounge Access and Free Wifi", "Premier Queen Room", "Premier Single Room(Free breakfast) - REFUNDABLE - FREE CANCELLATION", "Premier Suite", "Royal Studio with Pool Access", "Special Offer - Executive Double or Twin Room with Free Wifi", "Standard Guest Room", "Standard Room, 1 King Bed, Mountain View - Advance Purchase", "Standard Room, 1 Queen Bed, Non Smoking - Advance Purchase Non Refundable(Free breakfast) - NON REFUNDABLE", "Suite, 1 King Bed, Mountain View - Romance Package", "Superior Double Room - Special conditions 2 - single occupancy - FREE cancellation - Book now, pay when you stay - Breakfast included - Free WiFi - Found your room online at a lower price? Weamp;#39;ll match it", "Superior Harbor View - 1 queen bed", "Superior Room", "Two Bedroom Apartment 4 single beds or 1 king and 2 single beds or 2 king beds", "Two Bedroom Beachfront Sunset Suite at Beach Village", "Villa King Garden", "Villa, 1 King Bed (Plunge Pool)", "[102TrivagoUK(Booking.com ^ Rank = 102)]", "[136TrivagoUK(Expedia ^ Rank = 136)]", "[13trivago(Hotelreserv. ^ Rank = 13)]", "[154TrivagoUK(Hotels.com ^ Rank = 154)]", "[15trivago(Booking.com ^ Rank = 15)]", "[20TrivagoUK(Elvoline ^ Rank = 20)]", "[24trivago(Zleepinghotels ^ Rank = 24)]", "[25TrivagoUK(HRS ^ Rank = 25)]", "[285TrivagoUK(Hotelsclick.com ^ Rank = 285)]", "[3trivago(Hotels.com ^ Rank = 3)]", "[6TrivagoUK(Amoma.com ^ Rank = 6)]", "[84trivago(Snoozee.com ^ Rank = 84)]", "[Tripadvisor(Ctrip.com ^ Rank = 11)]", "[Tripadvisor(Ctrip.com ^ Rank = 9)]" };
        public List<string> RateDescription = new List<string>() { "BEST AVAILABLE RATE. KING GUESTROOM WATERFRONT. SPACIOUS BRIGHT MODERN STYLE ROOM.", "Book now, pay later No Expedia booking or credit card fees Free Cancellation  until Tue, 2 Feb", "FREE cancellation before Apr 15, 2016 PAY LATER – no deposit needed. Not included : 16 % TAX, 1.5 % service charge Meals: There is no meal option with this room. Cancellation: If canceled up to 1 day before the date of arrival, no fee will be charged. If canceled later or in case of no-show, 100 percent of the first night will be charged. Prepayment: No deposit will be charged", "FREE cancellation before Mar 29, 2016 Breakfast included Breakfast is included in the room rate.. Included : 7 % VAT, 1 % City tax, 10 % service charge Meals: Breakfast is included in the room rate. Cancellation: If canceled or modified up to 14 days before date of arrival, no fee will be charged. If canceled or modified later or in case of no-show, the total price of the reservation will be charged. Prepayment: The total price of the reservation will be charged at least 2 days before arrival", "Free Cancellation until Mon, Mar 7 Book now, pay later Free Internet No Expedia booking or credit card fees", "Half Board Pkg. No penalty for changes/cancellations until 01/27/2016. No deposit required", "Internet  Free cancellation before 2016-01-26 01:00 Beijing time / 2016-01-26 00:00 local time Prepay now", "Jetzt buchen, sp#228;ter zahlen  Expedia erhebt keine Buchungs- oder Kreditkartengeb#252;hren  Kostenlose Stornierung   bis Do. , 4.  Feb. ", "STANDARD RATE. DORRINGTON VILLA ACCESSIBLE. GDSGuaranteeType: Guarantee" };
        public List<string> ExtraCharge = new List<string>() { "O", "C", "ND", "RF", "NF" };
        public List<string> RatePlan = new List<string>() { "A07C5L", "A0CC5L", "A0JLV4", "A1DP29", "A1DPR1", "A1KPK2", "A1KPR9", "A1KPRO", "A1KRC0", "A1SP50", "A2DYY1", "A2KPRO", "A2TPRO", "AKTADPR", "AP1PRO", "AROEZZZ", "AUKRAC", "B1KARP", "B1KR29", "B1KRA3", "B1QRAC", "B2TRA3", "BNK1RAC", "C1DPRO", "C1KFAM", "C1KSPL", "D2DRMQ", "DKAARP", "DR12ZZZ", "DR5ZZZZ", "E2DRA9", "G2DBA7", "GXSRACK", "H1HP08", "H1QPRO", "H1QR29", "IA00197", "KSNN5GM", "N2DARP", "NQQ2RAC", "P1KCLB", "S1KPR9", "S1UARP", "S2QP08", "SC1GOV", "SPO3ZZZ", "T1KPR9", "VILP94", "W1KRACA" };
        public List<string> CancelPolicy = new List<string>() { "O", "C", "ND", "RF", "NF" };
        public List<string> Currency = new List<string>() { "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTN", "BWP", "BYR", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EEK", "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL", "GGP", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR", "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LTL", "LVL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRO", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLL", "SOS", "SPL", "SRD", "STD", "SVC", "SYP", "SZL", "THB", "TJS", "TMM", "TMT", "TND", "TOP", "TRY", "TTD", "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VEB", "VEF", "VND", "VUV", "WST", "XAF", "XAG", "XAU", "XCD", "XDR", "XOF", "XPD", "XPF", "XPT", "YER", "ZAR", "ZMK", "ZWD" };
        public List<string> ErrorCodeList = new List<string>() { "SYS01", "SYS00", "WB01" };

        public static List<decimal> Amount
        {

            get
            {
                var lst = new List<decimal>();
                decimal i = 1;
                do
                {
                    lst.Add(i);
                } while (i++ <= 10000);
                return lst;
            }
        }


        public static List<int> RateCount
        {

            get
            {
                var lst = new List<int>();
                int i = 2;
                do
                {
                    lst.Add(i);
                } while (i++ <= ConfigMaster.AppSetting<int>("MaxRatesCount"));
                return lst;
            }
        }

        public static List<int> ThreadDelaySeconds
        {

            get
            {
                var lst = new List<int>();
                int i = 1;
                do
                {
                    lst.Add(i);
                } while (i++ <= ConfigMaster.AppSetting<int>("MaxResponseDelaySeconds"));
                return lst;
            }
        }

        public List<int> MaxPersons = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
        public List<char> MerchantRateIndicators = new List<char>() { 'Y', 'N' };
        public List<char> RateChangeIndicators = new List<char>() { 'N' };

    }
}
