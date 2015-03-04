using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dorch.Utilities
{
    static class ApplicationSettingsHelper
    {
        
        public static object ReadResetSettingsValue(string key)
        {
            Debug.WriteLine(key);
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                Debug.WriteLine("FG(read Settings) null returned");
                return null;
            }
            else
            {
                var value = ApplicationData.Current.LocalSettings.Values[key];
                ApplicationData.Current.LocalSettings.Values.Remove(key);
                Debug.WriteLine("FG(read Settings) value found " + value.ToString());
                return value;
            }
        }

        public static void SaveSettingsValue(string key, object value)
        {
            Debug.WriteLine("FG " + key + ":" + value.ToString());
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

    }
}
