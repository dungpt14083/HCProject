using BestHTTP;
using BestHTTP.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HcGames.Solitaire;
using Sirenix.OdinInspector;

namespace Solitaire
{
    public class NetworkController
    {
        WebSocket _webSocket = null;

        public class NetworkHeader
        {
            public const int REGISTER = 0;
            public const int FINDING_ROOM = 1;
            public const int FINDING_ROOM_RESP = 2;
            public const int CREATE_ROOM = 3;
            public const int CREATE_ROOM_RESP = 4;
            public const int UPDATE_JOIN_ROOM = 5;
            public const int DISCONNECTED = 7;
            public const int CANCEL_MATCHING_REQUEST = 601;
            public const int CANCEL_MATCHING_RESPONSE = 602;
            public const int RECEIVE_CARD_DATA = 3000;
            public const int UPDATE_POINT = 3001;
            public const int COMMIT_POINT = 3002;
            public const int END_GAME = 3003;
            public const int TIME_OUT = 3004;
            public const int MOVE_CARD = 3005;
            public const int UPDATE_POINT_COMPETITION = 3006;
            public const int UNDO_SEND_REQUEST = 30007;
            public const int SOLITAIRE_UNDO_ACTION = 3008;
            public const int SOLITAIRE_ROLL_REMAIN_ACTION = 3009;

            public const int SOLITAIRE_SEND_DATA_UNDO = 3010;

            public const int SOLITAIRE_AUTO_COMPLETE = 3011;
            //public const int UNDO_RECEIVE_DATA = 3008;
            public const int READY = 3007;
			public const int ERROR = 100000;

        }

        public enum EGeneralError
        {
            GE_INVALID_DATA = 1,
            GE_ALREADY_LOGIN = 2,
            GE_DESYNC_UNDO_DATA = 1000,
        }

        private const int GAMETYPE_SOLITAIRE = 4;

        private Action _OnUpdateRoomCallback = null;

        public Action<string> OnReceivedMessage = null;

        public Action OnConnectedCallback = null;
        public Action<bool> OnCreateRoomCallback = null;
        public Action<CardData_Soli> OnReceiveCardDataCallback = null;
        public Action OnTimeOutCallback = null;
        public Action<bool> OnReceivedEndGameWinnerCallback = null;
        public Action<int> OnUpdateUserPointCallback = null;
        public Action<int> OnUpdateCompetitionPointCallback = null;
        public Action<DataUndo> OnUndoCardsCallback = null;
        public Action<DataUndo> OnUndoCardsClickCallback = null;
        public Action<EGeneralError> OnErrorCallback = null;

        private Action<PackageData> _responseDataCallback = null;
        private string _userId;
        public bool canUndo = false;

        private void SendMessage(string message)
        {
            Debug.Log("Send message!!! " + message);
            if(null != _webSocket)
            {
                _webSocket.Send(message);
            }
        }

        private void SendBinaryData(byte[] data)
        {
            if(null != data && null != _webSocket)
            {
                _webSocket.Send(data);
            }
        }

        private void OnWebSocketOpen(WebSocket webSocket)
        {
            Debug.Log("Websocket!!! Soliaire open!!!");
            OnConnectedCallback?.Invoke();
        }

        private void OnMessageReceived(WebSocket webSocket, string message)
        {
            Debug.Log("Websocket!!! Soliaire!!! Receive message:\n" + message);
            OnReceivedMessage?.Invoke(message);
        }

