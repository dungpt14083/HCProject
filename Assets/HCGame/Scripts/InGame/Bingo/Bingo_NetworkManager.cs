using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using AssemblyCSharp.GameNetwork;
using Google.Protobuf;
using Scratch;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using HcGames;
using HcGames.Bingo;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PackageData = HcGames.PackageData;

namespace Bingo
{
    public class Bingo_NetworkManager : MonoBehaviour
    {
        #region singleton
        public static Bingo_NetworkManager instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }

        #endregion

        #region webSocket Intial
        Action OnConnectedSocket; //all function registered on editor
        CustomEventString OnDisconnectedSocket; //all function registered on editor
        CustomEventString OnMessageReceive; //all function registered on editor
        Action<byte[]> OnByteDataReceive; //all function registered on editor

        WebSocket webSocket;
        public bool isConnected;
        public string url;
        public HcGames.CCData cData;
        public int currentMode;//1 bất đồng bộ, 2 đồng bộ
      
        #endregion

        #region webSocket Function

        [Button]
        public void Connect(HcGames.CCData _cCData,Action action =null)
        {
            Debug.Log("Bingo_NetworkManager to socket");
            Disconnect();
            //connect new socket
            _ws = HCAppController.Instance.GetBingoWs();
            webSocket = new WebSocket(new Uri(_ws));

            webSocket.StartPingThread = true;

#if !UNITY_WEBGL || UNITY_EDITOR
            this.webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
            if (HTTPManager.Proxy != null)
                this.webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                    internalRequest.Proxy =
                        new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif
            OnConnectedSocket = action;
            this.webSocket.OnOpen += OnOpen;
            this.webSocket.OnMessage += OnMessageReceived;
            this.webSocket.OnBinary += OnByteDataReceived;
            this.webSocket.OnClosed += OnClosed;
            this.webSocket.OnError += OnError;
            this.cData = _cCData;
            webSocket.Open();
            Debug.Log("Connecting to bingo game server: " +_ws);
        }
        public void Disconnect()
        {
            if (webSocket != null)
            {
                webSocket.Close();
                webSocket = null;
            }
        }


        [Button]
        public void SendRequestFindingGame(UserDataProto userInfo, int mmr, string token, HcGames.CCData cCData = null)
        {
            HcGames.CCData _ccData = null;
            if(cCData == null)
            {
                _ccData = new HcGames.CCData();
                _ccData.WaitingTimeId = 2111;
                _ccData.Token = token;
                _ccData.MiniGameEventId = (ulong)_miniGameEventId;
                _ccData.GameMode = _modeType;
                _ccData.NumberInMiniGame = _numberPlayer;
            }
            else
            {
                _ccData = cCData;
            }

            HcGames.FindingRoom action = new HcGames.FindingRoom()
            {
                UserCodeId = userInfo.UserCodeId,
                Level = userInfo.Level,
                GameType = 2,
                PlayType = 4,
                Mmr = mmr,
                CcData = _ccData,
            };

            PackageData packageData = new PackageData();
            packageData.Header = 1;
            packageData.Data = action.ToByteString();

            Debug.LogWarning("WEBSOCKET" + webSocket);
            Debug.LogWarning("packageData" + packageData.ToByteArray());
            SendPackage(packageData.ToByteArray());
            Debug.Log("Bingo__ send request find game with mmr: " + mmr + " /token: " + token);

        }





        public bool gameStarted;
        public long _miniGameEventId;
        public int _modeType;
        public int _numberPlayer;
        public int _mmr;
        public string _ws;
        [Button]
        public void SendRequestFindRoomFromHCGameList(long miniGameEventId, int modeType, int numberPlayer, string ws, HcGames.CCData cCData)
        {
            Debug.Log("-----------Init_BINGO_GAME_-------> ");

            _miniGameEventId = miniGameEventId;
            _modeType = modeType;
            _numberPlayer = numberPlayer;
            _ws = ws;

            HCAppController.Instance.currentGameType = GameType.Bingo;
            SceneManager.LoadScene("Game Matching");
            Connect(cCData,(() => OnConnectedBingoServer(cCData)));// use _ws 
        }


