using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UBlock.Data
{
    public class DataCache
    {
        public const string DefaultKey = "default";
        private static Dictionary<string, Dictionary<string,object>> s_Items = new Dictionary<string, Dictionary<string, object>>();
        
        public static void Set(string category, object value)
        {
            Set(category, DefaultKey, value);
        }

        public static void Set(string category, string key, object value)
        {
            Dictionary<string, object> data;
            if (s_Items.TryGetValue(category, out data))
            {
                data[key] = value;
            }
            else
            {
                data = new Dictionary<string, object>();
                data[key] = value;
                s_Items[category] = data;
            }
        }

        public static T Get<T>(string category)
        {
            return Get<T>(category, DefaultKey);
        }
        public static T Get<T>(string category, string key)
        {
            return Get<T>(category, key, default (T));
        }

        public static T Get<T>(string category, string key, T defaultValue)
        {
            Dictionary<string, object> data;
            if (s_Items.TryGetValue(category, out data))
            {
                return (T) data[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public static void Remove(string category)
        {
            s_Items.Remove(category);
        }

        public static void Clear()
        {
            s_Items.Clear();
        }

        public static void Clear(string category)
        {
            Dictionary<string, object> data;
            if(s_Items.TryGetValue(category,out data))
            {
                data.Clear();
            }
        }
    }
}