        private void OnByteDataReceived(WebSocket webSocket, byte[] data)
        {
            Debug.Log(string.Format($"Raw data received:1111111111111111 {data} === {data.Length}"));
            PackageData packageData = PackageData.Parser.ParseFrom(data);
            Debug.Log(string.Format($"Raw data received: {data} === {packageData.Header}"));

            _responseDataCallback?.Invoke(packageData);
            _responseDataCallback = null;

            string dataResponse = string.Empty;
            switch(packageData.Header)
            {
                case (uint)NetworkHeader.CREATE_ROOM_RESP:
                    Debug.Log("Soliraite NetworkHeader.CREATE_ROOM_RESP");
                    dataResponse = CreateRoomResponse.Parser.ParseFrom(packageData.Data).ToString();
                    break;
                case (uint)NetworkHeader.FINDING_ROOM_RESP:
                    Debug.Log("Soliraite NetworkHeader.FINDING_ROOM_RESP");
                    FindingRoomResponse findingRoomResponse = FindingRoomResponse.Parser.ParseFrom(packageData.Data);
                    dataResponse = findingRoomResponse.ToString();
                    Debug.Log("Soliraite NetworkHeader.FINDING_ROOM_RESP dataResponse "+ dataResponse);
                    break;
                case (uint)NetworkHeader.UPDATE_JOIN_ROOM:
                    Debug.Log("Soliraite NetworkHeader.UPDATE_JOIN_ROOM");
                    UpdateJoinRoom joinRoomData = UpdateJoinRoom.Parser.ParseFrom(packageData.Data);
                    dataResponse = joinRoomData.ToString();
                    _OnUpdateRoomCallback?.Invoke();
                    break;
                case (uint)NetworkHeader.RECEIVE_CARD_DATA:
                    Debug.Log("Soliraite NetworkHeader.RECEIVE_CARD_DATA OnReceiveCardDataCallback == null "+(OnReceiveCardDataCallback==null));
                    CardData_Soli cardData = CardData_Soli.Parser.ParseFrom(packageData.Data);
                    Debug.Log("VAR Check"+cardData.TimePlay);
                    dataResponse = cardData.ToString();

                    OnReceiveCardDataCallback?.Invoke(cardData);
                    dataResponse = cardData.ToString();
                    break;
                case (uint)NetworkHeader.UPDATE_POINT:
                    Debug.Log("Soliraite NetworkHeader.UPDATE_POINT");
                    UpdateMove updatePointData = UpdateMove.Parser.ParseFrom(packageData.Data);
                    SGUIManager.Instance.isCanUndo = updatePointData.CanUndo;
                    SGUIManager.Instance.disableButtonUndo();
                    if(0 == updatePointData.ErrorCode)
                    {
                        dataResponse = updatePointData.ToString();
                        OnUpdateUserPointCallback?.Invoke((int)updatePointData.Point);
                    }
                    else
                    {
                        Debug.Log("Error update point!!! " + updatePointData.ErrorCode);
                    }
                    break;
                case (uint)NetworkHeader.END_GAME:
                    Debug.Log("Soliraite NetworkHeader.END_GAME");
                    EndGame_Soli endGameData = EndGame_Soli.Parser.ParseFrom(packageData.Data);
                    dataResponse = endGameData.ToString();
                    if (_userId == endGameData.WinnerId)
                    {
                        OnReceivedEndGameWinnerCallback?.Invoke(true);
                    }
                    else
                    {
                        OnReceivedEndGameWinnerCallback?.Invoke(false);
                    }
                    break;
                case (uint)NetworkHeader.TIME_OUT:
                    Debug.Log("Soliraite NetworkHeader.TIME_OUT");
                    OnTimeOutCallback?.Invoke();
                    break;
                case (uint)NetworkHeader.MOVE_CARD:
                   
                    UpdateMove undoData = UpdateMove.Parser.ParseFrom(packageData.Data);
                  //  Debug.Log("undo data solitarie: "+undoData.UndoCard);
                    dataResponse = undoData.ToString();
                    OnUpdateUserPointCallback?.Invoke((int)undoData.Point);
                  //  Debug.Log("canUndo: "+undoData.CanUndo);
                    canUndo = undoData.CanUndo;
                  //  Debug.Log("canUndo1: "+canUndo);
                    //   OnUndoCardsCallback?.Invoke(undoData.UndoCard);
                    break;
                case (uint)NetworkHeader.UPDATE_POINT_COMPETITION:
                    Debug.Log("Soliraite NetworkHeader.UPDATE_POINT_COMPETITION");
                    updateCompetitionPoint competitionPointData = updateCompetitionPoint.Parser.ParseFrom(packageData.Data);
                    dataResponse = competitionPointData.ToString();
                    OnUpdateCompetitionPointCallback?.Invoke((int)competitionPointData.CompetitionPoint);
                    break;
                case (uint)NetworkHeader.ERROR:
                    HcGames.Solitaire.GeneralError generalError = HcGames.Solitaire.GeneralError.Parser.ParseFrom(packageData.Data);
                    dataResponse = generalError.ToString();
                    OnErrorCallback?.Invoke((EGeneralError)generalError.ErrorCode);
                    break;
                case NetworkHeader.SOLITAIRE_SEND_DATA_UNDO:
                    DataUndo undodata = DataUndo.Parser.ParseFrom(packageData.Data);
                    OnUndoCardsClickCallback?.Invoke(undodata);
                    OnUndoCardsCallback?.Invoke(undodata);
                    break;
                case NetworkHeader.CANCEL_MATCHING_RESPONSE:
                    Debug.Log("======== CANCEL_MATCHING_RESPONSE =======");
                    var cancelResponse = StatusCancel.Parser.ParseFrom(packageData.Data);
                    if (cancelResponse.StatusCancel_)
                    {
                        ScreenManagerHC.Instance.GoToScreenViewWithFull(() => {
                            ScreenManagerHC.Instance.ShowGameModeUI(GameType.Solitaire);
                        });
                    }
                    break;
            }

            string receivedData = $"Header: {packageData.Header}= Data: {dataResponse}";
            Debug.Log("Received data: " + receivedData);
        }

        
        private void OnClosed(WebSocket webSocket, UInt16 code, string message)
        {
            string fullMessage = string.Format("WebSocket closed!!! Code: {0} Message: {1}", code, message);
            Debug.LogError(fullMessage);
            CloseNetwork();
        }

