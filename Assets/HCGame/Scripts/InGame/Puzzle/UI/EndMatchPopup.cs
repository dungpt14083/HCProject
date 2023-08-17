using MiniGame.MatchThree.Scripts.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMatchPopup : PopupBase
{
    public Button btEndNow;
    public Button btResume;
    public Button btClose;
    public Action endNowCallBack; 
    public Action resumeCallBack; 

    private void Awake()
    {
        btEndNow.onClick.AddListener(EndNow);
        btEndNow.onClick.AddListener(Resume);
        btClose.onClick.AddListener(DisableDisplay);
    }
    public void Show()
    {
        DisplaySelf();
    }
    public void EndNow()
    {
        endNowCallBack?.Invoke();
        MatchThreeNetworkManager.Instance.SendRequestAction(ROYAL_MATCH_TYPE.END_GAME);
        DisableDisplay();
    }
    public void Resume()
    {
        resumeCallBack?.Invoke();
        DisableDisplay();
    }
}
