using BestHTTP.WebSocket;
using BubblesShot;
using Google.Protobuf;
using HcGames;
using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum NetworkHeaderBubblesShot
{
    None = 0,
    FindRoomRequest = 1,
    FindRoomResponse = 2,
    GameHeader = 8000,
    ReadyRequest = 8001,
    ReadyResponse = 8002,
    UpdatePointRequest = 8003,
    UpdatePointResponse = 8004,
    EndGameRequest = 8005,
    EndGameResponse = 8006,
    QuitGameRequest = 8007,
    QuitGameResponse = 8008,
    CANCEL_MATCHING_REQUEST = 601,
    CANCEL_MATCHING_RESPONSE = 602,
}
public class BubbesShotManager : Singleton<BubbesShotManager>
{
    public NetworkBubblesShot networkBubblesShot = new NetworkBubblesShot();
    public int currentMode;//1 bất đồng bộ, 2 đồng bộ

    public void StartFindRoom(long miniGameEventId, int modeType, int numberPlayer, string ws, HcGames.CCData _data)
    {
        ConnectToServer(ws, () =>
        {
            HCAppController.Instance.currentGameType = GameType.Bubble;
            Debug.Log("-----------InitNetworkBubblesShot-------> " + miniGameEventId);
            string tokenLogin = HCAppController.Instance.GetTokenLogin();
            Debug.Log("-----------InitNetworkBubblesShot-------tokenLogin> " + tokenLogin);
            string initUrl = HCAppController.Instance.GetUrlBubblesShot();
            HcGames.CCData ccData = null;
            if (_data == null)
            {
                ccData = new CCData
                {
                    MiniGameEventId = (ulong)miniGameEventId,
                    Token = tokenLogin,
                    WaitingTimeId = 2111,
                    GameMode = modeType,
                    NumberInMiniGame = numberPlayer
                };
            }
            else
            {
                ccData = _data;
            }
            Debug.Log("On network controller connected!!!");
            SceneManager.LoadScene("Game Matching");
            FindRoom(ccData);
        });

    }
    private void FindRoom(HcGames.CCData data)
    {
        HcGames.FindingRoom requestData = new HcGames.FindingRoom()
        {
            GameType = (int)GameType.Bubble,
            Level = HCAppController.Instance.userInfo.Level,
            UserCodeId = HCAppController.Instance.userInfo.UserCodeId,
            Mmr = HCAppController.Instance.GetMmrByGameType(GameType.Bubble),
            CcData = data
        };
        SendRequest((uint)NetworkHeaderBubblesShot.FindRoomRequest, requestData.ToByteString());
    }
    public Action<string> OnErrorCallback = null;
    public void ConnectToServer(string url, Action actionOnConnected)
    {
        networkBubblesShot.OnConnectedCallback = actionOnConnected;
        networkBubblesShot.OnReceivedMessage = OnMessageReceived;
        networkBubblesShot.OnReceivedBinary = OnBinaryReceived;
        networkBubblesShot.OnClosedCallback = OnClosed;
        networkBubblesShot.OnErrorCallback = OnError;
        networkBubblesShot.InitNetwork(url);
    }
    
