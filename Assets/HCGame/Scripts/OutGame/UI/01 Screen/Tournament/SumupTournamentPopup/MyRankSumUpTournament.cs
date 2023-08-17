using System.Collections;
using System.Collections.Generic;
using BonusGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRankSumUpTournament : MonoBehaviour
{
    [SerializeField] Image avatar;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text txtPosition;

    private ResBonusGameRankDTO _resBonusGameRankDTO;

    public void ShowView(ResBonusGameRankDTO data)
    {
        _resBonusGameRankDTO = data;
        avatar.gameObject.SetActive(true);
        avatar.sprite = HCAppController.Instance.myAvatar;
        playerName.text = _resBonusGameRankDTO.username;
        txtPosition.text = _resBonusGameRankDTO.ToString();
    }
}