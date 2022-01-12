using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace HighConcurrency.Data.Core.Config
{
    public partial class ConfigHelper
    {
        /// <summary>
        /// 获取AppSetting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
