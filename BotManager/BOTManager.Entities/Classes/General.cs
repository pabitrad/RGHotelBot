using System;
using System.Data;
using System.Data.SqlClient;
//using BOTManager.Entities.Classes;

namespace TravelClickNew.Classes
{
    /// <summary>
    /// Summary description for General.
    /// </summary>
    public class General
    {
        public General()
        {
        }

        public static DateTime ConvertToDate(string Expression)
        {
            string dd, mm, yy;
            mm = Expression.Substring(0, 2);
            dd = Expression.Substring(2, 2);
            yy = Expression.Substring(4, 4);

            string dt = string.Empty;
            dt = mm + "/" + dd + "/" + yy;
            return Convert.ToDateTime(dt);
        }

        public static DateTime ConvertToDateTime(string Expression)
        {
            string dd, mm, yy, hh, MM, ss;
            mm = Expression.Substring(0, 2);
            dd = Expression.Substring(2, 2);
            yy = Expression.Substring(4, 4);

            hh = Expression.Substring(8, 2);
            MM = Expression.Substring(10, 2);
            ss = Expression.Substring(12, 2);

            string dt = string.Empty;
            dt = mm + "/" + dd + "/" + yy + " " + hh + ":" + MM + ":" + ss;
            return Convert.ToDateTime(dt);
        }

        //public static string RequestStatus(long RateRequest)
        //{
        //    string status = string.Empty;
            
        //    List<DbParameters> 
        //    SqlParameter[] parameter = {
        //                                   new SqlParameter("@RateRequestID",SqlDbType.BigInt),
        //                                   new SqlParameter("@ReturnStatus",SqlDbType.Int)
        //                               };
        //    parameter[0].Value = RateRequest;
        //    parameter[1].Direction = ParameterDirection.Output;

        //    DBConnection.Utility.UpdateProcedure("SP_RequestStatus_New", parameter);

        //    int RStatus = int.Parse(parameter[1].Value.ToString());
        //    if (RStatus == 1)
        //        status = "Fulfilled";
        //    else if (RStatus == 2)
        //        status = "Partial Fulfillment";
        //    else if (RStatus == 3)
        //        status = "System error";
        //    else if (RStatus == 4)
        //        status = "Property not found";
        //    else if (RStatus == 5)
        //        status = "Site Unavailble";
        //    return status;
        //}

        public static string DotTo(string Source)
        {
            string Desc = string.Empty;
            string temp = Source;
            Source = Source.ToUpper();
            switch (Source)
            {
                case "BOOKINGSDOTORG":
                    Desc = "bookingsorg";
                    break;
                case "BOOKING.COM":
                    Desc = "Booking";
                    break;
                case "BOOKINGDOTORG":
                    Desc = "bookingsorg";
                    break;
                case "HOTELSDOTNL":
                    Desc = "Hotelsnl";
                    break;
                case "PLACESTOSTAY":
                    Desc = "PlacesToStay";
                    break;
                case "DEVERE":
                    Desc = "DEVEREONLINECOUK";
                    break;
                case "JURYDOYLE":
                    Desc = "JURYSDOYLE";
                    break;
                case "JURYSDOYLE":
                    Desc = "JURYSDOYLE";
                    break;
                case "HOTELCLUB":
                    Desc = "Hotelclubnet";
                    break;
                case "HOTEL.DE": // convert HOTEL.DE to HotelDe
                    Desc = "HotelDe";
                    break;
                case "HOTELS.BE":
                    Desc = "Hotelsbe";
                    break;
                default:
                    Desc = temp;
                    break;
            }
            return Desc;
        }
        public static string FromDot(string Source)
        {

            string Desc = string.Empty;
            string temp = Source;
            Source = Source.ToUpper();

            switch (Source)
            {
                case "BOOKINGSORG":
                    Desc = "BookingsDotOrg";
                    break;
                case "BOOKING":
                    Desc = "Booking.com";
                    break;
                case "HOTELSNL":
                    Desc = "HotelsDotNl";
                    break;
                case "PLACESTOSTAY":
                    Desc = "PlacesToStay";
                    break;
                case "JURYSDOYLE":
                    Desc = "JuryDoyle";
                    break;
                case "DEVEREONLINECOUK":
                    Desc = "DeVere";
                    break;
                case "HILTON":
                    Desc = "Hilton";
                    break;
                case "Hotelclubnet":
                    Desc = "HOTELCLUB";
                    break;
                case "HOTELDE": // convert HotelDe
                    Desc = "Hotel.DE"; // Hotel.De
                    break;
                case "HOTELSBE":
                    Desc = "Hotels.be";
                    break;
                default:
                    Desc = temp;
                    break;
            }
            return Desc;
        }

