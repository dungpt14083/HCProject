using System.Collections;
using System.Collections.Generic;
using BonusGame;
using UnityEngine;
using UnityEngine.UI;

public class MsgItem : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI value;
    [SerializeField] private Image image;

    private wheelRewardType _type = wheelRewardType.noReward;

    public wheelRewardType Type
    {
        get { return _type; }
    }

    public void SetValue(WheelItemData data)
    {
        _type = data.rewardType;
        gameObject.SetActive(true);
        value.text = data.value.ToString();
        BonusGame_Manager.Instance.SetImageReward(image, data.rewardType);
    }

    public void SetValueWheelAndScratch(wheelRewardType type, int values)
    {
        _type = type;
        gameObject.SetActive(true);
        value.text = values.ToString();
        BonusGame_Manager.Instance.SetImageReward(image, type);
    }
}