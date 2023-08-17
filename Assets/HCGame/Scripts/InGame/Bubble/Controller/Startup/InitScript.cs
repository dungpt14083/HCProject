using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using BubblesShot;

namespace BubblesShot
{
    public enum BoostType
    {
        ItemBomb,
        ItemRocket,
        ItemColorful,
        ItemNormal
    }
}

namespace InitScriptName
{
    public class InitScript : MonoBehaviour
    {
        public static InitScript Instance { get; private set; }

        public static bool sound = false;
        public static bool music = false;

        public static int waitedPurchaseGems;

        public static List<string> selectedFriends;
        public static bool Lauched;
        public static int scoresForLeadboardSharing;
        public static int lastPlace;
        public static int savelastPlace;
        public static bool beaten;
        public static List<string> Beatedfriends;
        int messCount;
        public static bool loggedIn;
        //	public GameObject LoginButton;
        //	public GameObject InviteButton;
        public GameObject EMAIL;
        public GameObject MessagesBox;

        public static float RestLifeTimer;
        public static string DateOfExit;
        public static DateTime today;
        public static DateTime DateOfRestLife;
        public static string timeForReps;

        public static bool openNext;
        public static bool openAgain;

        public bool BoostActivated;

        Hashtable mapFriends = new Hashtable();

        UserData _userData;

        public void Awake()
        {
            Instance = this;
            _userData = Commons.GetUserData();


            if (PlayerPrefs.GetInt("Lauched") == 0)
            {
                PlayerPrefs.SetInt("Lauched", 1);
                PlayerPrefs.SetInt("Music", 1);
                PlayerPrefs.SetInt("Sound", 1);
                PlayerPrefs.Save();
            }

            SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");
        }

        void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Application.targetFrameRate = 60;
            }
        }
    }
}