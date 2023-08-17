using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubblesShot
{
    public class Logger : MonoBehaviour
    {
        public static void Log(string value)
        {
#if DISABLE_LOG
      return;
#endif

            Debug.Log(value);
        }

        public static void LogError(string value)
        {
#if DISABLE_LOG
      return;
#endif

            Debug.LogError(value);
        }

        public static void LogWarning(string value)
        {
#if DISABLE_LOG
      return;
#endif

            Debug.LogWarning(value);
        }

        public static void MyLog(object[] objs, bool shouldBreak = false, GameObject go = null, string color = null)
        {
#if DISABLE_LOG
      return;
#endif

            string s = "";
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != null)
                {
                    s += objs[i].ToString() + " _ ";
                }
                else
                {
                    s += "null" + " _ ";
                }
            }

            if (color != null)
            {
                Debug.Log("<color=" + color + ">" + s + "</color>", go);
            }
            else
            {
                Debug.Log("<color=magenta>" + s + "</color>", go);
            }

            if (shouldBreak)
            {
                Debug.Break();
            }
        }

        public static void MyLogError(object[] objs, bool shouldBreak = false, GameObject go = null, string color = null)
        {
#if DISABLE_LOG
      return;
#endif

#if KFF_RELEASE
      return;
#endif

            string s = "";
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != null)
                {
                    s += objs[i].ToString() + " _ ";
                }
                else
                {
                    s += "null" + " _ ";
                }
            }

            if (color != null)
            {
                Debug.LogError("<color=" + color + ">" + s + "</color>", go);
            }
            else
            {
                Debug.LogError("<color=magenta>" + s + "</color>", go);
            }

            if (shouldBreak)
            {
                Debug.Break();
            }
        }
    }
}
