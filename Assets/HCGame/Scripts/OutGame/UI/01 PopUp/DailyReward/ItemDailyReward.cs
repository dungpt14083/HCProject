using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemDailyReward : MonoBehaviour
{
    //QUY ĐỊNH VỚI 0 LÀ MÀU XÁM 1 LÀ CHƯA ĐỦ DKIEN NHẬN MÀU XANH DA TRỜI 2 LÀ MÀU XANH SÁNG ĐỦ DDKIEN NHẬN
    [SerializeField] protected List<Sprite> listSpriteBgItem;
    [SerializeField] protected Image imgBgItem;
    [SerializeField] protected Button claim;

    protected Action<int> claimReward;

    protected void ShowReceived()
    {
        Toast.Show("Da Nhan roi");
    }

    protected void ShowNotEnoughReceived()
    {
        Toast.Show("Chua Du Dieu Kien nhan");
    }

    protected virtual void SetDefault()
    {
        claim.onClick.RemoveAllListeners();
    }
}