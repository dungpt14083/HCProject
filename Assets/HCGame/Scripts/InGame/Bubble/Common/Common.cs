using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace BubblesShot
{
    public static class Commons
    {
        // Some c`lass or struct for represent items you want to roulette
        public class ItemRandom
        {
            public int itemId; // not only string, any type of data
            public int chance;  // chance of getting this Item
        }
        public static Random rnd = new Random();

        // Static method for using from anywhere. You can make its overload for accepting not only List, but arrays also: 
        // public static Item SelectItem (Item[] items)...
        public static ItemRandom SelectItemByPercentage(List<ItemRandom> items)
        {
            // Calculate the summa of all portions.
            int poolSize = 0;
            for (int i = 0; i < items.Count; i++)
            {
                poolSize += items[i].chance;
            }

            // Get a random integer from 0 to PoolSize.
            int randomNumber = rnd.Next(0, poolSize) + 1;

            // Detect the item, which corresponds to current random number.
            int accumulatedProbability = 0;
            for (int i = 0; i < items.Count; i++)
            {
                accumulatedProbability += items[i].chance;
                if (randomNumber <= accumulatedProbability)
                    return items[i];
            }
            return null;    // this code will never come while you use this programm right :)
        }
        public static bool IsConnectionNetwork()
        {
            bool isConnected = true;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                isConnected = false;
                Debug.Log("Error. Check internet connection!");
            }
            return isConnected;
        }
        //
        public static void SaveUserData(UserData userData)
        {
            SessionPref.SetUserData(userData);
            //PlayerPrefs.SetString(GameConst.CURRENT_DATA, JsonConvert.SerializeObject(userData));
        }
        public static UserData GetUserData()
        {
            if (SessionPref.GetUserData() == null)
            {
                UserData userData = new UserData()
                {
                    uId = "4",
                    username = "Tiger Games",
                    avatar = 0,
                    frame = 0,
                    isFinishTutorial = false,
                    bgId = 1,
                    level = 3,
                    lifes = 3,
                    tigaCoin = 69696969,
                    timeLoginFirst = DateTime.Now
                };
                SaveUserData(userData);
            }
            return SessionPref.GetUserData();
        }
        public static void SaveDataGamePlay(DataGamePlay dataGamePlay)
        {
            SessionPref.SetDataGamePlay(dataGamePlay);
            //PlayerPrefs.SetString(GameConst.CURRENT_DATA, JsonConvert.SerializeObject(userData));
        }
        //public static int GetTutorialStatus()
        //{
        //    if (!PlayerPrefs.HasKey(GameConst.BUBBLE_TUTORIAL))
        //        PlayerPrefs.SetInt(GameConst.BUBBLE_TUTORIAL, 0);
        //
        //    return PlayerPrefs.GetInt(GameConst.BUBBLE_TUTORIAL);
        //}
        //public static void SetTutorialDone()
        //{
        //    PlayerPrefs.SetInt(GameConst.BUBBLE_TUTORIAL, 1);
        //}
        public static DataGamePlay GetDataGamePlay(bool isResetData = false)
        {
            if (SessionPref.GetDataGamePlay() == null || isResetData)
            {
                DataGamePlay dataGamePlay = new DataGamePlay()
                {
                    itemBomb = 1,
                    itemColorful = 1,
                    //itemRocket = 3,
                    moves = 50,
                    scores = 0
                };
                SaveDataGamePlay(dataGamePlay);
            }
            return SessionPref.GetDataGamePlay();
        }
        public static int GetPositiveNumberInt(int number)
        {
            return number > 0 ? number : 0;
        }
        public static float GetPositiveNumberFloat(int number)
        {
            return number > 0 ? number : 0;

        }

        private static Random rng = new Random();
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static int GetIdRandom(List<int> lst)
        {
            var idx = rng.Next(lst.Count);
            return lst[idx];
        }
        public static int GetIdxRandom(List<string> lst)
        {
            var idx = rng.Next(lst.Count);
            return idx;
        }

        public static int RandomArr(params int[] Values)
        {
            int TotalProb = 0;
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] <= 0) throw new ArgumentException("Each value must positive number and larger than zero.");
                TotalProb += Values[i];
            }
            int Random = new Random().Next(0, TotalProb);
            int Temp = 0;
            for (int i = 0; i < Values.Length; i++)
            {
                if (Random <= (Temp += Values[i])) return i;
            }
            return -1;
        }

        public static void ShowDialog(string title = "", string message = "", Action action = null)
        {
            //Set parameter and call popup
            ScreenInfo screenInfo = new ScreenInfo();
            screenInfo.Add("title", title);
            screenInfo.Add("message", message);
            screenInfo.Add("action", action);

            UIManager.Instance.ShowPopup("Prefabs/GamePlay/Notice/PopUpPanel", screenInfo);
        }
        // Get Android DeviceID
        public static string GetDeviceID()
        {
            // Get Android ID
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");

            string android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");

            // Get bytes of Android ID
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(android_id);

            // Encrypt bytes with md5
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            string device_id = hashString.PadLeft(32, '0');

            return device_id;
        }
        public static string ConvertIntToTimeStr(ConvertTimeType convertTimeType, double totalSeconds)
        {
            string result = String.Empty;
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            switch (convertTimeType)
            {
                case ConvertTimeType.ddHH:
                    result = string.Format("{0:D2}d:{1:D2}h",
                        time.Days,
                        time.Hours);
                    break;
                case ConvertTimeType.HHmmss:
                    result = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                        time.Hours,
                        time.Minutes,
                        time.Seconds);
                    break;
                case ConvertTimeType.mmss:
                    result = string.Format("{0:D2}:{1:D2}",
                        time.Minutes,
                        time.Seconds);
                    break;
            }
            return result;
        }
        public static ArrayList FilterByColor(ArrayList bubbles, BubbleColorType color)
        {
            ArrayList filtered = new ArrayList();
            foreach (Bubble bubble in bubbles)
            {
                if (bubble.colorType == color)
                    filtered.Add(bubble);
            }
            return filtered;
        }
    }
}
