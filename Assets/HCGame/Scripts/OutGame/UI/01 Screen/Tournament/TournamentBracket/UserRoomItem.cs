using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRoomItem : MonoBehaviour
{
    [SerializeField] GameObject titleWin;

    [SerializeField] Image avatar;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text pointRank;

    //Cho các trạng thái của việc người win làm mình và việc xám chờ người chơi bla bla
    [SerializeField] Image bgItem;
    [SerializeField] List<Sprite> listSpriteBgItem;

    private UserTournament _userTournament;
    private string _urlAvatar;

    public void ShowView(UserTournament data, int statusRoom)
    {
        _userTournament = data;
        avatar.gameObject.SetActive(true);
        if (HCAppController.Instance.userInfo.UserCodeId != _userTournament.UserCodeId.ToString() &&
            _userTournament.Avatar != _urlAvatar)
        {
            _urlAvatar = _userTournament.Avatar;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, avatar));
        }
        else avatar.sprite = HCAppController.Instance.myAvatar;

        if (statusRoom == 3)
        {
            pointRank.text = _userTournament.Point.ToString();//StringUtils.FormatMoneyK(_userTournament.Point);
            titleWin.gameObject.SetActive(_userTournament.Result == 2);
            switch (_userTournament.Result)
            {
                case 2:
                    bgItem.sprite = listSpriteBgItem[0];
                    break;
                case 1:
                    bgItem.sprite = listSpriteBgItem[2];
                    break;
                default:
                    break;
            }
        }
        else
        {
            //pointRank.text = "Completed";
            switch (_userTournament.PlayStatus)
            {
                case 1:
                    pointRank.text = "yet to play";
                    break;
                case 2:
                    pointRank.text = "playing now";
                    break;
                default:
                    pointRank.text = "";
                    break;
            }

            bgItem.sprite = listSpriteBgItem[1];
            titleWin.gameObject.SetActive(false);
        }

        bgItem.SetNativeSize();
        /*
        bgItem.sprite = HCAppController.Instance.userInfo.userCodeId != _userTournament.UserCodeId.ToString()
            ? listSpriteBgItem[0]
            : listSpriteBgItem[1];
            */

        playerName.text = _userTournament.Username;
    }

    public void SetDefault()
    {
        titleWin.gameObject.SetActive(false);
        avatar.sprite = null;
        avatar.gameObject.SetActive(false);
        playerName.text = "To be determined";
        pointRank.text = "";
        bgItem.sprite = listSpriteBgItem[1];
        bgItem.SetNativeSize();
        //bgItem.sprite=
    }
}