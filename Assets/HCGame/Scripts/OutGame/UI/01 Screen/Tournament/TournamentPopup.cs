using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TournamentPopup : UIView<TournamentPopup>
{
    /*
    [SerializeField] TournamentTypeDiagramController tournamentTypeDiagramController;
    [SerializeField] TournamentRoundRobinController tournamentRoundRobinController;

    [SerializeField] TournamentSumupPopup tournamentSumUpPopup;


    protected void Awake()
    {
        TournamentDataSignals.ReceivedTournamentRankData.AddListener(ShowResultTopPlayer);
    }

    protected void OnDestroy()
    {
        TournamentDataSignals.ReceivedTournamentRankData.RemoveListener(ShowResultTopPlayer);
    }

    private void OnEnable()
    {
        tournamentSumUpPopup.gameObject.SetActive(false);
        GameSignals.CloseAllPopup.AddListener(CloseView);
    }

    private void OnDisable()
    {
        GameSignals.CloseAllPopup.RemoveListener(CloseView);
    }


    public void ShowTest()
    {
        this.gameObject.SetActive(true);
    }


    #region SHOWRESULT

    public void ShowResultTopPlayer(ListUserTournament data)
    {
        ShowViewWithType1VsMany(data);
    }

    #endregion


    #region SHOWTOURNAMENT

    public void ShowViewWithType1VsMany(ListUserTournament data)
    {
        tournamentSumUpPopup.gameObject.SetActive(true);
        tournamentTypeDiagramController.gameObject.SetActive(false);
        tournamentRoundRobinController.gameObject.SetActive(false);
        tournamentSumUpPopup.InitData(data);
    }

    #endregion
    */
}