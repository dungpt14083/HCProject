using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRewardTopTournament : MonoBehaviour
{
    [SerializeField] TMP_Text txtRank;
    [SerializeField] TMP_Text txtValuesReward;
    [SerializeField] Image iconReward;

    [SerializeField] Image bgItem;
    [SerializeField] List<Sprite> listSpritelistSprite;
    [SerializeField] Image imageCup;
    [SerializeField] List<Sprite> listSpriteImageCup;


    //HIỂN THỊ SHOW GIẢI THƯỞNG SẼ NHẬN ĐƯỢC
    public void ShowView(RewardTournament data)
    {
        imageCup.SetNativeSize();
        switch (data.Position)
        {
            case 1:
                bgItem.sprite = listSpritelistSprite[0];
                imageCup.sprite = listSpriteImageCup[0];
                break;
            case 2:
                bgItem.sprite = listSpritelistSprite[1];
                imageCup.sprite = listSpriteImageCup[1];
                break;
            case 3:
                bgItem.sprite = listSpritelistSprite[2];
                imageCup.sprite = listSpriteImageCup[2];
                break;
            //ToDo
            default:
                bgItem.sprite = listSpritelistSprite[3];
                imageCup.gameObject.SetActive(false);
                break;
        }

        bgItem.SetNativeSize();
        txtRank.text = StringUtils.ConvertNumberToStt(data.Position);
        txtValuesReward.text = data.Reward.ToString();
        iconReward.sprite = ResourceManager.Instance.GetIconMoney((MoneyType)data.TypeReward);
    }
}