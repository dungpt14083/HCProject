using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTutorial8Ball : MonoBehaviour
{
    public Button btTryAgain;
    public Button btPlayNow;

    private void Awake()
    {
        btTryAgain.onClick.AddListener(TryAgain);
        btPlayNow.onClick.AddListener(PlayNow);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void TryAgain()
    {
        EightBallGameSystem.Instance.StartPractice(HCAppController.Instance.GetUrlEightBall(), HCAppController.Instance.userInfo.UserCodeId);
    }
    public void PlayNow()
    {
        ScreenManagerHC.Instance.GoToScreenViewWithFull(() => ScreenManagerHC.Instance.ShowGameModeUI(GameType.Billard));
    }
}
