using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingItemTop : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text valueMoney;
    [SerializeField] private Image iconMoney;
    [SerializeField] private TMP_Text valueReward;
    [SerializeField] private Image iconReward;
    [SerializeField] private GameObject gameObjectRewardValue;
    [SerializeField] private GameObject highLight;

    private string _urlAvatar;
    private UserRankingProto _data;

    public void ShowView(UserRankingProto userRankingProto, bool isShowTokenRanking)
    {
        SetDefault();
        _data = userRankingProto;
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
            gameObjectRewardValue.gameObject.SetActive(true);
            valueReward.text = StringUtils.FormatMoneyK(_data.Reward);
            iconReward.sprite = ResourceManager.Instance.GetIconRewardBonusGameSmall((RewardType)_data.TypeReward);
        }

        if (highLight != null)
        {
            highLight.gameObject.SetActive(HCAppController.Instance.userInfo.UserCodeId ==
                                           _data.UserId.ToString());
        }
    }

    public void ShowDefault(bool isShowTokenRanking)
    {
        SetDefault();
        avatar.sprite = ResourceManager.Instance.GetAvatarDefault();
        playerName.text = "--";
        iconMoney.sprite =
            ResourceManager.Instance.GetIconRewardBonusGameSmall(
                isShowTokenRanking ? RewardType.Token : RewardType.Gold);
        valueMoney.text = "--";
    }

    private void SetDefault()
    {
        gameObjectRewardValue.gameObject.SetActive(false);
        highLight.gameObject.SetActive(false);
    }
}