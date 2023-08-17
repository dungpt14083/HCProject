using Cysharp.Threading.Tasks;
using Google.Protobuf;
using RoyalMatch;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using BestHTTP;
using BestHTTP.WebSocket;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using UnityEngine;
using BestHTTP.WebSocket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BonusGame;
using MiniGame.MatchThree;
using MiniGame.MatchThree.Scripts.Network;
using UnityEngine.SceneManagement;


namespace MiniGame.MatchThree.Scripts.Network
{
    public class MatchThreeNetworkManager : MonoBehaviour
    {
        public Action OnConnectedSocketCallback;
        public Action<string> OnDisconnectedSocketCallback;
        private Action<Response> responseDataCallback;
        public Action<string> OnDebugLog;

        public Action OnConnectedSocket;
        public Action<string> OnDisconnectedSocket;
        public Action<string> OnMessageReceive;
        public Action<byte[]> OnByteDataReceive;

        private string __url;
        public bool isConnected = false;
        WebSocket webSocket;
        public HcGames.CCData _data;
        public long _miniGameEventId;
        public int _modeType;
        public int _numberPlayer;

        public static MatchThreeNetworkManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //Debug.LogError("MULTI INSTANCE");
                Destroy(gameObject);
            }
        }









        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        #region Event

        public void Connect(string url, HcGames.CCData cCData)
        {
            Debug.Log("puzzle network connect to socket");
            Disconnect();
            __url = url;
            //__url = "ws://192.168.2.99:8080";
            //connect new socket
            webSocket = new WebSocket(new Uri(__url));

            this.webSocket.StartPingThread = true;

#if !UNITY_WEBGL || UNITY_EDITOR
            this.webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
            if (HTTPManager.Proxy != null)
                this.webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                    internalRequest.Proxy =
                        new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif
            this.webSocket.PingFrequency = 5000;
            this.webSocket.OnOpen += OnOpen;
            this.webSocket.OnMessage += OnMessageReceived;
            this.webSocket.OnBinary += OnByteDataReceived;
            this.webSocket.OnClosed += OnClosed;
            this.webSocket.OnError += OnError;
            this._data = cCData;
            webSocket.Open();
            Debug.Log("match 3 Connecting url: " + __url);
        }


        public void Disconnect()
        {
            if (webSocket != null)
            {
                Debug.Log("close socket ");
                webSocket.Close();
                webSocket = null;
            }
        }

        [Button]
        public void ForceDisconnect()
        {
            if (webSocket != null)
            {
                Debug.Log("close socket ");
                webSocket.Close();
                webSocket = null;

                MatchThreeGameSystem.Instance.BackHC();
            }
        }


        void OnOpen(WebSocket ws)
        {
            Debug.Log("match 3 _ CONNECTED to game match 3 server: " + __url);
            isConnected = true;
            OnConnectedSocket?.Invoke();
            SendRequestFindRoom(this._data);
        }


        public void SendRequestFindRoom(HcGames.CCData cCData)
        {
            Debug.Log("match 3 _ public void SendRequestFindRoom()");

            HcGames.CCData _ccData = null;

            if (cCData == null)
            {

                _ccData = new HcGames.CCData
                {
                    MiniGameEventId = (ulong)_miniGameEventId,
                    Token = HCAppController.Instance.GetTokenLogin(),
                    WaitingTimeId = 2111,
                    GameMode = this._modeType,
                    NumberInMiniGame = this._numberPlayer
                };
            }
            else
            {
                _ccData = cCData;
            }

            _ccData.WaitingTimeId = 2111;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            //_ccData.MiniGameEventId = (ulong)_miniGameEventId;

            HcGames.FindingRoom action = new HcGames.FindingRoom()
            {
                UserCodeId = HCAppController.Instance.userInfo.UserCodeId,
                Level = HCAppController.Instance.userInfo.Level,
                GameType = 3,
                PlayType = 4,
                Mmr = HCAppController.Instance.GetMmrByGameType(GameType.Puzzle),
                CcData = _ccData,
            };
            PackageData packageData = new PackageData();
            packageData.Header = (uint)PACKAGE_HEADER.FIND_ROOM;
            packageData.Data = action.ToByteString();
            SendBinaryData(packageData.ToByteArray());
        }


        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            var fullMsg = string.Format("WebSocket closed! Code: {0} Message: {1}", code, message);
            Debug.LogError(fullMsg);
            webSocket = null;
            OnDisconnectedSocket?.Invoke(fullMsg);
            _isReconnect = false;
        }

        /// <summary>
        /// Called when an error occured on client side
        /// </summary>
        void OnError(WebSocket ws, string error)
        {
            var fullMsg = string.Format("An error occured: <color=red>{0}</color>", error);
            Debug.LogError(fullMsg);
            webSocket = null;
            OnDisconnectedSocket?.Invoke(fullMsg);
        }

        #endregion

        #region Request

        public void SendRequestStartGame()
        {
            Debug.Log("math 3 Load scene M3_NetworkGamePlay");


            RoyalMatchAction action = new RoyalMatchAction()
            {
                Type = 1
            };
            string requestData = $"type: {1} ";
            OnDebugLog?.Invoke("request Data: " + requestData);
            SendByteString((uint)PACKAGE_HEADER.ROYAL_MATCH_ACTION, action.ToByteString());
        }


        public string _ws;

        public bool _isReconnect = false;


        #region deconnect lai va gửi lại

        /*
        private void SendToServerToNextRound()
        {
        
        
            //ĐÂY LÀ FINDROOOM HEADER:::
            CCData _ccData = new CCData();
            _ccData.MiniGameEventId = (ulong)_tournamentProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.Round = _detailMiniGameProto.Round + 1;
            _ccData.GroupRoomId = _detailMiniGameProto.G;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            HCAppController.Instance.networkControlerHCApp.SendRequestNextRoomRematch(_ccData);
            
            
        }\*/


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

        [Button]
        public void SendRequestFindRoomFromHCGameList(long miniGameEventId, int modeType, int numberPlayer, string ws, HcGames.CCData data)
        {
            //_detailMiniGameProto = data;
            //_isReconnect = isFindNewRoom;
            this._miniGameEventId = miniGameEventId;
            this._modeType = modeType;
            this._numberPlayer = numberPlayer;
            Debug.Log("match 3 _ SendRequestFindRoomFromHCGameList()");
            Debug.Log("math 3 LoadSceneAsync  Game Matching");
            HCAppController.Instance.currentGameType = GameType.Puzzle;
            SceneManager.LoadSceneAsync("Game Matching");
            Connect(ws, data);
        }


        public void SendRequestAction(ROYAL_MATCH_TYPE type, int row = 0, int col = 0)
        {
            Debug.LogWarning("match 3 SendRequestAction() " + type);
            MatchThreeGameSystem.Instance.cs_count2++;
            RoyalMatchAction action = new RoyalMatchAction()
            {
                Type = (int)type,
                Row = row,
                Col = col,
            };

            PackageData packageData = new PackageData();
            packageData.Header = (uint)PACKAGE_HEADER.ROYAL_MATCH_ACTION;
            packageData.Data = action.ToByteString();
            SendBinaryData(packageData.ToByteArray());
        }


        bool isSend = false;

        public void SendRequestClearMassive()
        {


            if (isSend) return;
            isSend = true;
            SendRequestAction(ROYAL_MATCH_TYPE.CLEAR);
            Debug.LogWarning("SEND REQUEST CLEAR");

            StartCoroutine(delayResume());
            IEnumerator delayResume()
            {
                yield return new WaitForSeconds(1.5f);
                isSend = false;

            }
        }





        #endregion

        #region Receive

        void OnSocketConnected()
        {
            Debug.Log(" math 3 __ WebSocket Open URL: " + __url);
            isConnected = true;
            OnConnectedSocketCallback?.Invoke();
        }

        void OnSocketDisconnected(string message)
        {
            Debug.LogError(message);
            isConnected = false;
            OnDisconnectedSocketCallback?.Invoke(message);
        }


        void OnMessageReceived(WebSocket ws, string message)
        {
            Debug.Log("match 3 __ OnMessageReceived()");
        }

        void OnByteDataReceived(WebSocket ws, byte[] responseData)
        {
            PackageData response;
            Debug.LogWarning("match 3 __ OnByteDataReceived()");
            response = PackageData.Parser.ParseFrom(responseData);
            if (response.Header == (uint)PACKAGE_HEADER.FIND_ROOM_RESPONSE) // ==2 parse finding room 
            {
                HcGames.FindingRoomResponse _findingRoom = HcGames.FindingRoomResponse.Parser.ParseFrom(response.Data); //chứa room data có thể dùng hoặc ko
                HCAppController.Instance.findingRoomResponse = _findingRoom;
                if (_findingRoom.RoomId.Length > 0) // có phòng thì vào 
                {
                    Debug.LogWarning("match 3 __ Found room and load M3_NetworkGamePlay");
                    var tmpX = SceneManager.GetActiveScene();
                    SceneManager.UnloadSceneAsync(tmpX);
                    SceneManager.LoadScene("M3_NetworkGamePlay");
                    HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Puzzle);
                }
            }
            else if (response.Header == (uint)PACKAGE_HEADER.ROYAL_MATCH_ACTION) // ==7000 parse action puzzle 
            {
                Response data = Response.Parser.ParseFrom(response.Data);

                responseDataCallback?.Invoke(data);
                Debug.Log("match 3 __  on message received: " + data.Type);


                if (data.Type == 1) // player
                {
                    Debug.Log("match 3 __  on message received: " + data.Type + " DATRA FOR PLAYER ");

                    string dataResponse = string.Empty;
                    switch ((ROYAL_MATCH_STATUS_PLAY)data.DataGame.Status)
                    {
                        case ROYAL_MATCH_STATUS_PLAY.INIT:
                            break;
                        case ROYAL_MATCH_STATUS_PLAY.PLAYING:
                            MatchThreeGameSystem.Instance.OnStartGame(data.DataGame);
                            break;
                        case ROYAL_MATCH_STATUS_PLAY.END:
                            MatchThreeGameSystem.Instance.WinResult();
                            break;
                        case ROYAL_MATCH_STATUS_PLAY.FINAL_ROUND:
                            MatchThreeGameSystem.Instance.LoseResult();
                            break;
                    }
                }
                else if (data.Type == 2) // enemy
                {
                    Debug.Log("match 3 __  on message received: " + data.Type + " DATRA FOR ENEMY ");
                    Match3_TopPanel.Instance.UpdateData(data);
                }
            }
            else if (response.Header == (uint)PACKAGE_HEADER.CANCEL_MATCHING_RESPONSE) // cancel matching
            {
                Debug.Log("======== CANCEL_MATCHING_RESPONSE =======");
                var cancelResponse = StatusCancel.Parser.ParseFrom(response.Data);
                if (cancelResponse.StatusCancel_)
                {
                    ScreenManagerHC.Instance.GoToScreenViewWithFull(() => {
                        ScreenManagerHC.Instance.ShowGameModeUI(GameType.Puzzle);
                    });
                }
            }    
        }

        #endregion

        #region utils

        public void SendByteString(uint actionID)
        {
            PackageData packageData = new PackageData();
            packageData.Header = (uint)actionID;
            SendBinaryData(packageData.ToByteArray());
        }

        public void SendByteString(uint actionID, ByteString data)
        {
            PackageData packageData = new PackageData();
            packageData.Header = (uint)actionID;
            packageData.Data = data;
            SendBinaryData(packageData.ToByteArray());
        }

        #endregion


        public void SendBinaryData(byte[] data)
        {
            if (data != null && webSocket != null)
            {
                Debug.Log("send binary Data : " + Convert.ToBase64String(data));
                webSocket.Send(data);
            }
        }


        public void SendRequestQuitGame()
        {
            Debug.Log("math 3 Send request quit game");
            RoyalMatchAction action = new RoyalMatchAction()
            {
                Type = 2
            };
            SendByteString((uint)PACKAGE_HEADER.ROYAL_MATCH_ACTION, action.ToByteString());
        }
        public void CancelMatching()
        {
            SendCancelMatching();
        }
        private void SendCancelMatching()
        {
            SendByteString((uint)PACKAGE_HEADER.CANCEL_MATCHING_REQUEST, ByteString.Empty);
        }
    }
    
}


public enum PACKAGE_HEADER
{
    // 7000 -> Royal match
    ROYAL_MATCH_ACTION = 7000,
    FIND_ROOM = 1,
    TODO_THANG = 10,
    FIND_ROOM_RESPONSE = 2,
    QuitGame = 6,
    CANCEL_MATCHING_REQUEST = 601,
    CANCEL_MATCHING_RESPONSE = 602,
}

public enum ROYAL_MATCH_TYPE
{
    None = 0,
    PLAY_GAME = 1,
    END_GAME = 2,
    CLEAR = 3,
    RESET = 4,
    SWAP_UP = 5,
    SWAP_DOWN = 6,
    SWAP_LEFT = 7,
    SWAP_RIGHT = 8,
    CLICK = 9,
    START_COUNT_TIME = 10,
}

public enum ROYAL_MATCH_STATUS_PLAY
{
    INIT = 0,
    PLAYING = 1,
    END = 2,
    FINAL_ROUND = 3,
}