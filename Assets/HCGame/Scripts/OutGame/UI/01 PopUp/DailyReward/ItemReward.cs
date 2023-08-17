using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemReward : MonoBehaviour
{
    public TMP_Text valueReward;
    public Image iconReward;

    public void ShowView(long value, RewardType rewardType)
    {
        valueReward.text = StringUtils.FormatMoneyK(value);
        iconReward.sprite = ResourceManager.Instance.GetIconReward(rewardType);
        iconReward.SetNativeSize();
    }

    public void ShowView(Reward reward)
    {
        valueReward.text = StringUtils.FormatMoneyK(reward.Reward_);
        iconReward.sprite = ResourceManager.Instance.GetIconRewardBiggest((RewardType)reward.RewardType);
        iconReward.SetNativeSize();
    }

    public void ChangeAlphaImage(float value)
    {
        iconReward.ChangeAlphaImageToFloat(value);
    }
}