    private void SendRequest(uint actionID, ByteString data)
    {
        PackageData packageData = new PackageData();
        packageData.Header = (uint)actionID;
        if (null != data)
        {
            packageData.Data = data;
        }
        if(actionID == 8003)
        {
            Debug.Log("packageData.ToByteArray() " + packageData.ToByteArray());
        }
        networkBubblesShot?.SendBinaryData(packageData.ToByteArray());
    }
    public void SendReady()
    {
        Debug.Log("Bubble Shot  SendReady");
        SendRequest((uint)NetworkHeaderBubblesShot.ReadyRequest, null);
    }
    public void SendPoint(int point)
    {
        Debug.Log("Bubble Shot  SendPoint");
        UpdatePoints requestData = new UpdatePoints()
        {
            Score = point
        };
        SendRequest((uint)NetworkHeaderBubblesShot.UpdatePointRequest, requestData.ToByteString());
    }
    public void SendEndGame(int gameOverType, int score, int clearBoard, int timeBonus, int finalScore)
    {
        Debug.Log("Bubble Shot  SendEndGame");
        BubbleShooterEndGame requestData = new BubbleShooterEndGame()
        {
            Score = score,
            ClearBoard = clearBoard,
            TimeBonus = timeBonus,
            FinalScores = finalScore,
            GameOverType = gameOverType
        };
        SendRequest((uint)NetworkHeaderBubblesShot.EndGameRequest, requestData.ToByteString());
        Commons.GetDataGamePlay(true);
    }
    public void CancelMatching()
    {
        SendCancelMatching();
    }
    private void SendCancelMatching()
    {
        SendRequest((uint)NetworkHeaderBubblesShot.CANCEL_MATCHING_REQUEST, ByteString.Empty);
    }
    private void OnMessageReceived(string message)
    {
    }
    private void OnBinaryReceived(byte[] data)
    {
        Debug.Log(string.Format($"Raw data received:1111111111111111 {data} === {data.Length}"));
        PackageData packageData = PackageData.Parser.ParseFrom(data);
        Debug.Log(string.Format($"Raw data received: {data} === {packageData.Header}"));

        string dataResponse = string.Empty; 
        switch (packageData.Header)
        {
            case (uint)NetworkHeaderBubblesShot.FindRoomResponse:
                Debug.Log("BubblesShot NetworkHeader.FindRoomResponse");
                //dataResponse = CreateRoomResponse.Parser.ParseFrom(packageData.Data).ToString();
                HcGames.FindingRoomResponse findRoomResponse = HcGames.FindingRoomResponse.Parser.ParseFrom(packageData.Data);
                HCAppController.Instance.findingRoomResponse = findRoomResponse;
                Debug.Log("Bubble Shot findRoomResponse " + findRoomResponse.ToString());
                if (null != findRoomResponse && false == string.IsNullOrEmpty(findRoomResponse.RoomId.Trim()))
                {
                    Debug.Log("Found room!!! " + findRoomResponse.RoomId + " === " + findRoomResponse.MasterName + " == " + findRoomResponse.OtherPlayerName + "==" + findRoomResponse.Mode);
                    this.currentMode = findRoomResponse.Mode;
                    
                    if (findRoomResponse.RoomId.Length > 0)
                    {
                        
                        HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Bubble);
                        Debug.Log("on message BYTE received:  FOUND ROOM NOW SEND MESSAGE START GAME");
                    }
                }
                else
                {
                }
                break;
            case (uint)NetworkHeaderBubblesShot.ReadyResponse:
                Debug.Log("BubblesShot NetworkHeader.ReadyResponse");
                BubblesShot.mainscript.Instance.StartGame();
                break;
            case (uint)NetworkHeaderBubblesShot.UpdatePointResponse:
                UpdatePoints updatePoints = UpdatePoints.Parser.ParseFrom(packageData.Data);
                Debug.Log($"BubblesShot NetworkHeader.ReadyResponse currentMode = {currentMode} updatePoints.Score {updatePoints.Score}");
                if (currentMode == 2 && updatePoints != null)//choi dong bo
                {
                    BubblesShot.InGameUI.Instance.UpdateScorePlayer2(updatePoints.Score);
                }
                break;
            case (uint)NetworkHeaderBubblesShot.CANCEL_MATCHING_RESPONSE:
                Debug.Log("======== CANCEL_MATCHING_RESPONSE =======");
                var cancelResponse = StatusCancel.Parser.ParseFrom(packageData.Data);
                if (cancelResponse.StatusCancel_)
                {
                    ScreenManagerHC.Instance.GoToScreenViewWithFull(() => {
                        ScreenManagerHC.Instance.ShowGameModeUI(GameType.Bubble);
                    });
                }
                break;
        }
    }
    private void OnError(string error)
    {
        string fullMessage = string.Format("WebSocket error!!! {0}", error);
        Debug.LogError(fullMessage);
//#if Test
//        PopupDontDestroy.Instance.ShowNotifyPopup(fullMessage, "HC Websocket OnClosed");
//#endif
        networkBubblesShot?.CloseNetwork();
    }
    private void OnClosed(UInt16 code, string message)
    {
        string fullMessage = string.Format("WebSocket closed!!! Code: {0} Message: {1}", code, message);
        Debug.LogError(fullMessage);
        networkBubblesShot?.CloseNetwork();
    }
}
