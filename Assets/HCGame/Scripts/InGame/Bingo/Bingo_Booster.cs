using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bingo
{
    public class Bingo_Booster : MonoBehaviour
    {



        public static Bingo_Booster instance;

        private void Awake()
        {
            instance = this;
        }

        public GameObject pickerGr;
        public void OnClickBooster()
        {
            pickerGr.gameObject.SetActive(true);
        }

        public void OnClickClosePopup()
        {
            EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
        }


        public void OnClickDoubleScore()
        {
            Bingo_NetworkManager.instance.SendMessage_DoubleScore(0);
            OnClickClosePopup();
        }


        public void OnClickBonusTime()
        {
            Bingo_NetworkManager.instance.SendMessage_BonusTime(1);
            OnClickClosePopup();

        }



    }
}