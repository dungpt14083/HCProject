using System.Collections;
using System.Collections.Generic;
using BonusGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnGoingEventsMediator : BaseEventBanner
{
    protected override void OpenEventButton()
    {
        base.OpenEventButton();

        switch (_currentPage)
        {
            default:
                btnToLink.onClick.AddListener(LinkToBonusGame);
                break;
        }
    }

    //TOI GAME BONUS
    private void LinkToBonusGame()
    {
        ScreenManagerHC.Instance.ShowBonusGameScreen();
    }
}