        public void OnConnectedBingoServer(HcGames.CCData cCData)
        {
            Debug.Log("bingo _ OnConnectedBingoServer()");
            gameStarted = false;
            var userInfo = HCAppController.Instance.userInfo;
            var mmrBingo = HCAppController.Instance.GetMmrByGameType(GameType.Bingo);
            var token = HCAppController.Instance.GetTokenLogin();


            SendRequestFindingGame(userInfo, mmrBingo, token, cCData);

            StartCoroutine(CheckTimeOut());
            IEnumerator CheckTimeOut()
            {
                var timeout = 40;
                yield return new WaitForSeconds(timeout);
                if (gameStarted == false)
                    Debug.LogError("start game bingo fail");
            }
        }


        public void SendRequestStartGame()
        {
            Debug.Log("SEND REQUEST START GAME");
            //SceneManager.LoadScene("Game Bingo");
            
            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 1;
            _packageData.Data = _request.ToByteString();
            SendPackage(_packageData.ToByteArray());
        }




        public void TESTSendRequestFindingGameCustom1()
        {

            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.Token = "create_token_6a5fcf9f3cdf546c2636e821cd5d4d10eb06d3192862aeb8d10df8e777b78b9e";
            _ccData.MiniGameEventId = 62;
            HcGames.FindingRoom action = new HcGames.FindingRoom()
            {
                UserCodeId = "02e668e7a325b5cbe6ec629cdf974c8f227f0e13_tson2_1675159292233",
                Level = 1,
                GameType = 2,
                PlayType = 4,
                Mmr = 0,
                CcData = _ccData,
            };

            PackageData packageData = new PackageData();
            packageData.Header = 1;
            packageData.Data = action.ToByteString();
            SendPackage(packageData.ToByteArray());
        }

        public void TESTSendRequestFindingGameCustom2()
        {

            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.Token = "create_token_6a5fcf9f3cdf546c2636e821cd5d4d10eb06d3192862aeb8d10df8e777b78b9e";
            _ccData.MiniGameEventId = 62;
            HcGames.FindingRoom action = new HcGames.FindingRoom()
            {
                UserCodeId = "c19d3bbdf7a3d3ae10ded9d6246cda247eb62f5a_1111_1675190424754",
                Level = 2,
                GameType = 2,
                PlayType = 4,
                Mmr = 0,
                CcData = _ccData,
            };

            PackageData packageData = new PackageData();
            packageData.Header = 1;
            packageData.Data = action.ToByteString();
            SendPackage(packageData.ToByteArray());
        }




        public void SendBinaryData(byte[] data)
        {
            if (data != null && webSocket != null)
            {
                //
                SendPackage(data);
            }
        }
        #endregion

        #region WebSocket Event Handlers

        /// <summary>
        /// Called when the web socket is open, and we are ready to send and receive data
        /// </summary>
        void OnOpen(WebSocket ws)
        {
            isConnected = true;
            Debug.Log("Connected!!");
            OnConnectedSocket?.Invoke();
           // OnConnectedBingoServer(this.cData);
        }

        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        void OnMessageReceived(WebSocket ws, string message)
        {
            Debug.Log("on message STRING received" + message);
            //UpdateGame(message);
        }

        /// <summary>
        /// Called when we received a byte data message from the server
        /// </summary>
        /// 

