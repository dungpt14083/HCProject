using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using BonusGame;
using System;
using Bingo;
using UnityEngine.Events;


public class UIMainScene : MonoBehaviour
{
    /*
    public static UIMainScene ins;
    public bool isInitDone = false;

    private void Awake()
    {
        if (ins != null)
        {
            Debug.LogError("Multi ins" + gameObject.name);
            Destroy(this);
        }

        ins = this;
    }
    
    private void Start()
    {
        isInitDone = true;
    }
    */
    [Button]
    public void StartTutorial(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Billard:
                EightBallGameSystem.Instance.StartPractice(HCAppController.Instance.GetUrlEightBall(), HCAppController.Instance.userInfo.UserCodeId);
                break; 
            case GameType.Bingo:
               Bingo_NetworkManager.instance.StartPractice(HCAppController.Instance.GetBingoWs(), HCAppController.Instance.userInfo.UserCodeId);
               break;
                
        }
    }
}