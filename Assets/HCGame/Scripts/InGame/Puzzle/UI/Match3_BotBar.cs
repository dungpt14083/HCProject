using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;
using RoyalMatch;
using UnityEngine.UIElements;

namespace MiniGame.MatchThree.Scripts.Network
{
    public class Match3_BotBar : MonoBehaviour
    {
        public static Match3_BotBar Instance;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //Debug.LogError("MULTI INSTANCE");
                Destroy(gameObject);
            }

        }



        public GameObject panelGuide;
        public GameObject endTheMatch, quitUI;
        public GameObject settingGr, posSetting_In, posSetting_Out;
        public float timeMoveSettting;
        public bool isMoving;
        [Button]
        public void OnClickQuit()
        {
            endTheMatch.gameObject.SetActive(true);
        }

        public void OnClickSetting()
        {
            quitUI.gameObject.SetActive(true);
        }

        public void onShowGUide()
        {
            panelGuide.gameObject.SetActive(true);
        }
        public void onHideGUide()
        {
            panelGuide.gameObject.SetActive(false);
        }



        [Button]
        public void OnClickEndNow()
        {
            MatchThreeNetworkManager.Instance.SendRequestQuitGame();
        }




        public void OnClickHideSetting()
        {
            if (isMoving) return;

            isMoving = true;
            settingGr.SetActive(true);
            settingGr.transform.DOMove(posSetting_Out.transform.position, timeMoveSettting).OnComplete(() =>
            {
                isMoving = false;
                settingGr.SetActive(false);

            });
        }




        public void OnClickSuffle()
        {
            NetWorkBoard.instance.SendMessageResetBoard();
        }

        public TextMeshProUGUI _shuffleText;
        public void SetShuffleNum(PuzzleDataJson data)
        {
            _shuffleText.text = "" + data.numberShuffle;
        }

        public TextMeshProUGUI _curRoundText;
        public void SetTextRound2()
        {
            _curRoundText.text = "2/2";
        }




    }
}