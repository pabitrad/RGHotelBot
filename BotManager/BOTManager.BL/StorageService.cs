using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RG.Utility;
using RG.Core.Entities;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace BOTManager.BL
{

    public class StorageService
    {
        static int responseCount = 0;

        public static string SubmitBOTResponse(string segmentId, RateAvailabilityResponse response, string myIP, bool isExpired = false)
        {
            DataServiceClient client = null;
            RG.Core.Entities.BOTManager manager = new RG.Core.Entities.BOTManager();
            manager.ProductName = BMConfigMaster.AppSetting<string>("ProductName");
            manager.IPAddress = myIP;
            string resp = SerializerHelper.JsonSerialize(response);
            try
            {
                client = new DataServiceClient();
                ConsoleMaster.WriteLine(string.Format("Response recieved Count :{0}", ++responseCount));
                int reponseNumber = responseCount - 1;
                DateTime dtStart = DateTime.Now;
                ConsoleMaster.WriteLine("Sending Response");
                if (BMConfigMaster.Exists("LogBOTResponsePath") && Directory.Exists(BMConfigMaster.AppSetting<string>("LogBOTResponsePath")))
                {
                    //Check if directory of current date exists if not then create it.
                    string dateDir = DateTime.Now.ToString("MMddyyyy");
                    if (!Directory.Exists(Path.Combine(BMConfigMaster.AppSetting<string>("LogBOTResponsePath"), dateDir)))
                    {
                        Directory.CreateDirectory(Path.Combine(BMConfigMaster.AppSetting<string>("LogBOTResponsePath"), dateDir));
                    }
                    File.WriteAllText(Path.Combine(BMConfigMaster.AppSetting<string>("LogBOTResponsePath"), dateDir, segmentId + ".xml"), resp);
                }
                var i = client.Echo("Hello");
                //if (isExpired)
                //    client.RemoveQueueFromCache(Convert.ToInt32(segmentId));
                string availStatus = string.Empty;
                if (response != null && response.Reply != null && response.Reply.CheckInInfoSegments.Length > 0 && response.Reply.CheckInInfoSegments[0].Availability != null)
                    availStatus = response.Reply.CheckInInfoSegments[0].Availability.AvailStatus;
                if (availStatus != "O" || SendResponse(response.Location, resp, segmentId, availStatus))
                {
                    client.SubmitBOTResponse(manager, new KeyValuePair<long, string>(Convert.ToInt32(segmentId), availStatus));
                    TimeSpan ts = (DateTime.Now - dtStart);
                    var speed = (Convert.ToDouble(resp.Length) / Convert.ToDouble(ts.TotalMilliseconds));
                    ConsoleMaster.WriteLine(string.Format("Response Saved at speed of :{0} bytes/ms || Bytes:{1} for Reponse: {2}", speed.ToString(), resp.Length, reponseNumber));
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Failed to send message to data manager. The system will resend all failed requests in retry. Error:" + ex.ToString());
                if (BMConfigMaster.Exists("LogFailedBOTResponsePath") && Directory.Exists(BMConfigMaster.AppSetting<string>("LogFailedBOTResponsePath")))
                {
                    //Check if directory of current date exists if not then create it.
                    string dateDir = DateTime.Now.ToString("MMddyyyy");
                    if (!Directory.Exists(Path.Combine(BMConfigMaster.AppSetting<string>("LogFailedBOTResponsePath"), dateDir)))
                    {
                        Directory.CreateDirectory(Path.Combine(BMConfigMaster.AppSetting<string>("LogFailedBOTResponsePath"), dateDir));
                    }
                    File.WriteAllText(Path.Combine(BMConfigMaster.AppSetting<string>("LogFailedBOTResponsePath"), dateDir, segmentId + ".xml"), resp);
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Abort();
                    client = null;
                }
            }
            return "1";
        }

        /// <summary>
        /// Store on S3
        /// </summary>
        /// <returns></returns>
        public static bool SendResponse(string urlToPost, string content, string segmentId,string availStatus)
        {
            try
            {
                if (availStatus != "O") return true;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                // Write the XML text into the stream
                if (BMConfigMaster.Exists("LogResponses") && BMConfigMaster.AppSetting<int>("LogResponses") == 1)
                {
                    Logger.LogInfo(string.Format("SegmentId:{0} URL:({1}) ContentLength:({2}) Content:{3} ", segmentId, urlToPost, content.Length, content));
                }
                int count = 1;
                do
                {
                    //WebResponse rsp = null;
                    var req = WebHelper.GetWebRequest(urlToPost) as HttpWebRequest;
                    req.Method = "PUT";        // Post method
                    req.ContentType = "text/json";     // content type
                    // Wrap the request stream with a text-based writer
                    using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
                    {
                        //Logger.LogInfo("Content (" + content+")");
                        writer.WriteLine(content);
                        writer.Close();
                    }
                    try
                    {
                        // Send the data to the webserver
                        using (WebResponse rsp = req.GetResponse())
                        {
                            var httpResponse = rsp as HttpWebResponse;
                            if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                            {
                                Logger.LogInfo(string.Format("SegmentId:{0} Time Taken to upload to S3 is {1} ms", segmentId, sw.ElapsedMilliseconds));
                                return true;
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        var webException = (HttpWebResponse)ex.Response;
                        if (webException.StatusCode != HttpStatusCode.InternalServerError)
                        {
                            string result = string.Empty;
                            using (var streamReader = new StreamReader(webException.GetResponseStream()))
                            {
                                result = streamReader.ReadToEnd();
                            }
                            string header = string.Empty;
                            string Error = string.Format("WebException Header:{0} Response:{1}", header, result);
                            Logger.LogException(string.Format("SegmentId:{0} Failed to post to url:{1} ContentLength:{2} Error:{3}", segmentId, urlToPost, content.Length, Error), ex);
                        }
                    }
                } while (count++ <= 3);
                Logger.LogInfo(string.Format("SegmentId:{0} Time Taken to upload to S3 is {1} ms", segmentId, sw.ElapsedMilliseconds));
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(string.Format("SegmentId:{0} Failed to post to url:{1} ContentLength:{2} Error:{3}", segmentId, urlToPost, content.Length, ex.ToString()));
                return false;
            }
        }



    }

}