        public BingoGameDataPlayer2 _bingoData2;
        public int doubs, bingos, multiBingos, doubleScore, penaties;
        void OnByteDataReceived(WebSocket ws, byte[] responseData)
        {
            Debug.Log("on message BYTE received");


            PackageData _responseData = PackageData.Parser.ParseFrom(responseData);
            if (_responseData.Header == 2)//start game
            {
                gameStarted = true;
                HcGames.FindingRoomResponse _findingRoom = HcGames.FindingRoomResponse.Parser.ParseFrom(_responseData.Data);//chứa room data có thể dùng hoặc ko
                HCAppController.Instance.findingRoomResponse = _findingRoom;
                this.currentMode = _findingRoom.Mode;
                if (_findingRoom.RoomId.Length > 0)
                {
                    HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Bingo);
                }
            }
            else if (_responseData.Header == 999) //data bingo game
            {
                var _parsedResponse = _responseData.Data.ToStringUtf8();
                var bingoData = JsonUtility.FromJson<BingoGameData>(_parsedResponse);
                if (bingoData.choseTarget == null)//data for player 2
                {

                    JObject jobj = JObject.Parse(_parsedResponse);
                    var score = Int32.Parse((jobj["scores"]["player2_score"]).ToString());

                    //_bingoData2.scores.player2_score = score;

                    _bingoData2 = JsonUtility.FromJson<BingoGameDataPlayer2>(_parsedResponse);
                    Debug.Log("UpdateGamePlayer_2 " + (_bingoData2));
                    UpdateGamePlayer_2(_bingoData2);
                }
                else
                {
                    JObject jobj = JObject.Parse(_parsedResponse);
                    doubs = Int32.Parse((jobj["scoreDetail"]["Doubs"]["point"]).ToString());
                    bingos = Int32.Parse((jobj["scoreDetail"]["Bingos"]["point"]).ToString());
                    multiBingos = Int32.Parse((jobj["scoreDetail"]["Multi Bingos"]["point"]).ToString());
                    doubleScore = Int32.Parse((jobj["scoreDetail"]["Double Score"]["point"]).ToString());
                    penaties = Int32.Parse((jobj["scoreDetail"]["Penalties"]["point"]).ToString());
                    
                    UpdateGamePlayer_1(_parsedResponse);  // data for player 1

                }

            }
            else if (_responseData.Header == 602) //cancel matching
            {
                Debug.Log("======== CANCEL_MATCHING_RESPONSE =======");
                var cancelResponse = StatusCancel.Parser.ParseFrom(_responseData.Data);
                if (cancelResponse.StatusCancel_)
                {
                    ScreenManagerHC.Instance.GoToScreenViewWithFull(() => {
                        ScreenManagerHC.Instance.ShowGameModeUI(GameType.Bingo);
                    });
                }
            }
            else if(_responseData.Header == 501)
            {
                Debug.Log("PRACTICE_RESPONSE: 501");
                this.currentMode = (int)PlayMode.Practice;
                Debug.Log("PRACTICE_RESPONSE:");
                HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Bingo);
            }
           
        }



        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            var fullMsg = string.Format("WebSocket closed! Code: {0} Message: {1}", code, message);
            Debug.LogError(fullMsg);
            webSocket = null;
            OnDisconnectedSocket?.Invoke(fullMsg);
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

        #region Tutorial
        
        public void StartPractice(string gameUrl, string usercodeID)
        {
           Connect(cData,() => {
                SendPractice((int)GameType.Bingo, usercodeID);
            });
        }
        public void SendPractice(int gameType, string userCodeID)
        {
            var requestData = new PracticeRequest
            {
                UserCodeId = userCodeID,
                GameType = gameType
            };
            SendMessageData((uint)PackageHeader.PRACTICE_REQUEST, requestData);
        }
        public void SendMessageData(uint actionID, IMessage data)
        {
            Debug.Log(" Send Message Data : " + (PackageHeader)actionID +"==="+ data);
            SendByteString(actionID, data.ToByteString());
        }
       


        #endregion
       
      
        public void SendMessageString(string input)
        {
            Debug.Log("SEND_MESSAGE:   " + input);
            if(webSocket.IsOpen) webSocket?.Send(input);
        }


        public void SendMessageBingo()
        {
            Debug.Log("SendMessage__Bingo: ");
            Request _request = new Request();
            _request.Type = 3; // call bingo

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;
            _packageData.Data = _request.ToByteString();

            SendPackage(_packageData.ToByteArray());
        }

        public void SendMessageClick(int num,int index)
        {

            if (Bingo_Bot_Booster.instance._isSelectingWildDaub == true)
            {

                Debug.LogWarning("SendMessage_WildDaub_ CLICK ( ):  " + num);

                PackageData _packageData = new PackageData();
                _packageData.Header = 999;

                Request _request = new Request();
                _request.Type = 5; // use booster
                _request.Num = num; // num picked

                _packageData.Data = _request.ToByteString();
                SendPackage(_packageData.ToByteArray());



            }
            else if (Bingo_Bot_Booster.instance._isPickABall == true)
            {
                Debug.LogWarning("SendMessage_CLICK A BALL ( ):  " + num);

                if (num == 0)
                {
                    Debug.LogWarning("SendMessage_CLICK That Right:  ");
                }
                PackageData _packageData = new PackageData();
                _packageData.Header = 999;

                Request _request = new Request();
                _request.Type = 8; // use booster
                _request.Num = num; // num picked

                _packageData.Data = _request.ToByteString();
                SendPackage(_packageData.ToByteArray());
            }
            else if(Bingo_PickABallTaget.instance.contents.active==true)
            {
                Debug.LogWarning("SendMessage_CLICK A BALL ( ):  " + num);

                if (num == 0)
                {
                    Debug.LogWarning("SendMessage_CLICK That Right:  ");
                }
                PackageData _packageData = new PackageData();
                _packageData.Header = 999;

                Request _request = new Request();
                _request.Type = 7; // use booster
                _request.Num = num; // num picked

                _packageData.Data = _request.ToByteString();
                SendPackage(_packageData.ToByteArray());
            }
            else
            {
                Debug.Log("SendMessage__ClickNumber: " + num);

                PackageData _packageData = new PackageData();
                _packageData.Header = 999;

                Request _request = new Request();
                _request.Type = 2; // select number
                _request.Num = num; // pick number
               
                _packageData.Data = _request.ToByteString();

                SendPackage(_packageData.ToByteArray());

            }


            Bingo_Bot_Booster.instance.SetSelecting_WildDaub(false);
            Bingo_Bot_Booster.instance.SetSelecting_PickABall(false);
        }


        public void SendMessageNoClickPickABall(int num,int index,int type)
        {

          
             if (Bingo_Bot_Booster.instance._isPickABall == true)
            {
                Debug.LogWarning("SendMessage_CLICK A BALL ( ):  " + num);

                if (num == 0)
                {
                    Debug.LogWarning("SendMessage_CLICK That Right:  ");
                }
                PackageData _packageData = new PackageData();
                _packageData.Header = 999;

                Request _request = new Request();
                _request.Type = type; // use booster
                _request.Num = num; // num picked

                _packageData.Data = _request.ToByteString();
                SendPackage(_packageData.ToByteArray());
            }


             Bingo_Bot_Booster.instance.SetSelecting_WildDaub(false);
            Bingo_Bot_Booster.instance.SetSelecting_PickABall(false);
        }
        public void SendMessageNoClickPickABalType7(int num,int index,int type)
        {

          
         
                Debug.LogWarning("SendMessage_CLICK A BALL ( ):  " + num);

                if (num == 0)
                {
                    Debug.LogWarning("SendMessage_CLICK That Right:  ");
                }
                PackageData _packageData = new PackageData();
                _packageData.Header = 999;

                Request _request = new Request();
                _request.Type = type; // use booster
                _request.Num = num; // num picked

                _packageData.Data = _request.ToByteString();
                SendPackage(_packageData.ToByteArray());
            


            Bingo_Bot_Booster.instance.SetSelecting_WildDaub(false);
            Bingo_Bot_Booster.instance.SetSelecting_PickABall(false);
        }

        public BingoGameData _bingoData;
        [Button]
        public void UpdateGamePlayer_1(string jsonData, bool isCountTimeStart = false)
        {
            _bingoData = JsonUtility.FromJson<BingoGameData>(jsonData);
            if (isCountTimeStart)
            {
                
            }
            else
            {
                
            }
            Bingo_GameBoard.instance.OnReceivedNewWebsocketMessage_UpdateGameBoard(_bingoData);
            Bingo_TopPanelGameInfo.instance.OnReceivedNewWebsocketMessage_UpdateInfo(_bingoData);
            Bingo_TopPanelGameInfo.instance.StartCountdown(_bingoData.secondTimeLeft);
            Bingo_GameTargetSpawner.instance.SpawnNewTarget(_bingoData);
            Bingo_Bot_Booster.instance.OnHandleNewData(_bingoData);
        }

        [Button]
        public void UpdateGamePlayer_2(BingoGameDataPlayer2 _data)
        {
            Bingo_TopPanelGameInfo.instance.UpdateDataPlayer2(_data);
        }

        public void SendMessage_DoubleScore(int index)
        {
            Debug.LogWarning("SendMessage__DoubleScore()");

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 4; // use booster
            _request.Num = 3; // double score
            _request.Index = index;
            _packageData.Data = _request.ToByteString();
            SendPackage(_packageData.ToByteArray());
        }


        public void SendMessage_BonusTime(int index)
        {
            Debug.LogWarning("SendMessage__BonusTime()");

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 4; // use booster
            _request.Num = 4; // bonus time
            _request.Index = index;
            _packageData.Data = _request.ToByteString();
            SendPackage(_packageData.ToByteArray());
        }

        [Button]
        public void SendMessage_PauseToPick_PickABall(int index)
        {
            Debug.LogWarning("SendMessage_PauseToPick_PickABall()");

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 4; // use booster 
            _request.Num = 2; // pick a ball
            _request.Index = index;
            _packageData.Data = _request.ToByteString();

            SendPackage(_packageData.ToByteArray());
        }


        [Button]
        public void SendMessage_PickABall(int num)
        {
            Debug.LogWarning("SendMessage_PickABall()" + 2);

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 5; // use booster
            _request.Num = num; // pick number
            _packageData.Data = _request.ToByteString();

            SendPackage(_packageData.ToByteArray());
        }




        [Button]
        public void SendMessage_PauseToPick_WildDaub(int index)
        {
            Debug.LogWarning("SendMessage_PauseToPick_WildDaub()");

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 4; // use booster
            _request.Num = 1; // wild daub id
            _request.Index = index;
            _packageData.Data = _request.ToByteString();
            SendPackage(_packageData.ToByteArray());
        }
        private void SendByteString(uint actionID, ByteString data)
        {
            PackageData packageData = new PackageData();
            packageData.Header = (uint)actionID;
            if (null != data)
            {
                packageData.Data = data;
            }
            SendBinaryData(packageData.ToByteArray());
        }
        public void CancelMatching()
        {
            SendCancelMatching();
        }
        private void SendCancelMatching()
        {
            SendByteString((uint)PackageHeader.CANCEL_MATCHING_REQUEST, ByteString.Empty);
        }
        [Button]
        public void SendMessage_Pick_WildDaub(int numPick)
        {
            Debug.LogWarning("SendMessage_WildDaub()");

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 5; // use booster
            _request.Num = numPick; // num picked

            _packageData.Data = _request.ToByteString();
            SendPackage(_packageData.ToByteArray());
        }
        public void SendPackage(byte[] data)
        {
            if(data != null && webSocket != null && webSocket.IsOpen) webSocket?.Send(data);
        }
        public void SendMessageQuit()
        {
            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 6; // end game
            _packageData.Data = _request.ToByteString();
            SendPackage(_packageData.ToByteArray());
        }
        [Button]
        public void SendMessage_ResumeFrom_PauseToPick()
        {
            Debug.LogWarning("SendMessage_ResumeFrom_PauseToPick()");

            PackageData _packageData = new PackageData();
            _packageData.Header = 999;

            Request _request = new Request();
            _request.Type = 5; // use booster 
            _request.Num = 0; // pause
            _packageData.Data = _request.ToByteString();

            SendPackage(_packageData.ToByteArray());
        }






        #endregion
    }







}

[Serializable]
public class CustomEventString : UnityEvent<string> { }