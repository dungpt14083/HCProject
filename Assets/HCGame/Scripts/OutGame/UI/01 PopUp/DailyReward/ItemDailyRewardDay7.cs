using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDailyRewardDay7 : ItemDailyReward
{
    [SerializeField] private ItemRewardDay7 prefabItemReward;
    [SerializeField] private Transform holder;

    private Date7 _data;

    public void ShowView(Date7 data, int countCheck, bool isEligibleToDay, Action<int> action)
    {
        SetDefault();
        claimReward = action;
        _data = data;
        if (_data == null) return;
        if (_data.Day <= countCheck)
        {
            imgBgItem.sprite = listSpriteBgItem[0];
            claim.onClick.AddListener(ShowReceived);
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

        for (int i = 0; i < data.Reward.Count; i++)
        {
            var tmp = BonusPool.Spawn(prefabItemReward, holder);
            if (i < data.Reward.Count - 1)
            {
                tmp.ShowView(data.Reward[i]);
            }
            else
            {
                tmp.ShowViewNotPlus(data.Reward[i]);
            }

            if (_data.Day <= countCheck)
            {
                tmp.ChangeAlphaImage(0.6f);
            }
        }
    }

    protected override void SetDefault()
    {
        base.SetDefault();
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }
}