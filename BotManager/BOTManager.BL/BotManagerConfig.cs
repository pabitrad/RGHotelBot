using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RG.Core.Entities;
using RG.Utility;
using System.Runtime.Caching;
using System.Net;

namespace BOTManager.BL
{
    public class BMConfigMaster
    {
        static string MYIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static BotServerConfig GetBotServerConfig(RG.Core.Entities.BOTManager manager)
        {
            var cache = CacheMaster.DefaultCache() as MemoryCache;
            BotServerConfig botServerconfig = new BotServerConfig();
            if (cache.Contains("BOTMANAGER_CONFIG_" + manager.IPAddress))
            {
                botServerconfig = cache["BOTMANAGER_CONFIG_" + manager.IPAddress] as BotServerConfig;
                if (botServerconfig != null)
                {
                    return botServerconfig;
                }
            }
            lock (new Object())
            {
                botServerconfig = (new DataServiceClient()).GetBotManagerConfiguration(manager);
                cache.Set("BOTMANAGER_CONFIG_" + manager.IPAddress, botServerconfig, DateTimeOffset.Now.AddMinutes(2));
            }
            return botServerconfig;
        }

        public static BotServerConfig ServerConfig
        {
            get
            {
                return GetBotServerConfig(new RG.Core.Entities.BOTManager() { IPAddress = MYIP });
            }
        }


        public static T AppSetting<T>(string key)
        {
            return (ConfigExists(key)) ? ServerConfig.PropertyBag.GetValue<T>(key) : BMConfigMaster.AppSetting<T>(key);
        }

        public static bool ConfigExists(string key)
        {
            return ServerConfig.PropertyBag[key] != null;
        }


        public static bool Exists(string key)
        {
            return (ServerConfig.PropertyBag[key] != null || ConfigMaster.Exists(key));
        }
    }
}
