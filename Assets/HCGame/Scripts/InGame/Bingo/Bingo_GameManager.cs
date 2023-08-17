using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using AssemblyCSharp.GameNetwork;
using TMPro;
using UnityEngine.UI;

namespace Bingo
{
    public class Bingo_GameManager : MonoBehaviour
    {

        public static Bingo_GameManager instance;
        public GameState currentGameState;
        public string startGameMessage;
        public MatchInformation MatchInformationData;
        public TMP_Text userName1;
        public Image userAvatar1;
        public TMP_Text userName2;
        public Image userAvatar2;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }
        
        [Button]
        public void SetCurrentGameState(GameState state, Action callBack = null)
        {
            gameObject.SetActive(true);
            switch (state)
            {
                case GameState.waitUserStartGame:
                    //reset
                    break;
                case GameState.startGame:

                    BingoGame_AnimationEffect.instance.PlayStartGameEff(() =>
                    {
                        //Bingo_NetworkManager.instance.SendMessageString(startGameMessage);
                        callBack?.Invoke();
                    });

                    break;
                case GameState.playing:
                    break;
                case GameState.displayResult:
                    //BingoGame_AnimationEffect.instance.PlayEndGameEff();
                    break;
            }
            currentGameState = state;
        }


        private void Start()
        {
            currentGameState = GameState.waitUserStartGame;
            HcPopupManager.Instance.ShowEightGameLoading(false);
            HCAppController.Instance.LoadUserInfoInGame(HCAppController.Instance.findingRoomResponse, userName1, userAvatar1, userName2, userAvatar2);
            if (Bingo_NetworkManager.instance.currentMode == (int)PlayMode.Practice)
            {
                Bingo_Tutorial.Instance.StartTutorial();
            }
            if (Bingo_NetworkManager.instance.currentMode == 2)//mode đồng bộ
            {
                Bingo_TopPanelGameInfo.instance.ShowPlayer2();
            }
            SetCurrentGameState(GameState.startGame, () => { 
                Bingo_NetworkManager.instance.SendRequestStartGame();
            });
        }


        public void OnClickStartGame()
        {
            SetCurrentGameState(GameState.startGame);
        }


        public string bingoMessage;
        public void OnClickBingo()
        {
            Bingo_NetworkManager.instance.SendMessageBingo();
        }



        public void DelayedCall(float time, Action callback)
        {
            StartCoroutine(_DelayCall());
            IEnumerator _DelayCall()
            {
                yield return new WaitForSeconds(time);
                callback.Invoke();
            }
        }

        public void OnDisable()
        {
            gameObject.SetActive(true);
        }
    }
}


public enum GameState
{
    waitUserStartGame,
    startGame,
    playing,
    displayResult,


}



