using System.Collections;
using System.Collections.Generic;
using BonusGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusGameRankItem : MonoBehaviour
{
    [SerializeField] Image avatar;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text valueMoney;

    [SerializeField] Image bgItem;
    [SerializeField] List<Sprite> listSpriteBgItem;
    [SerializeField] GameObject highLight;


    private ResBonusGameRankDTO _resBonusGameRankDTO;
    private string _urlAvatar;
    private int _indexRank = 0;


    //Cái background của item tính sau
    public void ShowView(ResBonusGameRankDTO data, int indexRank)
    {
        ShowDefault();
        _indexRank = indexRank;
        _resBonusGameRankDTO = data;
        avatar.gameObject.SetActive(true);
        if (HCAppController.Instance.userInfo.UserCodeId != _resBonusGameRankDTO.hcUserId.ToString() &&
            _resBonusGameRankDTO.avatar != _urlAvatar)
        {
            _urlAvatar = _resBonusGameRankDTO.avatar;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, avatar));
        }
        else avatar.sprite = HCAppController.Instance.myAvatar;

        playerName.text = _resBonusGameRankDTO.username;
        valueMoney.text = StringUtils.FormatMoneyK(_resBonusGameRankDTO.totalReward);
        if (_indexRank == 1 || _indexRank == 2 || _indexRank == 3)
        {
            if (highLight != null)
            {
                highLight.gameObject.SetActive(HCAppController.Instance.userInfo.UserCodeId ==
                                               _resBonusGameRankDTO.hcUserId.ToString());
            }
        }
        else
        {
            bgItem.sprite = HCAppController.Instance.userInfo.UserCodeId !=
                            _resBonusGameRankDTO.hcUserId.ToString()
                ? listSpriteBgItem[0]
                : listSpriteBgItem[1];
        }
    }

    private void ShowDefault()
    {
        highLight.gameObject.SetActive(false);
    }
}