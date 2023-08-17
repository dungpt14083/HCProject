using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGoingHomeMediator : BaseEventBanner
{
    protected override void OpenEventButton()
    {
        base.OpenEventButton();

        switch (_currentPage)
        {
            default:
                btnToLink.onClick.AddListener(ShowHome);
                break;
        }
    }

    //TOI GAME BONUS
    private void ShowHome()
    {
        Toast.Show("No events going on, please come back later!");
    }
}