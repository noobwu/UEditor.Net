using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;


namespace UEditor.Net
{
    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 
        /// </summary>
        public static string ConfigPath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configPath"></param>
        public static void InitConfig(string configPath)
        {
            if (Path.IsPathRooted(configPath))
            {
                ConfigPath = configPath;
            }
            else
            {
                ConfigPath = HttpContext.Current.Server.MapPath(configPath);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static bool noCache = true;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static JObject BuildItems()
        {
            var json = File.ReadAllText(ConfigPath);
            return JObject.Parse(json);
        }

        /// <summary>
        /// 
        /// </summary>
        public static JObject Items
        {
            get
            {
                if (noCache || _Items == null)
                {
                    _Items = BuildItems();
                }
                return _Items;
            }
        }
        private static JObject _Items;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(string key)
        {
            return Items[key].Value<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String[] GetStringList(string key)
        {
            return Items[key].Select(x => x.Value<String>()).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String GetString(string key)
        {
            return GetValue<String>(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(string key)
        {
            return GetValue<int>(key);
        }
    }
}