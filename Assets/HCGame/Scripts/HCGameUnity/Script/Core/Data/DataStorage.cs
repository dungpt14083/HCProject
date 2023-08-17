using UnityEngine;

namespace NBCore
{
    public static class DataStorage
    {
        public static void Save(string key, object obj)
        {
            string json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(key,json);
        }

        public static void Save(object obj)
        {
            var type = obj.GetType();
            Save(type.FullName, obj);
        }

        public static void LoadOverwrite(object obj)
        {
            string result = PlayerPrefs.GetString(obj.GetType().FullName, string.Empty);
            if (string.IsNullOrEmpty(result))
                return;
            else
            {
                JsonUtility.FromJsonOverwrite(result, obj);
            }
        }

        public static T Load<T>()
        {
            var fullName = typeof (T).FullName;
            return Load<T>(fullName);
        }

        public static T Load<T>(string key)
        {
            string result = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                return default(T);
            }
            else
            {
                return JsonUtility.FromJson<T>(result);
            }
        }
    }
}