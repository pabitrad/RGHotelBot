using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RG.Data;
using RG.Utility;
using System.Data.Common;
using System.Globalization;
using System.Net;
using BOTManager.Entities;
using RG.Core.Entities;

namespace BotManager.DbAccess
{
    public class DBAccess
    {

        private static IDBConnector GetDBConnector()
        {
            string database = ConfigMaster.AppSetting<string>("BotDatabase");
            string connectionString = ConfigMaster.AppSetting<string>("BotConnectionStringName");
            return DbConnectorFactory.GetDBConnector(database, connectionString);
        }


        public string SaveRequests(DataTable tblRequests)
        {
            using (IDBConnector dbConnector = GetDBConnector())
            {
                dbConnector.InsertDataTable(tblRequests);
            }
            return "1";
        }


        public List<KeyValuePair<string, string>> GetFreshRequests(int numberOfParallelRequests)
        {
            List<KeyValuePair<string, string>> freshRequests = new List<KeyValuePair<string, string>>();
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("@pNoOfRequests", DbType.Int32, numberOfParallelRequests, ParameterDirection.Input));
                using (IDataReader dataReader = dbConnector.ExecuteReader("SP_GET_FRESH_REQUESTS_V20", parameters.ToArray(), CommandType.StoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        freshRequests.Add(new KeyValuePair<string, string>(dataReader["requestId"].ToString(), dataReader["source"].ToString()));
                    }
                }
            }
            return freshRequests;
        }

        public List<RGRateRequest> GetFreshTVCRequests(int numberOfParallelRequests)
        {
            List<RGRateRequest> freshRequests = new List<RGRateRequest>();
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("@pNoOfRequests", DbType.Int32, numberOfParallelRequests, ParameterDirection.Input));
                using (IDataReader dataReader = dbConnector.ExecuteReader("Select * from tvc_raterequest where status = 3", null, CommandType.Text))
                {
                    while (dataReader.Read())
                    {
                        RGRateRequest request = new RGRateRequest();
                        request.RateRequestID = SafeReadInt(dataReader["RateRequestId"]).Value;
                        request.RequestID = SafeReadInt(dataReader["RequestId"]).Value;
                        request.RequestSegmentID = SafeReadInt(dataReader["RequestSegmentID"]).Value;
                        request.RequestType = SafeReadString(dataReader["RequestType"]);
                        request.RequestPriority = (RequestPriority)SafeReadInt(dataReader["RequestPriority"]).Value;
                        //request.RequestTimeStamp = SafeReadString(dataReader["RequestTimeStamp"]);
                        //request.FirstCheckInDate = SafeReadString(dataReader["FirstCheckInDate"]);
                        request.CheckInDate = SafeReadDateTime(dataReader["FirstCheckIn_Date"]).Value;
                        request.DaysOfData = SafeReadInt(dataReader["DaysOfData"]).Value;
                        request.Guests = SafeReadInt(dataReader["Guests"]).Value;
                        request.MinLengthOfStay = SafeReadInt(dataReader["MinimumLengthOfStay"]).Value;
                        request.SpecialRatePlan = SafeReadString(dataReader["SpecialRatePlan"]);
                        request.Source = SafeReadString(dataReader["Source"]);
                        request.AirportCityCode = SafeReadString(dataReader["AirportCityCode"]);
                        request.City = SafeReadString(dataReader["City"]);
                        request.State = SafeReadString(dataReader["State"]);
                        request.MailCode = SafeReadString(dataReader["MailCode"]);
                        request.Country = SafeReadString(dataReader["Country"]);
                        request.PropertyID = SafeReadString(dataReader["PropertyID"]);
                        request.PropertyName = SafeReadString(dataReader["PropertyName"]);
                        request.PropertyChainCode = SafeReadString(dataReader["PropertyChainCode"]);
                        request.CurrencyCode = SafeReadString(dataReader["CurrencyCode"]);
                        freshRequests.Add(request);
                    }
                }
            }
            return freshRequests;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_WebsiteID"></param>
        /// <returns></returns>
        public WebProxy GetProxySetting(int _WebsiteID)
        {
            WebProxy wp = new WebProxy();
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("webId", DbType.Int32, _WebsiteID, ParameterDirection.Input));
                using (IDataReader dataReader = dbConnector.ExecuteReader("USP_GetProxies", parameters.ToArray(), CommandType.StoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        wp.Address = new Uri(SafeReadString(dataReader["ip"]));
                        wp.BypassProxyOnLocal = true;
                        var credential = new NetworkCredential();
                        wp.Credentials = credential;
                        credential.UserName = SafeReadString(dataReader["UserName"]);
                        credential.Password = SafeReadString(dataReader["Password"]);
                    }
                }
            }
            return wp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="websiteName"></param>
        /// <returns></returns>
        public BOTManager.Entities.RGBot GetBot(string websiteName)
        {
            string sql = "SP_GetWebsiteBotV20";
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("@pWebsiteName", DbType.String, websiteName, ParameterDirection.Input));
                using (IDataReader dataReader = dbConnector.ExecuteReader(sql, parameters.ToArray(), CommandType.StoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        RGBot botToInvoke = new RGBot();
                        botToInvoke.Source = dataReader["Name"].ToString();
                        botToInvoke.Namespace = dataReader["Namespace"].ToString();
                        botToInvoke.MainMethod = dataReader["EntryPoint"].ToString();
                        botToInvoke.ClassName = dataReader["ClassName"].ToString();
                        botToInvoke.SourceId = SafeReadInt(dataReader["WebsiteId"]).Value;
                        botToInvoke.Config = new RGBotConfig(null);
                        //botToInvoke.Config.Proxy = GetProxySetting(botToInvoke.SourceId);
                        botToInvoke.Config.AllowProxy = false;
                        return botToInvoke;
                    }
                }
            }
            throw new KeyNotFoundException("Failed to found entry in tvc_website table for " + websiteName);
        }

        public void UpdateRequestStatus(string requestId, int status)
        {
            string sql = "USP_UpdateReqCompletedStatus";
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("@I_ExitCode", DbType.Int16, status, ParameterDirection.Input));
                parameters.Add(dbConnector.CreateParameter("@I_RequestId", DbType.Int64, Convert.ToInt64(requestId), ParameterDirection.Input));
                dbConnector.ExecuteNonQuery(sql, parameters.ToArray(), CommandType.StoredProcedure);
            }
        }

        public long GetMaxRequestIdForSegment(long segmentId)
        {
            string sql = "SELECT max(RequestId) FROM tvc_Requests WHERE requestSegmentId = @segmentId";
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("@segmentId", DbType.Int32, segmentId, ParameterDirection.Input));
                using (IDataReader dataReader = dbConnector.ExecuteReader(sql, parameters.ToArray(), CommandType.Text))
                {
                    while (dataReader.Read())
                    {
                        return Convert.ToInt32(dataReader[0].ToString());
                    }
                }
            }
            throw new Exception("Failed to get request id from TVC_Request table for segment" + segmentId);
        }

        public string GetReplyLocation(long requestId)
        {
            using (IDBConnector dbConnector = GetDBConnector())
            {
                string sql = string.Format("Select replylocation from tvc_requests where requestid = {0}",requestId);
                using (IDataReader dataReader = dbConnector.ExecuteReader(sql,null,CommandType.Text))
                {
                    while (dataReader.Read())
                    {
                        return SafeReadString(dataReader[0]);
                    }
                }
            }
            return string.Empty;
        }


        public RateAvailabilityResponse GetRequestResponse(long requestId)
        {
            RateAvailabilityResponse response = new RateAvailabilityResponse();
            List<CheckInInfoSegment> segments = new List<CheckInInfoSegment>();
            List<Rate> rates = new List<Rate>();
            CheckInInfoSegment segment = null;
            //response.Reply = new RateAvailabilityReply();
            response.Location = GetReplyLocation(requestId);
            string sql = "SP_GetRequestDataV20";
            using (IDBConnector dbConnector = GetDBConnector())
            {
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(dbConnector.CreateParameter("@RequestID", DbType.Int64, requestId, ParameterDirection.Input));
                using (IDataReader dataReader = dbConnector.ExecuteReader(sql, parameters.ToArray(), CommandType.StoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        if (response.Reply == null)
                        {
                            response.Reply = new RateAvailabilityReply();
                            response.Reply.DataSource = TravelClickNew.Classes.General.FromDot(SafeReadString(dataReader["name"]));
                            response.Reply.HOST = string.Empty;
                            response.Reply.ID = SafeReadString(dataReader["RequestSegmentID"]);
                            response.Reply.PORT = string.Empty;
                            response.Reply.RequestStatus = GetRequestStatus(SafeReadString(dataReader["RateRequestId"]));
                            response.Reply.RequestTimeStamp = SafeReadDateTime(dataReader["RequestTimeStamp"]).Value.ToString("MM/dd/yyyy HH:mm:ss",CultureInfo.InvariantCulture);
                            response.Reply.RequestType = SafeReadString(dataReader["RequestType"]);
                        }
                        segment = segments.FirstOrDefault(s => s.SegmentId == SafeReadInt(dataReader["RequestSegmentId"]).Value);
                        if (segment == null)
                        {
                            segment = new CheckInInfoSegment();
                            segment.Availability = new Availability();
                            segment.SegmentId = SafeReadInt(dataReader["RequestSegmentId"]).Value;
                            segment.CheckInDate = SafeReadDateTime(dataReader["CheckInDate"]).Value.ToString("MMddyyyy", CultureInfo.InvariantCulture);
                            segment.Availability.AvailStatus = TravelClickNew.Classes.General.ErrorDescription(SafeReadString(dataReader["ErrorCode"]));
                            segment.Availability.Rates = new List<Rate>();
                            segments.Add(segment);
                        }
                        if (segment.Availability.AvailStatus == "O" && SafeReadDecimal(dataReader["StayAmount"]).HasValue)
                        {
                            //Rates are present adding them to the list
                            Rate rate = new Rate();
                            rate.RateCode = SafeReadString(dataReader["RateCode"]);
                            rate.RateCategory = SafeReadString(dataReader["RateCategory"]);
                            rate.DailyRateAmount = SafeReadDecimal(dataReader["DailyRateAmount"]).HasValue ? SafeReadDecimal(dataReader["DailyRateAmount"]).Value : 0;
                            rate.StayAmount = SafeReadDecimal(dataReader["StayAmount"]).Value;
                            rate.Currency = SafeReadString(dataReader["Currency"]);
                            rate.AveDailyAmount = SafeReadDecimal(dataReader["AveDailyAmount"]).HasValue ? SafeReadDecimal(dataReader["AveDailyAmount"]).Value : 0;
                            rate.RateChangeInd = SafeReadString(dataReader["RateChangeIndicator"]);
                            rate.RateDescription = SafeReadString(dataReader["RateDescription"]);
                            rate.RoomDescription = SafeReadString(dataReader["RoomDescription"]);
                            rate.RateDescription = System.Web.HttpUtility.HtmlEncode(rate.RateDescription);
                            rate.RoomDescription = System.Web.HttpUtility.HtmlEncode(rate.RoomDescription);
                            rate.MerchantRate = SafeReadString(dataReader["MerchantRate"]);
                            segment.Availability.Rates.Add(rate);
                        }
                    }
                    response.Reply.CheckInInfoSegments = segments.ToArray();
                }
            }

            //Serialize to JSON
            return response;
        }

        public int GetTotalFreshRequests()
        {
            int count = 0;
            string sql = "SP_GET_FRESH_REQUEST_COUNT";
            using (IDBConnector dbConnector = GetDBConnector())
            {
                using (IDataReader dataReader = dbConnector.ExecuteReader(sql, null, CommandType.StoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        return SafeReadInt(dataReader[0]).Value;
                    }
                }
            }
            return 0;
        }



        private string GetRequestStatus(string Ratereq)
        {

            DataSet dsTemp = new DataSet();
            int returnStatus = 2;
            string Query = "SELECT COUNT(*) as total,ISNULL(errorcode,'') as errorcode FROM TVC_RateDetail Where RateRequestID =  " + Ratereq + "   group by errorcode  ";
            using (IDBConnector dbConnector = GetDBConnector())
            {
                using (IDataReader dataReader = dbConnector.ExecuteReader(Query, null, CommandType.Text))
                {
                    int count = 1;
                    while (dataReader.Read())
                    {
                        if (count == 2)
                        {
                            returnStatus = 2;
                        }
                        Query = SafeReadString(dataReader["errorcode"]);
                        if (count == 1)
                        {
                            if (Query.Equals("OK"))
                                returnStatus = 1;
                            else if (Query.Equals("PRP01"))
                                returnStatus = 1;
                            else if (Query.Equals("AVL01"))
                                returnStatus = 1;
                            else if (Query.Equals("RT01"))
                                returnStatus = 1;
                            else if (Query.Equals("AVL02"))
                                returnStatus = 1;
                            else if (Query.Equals("SYS01"))
                                returnStatus = 3;
                            else if (Query.Equals("SYS00"))
                                returnStatus = 3;
                            else if (Query.Equals("WB01"))
                                returnStatus = 5;

                            count++;
                            continue;
                        }
                        if (count > 1)
                        {
                            if (Query.Equals("SYS01"))
                                returnStatus = 5;
                        }
                    }
                }
            }

            if (returnStatus == 1)
                Query = "Fulfilled";
            else if (returnStatus == 2)
                Query = "Partial Fulfillment";
            else if (returnStatus == 3)
                Query = "System error";
            else if (returnStatus == 4)
                Query = "Property not found";
            else if (returnStatus == 5)
                Query = "Site Unavailble";
            return Query;
        }


        private static string SafeReadString(object str)
        {
            return (str == DBNull.Value) ? null : str.ToString();
        }

        private static int? SafeReadInt(object intValue)
        {
            int outParameter;
            if (int.TryParse(intValue.ToString(), out outParameter))
                return outParameter;
            else
                return (int?)null;
        }

        private static decimal? SafeReadDecimal(object decimalValue)
        {
            return (decimalValue == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(decimalValue.ToString());
        }


        private static DateTime? SafeReadDateTime(object dtValue)
        {
            return (dtValue == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(dtValue);
        }


        public void ResetBotManager()
        {
            using (IDBConnector dbConnector = GetDBConnector())
            {
                dbConnector.ExecuteNonQuery("RG_RESET_BOTMANAGER_V21", null, CommandType.StoredProcedure);
            }
        }
    }
}
