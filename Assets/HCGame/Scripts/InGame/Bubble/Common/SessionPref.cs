using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubblesShot
{
    public static class SessionPref
    {
        private static UserData _userData;
        private static DataGamePlay _dataGamePlay;
        private static bool _isTutorial = false;
        public static UserData GetUserData()
        {
            return _userData;
        }

        public static void SetUserData(UserData userData)
        {
            _userData = userData;
        }
        public static DataGamePlay GetDataGamePlay()
        {
            return _dataGamePlay;
        }
        public static void SetDataGamePlay(DataGamePlay dataGamePlay)
        {
            _dataGamePlay = dataGamePlay;
        }
        public static bool GetTutorial()
        {
            return _isTutorial;
        }
        public static void SetTutorial(bool isTutorial)
        {
            _isTutorial = isTutorial;
        }
        public static void ResetSessionPref()
        {
            _isTutorial = false;
            _userData = null;
            _dataGamePlay = null;
        }
    }

}