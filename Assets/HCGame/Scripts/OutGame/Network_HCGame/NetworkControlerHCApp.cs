using BestHTTP;
using BestHTTP.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System.Linq;
using HcGames;
using ModestTree;
using UnityEngine.SceneManagement;

public class NetworkControlerHCApp
{
    WebSocket _webSocket = null;

    public Action<string> OnReceivedMessage = null;
    public Action OnConnectedCallback = null;

    public Action<bool> OnCreateRoomCallback = null;

    //public Action OnTimeOutCallback = null;
    public Action<bool> OnReceivedEndGameWinnerCallback = null;

    private Action<PackageDataApp> _responseDataCallback = null;

    public void SendMessage(string message)
    {
        Debug.Log("Send message!!! " + message);
        if (null != _webSocket)
        {
            _webSocket.Send(message);
        }
    }

    private void SendBinaryData(byte[] data)
    {
        if (null != data && null != _webSocket)
        {
            _webSocket.Send(data);
        }
    }

    public void SendPing()
    {
        if (_webSocket != null && _webSocket.IsOpen)
        {
            Debug.Log("_webSocket.Sendping");
            _webSocket.Send("ping");
        }
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("Websocket!!! HC_APP open!!!");
        OnConnectedCallback?.Invoke();
        ScreenManagerHC.Instance.ShowFirstWhenConnectSocket();
    }

    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        if (message == "pong")
        {
            Debug.Log("Server HCAPP is healthy.");
        }

