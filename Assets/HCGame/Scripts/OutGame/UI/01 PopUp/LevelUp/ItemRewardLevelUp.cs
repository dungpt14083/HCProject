using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRewardLevelUp : MonoBehaviour
{
    public Image iconReward;
    public TMP_Text valueReward;
    public TMP_Text nameReward;
    public void Show(Reward reward)
    {
        RewardType rewardType = (RewardType)reward.RewardType;
        var spr = ResourceManager.Instance?.GetIconRewardLevelUp(rewardType);
        if(spr == null )
        {
            iconReward.gameObject.SetActive(false);
            //if (rewardType == RewardType.X2XP)
            //{
            //    nameReward.gameObject.SetActive(true);
            //    nameReward.text = "X2XP";
            //}
        }
        else
        {
            nameReward.gameObject.SetActive(false);
            iconReward.sprite = spr;
        }
        
        valueReward.text = "x"+reward.Reward_.ToString();
    }
}
