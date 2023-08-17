using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace UBlock.Data
{
    /*
    public class DataBase
    {
        private string m_DatabaseName;

#if UNITY_WEBGL
        public DataBase(string databaseName)
        {
            m_DatabaseName = databaseName;
        }

        [DllImport("__Internal")]
        private static extern void DBGetRecord(string databaseName, string storeName, string key);

        public void GetRecord<T>(string tableName, string key, Action<T> callback)
        {
#if UNITY_EDITOR
            callback(default(T));
#else
            NativeMessageListener.instance.ListenMessage_DBGetRecord((string content) =>
            {
                if (string.IsNullOrEmpty(content) && callback != null)
                {
                    callback(default(T));
                    return;
                }

                T result = JsonUtility.FromJson<T>(content);
                if (callback != null)
                    callback(result);
            });

            DBGetRecord(m_DatabaseName, tableName, key);
#endif
        }
#endif
    }
    */
}