        OnReceivedMessage?.Invoke(message);
    }

    private void OnByteDataReceived(WebSocket webSocket, byte[] data)
    {
        try
        {
            PackageDataApp packageData = PackageDataApp.Parser.ParseFrom(data);
            Debug.Log(string.Format($"Raw data received: {data} === {packageData.Type}"));

            _responseDataCallback?.Invoke(packageData);
            _responseDataCallback = null;
            string dataResponse = string.Empty;

            switch ((DataNetworkHCAppType)packageData.Type)
            {
                case DataNetworkHCAppType.UserData:
                    UserDataProto userDataProto = UserDataProto.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.userInfo = userDataProto;
                    HCAppController.Instance.myurlbackround = userDataProto.Background;
                    HCAppController.Instance.myUrlBoder = userDataProto.Frame;
                    var mmr8ball = HCAppController.Instance.GetMmrByGameType(GameType.Billard);
                    ScreenManagerHC.Instance.userDataUI.UpdateData(userDataProto);
                    if (!userDataProto.FirstEditUsername) HcPopupManager.Instance.ShowEditYourProfile();
                    break;

                case DataNetworkHCAppType.ListMiniGame:
                    ListMiniGameProto listMiniGame = ListMiniGameProto.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.listMiniGame = listMiniGame;
                    UISignals.OnUpdateListGameInHome.Dispatch();
                    break;
                case DataNetworkHCAppType.MiniGameEvent:
                    ListMiniGameEventProto listMiniGameEvent =
                        ListMiniGameEventProto.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.ListMiniGameEventProto = listMiniGameEvent;
                    break;


                // case DataNetworkHCAppType.TotalJackPot:
                //     TotalJackpotProto totalJackpot = TotalJackpotProto.Parser.ParseFrom(packageData.Data);
                //     HCAppController.Instance.totalJackpot = totalJackpot.Total;
                //     UISignals.OnUpdateJackPot.Dispatch();
                //     break;

                case DataNetworkHCAppType.TopJackpot:
                    ListTopJackpotProto listTopJackpot = ListTopJackpotProto.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.totalJackpot = listTopJackpot.TotalJackpot;
                    UISignals.OnUpdateJackPot.Dispatch();
                    break;


                // case DataNetworkHCAppType.UserGoldRanking:
                //     ListUserGoldRankingProto listUserGoldRanking =
                //         ListUserGoldRankingProto.Parser.ParseFrom(packageData.Data);
                //     HCAppController.Instance.listUserGoldRanking = listUserGoldRanking;
                //     UISignals.OnUpdateRanking.Dispatch(MoneyType.Gold);
                //     break;
                // case DataNetworkHCAppType.UserTokenRanking:
                //     ListUserTokenRankingProto listUserTokenRanking =
                //         ListUserTokenRankingProto.Parser.ParseFrom(packageData.Data);
                //     HCAppController.Instance.listUserTokenRanking = listUserTokenRanking;
                //     UISignals.OnUpdateRanking.Dispatch(MoneyType.Token);
                //     break;


                case DataNetworkHCAppType.EndRoom:
                    Debug.Log("-------------------->DataNetworkHCAppType.EndRoom");
                    EndRoomWebsocketProto endRoom = EndRoomWebsocketProto.Parser.ParseFrom(packageData.Data);
                    Debug.Log("-------------------->DataNetworkHCAppType.EndRoomWebsocketProto  " + endRoom);

                    HcPopupManager.Instance.ShowGameOverEndRoom(endRoom);
                    //OnTimeOutCallback?.Invoke();
                    break;
                case DataNetworkHCAppType.MatchInfo:
                    Debug.Log("-------------------->DataNetworkHCAppType.MatchInfo");
                    MatchInformation matchInfo = MatchInformation.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.actionMatching?.Invoke(matchInfo);
                    //OnTimeOutCallback?.Invoke(); 
                    break;
                case DataNetworkHCAppType.EndGameUser:
                    Debug.Log("-------------------->DataNetworkHCAppType.EndUser");
                    EndUserWebsocketProto endUser = EndUserWebsocketProto.Parser.ParseFrom(packageData.Data);
                    HcPopupManager.Instance.ShowGameOverEndUser(endUser);
                    break;

                case DataNetworkHCAppType.DetailMiniGameEvent:
                    Debug.Log("-------------------->DataNetworkHCAppType.DetailMiniGameEvent");
                    ListDetailMiniGameProto listDetailMiniGame =
                        ListDetailMiniGameProto.Parser.ParseFrom(packageData.Data);
                    Debug.Log("----------->listDetailMiniGame " + listDetailMiniGame.ListDetailMiniGameProto_.Count);
                    HCAppController.Instance.listDetailMiniGame = listDetailMiniGame;
                    Debug.Log("----------->listDetailMiniGame " +
                              HCAppController.Instance.listDetailMiniGame.ListDetailMiniGameProto_.Count);
                    //OnTimeOutCallback?.Invoke();
                    break;
                // case DataNetworkHCAppType.ActivityHistory:
                //     Debug.Log("-------------------->DataNetworkHCAppType.ActivityHistory");
                //     ListHcPlayHistoryProto listActivityHistory =
                //         ListHcPlayHistoryProto.Parser.ParseFrom(packageData.Data);
                //     Debug.Log("----------->listActivityHistory " + listActivityHistory.ListHcPlayHistoryProto_.Count);
                //     HCAppController.Instance.listActivityHistory = listActivityHistory;
                //     //OnTimeOutCallback?.Invoke();
                //     break;
                // case DataNetworkHCAppType.DailyReward:
                //     Debug.Log("-------------------->DataNetworkHCAppType.DailyReward");
                //     ListHcDailyRewardProto listHcDailyReward =
                //         ListHcDailyRewardProto.Parser.ParseFrom(packageData.Data);
                //     Debug.Log("-------------------->DataNetworkHCAppType.DailyReward " +
                //               listHcDailyReward.ListHcDailyReward.Count);
                //     HCAppController.Instance.listHcDailyReward = listHcDailyReward;
                //     //PopupManager.ins.DisplayDailyReward();
                //     //UIManager.ins.
                //     break;
                case DataNetworkHCAppType.DailyMission:
                    ListHcDailyMission listHcDailyMission = ListHcDailyMission.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.listHcDailyMission = listHcDailyMission;
                    HcPopupManager.Instance.UpdateDailyMission();
                    break;
                case DataNetworkHCAppType.Notification:
                    Debug.Log("-------------------->DataNetworkHCAppType.Notification");
                    ListHcNotificationProto listHcNotification =
                        ListHcNotificationProto.Parser.ParseFrom(packageData.Data);
                    Debug.Log("-------------------->DataNetworkHCAppType.Notification " +
                              listHcNotification.ListHcNotification.Count);
                    foreach (var noti in listHcNotification.ListHcNotification)
                    {
                        Debug.Log($"ListHcNotification NotificationId {noti.NotificationId}");
                    }

                    HCAppController.Instance.listHcNotification =
                        listHcNotification.ListHcNotification.ToDictionary(x => x.NotificationId, y => y);
                    Debug.Log(
                        $"HCAppController.Instance.listHcNotification {HCAppController.Instance.listHcNotification.Count}");
                    break;
                case DataNetworkHCAppType.UpdateNotification:
                    Debug.Log("-------------------->DataNetworkHCAppType.UpdateNotification");
                    ListHcNotificationProto listHcNotificationUpdate =
                        ListHcNotificationProto.Parser.ParseFrom(packageData.Data);
                    Debug.Log("-------------------->DataNetworkHCAppType.UpdateNotification " +
                              listHcNotificationUpdate.ListHcNotification.Count);
                    if (listHcNotificationUpdate != null && listHcNotificationUpdate.ListHcNotification != null)
                    {
                        foreach (var noti in listHcNotificationUpdate.ListHcNotification)
                        {
                            HCAppController.Instance.listHcNotification[noti.Type] = noti;
                        }
                    }

                    HcPopupManager.Instance.UpdateNotification();
                    break;
                case DataNetworkHCAppType.Tournament:
                    TournamentProto tournamentProto = TournamentProto.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.tournamentProto = tournamentProto;
                    break;


                case DataNetworkHCAppType.GET_IMAGE:
                    ListImage ListImage1 = ListImage.Parser.ParseFrom(packageData.Data);
                    HCAppController.Instance.chooseCurrentAvatar = ListImage1.ListAvatar.CurrentChoose;
                    HCAppController.Instance.chooseCurrentBackground = ListImage1.ListBackground.CurrentChoose;
                    HCAppController.Instance._ListFrameAvatar = ListImage1.ListFrameAvatar;
                    HCAppController.Instance.ListAvatar = ListImage1.ListAvatar;
                    HCAppController.Instance.ListBackground = ListImage1.ListBackground;
                    HCAppController.Instance.startLoadSpriteFrameAvatar();
                    HCAppController.Instance.startLoadSpriteCCC();
                    HCAppController.Instance.startLoadSpriteBackground();
                    break;
                case DataNetworkHCAppType.UpdateMoney:
                    UpdateMoney updateMoney = UpdateMoney.Parser.ParseFrom(packageData.Data);
                    if (updateMoney == null)
                    {
                        Debug.LogError("xsssssssssss");
                        return;
                    }

                    ;
                    var currentMoney = new UpdateMoney()
                    {
                        Gold = HCAppController.Instance.userInfo.UserGold,
                        Ticket = HCAppController.Instance.userInfo.UserTicket,
                        Token = HCAppController.Instance.userInfo.UserToken
                    };
                    UpdateCurrencySignals.UpdateMoney.Dispatch(currentMoney, updateMoney);
                    Executors.Instance.StartCoroutine(UpdateMoneyToUserInfo(updateMoney));
                    break;
                case DataNetworkHCAppType.LevelUp:
                    Debug.Log("DataNetworkHCAppType.LevelUp");
                    LevelUp levelUp = LevelUp.Parser.ParseFrom(packageData.Data);
                    Debug.Log("DataNetworkHCAppType.LevelUp " + levelUp);
                    if (HCAppController.Instance.listLevelUp == null)
                        HCAppController.Instance.listLevelUp = new List<LevelUp>();
                    HCAppController.Instance.listLevelUp.Add(levelUp);
                    HCAppController.Instance.listLevelUp =
                        HCAppController.Instance.listLevelUp.OrderByDescending(x => x.Level).ToList();
                    if (SceneManager.GetActiveScene().name == "Home")
                    {
                        HcPopupManager.Instance.ShowLevelup();
                    }

                    break;
                case DataNetworkHCAppType.FREE_GOLD:
                    HcFreeGoldProto hcFreeGold = HcFreeGoldProto.Parser.ParseFrom(packageData.Data);
                    Debug.Log("DataNetworkHCAppType.FREE_GOLD " + hcFreeGold);
                    var timeCountDown = TimeSpan.FromSeconds(hcFreeGold.RewardTime);
                    Debug.Log("timeCountDown " + timeCountDown);
                    HCAppController.Instance.timeGoldGift = timeCountDown;
                    break;
            }

            string receivedData = $"Header: {(DataNetworkHCAppType)packageData.Type}= Data: {dataResponse}";
            Debug.Log("Received data: " + receivedData);
        }
        catch (Exception e)
        {
            Debug.LogError("HC Websocket ERROR: " + e.StackTrace);
            HcPopupManager.Instance.ShowNotifyPopup(e.StackTrace, "HC Websocket ERROR");
        }
    }

    private IEnumerator UpdateMoneyToUserInfo(UpdateMoney updateMoney)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        HCAppController.Instance.userInfo.UserGold = updateMoney.Gold;
        HCAppController.Instance.userInfo.UserTicket = updateMoney.Ticket;
        HCAppController.Instance.userInfo.UserToken = updateMoney.Token;
    }

    private void OnClosed(WebSocket webSocket, UInt16 code, string message)
    {
        string fullMessage = string.Format("WebSocket closed!!! Code: {0} Message: {1}", code, message);
        HcPopupManager.Instance.ShowNotifyPopup(fullMessage, "HC Websocket OnClosed");
        CloseNetwork();
        TryConnectNetwork(5);
    }

    private void OnError(WebSocket websocket, string error)
    {
        string fullMessage = string.Format("WebSocket error!!! {0}", error);
        HcPopupManager.Instance.ShowNotifyPopup(fullMessage, "HC Websocket ERROR");
        CloseNetwork();
        TryConnectNetwork(5);
    }

    private void TryConnectNetwork(int count)
    {
        while (_webSocket == null || !_webSocket.IsOpen)
        {
            if (count == 0)
            {
                if (_webSocket != null && !_webSocket.IsOpen) _webSocket.Open();
                return;
            }

            count -= 1;
        }

        HcPopupManager.Instance.ShowNotifyPopup("HC Websocket Reconnect Fail", "Reconnect");
    }

    private int GetTimestamp()
    {
        return (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public void InitNetwork(string url)
    {
        Debug.Log("NetworkControlerHCApp to socket");
        if (null != _webSocket)
        {
            _webSocket.Close();
            _webSocket = null;
        }

        _webSocket = new WebSocket(new Uri(url));

#if !UNITY_WEBGL || UNITY_EDITOR
        this._webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
        if (HTTPManager.Proxy != null)
            this._webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                internalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif

        _webSocket.OnOpen += OnWebSocketOpen;
        _webSocket.OnMessage += OnMessageReceived;
        _webSocket.OnBinary += OnByteDataReceived;
        _webSocket.OnClosed += OnClosed;
        _webSocket.OnError += OnError;

        _webSocket.Open();
    }

    public void CloseNetwork()
    {
        _webSocket?.Close();
        _webSocket = null;
        Debug.Log("Websocket!!! HCAPP close!!!");
    }

    private async UniTask<ByteString> SendRequestData(int packageHeader, ByteString data)
    {
        UniTaskCompletionSource<ByteString> responseWaiting = new UniTaskCompletionSource<ByteString>();
        _responseDataCallback = p => { responseWaiting.TrySetResult(p.Data); };
        SendByteString((uint)packageHeader, data);
        return await responseWaiting.Task;
    }

    private void SendByteString(uint actionID, ByteString data)
    {
        PackageDataApp packageData = new PackageDataApp();
        packageData.Type = (uint)actionID;
        packageData.Data = data;
        SendBinaryData(packageData.ToByteArray());
    }

    #region API

    //public async UniTask<FindingRoomResponse> FindRoom(int level = 1)
    //{
    //    FindingRoom requestData = new FindingRoom()
    //    {
    //        GameType = GAMETYPE_SOLITAIRE,
    //        Level = level,
    //        UserId = _userId
    //    };
    //    var byteStringResponse = await SendRequestData(NetworkHeader.FINDING_ROOM, requestData.ToByteString());
    //    return FindingRoomResponse.Parser.ParseFrom(byteStringResponse);
    //}

    //public async UniTask<CreateRoomResponse> CreateRoom(Action onUpdateRoom, int level = 1)
    //{
    //    CreateRoom requestData = new CreateRoom()
    //    {
    //        GameType = GAMETYPE_SOLITAIRE,
    //        Level = level,
    //        UserId = _userId
    //    };
    //    _OnUpdateRoomCallback = onUpdateRoom;
    //    var byteStringResponse = await SendRequestData(NetworkHeader.CREATE_ROOM, requestData.ToByteString());
    //    return CreateRoomResponse.Parser.ParseFrom(byteStringResponse);
    //}

    //public void UpdatePoint(int inputPoint)
    //{
    //    UpdatePoint_Soli pointData = new UpdatePoint_Soli()
    //    {
    //        Point = inputPoint,
    //    };

    //    SendRequestData(NetworkHeader.UPDATE_POINT, pointData.ToByteString());
    //}

    //public void CommitPoint(int inputPoint)
    //{
    //    PointSolitaire commitPointData = new PointSolitaire()
    //    {
    //        Point = inputPoint,
    //    };
    //    SendRequestData(NetworkHeader.COMMIT_POINT, commitPointData.ToByteString());
    //}

    //public void LeaveRoom()
    //{
    //}

    #endregion


    #region GOILENNEXTRUOND

    public void SendRequestNextRoomRematch(CCData dataNextRound)
    {
        FindingRoom action = new FindingRoom()
        {
            UserCodeId = HCAppController.Instance.userInfo.UserCodeId,
            CcData = dataNextRound,
        };
        PackageData packageData = new PackageData();
        packageData.Header = (uint)PACKAGE_HEADER.FIND_ROOM;
        packageData.Data = action.ToByteString();
        SendBinaryData(packageData.ToByteArray());
    }

    #endregion
}