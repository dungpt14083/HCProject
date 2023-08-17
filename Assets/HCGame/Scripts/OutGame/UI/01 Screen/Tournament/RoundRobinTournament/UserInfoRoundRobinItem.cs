using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoRoundRobinItem : MonoBehaviour
{
    [SerializeField] Image avatar;
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text point;
    [SerializeField] TMP_Text txtResult;

    private UserTournament _userTournament;
    private string _urlAvatar;


    public void ShowView(UserTournament data, int statusRoom)
    {
        _userTournament = data;

        if (HCAppController.Instance.userInfo.UserCodeId != _userTournament.UserCodeId.ToString() &&
            _userTournament.Avatar != _urlAvatar)
        {
            _urlAvatar = _userTournament.Avatar;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, avatar));
        }
        else avatar.sprite = HCAppController.Instance.myAvatar;


        name.text = _userTournament.Username;
        point.text = _userTournament.Point.ToString();
        if (statusRoom == 3)
        {
            switch (_userTournament.Result)
            {
                case 1:
                    txtResult.text = "WIN";
                    txtResult.color = Color.yellow;
                    break;
                case 2:
                    txtResult.text = "LOSE";
                    txtResult.color = Color.white;
                    break;
                case 3:
                    txtResult.text = "--";
                    txtResult.color = Color.white;
                    break;
                default:
                    break;
            }
        }
        else
        {
            txtResult.text = "--";
            txtResult.color = Color.white;
        }
    }

    public void SetDefault()
    {
        avatar.sprite = null;
        avatar.color = Color.blue;
        name.text = "__";
        point.text = "__";
        txtResult.text = "__";
    }
}