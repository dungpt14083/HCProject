using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingItemNormal : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> listBg;

    [SerializeField] private TMP_Text txtRank;
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text valueMoney;
    [SerializeField] private Image iconMoney;
    [SerializeField] private TMP_Text valueReward;
    [SerializeField] private Image iconReward;
    [SerializeField] private GameObject reward;

    private string _urlAvatar;
    private UserRankingProto _data;

    public void ShowView(UserRankingProto userRankingProto, bool isShowTokenRanking)
    {
        SetDefault();
        _data = userRankingProto;
        txtRank.text = _data.Rank == 0 ? "9999" : _data.Rank.ToString();
        if (_data == null) return;
        if (HCAppController.Instance.userInfo.UserCodeId != _data.UserId && _urlAvatar != _data.Avatar)
        {
            _urlAvatar = _data.Avatar;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, avatar));
        }
        else avatar.sprite = HCAppController.Instance.myAvatar;

        playerName.text = _data.HcUserName;
        iconMoney.sprite =
            ResourceManager.Instance.GetIconRewardBonusGameSmall(
                isShowTokenRanking ? RewardType.Token : RewardType.Gold);
        valueMoney.text = StringUtils.FormatMoneyK(_data.Value);

        if (!isShowTokenRanking)
        {
            if (_data.Reward > 0)
            {
                reward.gameObject.SetActive(true);
                valueReward.text = StringUtils.FormatMoneyK(_data.Reward);
                iconReward.sprite = ResourceManager.Instance.GetIconRewardBonusGameSmall((RewardType)_data.TypeReward);
            }
        }

        background.sprite = HCAppController.Instance.userInfo.UserCodeId == _data.UserId ? listBg[0] : listBg[1];
    }

    public void ShowViewMyRank(UserRankingProto userRankingProto, bool isShowTokenRanking)
    {
        SetDefault();
        _data = userRankingProto;
        if (_data == null) return;
        txtRank.text = _data.Rank == 0 ? "9999" : _data.Rank.ToString();
        avatar.sprite = HCAppController.Instance.myAvatar;
        playerName.text = _data.HcUserName;
        iconMoney.sprite =
            ResourceManager.Instance.GetIconRewardBonusGameSmall(
                isShowTokenRanking ? RewardType.Token : RewardType.Gold);
        valueMoney.text = StringUtils.FormatMoneyK(_data.Value);
        if (!isShowTokenRanking)
        {
            if (_data.Reward > 0)
            {
                reward.gameObject.SetActive(true);
                valueReward.text = StringUtils.FormatMoneyK(_data.Reward);
                iconReward.sprite = ResourceManager.Instance.GetIconRewardBonusGameSmall((RewardType)_data.TypeReward);
            }
        }

        background.sprite = listBg[0];
    }

    public void ShowViewMyRankNull(bool isShowTokenRanking)
    {
        SetDefault();
        avatar.sprite = ResourceManager.Instance.GetAvatarDefault();
        txtRank.text = "--";
        playerName.text = HCAppController.Instance.userInfo.UserName;
        valueMoney.text = "--";
        iconMoney.sprite =
            ResourceManager.Instance.GetIconRewardBonusGameSmall(
                isShowTokenRanking ? RewardType.Token : RewardType.Gold);
    }

    private void SetDefault()
    {
        reward.gameObject.SetActive(false);
    }
}