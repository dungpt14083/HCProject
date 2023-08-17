using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRewardHistory : MonoBehaviour
{
    public Image iconReward;
    public TMP_Text valueReward;
    public void Show(RewardType rewardType, long reward)
    {
        iconReward.sprite = ResourceManager.Instance.GetIconReward(rewardType);
        valueReward.text = reward.ToString();
    }
}
