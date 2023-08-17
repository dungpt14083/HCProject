using System;
using System.Collections;
using System.Collections.Generic;
using BonusGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentSumupItem : MonoBehaviour
{
    [SerializeField] Image avatar;
    [SerializeField] TMP_Text playerName;

    [SerializeField] Image bgItem;
    [SerializeField] List<Sprite> listSpriteBgItem;
    [SerializeField] GameObject highLight;


    private TournamentRankingProto _tournamentRankingProto;
    private int _indexRank;

    private string _urlAvatar;

    #region SHOWNORMAL

    [SerializeField] GameObject ticket;
    [SerializeField] GameObject token;
    [SerializeField] GameObject gold;

    [SerializeField] TMP_Text ticketReward;
    [SerializeField] TMP_Text tokenReward;
    [SerializeField] TMP_Text goldReward;

    public void ShowViewNormal(TournamentRankingProto data, int indexRank)
    {
        _indexRank = indexRank;
        _tournamentRankingProto = data;
        point.gameObject.SetActive(false);
        ShowGeneral(data);

        ticketReward.text = _tournamentRankingProto.TicketReward.ToString();
        ticket.SetActive(_tournamentRankingProto.TicketReward > 0);

        tokenReward.text = _tournamentRankingProto.TokenReward.ToString();
        token.SetActive(_tournamentRankingProto.TokenReward > 0);

        goldReward.text = _tournamentRankingProto.GoldReward.ToString();
        gold.SetActive(_tournamentRankingProto.GoldReward > 0);
    }

    #endregion


    #region SHOWWITH1VSMANYTYPEITEM

    [SerializeField] GameObject point;
    [SerializeField] TMP_Text pointRank;

    public void ShowView1VsMany(TournamentRankingProto data, int indexRank)
    {
        _indexRank = indexRank;
        _tournamentRankingProto = data;
        point.gameObject.SetActive(false);
        //SẼ HIỂN THIJ ITEM TRỐNG DẠNG SEARCHING PPONENT VÀ THÔNG AVATAR BLA BLA VÀ CHỈ HIỂN MỖI CÁI RANK TRỐNG CHỜ ĐIỂM
        if (data == null)
        {
            ticket.SetActive(false);
            token.SetActive(false);
            gold.SetActive(false);
            pointRank.text = "__";
            ShowDefault();
        }
        //CÒN THÔNG THÌ SẼ FULL CHO VIỆC HIỂN THỊ DATA CỦA NÓ GỒM CẢ POINT BLA BLA 
        else
        {
            ShowGeneral(_tournamentRankingProto);

            ticketReward.text = _tournamentRankingProto.TicketReward.ToString();
            ticket.SetActive(_tournamentRankingProto.TicketReward > 0);

            tokenReward.text = _tournamentRankingProto.TokenReward.ToString();
            token.SetActive(_tournamentRankingProto.TokenReward > 0);

            goldReward.text = _tournamentRankingProto.GoldReward.ToString();
            gold.SetActive(_tournamentRankingProto.GoldReward > 0);

            pointRank.text = _tournamentRankingProto.Rating.ToString();
        }
    }

    #endregion

    #region GENERAL

    public void ShowView(TournamentRankingProto data, bool is1VsMany, int indexRank = 999)
    {
        if (is1VsMany)
        {
            ShowView1VsMany(data, indexRank);
        }
        else
        {
            ShowViewNormal(data, indexRank);
        }
    }


    private void ShowGeneral(TournamentRankingProto data)
    {
        if (highLight != null)
        {
            highLight.gameObject.SetActive(false);
        }

        _tournamentRankingProto = data;
        avatar.gameObject.SetActive(true);
        if (HCAppController.Instance.userInfo.UserCodeId != _tournamentRankingProto.UserCodeId.ToString() &&
            _tournamentRankingProto.Avatar != _urlAvatar)
        {
            _urlAvatar = _tournamentRankingProto.Avatar;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, avatar));
        }
        else avatar.sprite = HCAppController.Instance.myAvatar;

        if (_indexRank == 1 || _indexRank == 2 || _indexRank == 3)
        {
            if (highLight != null)
            {
                highLight.gameObject.SetActive(HCAppController.Instance.userInfo.UserCodeId ==
                                               _tournamentRankingProto.UserCodeId.ToString());
            }
        }
        else
        {
            if (listSpriteBgItem.Count > 1)
            {
                bgItem.sprite = HCAppController.Instance.userInfo.UserCodeId !=
                                _tournamentRankingProto.UserCodeId.ToString()
                    ? listSpriteBgItem[0]
                    : listSpriteBgItem[1];
            }
        }

        playerName.text = _tournamentRankingProto.Username;
    }

    private void ShowDefault()
    {
        if (highLight != null)
        {
            highLight.gameObject.SetActive(false);
        }
        avatar.gameObject.SetActive(true);
        avatar.sprite = null;
        avatar.color = Color.blue;
        // if (listSpriteBgItem.Count > 0)
        // {
        //     bgItem.sprite = listSpriteBgItem[0];
        // }
        playerName.text = "Searching Opponent";
    }

    #endregion
}