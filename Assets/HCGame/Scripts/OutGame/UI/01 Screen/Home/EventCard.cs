using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BonusGame;

public class EventCard : MonoBehaviour
{
    [SerializeField] private Button eventBtn;

    private void Start()
    {
        eventBtn.onClick.AddListener(ShowEvent);
    }

    private void ShowEvent()
    {
        ScreenManagerHC.Instance.ShowBonusGameScreen();
    }
}
