using System;
using System.Collections;
using System.Collections.Generic;
using BonusGamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JackPot : UIPopupView<JackPot>
{
    [SerializeField] private List<ItemTopJackpot> itemTopJackpots;
    [SerializeField] private TMP_Text timeRemain;
    [SerializeField] private TMP_Text txtTotalJackpot;

    [SerializeField] private List<UIButtonTab> listButtonTab;

    private TopJackPotData _topJackPotData;


    protected override void Awake()
    {
        base.Awake();
        InitButton();
    }

    private void OnEnable()
    {
        UISignals.OnUpdateJackPot.AddListener(UpdateTotalJackpot);
    }

    private void OnDisable()
    {
        UISignals.OnUpdateJackPot.RemoveListener(UpdateTotalJackpot);
    }


    private void InitButton()
    {
        for (int i = 0; i < listButtonTab.Count; i++)
        {
            listButtonTab[i].OnShow(OnChangeFilterSelect);
        }
    }

    private void UpdateTotalJackpot()
    {
        txtTotalJackpot.text = HCAppController.Instance.totalJackpot.ToString("N0");
    }

    private void OnChangeFilterSelect(TypeButtonTab type)
    {
        switch (type)
        {
            case TypeButtonTab.Tab1:
                ShowViewTopAllTime();
                break;
            case TypeButtonTab.Tab2:
                ShowViewTopWeekly();
                break;
            default:
                break;
        }

        foreach (var item in listButtonTab)
        {
            item.OnSelectFilter(type);
        }
    }

    public void ShowView(TopJackPotData data)
    {
        _topJackPotData = data;
        UpdateTotalJackpot();
        OnChangeFilterSelect(TypeButtonTab.Tab1);
    }

    private void ShowViewTopAllTime()
    {
        timeRemain.gameObject.SetActive(false);
        SetDefaultTime();
        ShowViewTop(_topJackPotData.listAllTime);
    }

    private void ShowViewTopWeekly()
    {
        timeRemain.gameObject.SetActive(true);
        SetDefaultTime();
        ShowTimeRemain();
        ShowViewTop(_topJackPotData.listWeekly);
    }

    private void ShowViewTop(List<ItemTopJackPot> listItemTop)
    {
        SetDefault();
        for (int i = 0; i < listItemTop.Count && i < itemTopJackpots.Count; i++)
        {
            itemTopJackpots[i].ShowView(listItemTop[i], i);
        }
    }

    private void SetDefault()
    {
        for (int i = 0; i < itemTopJackpots.Count; i++)
        {
            itemTopJackpots[i].SetDefault();
        }

        timeRemain.gameObject.SetActive(false);
    }


    #region TIMEREMAIN

    private long _timeCount = 0;
    private Coroutine _coroutineTime;

    private void SetDefaultTime()
    {
        _timeCount = _topJackPotData.timeRemain;
        if (_coroutineTime != null)
        {
            StopCoroutine(_coroutineTime);
        }
    }

    private void ShowTimeRemain()
    {
        _coroutineTime = StartCoroutine(ShowTime());
    }

    private IEnumerator ShowTime()
    {
        var tmpFlag = false;
        while (true)
        {
            _timeCount -= 1;
            if (_timeCount < 0)
            {
                timeRemain.gameObject.SetActive(false);
                if (tmpFlag)
                {
                    UISignals.WeeklyJackPotRefresh.Dispatch();
                }

                tmpFlag = false;
                yield break;
            }
            else
            {
                if (!timeRemain.gameObject.activeSelf)
                {
                    timeRemain.gameObject.SetActive(true);
                }

                tmpFlag = true;
            }

            yield return new WaitForSeconds(1.0f);
            var tmp = TimeSpan.FromSeconds(_timeCount);
            timeRemain.text = $"Reset in : {tmp.Days}d {tmp.Hours:00}h {tmp.Minutes:00}m";
        }
    }

    #endregion
}