        public static string ErrorDescription(string ErrorCode)
        {
            if (string.IsNullOrWhiteSpace(ErrorCode))
                ErrorCode = string.Empty;
            
            string Desc = string.Empty;
            ErrorCode = ErrorCode.ToUpper();
            switch (ErrorCode)
            {
                case "AUT01":
                    Desc = "Invalid password";
                    break;
                case "AUT02":
                    Desc = "Un-authorized request type";
                    break;
                case "AVL01":
                    Desc = "C";
                    break;
                case "AVL02":
                    Desc = "C";
                    break;
                case "BKP19":
                    Desc = "Booking was previously canceled";
                    break;
                case "CCE01":
                    Desc = "Unable to process credit card";
                    break;
                case "CCE02":
                    Desc = "Credit card denied due to possible fraudulent activity";
                    break;
                case "CNL01":
                    Desc = "Unable to find booking";
                    break;
                case "DBE01":
                    Desc = "Database error";
                    break;
                case "DTE01":
                    Desc = "Invalid Dates";
                    break;
                case "DTE02":
                    Desc = "Arrival date has passed";
                    break;
                case "DTE03":
                    Desc = "Departure date does not follow arrival date";
                    break;
                case "NGT01":
                    Desc = "Minimum number of nights stay requirement";
                    break;
                case "NGT02":
                    Desc = "Maximum number of nights exceeded";
                    break;
                case "NGT03":
                    Desc = "Cutoff day requirement not met";
                    break;
                case "NGT04":
                    Desc = "Closed for arrival";
                    break;
                case "NGT05":
                    Desc = "Closed for departure";
                    break;
                case "PPL01":
                    Desc = "Maximum number of people exceeded";
                    break;
                case "PPL02":
                    Desc = "Maximum number of adults exceeded";
                    break;
                case "PPL03":
                    Desc = "Maximum number of children exceeded";
                    break;
                case "PRP01":
                    Desc = "NF";
                    break;
                case "PRP02":
                    Desc = "NF";
                    break;
                case "SYS00":
                    Desc = "RF";
                    break;
                case "SYS01":
                    Desc = "RF";
                    break;
                case "SY01":
                    Desc = "RF";
                    break;
                case "XML01":
                    Desc = "RF";
                    break;
                case "XML02":
                    Desc = "Missing element";
                    break;
                case "XML03":
                    Desc = "Duplicate elements exist where not allowed";
                    break;
                case "XML04":
                    Desc = "Missing attribute";
                    break;
                case "XML05":
                    Desc = "Number of elements of this type exceeds maximum";
                    break;
                case "WB01":
                    Desc = "RF";
                    break;
                case "WB02":
                    Desc = "RF";
                    break;
                case "INC01":
                    Desc = "RF";
                    break;
                case "OK":
                    Desc = "O";
                    break;
                case "RT01":
                    Desc = "NR";
                    break;
                default:
                    Desc = "RF";
                    break;
            }
            return Desc;
        }
        public static string BrandChange(string propertyCode)
        {

            switch (propertyCode)
            {
                case "N1":
                    propertyCode = "CHIFLEY";
                    break;
                case "N2":
                    propertyCode = "CONSTELLATION";
                    break;
                case "N3":
                    propertyCode = "MIRVAC";
                    break;
                case "N4":
                    propertyCode = "DUXTON";
                    break;
                case "N5":
                    propertyCode = "AMORAHOTELS";
                    break;
                case "N6":
                    propertyCode = "MillerApartments";
                    break;
                case "Y1":
                    propertyCode = "STAMFORD";
                    break;
                case "Y2":
                    propertyCode = "SAVILLE";
                    break;
                case "C4":
                    propertyCode = "CairnHotel";
                    break;
                default:
                    //propertyCode = propertyCode;
                    break;

            }

            return propertyCode;

        }
    }
}
