using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDailyRewardDay1To6 : ItemDailyReward
{
    [SerializeField] private ItemReward itemReward;
    [SerializeField] private TMP_Text txtDay;
    [SerializeField] private GameObject nextPointer;
    [SerializeField] private GameObject tickReceived;
    
    private HcDailyRewardProto _data;

    public void ShowView(HcDailyRewardProto data, int countCheck, bool isEligibleToDay, Action<int> action)
    {
        SetDefault();
        claimReward = action;
        _data = data;
        txtDay.text = $"Day {_data.Day}";
        itemReward.ShowView(_data.Reward);
        if (_data == null) return;
        if (_data.Day <= countCheck)
        {
            nextPointer.gameObject.SetActive(true);
            imgBgItem.sprite = listSpriteBgItem[0];
            itemReward.ChangeAlphaImage(0.6f);
            claim.onClick.AddListener(ShowReceived);
            tickReceived.gameObject.Show();
        }
        else if (_data.Day == (countCheck + 1))
        {
            if (isEligibleToDay)
            {
                imgBgItem.sprite = listSpriteBgItem[2];
                claim.onClick.AddListener(() => claimReward?.Invoke(_data.Day));
            }
            else
            {
                imgBgItem.sprite = listSpriteBgItem[1];
                claim.onClick.AddListener(ShowNotEnoughReceived);
            }
        }
        else
        {
            imgBgItem.sprite = listSpriteBgItem[1];
            claim.onClick.AddListener(ShowNotEnoughReceived);
        }
    }

    protected override void SetDefault()
    {
        base.SetDefault();
        itemReward.ChangeAlphaImage(1);
        nextPointer.gameObject.SetActive(false);
        tickReceived.gameObject.Hide();
    }
}