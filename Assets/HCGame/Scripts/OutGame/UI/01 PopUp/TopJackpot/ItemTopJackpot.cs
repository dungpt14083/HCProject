using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTopJackpot : UIItemRankingBase<ItemTopJackPot>
{
    public override void ShowView(ItemTopJackPot data, int indexRank)
    {
        base.ShowView(data, indexRank);
        avatar.gameObject.SetActive(true);
        if (_data == null) return;
        if (HCAppController.Instance.userInfo.UserCodeId != _data.userId &&
            _data.avatar != _urlAvatar)
        {
            _urlAvatar = _data.avatar;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, avatar));
        }
        else avatar.sprite = HCAppController.Instance.myAvatar;

        playerName.text = _data.username;
        valueMoney.text = StringUtils.FormatMoneyK(_data.reward);
        iconMoney.sprite = ResourceManager.Instance.GetIconMoney(MoneyType.Token);

        if (_indexRank == 1 || _indexRank == 2 || _indexRank == 3)
        {
            if (highLight != null)
            {
                highLight.gameObject.SetActive(HCAppController.Instance.userInfo.UserCodeId ==
                                               _data.userId);
            }
        }
    }
}