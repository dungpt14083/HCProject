using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDailyMission : MonoBehaviour
{
    [SerializeField] private List<Sprite> spriteBgItem;
    [SerializeField] Image imgBgItemMission;

    [SerializeField] TMP_Text titleItemMission;
    [SerializeField] TMP_Text descriptionItemMission;

    [SerializeField] private List<Sprite> spriteProcess;
    [SerializeField] Image imgProcess;

    [SerializeField] GameObject processMission;
    [SerializeField] GameObject doneMission;

    [SerializeField] Image iconReward;
    [SerializeField] TMP_Text valuesReward;

    [SerializeField] Button btSubmit;

    private Action<long> _actionPressMission;
    private HcDailyMissionProto _data;

    private void Awake()
    {
        btSubmit.onClick.AddListener(Submit);
    }


    public void ShowView(HcDailyMissionProto data, Action<long> callback)
    {
        _data = data;
        _actionPressMission = callback;
        if (_data == null) return;

        titleItemMission.text = _data.Name;
        descriptionItemMission.text = _data.Description + $" ({_data.InProgress}/{_data.ToComplete})";
        doneMission.gameObject.SetActive(_data.IsClaim == 1);
        processMission.gameObject.SetActive(_data.IsClaim == 0);

        iconReward.sprite = ResourceManager.Instance.GetIconRewardMission((RewardType)_data.TypeReward);
        iconReward.SetNativeSize();
        valuesReward.text = StringUtils.FormatMoneyK(_data.Reward);

        if (_data.IsClaim == 0)
        {
            imgProcess.sprite = _data.InProgress == _data.ToComplete ? spriteProcess[1] : spriteProcess[0];
            imgProcess.fillAmount = _data.InProgress / (_data.ToComplete * 1.0f);
            imgBgItemMission.sprite = _data.InProgress == _data.ToComplete ? spriteBgItem[1] : spriteBgItem[0];
        }
        else
        {
            imgBgItemMission.sprite = spriteBgItem[2];
        }
    }

    private void Submit()
    {
        if (_data == null)
        {
            Toast.Show("Not Infomation");
            return;
        }

        if (_data.IsClaim == 1)
        {
            Toast.Show("Bạn đã nhận thưởng rồi.Keke!");
        }
        else if (_data.IsClaim == 0 && _data.InProgress == _data.ToComplete)
        {
            _actionPressMission?.Invoke(_data.Id);
        }
    }
}