        private void OnError(WebSocket websocket, string error)
        {
            string fullMessage = string.Format("WebSocket error!!! {0}", error);
            Debug.LogError(fullMessage);
            CloseNetwork();
        }

        private int GetTimestamp()
        {
            return (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public void InitNetwork(string url)
        {
            Debug.Log("Solirate network connect to socket");
            if (null != _webSocket)
            {
                _webSocket.Close();
                _webSocket = null;
            }

            //_webSocket = new WebSocket(new Uri(url), protocol: "echo-protocol", origin: "");
            _webSocket = new WebSocket(new Uri(url));
            _userId = SystemInfo.deviceUniqueIdentifier;
            //_userId = SystemInfo.deviceUniqueIdentifier + "B";
#if UNITY_EDITOR
            _userId += "E";
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
            this._webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
            if (HTTPManager.Proxy != null)
                this._webSocket.OnInternalRequestCreated = (ws, internalRequest) => internalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
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
            OnConnectedCallback = null;
            OnReceiveCardDataCallback = null;
            OnReceivedEndGameWinnerCallback = null;
            OnTimeOutCallback = null;
            OnUpdateUserPointCallback = null;
			OnUpdateCompetitionPointCallback = null;
			OnUndoCardsCallback = null;
			OnErrorCallback = null;
            _webSocket?.Close();
            _webSocket = null;
            Debug.Log("Websocket!!! Soliaire close!!!");
        }

        private async UniTask<ByteString> SendRequestData(int packageHeader, ByteString data)
        {
            UniTaskCompletionSource<ByteString> responseWaiting = new UniTaskCompletionSource<ByteString>();
            _responseDataCallback = p => 
            {
                responseWaiting.TrySetResult(p.Data);
            };
            SendByteString((uint)packageHeader, data);
            return await responseWaiting.Task;
        }

        private void SendByteString(uint actionID, ByteString data)
        {
            PackageData packageData = new PackageData();
            packageData.Header = (uint)actionID;
            if(null != data)
            {
                packageData.Data = data;
            }
            SendBinaryData(packageData.ToByteArray());
        }
        
        

        #region API
        public async UniTask<HcGames.FindingRoomResponse> FindRoom(int gameType, int level, string userCodeID, int mmr, HcGames.CCData ccData)
        {
            HcGames.FindingRoom requestData = new HcGames.FindingRoom()
            {
                GameType = gameType,
                Level = level,
                UserCodeId = userCodeID,
                Mmr = mmr,
                CcData = ccData
            };
            var byteStringResponse = await SendRequestData(NetworkHeader.FINDING_ROOM, requestData.ToByteString());
            return HcGames.FindingRoomResponse.Parser.ParseFrom(byteStringResponse);
        }

        public async UniTask<CreateRoomResponse> CreateRoom(Action onUpdateRoom, int level = 1)
        {
            CreateRoom requestData = new CreateRoom()
            {
                GameType = GAMETYPE_SOLITAIRE,
                Level = level,
                UserCodeId = _userId,
                PlayType = 1,
                Mmr = 1,
                CcData = new CCData()
                {
                    MiniGameEventId = 1,
                    Token = "1",
                    WaitingTimeId = 1,
                }
            };
            _OnUpdateRoomCallback = onUpdateRoom;
            Debug.Log("Create room!!! " + requestData.ToString());
            var byteStringResponse = await SendRequestData(NetworkHeader.CREATE_ROOM, requestData.ToByteString());
            return CreateRoomResponse.Parser.ParseFrom(byteStringResponse);
        }

        public void SendMoveCardData(Card[] cards, int inputFrom, int inputTo)
        {
            moveCard moveCardData = new moveCard() 
            {
                From = inputFrom,
                To = inputTo,
            };
            foreach(Card card in cards)
            {
                HcGames.Solitaire.Card cardData = Utils.ParseCardData(card.cardData);
                moveCardData.Card.Add(cardData);
            }
            SendRequestData(NetworkHeader.MOVE_CARD, moveCardData.ToByteString());
        }

        public void RequestUndo()
        {
            SendRequestData(NetworkHeader.UNDO_SEND_REQUEST, null);
        }

        public void RequestSOLITAIRE_UNDO_ACTION()
        {
            SendRequestData(NetworkHeader.SOLITAIRE_UNDO_ACTION, null);
        }
        public void RequestSOLITAIRE_ROLL_REMAIN_ACTION()
        {
            SendRequestData(NetworkHeader.SOLITAIRE_ROLL_REMAIN_ACTION, null);
        }

        public void Ready()
        {
            moveCard moveCardData = new moveCard()
            {
                
            };
            SendRequestData(NetworkHeader.READY, moveCardData.ToByteString());
            Debug.Log("Soliraite NetworkHeader.Ready");
        }
        public void LeaveRoom()
        {
        }
        public void CommitPoint()
        {
            PointSolitaire commitPointData = new PointSolitaire()
            {
            };
            SendRequestData(NetworkHeader.COMMIT_POINT, commitPointData.ToByteString());
        }
        public void SendMessageQuit()
        {
            PackageData _packageData = new PackageData();
            _packageData.Header = 3002;
            _webSocket?.Send(_packageData.ToByteArray());
        }

        public void SendMessageAutoComplte()
        {
            PackageData _packageData = new PackageData();
            _packageData.Header = 3011;
            _webSocket?.Send(_packageData.ToByteArray());
        }
        #endregion

        public void SendCancelMatching()
        {
            SendByteString(NetworkHeader.CANCEL_MATCHING_REQUEST, ByteString.Empty);
        }
    }
}