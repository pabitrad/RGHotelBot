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
using System.IO;

namespace BOTManager.BL
{

    public class Loader : MarshalByRefObject
    {

        //static System.Timers.Timer appLoaderTimer;
        //public static AppDomain BotDomain { get; set; }

        public object CallInternal(string dll, string typename, string method, object[] parameters)
        {
            try
            {
                Logger.LogInfo(string.Format("SegmentId:{0} DLL:{1} TypeName:{2} Method:{3}",parameters[0] ,dll, typename, method));
                Assembly a = Assembly.LoadFile(dll);
                Logger.LogInfo(string.Format("SegmentId:{0} Assembly loaded",parameters[0]));
                object o = a.CreateInstance(typename,true);
                Logger.LogInfo(string.Format("SegmentId:{0} Instance Created {1} for typename:{2}",parameters[0],o == null?"0":"1",typename));
                Type t = o.GetType();
                Logger.LogInfo(string.Format("SegmentId:{0} Type Created", parameters[0]));
                MethodInfo m = t.GetMethod(method);
                Logger.LogInfo(string.Format("SegmentId:{0} Invoking segment",parameters[0]));
                return m.Invoke(o, parameters);
            }
            catch (Exception ex)
            {
                Logger.LogInfo(string.Format("SegmentId:{0} Error Occured:{1} StackTrace:{2}",parameters[0],ex.ToString(),ex.StackTrace));
            }
            return 0;
        }

        public static object Call(string dll, string typename, string method, object[] parameters)
        {
            object result = 0;
            string domainName = typename + Guid.NewGuid().ToString().Replace("-", "");
            AppDomain dom = null;
            try
            {
                ConsoleMaster.WriteLine(string.Format("Segment:{0} Creating Appdomain", parameters[0]), ConsoleColor.Yellow);
                dom = AppDomain.CreateDomain(domainName);
                ConsoleMaster.WriteLine(string.Format("Segment:{0} Appdomain Created", parameters[0]), ConsoleColor.Yellow);
                dom.DomainUnload += new EventHandler(dom_DomainUnload);
                dom.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(dom_FirstChanceException);
                dom.AssemblyResolve += new ResolveEventHandler(dom_AssemblyResolve);
                dom.UnhandledException += new UnhandledExceptionEventHandler(dom_UnhandledException);
                ConsoleMaster.WriteLine(string.Format("Segment:{0} Loading Assembly", parameters[0]), ConsoleColor.Yellow);
                Loader ld = (Loader)dom.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(Loader).FullName);
                result = ld.CallInternal(dll, typename, method, parameters);
                ConsoleMaster.WriteLine(string.Format("Segment:{0} Call Completed", parameters[0]), ConsoleColor.Yellow);
                return result;
            }
            catch (System.ObjectDisposedException ex)
            {

            }
            catch (Exception ex)
            {
                Logger.LogInfo(string.Format("Segment:{0} Exception Occured:{1}", parameters[0], ex.ToString()));
            }
            finally
            {
                if (dom != null)
                {
                    AppDomain.Unload(dom);
                }
            }
            return result;
        }

        static void appLoaderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, AppDomain workingAppDomain)
        {
            if (workingAppDomain != null)
                AppDomain.Unload(workingAppDomain);
        }

        static void dom_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            //if (e != null && e.Exception != null)
            //    Logger.LogInfo("Unhandled first level exception occured - " + e.Exception.ToString());
            //if (BotDomain != null)
            //{
            //    AppDomain.Unload(BotDomain);
            //    BotDomain = null;
            //}
        }

        static void dom_DomainUnload(object sender, EventArgs e)
        {
            //if (BotDomain != null)
            //{
            //    BotDomain = null;
            //}
        }

        static Assembly dom_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string supportedFile = BMConfigMaster.AppSetting<string>("NETDLLPath") + args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            return Assembly.LoadFile(supportedFile);
        }

        static void dom_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.IsTerminating)
                Logger.LogInfo(e.ToString());
        }
    }




}
