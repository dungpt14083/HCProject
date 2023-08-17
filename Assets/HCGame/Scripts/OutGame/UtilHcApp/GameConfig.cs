using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : SingletonNotAbleNull<GameConfig>
{
    #region TOURNAMENT

    [SerializeField] private bool isUsingLocalServer;

    //Server đặt tại công ty port 8022 hiệp port 8080
    private static string API_URL_LOCAL_TOURNAMENT = "http://192.168.2.68:8022";

    //private static string API_URL_LOCAL_TOURNAMENT = "http://hcoutgamestg.techasians.com";//stg
    private static string API_URL_LOCAL_TOURNAMENT_HIEP = "http://192.168.2.6:8080";
    public static string API_URL = "";

    public static string API_TAIL_TOURNAMENTRANKING = "/api/tournament/rank?";
    public static string API_TAIL_SHOWTOURNAMENT = "/api/tournament?";
    public static string API_TAIL_CLAIMTOURNAMENT = "/api/tournament/claim?";
    public static string API_TAIL_CHECKSTATUSTOURNAMENT = "/api/mini-game-event/user-exist?";
    public static string API_TAIL_GETINFOTOURNAMENT = "/api/tournament/information";


    public static float TIME_DELAY_FOR_ERROR = 2.0f;

    protected override void Awake()
    {
        base.Awake();
        API_URL = isUsingLocalServer ? API_URL_LOCAL_TOURNAMENT : API_URL_LOCAL_TOURNAMENT_HIEP;
    }

    #endregion


    #region CLAIMBONUSGAME

    public static string API_TAIL_CLAIMBONUSGAME = "/api/user/get-user-info/";
    public static string API_TAIL_GETTOPUSERBONUSGAME = "/api/bonus-game/";

    #endregion

    #region LinkMiniGame

    public static Dictionary<LinkRefType, string> LinkGame8Ball = new Dictionary<LinkRefType, string>
    {
        { LinkRefType.Cloud, "" },
        { LinkRefType.TechAsians, "" },
        { LinkRefType.LocalHai, "" },
        { LinkRefType.LocalHiep, "" },
        { LinkRefType.LocalHung, "" },
        { LinkRefType.LocalNghia, "" },
        { LinkRefType.LocalQuang, "" },
    };

    #endregion

    #region LinkAPIGetTocken

    #endregion


    #region GetJackPot

    public static string API_TAIL_GETTOPJACKPOT = "/api/jackpot/total-jackpot";

    #endregion

    #region DailyMission

    public static string API_TAIL_GETREWARDDAILYMISSION = "/api/get-daily-mission";

    #endregion

    #region DailyReward

    public static string API_TAIL_GETDAILYREWARD = "/api/daily-reward?";
    public static string API_TAIL_GETREWARDDAILYREWARD = "/api/get-daily-reward";

    #endregion

    #region HistoryActivity

    public static string API_TAIL_GETHISTORYINPROGRESS = "/api/play-history/inprocess?";
    public static string API_TAIL_GETHISTORYCOMPLETED = "/api/play-history/complete?";

    public static string API_TAIL_CLAIMHISTORY = "/api/claim";
    public static string API_TAIL_CLAIMALLHISTORY = "/api/claim-all";
    public static string API_TAIL_GETREFUND = "/api/refund";
    public static string API_TAIL_GETDETAILMINIGAMEPROTO = "/api/mini-game-event?";

    #endregion

    #region RANKING

    public static string API_TAIL_TOKENRANKING = "/api/rank/token?";
    public static string API_TAIL_GOLDRANKING = "/api/rank/gold?";

    #endregion
}