using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WxaCode
{
    class LocalSettings
    {
        public static bool IfSaveAppidAndAppsecret()
        {
            return (bool)Get("saveAppidAndAppsecret", false);
        }

        public static bool IfSaveAccessToken()
        {
            return (bool)Get("saveAccesstoken",false);
        }

        public static bool IfSaveWxacodeParam()
        {
            return (bool)Get("saveWxacodeParam",false);
        }

        public static void Set(string key,object value)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static object Get(string key,object defvalue)
        {
            object value = Windows.Storage.ApplicationData.Current.LocalSettings.Values[key];
            if(value==null)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = defvalue;
                value = defvalue;
            }
            return value;
        }
    }
}
