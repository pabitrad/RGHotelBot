using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOTManager.Entities.Crawl;
using System.Net;
using System.Reflection;
using RG.Utility;
using System.IO;
using System.Diagnostics;
using BOTManager.Entities;
using BOTManager.Entities.Crawl.Interfaces;

using System.IO.Compression;
using System.Reflection.Emit;
using System.Runtime.Caching;
using System.Collections.Specialized;
using RG.Core.Entities;
using RG.Core.Entities.BOT;
using System.Globalization;

namespace BOTManager.BL
{
    public class DynamicInitializer
    {
        public static V NewInstance<V>() where V : class
        {
            return ObjectGenerator(typeof(V)) as V;
        }

        private static object ObjectGenerator(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty,
                          type,
                          new Type[0],
                          target.DeclaringType);
            var il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
            return method();
        }

        public static object NewInstance(Type type)
        {
            return ObjectGenerator(type);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class RGCrawlManager : MarshalByRefObject
    {
        private List<RGBot> botConfiguration = new List<RGBot>();

        public RGCrawlManager()
        {

        }

        //static System.Timers.Timer appLoaderTimer;

        private static AppDomain _botDomain = null;

        /// <summary>
        /// 
        /// </summary>
        public static AppDomain BotDomain
        {
            get
            {
                if (_botDomain == null)
                {
                    if (_botDomain != null)
                        AppDomain.Unload(_botDomain);
                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    _botDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                    _botDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(_botDomain_FirstChanceException);
                    _botDomain.AssemblyResolve += new ResolveEventHandler(_botDomain_AssemblyResolve);
                    _botDomain.UnhandledException += new UnhandledExceptionEventHandler(_botDomain_UnhandledException);
                    Logger.LogInfo(string.Format("APPDOMAIN CREATED IN {0} ms", sw.ElapsedMilliseconds));
                }
                return _botDomain;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _botDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_botDomain != null)
                AppDomain.Unload(_botDomain);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Assembly _botDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string supportedFile = BMConfigMaster.AppSetting<string>("NETDLLPath") + args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            return Assembly.LoadFile(supportedFile);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _botDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (_botDomain != null)
                AppDomain.Unload(_botDomain);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static RGCrawlManager GetCrawler()
        {
            return (RGCrawlManager)BotDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(RGCrawlManager).FullName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifestModule"></param>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public Assembly GetAssembly(string manifestModule, string assemblyPath)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var existingDLL = assemblies.FirstOrDefault(y => y.ManifestModule.ToString().ToUpper() == manifestModule.ToUpper());
            return (existingDLL == null) ? Assembly.LoadFile(assemblyPath) : existingDLL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="botAssemblyPath"></param>
        /// <param name="assemblyDLL"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object InvokeBotCrawl(string botAssemblyPath, string assemblyDLL, object[] parameters)
        {
            Stopwatch sw = new Stopwatch();
            RGRateRequest rateRequest = null;
            sw.Start();
            List<KeyValuePair<int, string>> CrawlResponses = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<int, string>> LevelUrls = new List<KeyValuePair<int, string>>();
            var crawlRequest = parameters[0] as CrawlRequest;
            rateRequest = crawlRequest.RequestParameterObject as RGRateRequest;
            try
            {
                sw.Restart();
                List<RGRateDetail> rateDetails = new List<RGRateDetail>();
                Assembly a = GetAssembly(assemblyDLL, botAssemblyPath);
                var crawlerType = a.GetTypes().First(y => y.GetInterfaces().Contains(typeof(IRGCrawler)));
                var parserType = a.GetTypes().First(y => y.GetInterfaces().Contains(typeof(IRGParser)));
                var crawler = (IRGCrawler)DynamicInitializer.NewInstance(crawlerType);
                var parser = (IRGParser)DynamicInitializer.NewInstance(parserType);
                Logger.LogInfo(string.Format("ASSEMBLY LOADED IN {0} ms", sw.ElapsedMilliseconds));
                if (crawler != null && parser != null)
                {
                    sw.Restart();
                    int level = 1;
                    //CrawlObject obj = crawler.GetCrawlObject(parameters[0] as TVC_RateRequest);
                    //CreateFolderStructure(rateRequest);
                    ParseResult parseResult = null;
                    BotRequestParameterObject nextLevelRequestObject = rateRequest;
                    do
                    {
                        if (level > 500)
                            throw new NotSupportedException("Breaking request as it crossed supported crawling level of 500 Segment:" + rateRequest.RequestSegmentID);
                        CrawlObject obj = crawler.GetCrawlObject(crawlRequest, level++);
                        if (obj != null)
                        {
                            obj.Level = level - 1;//Assign level to crawlobject in case it is not set in bot.
                            //Code to Read configuration of Bot
                            sw.Restart();
                            InvokeCrawlObject(obj, CrawlResponses, LevelUrls);
                            Logger.LogInfo(string.Format("SegmentId:{3} BOT:{0} Level:{1} CrawlResponseTime(ms):{2} ", rateRequest.Source, obj.Level, sw.ElapsedMilliseconds, rateRequest.RequestSegmentID));
                            sw.Restart();
                            parseResult = parser.ParseResponse(obj);
                            Logger.LogInfo(string.Format("SegmentId:{3}  BOT:{0} Level:{1} ParseResponseTime(ms):{2} ", rateRequest.Source, obj.Level, sw.ElapsedMilliseconds, rateRequest.RequestSegmentID));
                            if (parseResult != null)
                            {
                                //Get next crawl request parameter object. This should be supplied with 
                                //parser of current level and crawl request parameters will be input for 
                                //next level crawling.
                                nextLevelRequestObject = parseResult.NextLevelRequestParameterObject;
                                if (nextLevelRequestObject != null)
                                    crawlRequest = new CrawlRequest() { PreviousCrawlObject = obj, CurrentLevel = level, RequestParameterObject = nextLevelRequestObject };
                            }
                        }
                    } while (parseResult != null && nextLevelRequestObject != null);

                    return parseResult.RateDetail;
                }
            }
            catch (NRException nrException)
            {
                rateRequest.RateDetail.AvailStatus = "NR";
                rateRequest.RateDetail.ErrorCode = "RT01";
                Logger.LogWarning(string.Format("Set NR Status to process rate request {0} Error:{1}", rateRequest.RequestSegmentID, nrException.ToString()));
                return rateRequest.RateDetail;
            }
            catch (Exception ex)
            {
                if (rateRequest != null)
                {
                    Logger.LogWarning(string.Format("Failed to process rate request {0} Error:{1}", rateRequest.RequestSegmentID, ex.ToString()));
                }
                else
                {
                    Logger.LogException("Failed to process request", ex);
                }
            }
            finally
            {
                if (rateRequest != null)
                {
                    SaveCachePage(rateRequest, CrawlResponses, LevelUrls);
                }
            }
            return null;
        }

        private void SaveCachePage(RGRateRequest rgRateRequest, List<KeyValuePair<int, string>> CrawlResponses, List<KeyValuePair<int, string>> LevelUrls)
        {
            try
            {
                string segmentPath = Path.Combine(ConfigMaster.AppSetting<string>("CachePagePath"), rgRateRequest.RequestTimeStampUTC.ToString("MMddyyyy", CultureInfo.InvariantCulture), rgRateRequest.RequestSegmentID.ToString());
                if (!Directory.Exists(segmentPath))
                {
                    Directory.CreateDirectory(segmentPath);
                }
                string urlFile = Path.Combine(segmentPath, "Urls.txt");

                string responseFile = string.Format("{0}_{1}.html", 1, Guid.NewGuid());
                if (rgRateRequest.RateDetail != null && !string.IsNullOrWhiteSpace(rgRateRequest.RateDetail.CachePageContent))
                    File.WriteAllText(Path.Combine(segmentPath, responseFile), rgRateRequest.RateDetail.CachePageContent);

                StringBuilder sb = new StringBuilder();
                LevelUrls.OrderBy(y => y.Key).ToList().ForEach(levelURL =>
                {
                    sb.AppendLine();
                    sb.AppendLine(string.Format("Level:{0} || URL:{1}", levelURL.Key, levelURL.Value));
                    sb.AppendLine();
                });
                File.WriteAllText(urlFile, sb.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Failed to save cachepages for " + rgRateRequest.RequestSegmentID + ". Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="websiteName"></param>
        /// <returns></returns>
        public static RGBot GetBot(string websiteName)
        {
            var dataManagerClient = new DataServiceClient();
            //Get NameValue collection for a bot create
            var cache = CacheMaster.DefaultCache() as MemoryCache;
            var bot = cache["BOTCONFIG_" + websiteName.ToUpper()] as RGBot;
            if (bot == null)
            {
                lock (new object())
                {
                    //Create bot
                    bot = dataManagerClient.GetBot(websiteName);
                //    cache.Set("BOTCONFIG_" + websiteName.ToUpper(), bot, DateTimeOffset.Now.AddMinutes(BMConfigMaster.AppSetting<int>("BotConfigTimeOutMinutes")));
                }
            }
            //Set a random proxy
            if (bot.Proxylist != null && bot.Proxylist.Count > 0)
            {
                bot.CurrentProxy = bot.Proxylist.PickRandom<RGProxy>();
            }
            else
            {
                bot.CurrentProxy = null;
            }
            return bot;
        }

        private static T GetValue<T>(string keyName, NameValueCollection propertyBag, T defaultValue)
        {
            if (propertyBag[keyName] == null)
                return defaultValue;
            return (T)Convert.ChangeType(propertyBag[keyName], typeof(T));
        }


        public void InvokeCrawlObject(CrawlObject crawlObject, List<KeyValuePair<int, string>> CrawlResponses, List<KeyValuePair<int, string>> LevelUrls)
        {
            //Skip if url is missing 
            crawlObject.RGWebRequests.ForEach(rgWebRequest =>
            {
                var rateRequest = crawlObject.RateRequest;
                int iRetry = 3;
                //Invoke this request and send the response.
                int iCount = 1;
                bool isFetched = false;
                CrawlResponse crawlResponse = new CrawlResponse(crawlObject, rgWebRequest);
                rgWebRequest.Response = crawlResponse;
                WebRequest webRequest = rgWebRequest.WebRequest;
                if (crawlObject.RateRequest.Bot != null && crawlObject.RateRequest.Bot.AllowProxy && crawlObject.RateRequest.Bot.CurrentProxy != null)
                {
                    webRequest.Proxy = GetWebProxy(crawlObject.RateRequest.Bot.CurrentProxy);
                }
                if (!string.IsNullOrWhiteSpace(rgWebRequest.PostDetails))
                {
                    webRequest.ContentLength = rgWebRequest.PostDetails.Length;
                    using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        writer.Write(rgWebRequest.PostDetails);
                    }
                }

                do
                {
                    //Create HTTP Object
                    try
                    {
                        LevelUrls.Add(new KeyValuePair<int, string>(rgWebRequest.CrawlObject.Level, rgWebRequest.Url));
                        //if (rateRequest.Bot.AllowProxy)
                        //   Logger.LogInfo(string.Format("Segment:{0} IPAddress:{1} Category:{2} Source:{3}", rateRequest.RequestSegmentID, rateRequest.Bot.CurrentProxy.IpAddress, rateRequest.Bot.CurrentProxy.Category, rateRequest.Bot.Source));
                        using (WebResponse response = webRequest.GetResponse())
                        {
                            crawlResponse.Response = response;
                            crawlResponse.ResponseUri = response.ResponseUri;
                            crawlResponse.ResponseHeaders = GetHeaderCollection(response);
                            if (response is HttpWebResponse)
                            {
                                HttpWebResponse webResponse = response as HttpWebResponse;
                                crawlResponse.Cookies.Add(webResponse.Cookies);
                                if (webResponse.StatusCode == HttpStatusCode.Redirect || webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.SeeOther)
                                {
                                    isFetched = true;
                                }
                                if (webResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    Encoding defaultEncoding = Encoding.UTF8;
                                    if (!string.IsNullOrWhiteSpace(webResponse.CharacterSet)&&webResponse.CharacterSet.ToLower().Contains("windows-31j"))                                    
                                        defaultEncoding = Encoding.GetEncoding(932);                                    
                                    else if (!string.IsNullOrWhiteSpace(webResponse.CharacterSet) && Encoding.GetEncoding(webResponse.CharacterSet) != null)
                                        defaultEncoding = Encoding.GetEncoding(webResponse.CharacterSet);
                                    Stream ResponseStream = webResponse.GetResponseStream();
                                    if (webResponse.ContentEncoding.ToLower().Contains("gzip"))
                                        ResponseStream = new GZipStream(ResponseStream, CompressionMode.Decompress);
                                    else if (webResponse.ContentEncoding.ToLower().Contains("deflate"))
                                        ResponseStream = new DeflateStream(ResponseStream, CompressionMode.Decompress);
                                    using (StreamReader reader = new StreamReader(ResponseStream, defaultEncoding, true))
                                    {
                                        crawlResponse.ResponseString = reader.ReadToEnd();
                                    }
                                    isFetched = true;
                                }
                            }
                            response.Close();
                        }
                    }
                    catch (ObjectDisposedException objectDisposed)
                    {
                        Logger.LogWarning("Segment:" + rateRequest.RequestSegmentID + " ObjectDisposedException occured:" + objectDisposed.ToString() + " Retry:" + iCount);
                        break; // No need to further try as object is already disposed.
                    }
                    catch (WebException webException)
                    {

                        Logger.LogWarning("Segment:" + rateRequest.RequestSegmentID + "WebException occured:" + webException.ToString() + " Retry:" + iCount);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning("Segment:" + rateRequest.RequestSegmentID + "Exception occured:" + ex.ToString() + " Retry:" + iCount);
                    }
                } while (iCount++ <= iRetry && !isFetched);
            });
            crawlObject.ChildCollection.ForEach(y => InvokeCrawlObject(y, CrawlResponses, LevelUrls));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentId"></param>
        /// <param name="level"></param>
        /// <param name="url"></param>
        /// <param name="response"></param>
        private void StoreCachePage(RGRateRequest rgRateRequest, int level, string url, string response, List<KeyValuePair<int, string>> CrawlResponses)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(response))
                    CrawlResponses.Add(new KeyValuePair<int, string>(level, response));
            }
            catch (Exception ex)
            {
                Logger.LogInfo(string.Format("Failed to store cache page  for segment {0}. Error:{1}", rgRateRequest.RequestSegmentID, ex.ToString()));
            }
        }

        private void CreateFolderStructure(RGRateRequest rgRateRequest)
        {
            try
            {
                string segmentPath = Path.Combine(BMConfigMaster.AppSetting<string>("CachePagePath"), rgRateRequest.RequestTimeStampUTC.ToString("MMddyyyy", CultureInfo.InvariantCulture), rgRateRequest.RequestSegmentID.ToString());
                if (!Directory.Exists(segmentPath))
                {
                    Directory.CreateDirectory(segmentPath);
                }
                //string urlFile = Path.Combine(segmentPath, "Urls.txt");
                //if (!File.Exists(urlFile))
                //{
                //    lock (new object())
                //    {
                //        File.Create(urlFile);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Logger.LogWarning(string.Format("Failed to create cache page folders Segment({0}) ", rgRateRequest.RequestSegmentID.ToString()) + ex.ToString());
            }
        }



        private Dictionary<string, string> GetHeaderCollection(WebResponse response)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                if (response.Headers.HasKeys())
                {
                    var keys = response.Headers.AllKeys;
                    foreach (var key in keys)
                    {
                        headers.Add(key, response.Headers[key]);
                    }
                }
                return headers;
            }
            catch (System.ObjectDisposedException ex)
            {
                return new Dictionary<string, string>();
            }
        }

        private WebProxy GetWebProxy(RGProxy proxy)
        {
            WebProxy webProxy = new WebProxy();
            webProxy.Address = new Uri("http://" + proxy.IpAddress);
            webProxy.Credentials = new NetworkCredential(proxy.UserName, proxy.Password);
            return webProxy;
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



