using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOTManager.Entities;
using System.Reflection;
using RG.Core.Entities.BOT;
using System.Threading;
using RG.Utility;
using System.Net;
using System.Diagnostics;

namespace BOTManager.BL
{



    //<summary>
    //This class is only intended to invoke BOTs and raise events in case when response is received or any error has been occured.
    //NOTE: PLEASE DO NOT WRITE ANY CODE IN THIS CLASS THAT WILL BE SAVING DATA SOMEWHERE SO, THAT WE CAN REUSE IT GOING FORWARD
    //</summary>
    public class TPCLD : IDisposable
    {
        private static string MYIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        public event EventHandler<BOTArgs> BOTFailedEvent;
        public event EventHandler<BOTEventArgs> SubmitBOTResponseEvent;
        public event EventHandler<BOTArgs> BOTExecutionSuccessEvent;


        public string InvokeBot(string requestId, RGBot botToInvoke, string botAssemblyPath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                sw.Restart();
                ConsoleMaster.WriteLine(string.Format("Calling SegmentId:{0} TypeName:{1} Method:{2}", requestId, string.Format("{0}.{1}", botToInvoke.Namespace, botToInvoke.ClassName),botToInvoke.MainMethod), ConsoleColor.Yellow);
                var exitCode = Loader.Call(botAssemblyPath, string.Format("{0}.{1}", botToInvoke.Namespace, botToInvoke.ClassName),
                    botToInvoke.MainMethod, new object[] { int.Parse(requestId) });
                // to check if object returned by Invoke method is null
                if (exitCode != null)
                {
                    if (BOTExecutionSuccessEvent != null && Convert.ToInt16(exitCode) == 1)
                        BOTExecutionSuccessEvent(this, new BOTArgs() { RequestId = requestId, BotInvoked = botToInvoke, exitCode = Convert.ToInt16(exitCode), TimeTakenMilliSeconds = sw.ElapsedMilliseconds });
                    else
                        throw new Exception("DLL is invoked but BOT returned exit code other than 1");
                }
                else
                {
                    throw new EntryPointNotFoundException(string.Format("DLL Loading failed for {0} bot RequestId:{1} Server:{2}", botToInvoke.Source, requestId, MYIP));
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(string.Format("TPCLD call failed for {0} bot RequestId:{1} Server:{2} Error:{3}", botToInvoke.Source, requestId, MYIP, ex.ToString()));
                if (BOTFailedEvent != null)
                    BOTFailedEvent(this, new BOTArgs() { BotInvoked = botToInvoke, RequestId = requestId, TimeTakenMilliSeconds = sw.ElapsedMilliseconds });
            }
            return "1";
        }

        public string InvokeBot(RGRateRequest request, RGBot botToInvoke, string botAssemblyPath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                sw.Restart();
                var exitCode = Loader.Call(botAssemblyPath, string.Format("{0}.{1}", botToInvoke.Namespace, botToInvoke.ClassName),
                    botToInvoke.MainMethod, new object[] { request });
                // to check if object returned by Invoke method is null
                if (exitCode != null)
                {
                    if (BOTExecutionSuccessEvent != null)
                    {
                        if (exitCode.GetType() == typeof(int))
                            BOTExecutionSuccessEvent(this, new BOTArgs() { RequestId = request.RequestID.ToString(), Request = new List<RGRateRequest> { request }, BotInvoked = botToInvoke, Responses = null, exitCode = Convert.ToInt16(exitCode), TimeTakenMilliSeconds = sw.ElapsedMilliseconds });
                        if (exitCode.GetType() == typeof(List<RGRateDetail>))
                            BOTExecutionSuccessEvent(this, new BOTArgs() { RequestId = request.RequestID.ToString(), Request = new List<RGRateRequest> { request }, BotInvoked = botToInvoke, Responses = exitCode as List<RGRateDetail>, TimeTakenMilliSeconds = sw.ElapsedMilliseconds });
                    }
                    else
                        throw new Exception("DLL is invoked but BOT returned exit code other than 1");
                }
                else
                {
                    throw new EntryPointNotFoundException(string.Format("DLL Loading failed for {0} bot RequestId:{1} Server:{2}", botToInvoke.Source, request.RequestID, MYIP));
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(string.Format("TPCLD call failed for {0} bot RequestId:{1} Server:{2} Error:{3}", botToInvoke.Source, request.RequestID.ToString(), MYIP, ex.ToString()));
                if (BOTFailedEvent != null)
                {
                    BOTFailedEvent(this, new BOTArgs() { BotInvoked = botToInvoke, Request = new List<RGRateRequest> { request }, RequestId = request.RequestID.ToString(), TimeTakenMilliSeconds = sw.ElapsedMilliseconds });
                }
            }
            return "1";
        }

        public string InvokeBotOld(string requestId, RGBot botToInvoke, string botAssemblyPath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {

                var botAssembly = Assembly.LoadFrom(botAssemblyPath);
                var botInstance = botAssembly.CreateInstance(string.Format("{0}.{1}", botToInvoke.Namespace, botToInvoke.ClassName), true);
                if (botInstance is IRGBot)
                {
                    IRGBot rgBot = botInstance as IRGBot;
                    rgBot.SubmitResponse += new EventHandler<BOTEventArgs>(rgBot_SubmitResponse);
                }
                //Thread.Sleep(100);
                var botType = botInstance.GetType();
                var mainMethod = botType.GetMethod(botToInvoke.MainMethod);
                sw.Restart();
                var exitCode = mainMethod.Invoke(botInstance, new object[] { int.Parse(requestId) });
                sw.Stop();
                //System.Threading.Thread.Sleep(100);
                // to check if object returned by Invoke method is null
                if (exitCode != null)
                {
                    if (BOTExecutionSuccessEvent != null && Convert.ToInt16(exitCode) == 1)
                        BOTExecutionSuccessEvent(this, new BOTArgs() { RequestId = requestId, BotInvoked = botToInvoke, exitCode = Convert.ToInt16(exitCode), TimeTakenMilliSeconds = sw.ElapsedMilliseconds });
                    else
                        throw new Exception("DLL is invoked but BOT returned exit code other than 1");
                }
                else
                {
                    throw new EntryPointNotFoundException(string.Format("DLL Loading failed for {0} bot RequestId:{1} Server:{2}", botToInvoke.Source, requestId, MYIP));
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(string.Format("TPCLD call failed for {0} bot RequestId:{1} Server:{2} Error:{3}", botToInvoke.Source, requestId, MYIP, ex.ToString()));
                if (BOTFailedEvent != null)
                    BOTFailedEvent(this, new BOTArgs() { BotInvoked = botToInvoke, RequestId = requestId });
            }
            return "1";
        }

        void rgBot_SubmitResponse(object sender, BOTEventArgs e)
        {
            try
            {
                if (SubmitBOTResponseEvent != null)
                    SubmitBOTResponseEvent(this, e);
            }
            catch (Exception ex)
            {
                Logger.LogException(string.Format("Failed to submit response from TPCLD for request {0} Server:{1}", e.RequestId, MYIP), ex);
            }
        }

        public void Dispose()
        {

        }
    }
}
