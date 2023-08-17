using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRewardDay7 : MonoBehaviour
{
    public List<Sprite> listSpriteReward;
    public Image iconReward;
    public TMP_Text valueReward;
    public GameObject plus;
    public GameObject isClaim;

    public void ShowView(Reward reward)
    {
        iconReward.ChangeAlphaImageToFloat(1);
        plus.gameObject.SetActive(true);
        isClaim.gameObject.SetActive(false);
        valueReward.text = StringUtils.FormatMoneyK(reward.Reward_);
        switch ((RewardType)reward.RewardType)
        {
            case RewardType.Gold:
                iconReward.sprite = listSpriteReward[0];
                break;
            case RewardType.Token:
                iconReward.sprite = listSpriteReward[1];
                break;
            case RewardType.Ticket:
                iconReward.sprite = listSpriteReward[2];
                break;
            case RewardType.X2XP:
                iconReward.sprite = listSpriteReward[3];
                break;
            default:
                break;
        }

        iconReward.SetNativeSize();
    }

    public void ShowViewNotPlus(Reward reward)
    {
        iconReward.ChangeAlphaImageToFloat(1);
        isClaim.gameObject.SetActive(false);
        plus.gameObject.SetActive(false);
        valueReward.text = StringUtils.FormatMoneyK(reward.Reward_);
        switch ((RewardType)reward.RewardType)
        {
            case RewardType.Gold:
                iconReward.sprite = listSpriteReward[0];
                break;
            case RewardType.Token:
                iconReward.sprite = listSpriteReward[1];
                break;
            case RewardType.Ticket:
                iconReward.sprite = listSpriteReward[2];
                break;
            default:
                break;
        }

        iconReward.SetNativeSize();
    }

    public void ChangeAlphaImage(float value)
    {
        isClaim.gameObject.SetActive(true);
        iconReward.ChangeAlphaImageToFloat(value);
    }
}