using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentInfoItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtStt;
    [SerializeField] private TMP_Text valueReward;
    [SerializeField] private Image iconMoney;

    [SerializeField] private Image bg;
    [SerializeField] List<Sprite> listBgTop;

    [SerializeField] Image imageCup;
    [SerializeField] List<Sprite> listSpriteImageCup;

    private RewardRanking _dataItem;

    public void ShowView(MoneyType moneyType, RewardRanking dataItem)
    {
        _dataItem = dataItem;
        txtStt.text = _dataItem.RangeTo - _dataItem.RangeFrom > 0
            ? StringUtils.ConvertNumberToStt(_dataItem.RangeFrom) + "-" + StringUtils.ConvertNumberToStt(
                _dataItem.RangeTo)
            : StringUtils.ConvertNumberToStt(_dataItem.RangeFrom);
        var reward = dataItem.Reward.FirstOrDefault(x => x.RewardType == (int)moneyType);
        if (reward != null)
        {
            iconMoney.gameObject.SetActive(true);
            valueReward.gameObject.SetActive(true);
            iconMoney.sprite = ResourceManager.Instance.GetIconReward((RewardType)reward.RewardType);
            valueReward.text = "+" + reward.Reward_.ToString();
        }
        else
        {
            valueReward.gameObject.SetActive(false);
            iconMoney.gameObject.SetActive(false);
        }

        switch (_dataItem.RangeFrom)
        {
            case 1:
                bg.sprite = listBgTop[0];
                imageCup.sprite = listSpriteImageCup[0];
                imageCup.SetNativeSize();

                break;
            case 2:
                bg.sprite = listBgTop[1];
                imageCup.sprite = listSpriteImageCup[1];
                imageCup.SetNativeSize();
                break;
            case 3:
                bg.sprite = listBgTop[2];
                imageCup.sprite = listSpriteImageCup[2];
                imageCup.SetNativeSize();
                break;
            //ToDo
            default:
                bg.sprite = listBgTop[3];
                imageCup.gameObject.SetActive(false);
                break;
        }

        bg.SetNativeSize();
    }
}