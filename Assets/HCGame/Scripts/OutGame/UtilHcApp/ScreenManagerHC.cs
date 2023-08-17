using System;
using System.Collections;
using System.Collections.Generic;
using BonusGame;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScreenManagerHC : SingletonMonoAwake<ScreenManagerHC>
{
    #region 00_SCREEN GENERIC UTILYTY

    public List<UIView> views = new List<UIView>();

    [SerializeField] private Transform holder;
    [SerializeField] private GameObject backGroundMainScreen;


    private static ScreenData _ScreenData;

    public static ScreenData ScreenData
    {
        get
        {
            if (!_ScreenData)
            {
                _ScreenData = Resources.Load<ScreenData>("DataUI/ScreenData");
            }

            return _ScreenData;
        }
    }

    public bool IsOnScreen<T>() where T : UIView
    {
        return UIView.CurrentScreen is T;
    }

    public T GetView<T>() where T : UIView
    {
        foreach (var view in Instance.views)
        {
            if (view is T)
            {
                return view as T;
            }
        }

        foreach (var tmp in ScreenData.screenInfos)
        {
            if (tmp.uiView is T)
            {
                var tmpGameObject = BonusPool.Spawn(tmp as T, holder);
                tmpGameObject.gameObject.SetActive(false);
                return tmpGameObject;
            }
        }

        return null;
    }

    public bool isInitDone = false;

    public void CloseAllScreen()
    {
        backGroundMainScreen.gameObject.SetActive(false);
        for (int i = 0; i < views.Count; i++)
        {
            if (views[i] != null)
            {
                if (views[i].gameObject.activeSelf)
                {
                    views[i].CloseView();
                }
            }
        }
        //userDataUI.gameObject.SetActive(false);
        //groupBottomHomeController.gameObject.SetActive(false);
    }

    #endregion


    #region 01_BUTTON NAVIGATION

    public UserDataUI userDataUI;
    public GroupBottomHomecController groupBottomHomeController;


    private void Start()
    {
        Application.targetFrameRate = 60;

        if (userDataUI == null)
        {
            userDataUI = BonusPool.Spawn(ScreenData.userDataUI, transform);
            userDataUI.transform.SetAsLastSibling();
            userDataUI.gameObject.SetActive(true);
        }

        if (this.groupBottomHomeController == null)
        {
            this.groupBottomHomeController = BonusPool.Spawn(ScreenData.groupBottomHomeController, transform);
            this.groupBottomHomeController.transform.SetAsLastSibling();
            this.groupBottomHomeController.gameObject.SetActive(true);
        }

        this.groupBottomHomeController.OnClickNavigationInvoke += ReceivedOnClickNavigation;
        TournamentDataSignals.ReceivedTournamentRankData.AddListener(ShowResultTopPlayer);
        //HcPopupManager.Instance.ShowLevelup();
        isInitDone = true;
    }

    private void OnDestroy()
    {
        TournamentDataSignals.ReceivedTournamentRankData.RemoveListener(ShowResultTopPlayer);
        if (Instance.groupBottomHomeController != null)
        {
            this.groupBottomHomeController.OnClickNavigationInvoke -= ReceivedOnClickNavigation;
        }
    }


    [Button]
    public void ShowLoginScreen()
    {
        WelcomeAndLoginScreen.OpenScreen();
    }


    public void ShowFirstWhenConnectSocket()
    {
        Instance.groupBottomHomeController.OnChangeFilterSelect(TypeButtonHome.Home);
    }

    public void ReceivedOnClickNavigation(TypeButtonHome info)
    {
        switch (info)
        {
            case TypeButtonHome.Home:
                OnClickShowFullHomeScreen();
                break;
            case TypeButtonHome.Event:
                OnClickShowFullEventsScreen();
                break;
            case TypeButtonHome.Ranking:
                OnClickShowFullRankingScreen();
                break;
            case TypeButtonHome.History:
                OnClickShowFullHistoryScreen();
                break;
            default:
                break;
        }
    }

    private void OnClickShowFullHomeScreen()
    {
        GameSignals.CloseAllPopup.Dispatch();
        ShowUserDataAndNavigation(true);
        //CloseAllScreen();
        HomePage.OpenScreen(true);
    }


    public void OnClickShowFullEventsScreen()
    {
        GameSignals.CloseAllPopup.Dispatch();
        ShowUserDataAndNavigation(true);
        //CloseAllScreen();
        EventsPage.OpenScreen();
    }

    public void OnClickShowFullRankingScreen()
    {
        GameSignals.CloseAllPopup.Dispatch();
        ShowUserDataAndNavigation(true);
        //CloseAllScreen();
        RankingPage.OpenViewWithCallBack(page => page.ShowViewFirst());
    }

    public void OnClickShowFullHistoryScreen()
    {
        GameSignals.CloseAllPopup.Dispatch();
        ShowUserDataAndNavigation(true);
        ActivityPage.OpenViewWithCallBack(page => page.ShowViewFirst());
    }

    /// <summary>
    /// IMPORTAIN===DÀNH CHO CÁC SCREEN DẠNG TOURNAMENT EVENT BLA BLA SẼ CÓ VIỆC NÓ SẼ ĐƯỢC DÙNG SHOW Ở MÀN EVENT BLA BLA DỰA VÀO TYPE
    /// CÒN MỞ SCREEN NÀO SẼ MỞ SCENE RA
    /// SAU NÀY CÓ THỂ CUSTOM CHO VIỆC HIỆN HÌNH CỦA NÚT
    /// </summary>
    public void ShowOnlyUserDataUIAndNavigation(TypeButtonHome typeButtonHome)
    {
        Instance.groupBottomHomeController.SelectBottomIcon(typeButtonHome);
        ShowUserDataAndNavigation(true);
    }

    public void ShowUserDataAndNavigation(bool isShow)
    {
        backGroundMainScreen.gameObject.SetActive(isShow);
        userDataUI.gameObject.SetActive(isShow);
        this.groupBottomHomeController.gameObject.SetActive(isShow);
    }

    private void ShowOnlyNavigation(bool isShow)
    {
        backGroundMainScreen.gameObject.SetActive(isShow);
        userDataUI.gameObject.SetActive(false);
        this.groupBottomHomeController.gameObject.SetActive(isShow);
    }

    #endregion


    #region 03-SHOW OTHER SCREEN DIFFERENT MAINS CREEN

    //SHOW LÊN SCENE PROFILE
    public void ShowProfile()
    {
        ShowOnlyNavigation(true);
        ProfileManager.OpenViewWithCallBack(manager => manager.ShowProfile());
    }
    
    public void ShowUpdateProfile()
    {
        ShowOnlyNavigation(true);
        ProfileManager.OpenViewWithCallBack(manager => manager.ShowUpdateProfileFromProfile());
    }

    //SHOW LEN GAME MODE UI SCREEN::::CÁI NÀY NAVIGATION CỦA THẰNG NÀO CURRENT NÊN K CẦN CAN THIỆP
    public void ShowGameModeUI(GameType gameType)
    {
        backGroundMainScreen.gameObject.SetActive(true);
        if (gameType == GameType.None) return;
        var data = HCAppController.Instance.GetListDetailMiniGameProto(gameType);
        GameModeUI.OpenViewWithCallBack(ui => ui.ShowView(gameType, data, true));
    }

    //TƯƠNG TỰ CHO CÁI NÀY CHỈ CẦN HIỆN SCENE
    public void ShowGameModeInfo(DetailMiniGameProto data)
    {
        backGroundMainScreen.gameObject.SetActive(true);
        ModeInfo.OpenViewWithCallBack(info => info.ShowView(data));
    }

    //NÓ LÀ SCREEN MÀ BỊ COI LÀ POPUP NÓ TỪ CURRENT NAVIGATION NÊN KHÔNG UAN TÂM NAVIGATION
    public void ShowDetailHistoryScreen(HcPlayHistoryProto data, bool isShowHistoryInProgress)
    {
        backGroundMainScreen.gameObject.SetActive(true);
        DetailHistoryScreen.OpenViewWithCallBack(popup => popup.ShowView(data, isShowHistoryInProgress));
    }


    #region OPENBONUSGAME

    private BonusGameType _tmpBonusGameType = BonusGameType.Wheel;

    //MỞ BONUSGAME LÊN VÀ HIEEJN THEO EVENT VÌ NÓ TỪ MÀN EVENT VÀO VÀO ĐÂU CÒN PHẢI CHỌN THEO BANNER KHÔNG PHẢI MỞ LÊN LÀ LÊN WHEEL LIỀN
    public void ShowBonusGameScreen(BonusGameType bonusGameType = BonusGameType.Wheel)
    {
        _tmpBonusGameType = bonusGameType;
        BonusgameConnectionManager.Instance.TryConnect(SuccessConnectBonusGame, ErrorConnectBonusGame);
        backGroundMainScreen.gameObject.SetActive(true);
    }

    private void SuccessConnectBonusGame()
    {
        BonusGame_Manager.OpenViewWithCallBack(manager => manager.ShowViewBonusGame(_tmpBonusGameType));
    }

    private void ErrorConnectBonusGame(string error)
    {
        Toast.Show(error);
    }

    #endregion

    #endregion


    #region 04-SCREENTOURNAMENTMANAGER

    #endregion


    #region GOTOHOMEBYERROR

    private static Coroutine _coroutineGoToHome;

    public void GotoHomeByDelay(string error)
    {
        if (_coroutineGoToHome != null)
        {
            StopCoroutine(_coroutineGoToHome);
        }

        _coroutineGoToHome = StartCoroutine(GotoHomeCoroutine());
    }

    private IEnumerator GotoHomeCoroutine()
    {
        yield return new WaitForSeconds(GameConfig.TIME_DELAY_FOR_ERROR);
        HCAppController.Instance.GotoHome();
    }

    #endregion


    #region GOTOEVENTSCREEN

    private Action _callbackLoadScene;

    public void GoToScreenViewWithFull(Action callback)
    {
        _callbackLoadScene = callback;
        HCAppController.Instance.LoadScene("Home", CallBackLoad);
    }


    public void GoToScreenViewOnlyShowButton(Action callback)
    {
        _callbackLoadScene = callback;
        HCAppController.Instance.LoadScene("Home", CallBackLoadOnlyShowButton);
    }

    private void CallBackLoadOnlyShowButton()
    {
        if (HCAppController.Instance.currentDetailMiniGameProto.FromScreen == 1)
        {
            groupBottomHomeController.SelectBottomIcon(TypeButtonHome.Home);
        }
        else
        {
            groupBottomHomeController.SelectBottomIcon(TypeButtonHome.Event);
        }

        _callbackLoadScene?.Invoke();
    }


    private void CallBackLoad()
    {
        if (HCAppController.Instance.currentDetailMiniGameProto != null)
        {
            if (HCAppController.Instance.currentDetailMiniGameProto.FromScreen == 1)
            {
                ShowGameModeUI((GameType)HCAppController.Instance.currentDetailMiniGameProto.MiniGameId);
            }
            else
            {
                groupBottomHomeController.OnChangeFilterSelect(TypeButtonHome.Event);
            }
        }

        _callbackLoadScene?.Invoke();
    }

    #endregion


    #region TournamentScreen

    #region CHECKSTATUSTOURNAMENTANDSHOWTOURNAMENT

    private DetailMiniGameProto _data;
    private Coroutine _sendToServerToShowTournament;
    private Coroutine _sendToCheckTournament;


    public void CheckTournamentStatusAndShow(DetailMiniGameProto data)
    {
        _data = data;
        if (_sendToCheckTournament != null)
        {
            StopCoroutine(_sendToCheckTournament);
        }

        HCAppController.Instance.SetCurrentDetailMiniGame(data);
        var url = GameConfig.API_URL + GameConfig.API_TAIL_CHECKSTATUSTOURNAMENT +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" +
                  $"miniGameEvent={data.MiniGameEventId}";
        _sendToCheckTournament =
            StartCoroutine(APIUtils.RequestWebApiGetJson(url, CalStatusTournament, GetStatusTournamentError));
    }

    private void GetStatusTournamentError(string error)
    {
        ShowTournamentError(error);
    }

    private void CalStatusTournament(JObject data)
    {
        if (data == null) return;
        var tmpStatus = (int)data["status"];
        if (tmpStatus == 1)
        {
            var linkAccount = data.TryGetValue("groupRoomId", out var dataGroupRoomId);
            if (linkAccount)
            {
                ShowTournament((string)dataGroupRoomId);
            }
            else
            {
                EnterInfoTournament(_data);
            }
        }
        else
        {
            EnterInfoTournament(_data);
        }
    }

    [Button]
    public void ShowTournament(string tmpGroupId)
    {
        if (_sendToServerToShowTournament != null)
        {
            StopCoroutine(_sendToServerToShowTournament);
        }

        var url = GameConfig.API_URL + GameConfig.API_TAIL_SHOWTOURNAMENT +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" + $"groupRoomId={tmpGroupId}";
        _sendToServerToShowTournament =
            StartCoroutine(APIUtils.RequestWebApiGetByte(url, ShowTournamentPopup, ShowTournamentError));
    }

    private void ShowTournamentPopup(byte[] data)
    {
        ShowTournamentSameTopPlayerPopup(data);
    }

    private void ShowTournamentSameTopPlayerPopup(byte[] data)
    {
        if (data == null)
        {
            Toast.Show("Not Info");
            return;
        }

        var parser = new MessageParser<TournamentProto>(() => new TournamentProto());
        TournamentProto tournamentProto = parser.ParseFrom(data);
        if (tournamentProto == null) return;
        switch (tournamentProto.TypeMode)
        {
            case 2:
                ShowViewWithTypeDiagram(tournamentProto);
                break;
            case 3:
                ShowViewWithTypeRoundRobin(tournamentProto);
                break;
            default:
                break;
        }
    }


    private void EnterInfoTournament(DetailMiniGameProto data)
    {
        ShowGameModeInfo(data);
    }

    private void ShowTournamentError(string error)
    {
        ShowGameModeInfo(_data);
    }


    public void ShowResultTopPlayer(ListUserTournament data)
    {
        backGroundMainScreen.gameObject.SetActive(true);
        TournamentSumupPopup.OpenViewWithCallBack(popup => popup.InitData(data));
    }

    public void ShowViewWithTypeRoundRobin(TournamentProto data)
    {
        backGroundMainScreen.gameObject.SetActive(true);
        TournamentRoundRobinController.OpenViewWithCallBack(controller => controller.ShowView(data));
    }

    public void ShowViewWithTypeDiagram(TournamentProto data)
    {
        backGroundMainScreen.gameObject.SetActive(true);
        TournamentTypeDiagramController.OpenViewWithCallBack(controller => controller.ShowView(data));
    }


    public void ShowResultOneVsMany(string groupRoomId)
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_TOURNAMENTRANKING +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" +
                  $"groupRoomId={groupRoomId}";
        StartCoroutine(APIUtils.RequestWebApiGetByte(url, GetDataResultOneVsManySuccess, GetDataResultOneVsManyError));
    }

    public void GetDataResultOneVsManySuccess(byte[] data)
    {
        var parser = new MessageParser<ListUserTournament>(() => new ListUserTournament());
        ListUserTournament listUserTournament = parser.ParseFrom(data);
        ShowResultTopPlayer(listUserTournament);
    }

    public void GetDataResultOneVsManyError(string error)
    {
        Toast.Show(error);
    }

    #endregion

    #endregion
}