using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BestHTTP.WebSocket;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HcGames;
using ModestTree;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Ping = HcGames.Ping;

namespace AssemblyCSharp.GameNetwork
{

    public class EightBallNetworkManager : Singleton<EightBallNetworkManager>
    {
        public Action OnConnectedSocketCallback;
        public Action<string> OnDisconnectedSocketCallback;

        //Main Action
        public Action<string> OnOtherJoinRoomCallback;
        public Action<Quaternion> OnCuePosChangedCallback;
        public Action<float, uint> OnPlayerHitBallCallback;
        public Action<HcGames.NetworkPhysicsMessage> OnUpdateBallCallback;
        public Action<HcGames.NetworkPhysicsMessages> OnUpdateBallPathCallback;
        public Action<string, EightBallShootStatus> OnChangeTurnCallback;
        public Action<string> OnEndGameCallback;
        public Action<int> OnBallOnPotCallback;
        public Action<BallGroup> OnGetBallGroupType;
        public Action<Vector3> OnWhiteBallUpdateCallback;
        public Action<Vector3> OnWhiteBallMoveStartCallback;
        public Action<Vector3> OnWhiteBallMoveEndCallback;
        public Action<string> OnFinalBallCallback;

        //Single
        public Action<Dictionary<int, Vector3>> OnInitBallCallback;

        public Action<List<int>,int> OnPointHolesUpdateCallback;

        public Action<int> OnRemainShootUpdateCallback;
        public Action<uint> OnSinglePointUpdateCallback;
        public Action<Vector3> OnSingleWhiteballUpdateCallback;
        public Action<uint> OnRemainTimeUpdateCallback;

        public Action<string> OnResponseTextDataCallback; // use for Debug
        private Action<PackageData> responseDataCallback;
        public MatchInformation MatchInformationData;

        private bool isConnected = false;

        public void Connect(string url, Action connectedCallBack = null)
        {
            HCSocketNetworkManager.Instance.Connect(url, connectedCallBack);
        }
        public bool GetIsConnected()
        {
            return isConnected;
        }
        public async UniTask Disconnect(bool isSendMsg = false)
        {
            if (isSendMsg)
            {
                SendByteString((uint)PackageHeader.DISCONNECTED, ByteString.Empty);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
            HCSocketNetworkManager.Instance.Disconnect();
        }

        public EightBallNetworkManager()
        {
            HCSocketNetworkManager.Instance.OnConnectedSocket += OnSocketConnected;
            HCSocketNetworkManager.Instance.OnDisconnectedSocket += OnSocketDisconnected;
            HCSocketNetworkManager.Instance.OnByteDataReceive += HandleByteDataReceived;
        }

        ~EightBallNetworkManager()
        {
            HCSocketNetworkManager.Instance.OnConnectedSocket -= OnSocketConnected;
            HCSocketNetworkManager.Instance.OnDisconnectedSocket -= OnSocketDisconnected;
            HCSocketNetworkManager.Instance.OnByteDataReceive -= HandleByteDataReceived;
        }

        public void Init()
        {

        }

        #region Request-Response

        public async UniTask<CreateRoomResponse> SendCreateRoomData(int gameType, int level, string userCodeID, EightBallPlayType playType = EightBallPlayType.GptMulti, int mmr =1000,
            CCData ccData = default(CCData))
        {
            CreateRoom requestData = new CreateRoom()
            {
                GameType = gameType,
                Level = level,
                UserCodeId = userCodeID,
                PlayType = (int)playType,
                Mmr = mmr,
                CcData = ccData
            };
            var byteStringResponse = await SendRequestData(PackageHeader.CREATE_ROOM, PackageHeader.CREATE_ROOM_RESP, requestData);
            return CreateRoomResponse.Parser.ParseFrom(byteStringResponse);
        }

        public async UniTask<FindingRoomResponse> SendFindRoomData(int gameType, int level, string userCodeID, EightBallPlayType playType = EightBallPlayType.GptMulti, int mmr =1000,
            CCData ccData = default(CCData))
        {
            FindingRoom requestData = new FindingRoom()
            {
                GameType = gameType,
                Level = level,
                UserCodeId = userCodeID,
                PlayType = (int)playType,
                Mmr = mmr,
                CcData = ccData
            };
            var byteStringResponse = await SendRequestData(PackageHeader.FINDING_ROOM, PackageHeader.FINDING_ROOM_RESP, requestData);
            return FindingRoomResponse.Parser.ParseFrom(byteStringResponse);
        }
        public void SendFindRoomNew(int gameType, int level, string userCodeID, EightBallPlayType playType = EightBallPlayType.GptMulti, int mmr = 1000,
            CCData ccData = default(CCData))
        {
            FindingRoom requestData = new FindingRoom()
            {
                GameType = gameType,
                Level = level,
                UserCodeId = userCodeID,
                PlayType = (int)playType,
                Mmr = mmr,
                CcData = ccData
            };
            SendMessageData((uint)PackageHeader.FINDING_ROOM, requestData);
        }
        public void SendCancelMatching()
        {
            SendByteString((uint)PackageHeader.CANCEL_MATCHING_REQUEST, ByteString.Empty);
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
        #endregion

        #region MESSAGE
        public void SendReady()
        {
            Shoot shoot = new Shoot()
            {

            };
            SendMessageData((uint)PackageHeader.BILLIARDS_S2C_READY, shoot);
        }
        public void SendShoot(float power, uint pathLength = 0)
        {
            Shoot shoot = new Shoot()
            {
                Power = power,
                PathLength = pathLength
            };
            SendMessageData((uint)PackageHeader.BILLIARDS_SHOOT, shoot);
        }
        public void QuitGame()
        {
            Shoot shoot = new Shoot()
            {
                
            };
            SendMessageData((uint)PackageHeader.BILLIARDS_S2C_QUIT_GAME, shoot);
        }
        public void SendUpdateCueRotate(Quaternion quaternion)
        {
            HCQuaternion hcQuaternion = quaternion.ToHCQuaternion();

            SendMessageData((uint)PackageHeader.BILLIARDS_UPDATE_CUE_POS, hcQuaternion);
        }

        public void SendUpdateBalls(NetworkPhysicsMessage physicsMessage)
        {
            SendMessageData((uint)PackageHeader.BILLIARDS_UPDATE_BALL, physicsMessage);
        }
        
        public void SendUpdateBallPaths(List<NetworkPhysicsMessage> message)
        {
            NetworkPhysicsMessages physicsMessages = new NetworkPhysicsMessages();
            physicsMessages.NetworkPhysicsMessage.AddRange(message);            
            Debug.Log($"shpt SendUpdateBallPaths message size = {(int)(physicsMessages.CalculateSize()/1024)} KB");
            
            SendMessageData((uint)PackageHeader.BILLIARDS_C2S_UPDATE_ALL_BALL_POS, physicsMessages);
        }


        public void SendBallOnPot(int ballID, int ballIndex)
        {
            UpdateBallDie updateBallDie = new UpdateBallDie()
            {
                BallId = ballID,
                Index = ballIndex
            };
            SendMessageData((uint)PackageHeader.BILLIARDS_UPDATE_BALL_DIE, updateBallDie);
        }

        public void SendEndBall()
        {
            var byteString = ByteString.FromStream(new MemoryStream(new byte[1] { 0 }));
            Debug.Log("Send End ball : ");
            SendByteString((uint)PackageHeader.BILLIARDS_UPDATE_BALLS_STOP, byteString);
        }

        public void SendBallCollider(NetworkObjectPhysicData whiteBall, NetworkObjectPhysicData otherBall)
        {
            BallCollider ballCollider = new BallCollider()
            {
                WhiteBall = whiteBall,
                OtherBall = otherBall
            };
            Debug.Log("send ball collider : " + ballCollider);
            SendMessageData((uint)PackageHeader.BILLIARDS_C2S_BALL_COLLIDER, ballCollider);
        }

        public void SendBumperCollider(NetworkObjectPhysicData ball)
        {
            BumperCollider bumperCollider = new BumperCollider()
            {
                Ball = ball
            };
            // Debug.Log("send bumper collider : " + bumperCollider);
            SendMessageData((uint)PackageHeader.BILLIARDS_C2S_BUMPER_COLLIDER, bumperCollider);
        }

        public void SendUpdateWhiteBall(Vector3 whiteballPos)
        {
            HCVector3 pos = whiteballPos.ToHCVector3();
            
            // Debug.Log("send white ball update : " + whiteballPos);
            
            SendMessageData((int)PackageHeader.BILLIARDS_C2S_UPDATE_WHITE_BALL_POS, pos);
        }

        public void SendStartWhiteBallMove(Vector3 whiteballPos)
        {
            // Debug.Log("send white ball update : " + whiteballPos);

            SendMessageData((int)PackageHeader.BILLIARDS_C2S_START_MOVE_WHITE_BALL, whiteballPos.ToHCVector3());
        }

        public void SendEndWhiteBallMove(Vector3 whiteballPos)
        {
            SendMessageData((int)PackageHeader.BILLIARDS_C2S_END_MOVE_WHITE_BALL, whiteballPos.ToHCVector3());
        }
        
        #endregion

        private async UniTask<ByteString> SendRequestData(PackageHeader requestHeader, PackageHeader responseHeader, IMessage requestData)
        {
            UniTaskCompletionSource<ByteString>
                responseWaiting = new UniTaskCompletionSource<ByteString>();
            responseDataCallback = async p =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                if (p.Header == (uint)responseHeader)
                {
                    responseDataCallback = null;
                    responseWaiting.TrySetResult(p.Data);
                }
            };
            // Debug.Log($"send request data : {requestHeader} =={requestData}");
            SendMessageData((uint)requestHeader, requestData);
            return await responseWaiting.Task;
        }
        public void SendMessageData(uint actionID, IMessage data)
        {
            Debug.Log(" Send Message Data : " + (PackageHeader)actionID +"==="+ data);
            SendByteString(actionID, data.ToByteString());
        }

        public void SendByteString(uint actionID, ByteString data)
        {
            PackageData packageData = new PackageData();
            packageData.Header = (uint)actionID;
            packageData.Data = data;
            HCSocketNetworkManager.Instance.SendBinaryData(packageData.ToByteArray());
        }


        #region WebSocket Event Handlers

        /// <summary>
        /// Called when the web socket is open, and we are ready to send and receive data
        /// </summary>
        void OnSocketConnected()
        {
            Debug.Log("WebSocket Open!");
            isConnected = true;
            OnConnectedSocketCallback?.Invoke();
        }

        /// <summary>
        /// Called when the web socket disconnected
        /// </summary>
        ///
        /// 
        void OnSocketDisconnected(string message)
        {
            Debug.LogError(message);
            isConnected = false;
            OnDisconnectedSocketCallback?.Invoke(message);
        }

        /// <summary>
        /// Called when we received a byte data message from the server
        /// </summary>
        void HandleByteDataReceived(byte[] responseData)
        {
            //Debug.Log(string.Format($"Message received: <color=yellow>{Convert.ToBase64String(responseData)}</color>"));
            PackageData packageData = PackageData.Parser.ParseFrom(responseData);
            if (packageData.Header != (int)PackageHeader.PING)
            {
                Debug.Log(string.Format($"header received: <color=yellow>{packageData.Header}==={packageData.Data.ToBase64()}</color>"));
            }


            responseDataCallback?.Invoke(packageData);
            string dataResponse = string.Empty;
            switch ((PackageHeader)packageData.Header)
            {
                case PackageHeader.CREATE_ROOM_RESP: // create room response
                    dataResponse = CreateRoomResponse.Parser.ParseFrom(packageData.Data).ToString();
                    break;
                case PackageHeader.FINDING_ROOM_RESP: // find room response
                    var data = FindingRoomResponse.Parser.ParseFrom(packageData.Data);
                    dataResponse = data.ToString();
                    HCAppController.Instance.findingRoomResponse = data;
                    Debug.Log("8 ball FINDING_ROOM_RESP " + data);
                    this.currentMode = data.Mode;
                    EightBallGameSystem.Instance.PlayGame((PlayMode)this.currentMode, HCAppController.Instance.userInfo.UserCodeId);
                    if (data.RoomId.Length > 0)
                    {
                        HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Billard);
                        Debug.Log("on message BYTE received:  FOUND ROOM NOW SEND MESSAGE START GAME");
                    }
                    break;

                case PackageHeader.PRACTICE_RESPONSE: // PRACTICE_RESPONSE response
                    this.currentMode = (int)PlayMode.Practice;
                    Debug.Log("PRACTICE_RESPONSE:");
                    EightBallGameSystem.Instance.PlayGame((PlayMode)this.currentMode, HCAppController.Instance.userInfo.UserCodeId);
                    HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Billard);
                    break;

                case PackageHeader.BILLIARDS_S2C_READY: // readyinitBilliard 
                    var initBilliard = InitBilliard.Parser.ParseFrom(packageData.Data);
                    Debug.Log("BILLIARDS_S2C_READY "+ initBilliard.ToString());
                    Debug.Log("GameController.Instance == null " + (GameController.Instance == null));
                    GameController.Instance?.StartGame(initBilliard);
                    break;
                case PackageHeader.UPDATE_JOIN_ROOM: // other join room response
                    var updateJoinRoomData = UpdateJoinRoom.Parser.ParseFrom(packageData.Data);
                    OnOtherJoinRoomCallback?.Invoke(updateJoinRoomData.Nickname);
                    dataResponse = updateJoinRoomData.ToString();
                    break;
                case PackageHeader.UPDATE_TURN: // update turn
                    var turnData = UpdateTurn.Parser.ParseFrom(packageData.Data);
                    OnChangeTurnCallback?.Invoke(turnData.UserCodeId, (EightBallShootStatus)turnData.ErrorCode);
                    dataResponse = turnData.ToString();
                    break;
                case PackageHeader.BILLIARDS_UPDATE_CUE_POS:
                    var cueRotation = HCQuaternion.Parser.ParseFrom(packageData.Data);
                    OnCuePosChangedCallback.Invoke(cueRotation.ToQuaternion());
                    dataResponse = cueRotation.ToString();
                    break;
                case PackageHeader.BILLIARDS_SHOOT:
                    var shootData = Shoot.Parser.ParseFrom(packageData.Data);
                    OnPlayerHitBallCallback?.Invoke(shootData.Power, shootData.PathLength);
                    dataResponse = shootData.ToString();
                    break;
                case PackageHeader.BILLIARDS_UPDATE_BALL:
                    var ballDatas = NetworkPhysicsMessage.Parser.ParseFrom(packageData.Data);
                    OnUpdateBallCallback?.Invoke(ballDatas);
                    dataResponse = ballDatas.ToString();
                    break;
                
                case PackageHeader.BILLIARDS_C2S_UPDATE_ALL_BALL_POS:
                    var allBallDatas = NetworkPhysicsMessages.Parser.ParseFrom(packageData.Data);
                    OnUpdateBallPathCallback?.Invoke(allBallDatas);
                    dataResponse = allBallDatas.ToString();
                    break;
                
                case PackageHeader.PING:
                    var pingData = Ping.Parser.ParseFrom(packageData.Data);
                    dataResponse = pingData.ToString();
                    OnGetPing(pingData.Index);
                    break;
                case PackageHeader.BILLIARDS_UPDATE_END_GAME:
                    var endData = EndGame.Parser.ParseFrom(packageData.Data);
                    dataResponse = endData.ToString();
                    OnEndGameCallback?.Invoke(endData.WinnerId);
                    break;

                case PackageHeader.BILLIARDS_UPDATE_BALL_DIE:
                    var ballOnPot = UpdateBallDie.Parser.ParseFrom(packageData.Data);
                    OnBallOnPotCallback?.Invoke(ballOnPot.BallId);
                    dataResponse = ballOnPot.ToString();
                    break;
                
                case PackageHeader.BILLIARDS_S2C_USER_BALL_GROUP_TYPE:
                    var ballGroupData = HcGames.BallGroupType.Parser.ParseFrom(packageData.Data);
                    OnGetBallGroupType?.Invoke((BallGroup) ballGroupData.BallGroupType_);
                    dataResponse = ballGroupData.ToString();
                    break;
                case PackageHeader.BILLIARDS_C2S_UPDATE_WHITE_BALL_POS:
                    var whiteBallUpdateData = HCVector3.Parser.ParseFrom(packageData.Data);
                    OnWhiteBallUpdateCallback?.Invoke(whiteBallUpdateData.ToVector3());
                    dataResponse = whiteBallUpdateData.ToString();
                    break;
                case PackageHeader.BILLIARDS_C2S_START_MOVE_WHITE_BALL:
                    var whiteBallStartMoveData = HCVector3.Parser.ParseFrom(packageData.Data);
                    OnWhiteBallMoveStartCallback?.Invoke(whiteBallStartMoveData.ToVector3());
                    dataResponse = whiteBallStartMoveData.ToString();
                    break;
                case PackageHeader.BILLIARDS_C2S_END_MOVE_WHITE_BALL:
                    var whiteBallEndMoveData = HCVector3.Parser.ParseFrom(packageData.Data);
                    OnWhiteBallMoveEndCallback?.Invoke(whiteBallEndMoveData.ToVector3());
                    dataResponse = whiteBallEndMoveData.ToString();
                    break;
                case PackageHeader.BILLIARDS_S2C_END_GROUP:
                    var groupFinalData = AllGroupDie.Parser.ParseFrom(packageData.Data);
                    OnFinalBallCallback?.Invoke(groupFinalData.UserCodeId);
                    dataResponse = groupFinalData.ToString();
                    break;
                case PackageHeader.BILLIARDS_S2C_UPDATE_BALL_POS:
                    var ballPosData = BallsPos.Parser.ParseFrom(packageData.Data);
                    Dictionary<int, Vector3> ballsPos = new Dictionary<int, Vector3>();
                    foreach (var ball in ballPosData.ListBallPos)
                    {
                        if (ball.Pos != null && !ballsPos.ContainsKey(ball.BallId))
                        {
                            ballsPos.Add(ball.BallId, ball.Pos.ToVector3());
                        }
                    }
                    dataResponse = ballPosData.ToString();

                    OnInitBallCallback?.Invoke(ballsPos);
                    // OnBallOnPotCallback?.Invoke();
                    break;
                
                case PackageHeader.BILLIARDS_S2C_UPDATE_HOLE_POINT:
                    var ballInHoleData = HolePoints.Parser.ParseFrom(packageData.Data);
                    Debug.Log("OnPointHolesUpdateCallback == null " + (OnPointHolesUpdateCallback == null));
                    OnPointHolesUpdateCallback?.Invoke(ballInHoleData.Points.ToList(), ballInHoleData.JumpCount);
                    dataResponse = ballInHoleData.ToString();
                    break;
                
                case PackageHeader.BILLIARDS_S2C_UPDATE_REMAIN_SHOOT_ERROR_COUNT:
                    var remainShootData = RemainShootErrorCount.Parser.ParseFrom(packageData.Data);
                    OnRemainShootUpdateCallback?.Invoke(remainShootData?.Count ?? 0);
                    dataResponse = remainShootData?.ToString();
                    break;
                
                case PackageHeader.BILLIARDS_S2C_UPDATE_POINT:
                    var singlePointData = SingleRoomPoint.Parser.ParseFrom(packageData.Data);
                    OnSinglePointUpdateCallback?.Invoke(singlePointData.Point);
                    dataResponse = singlePointData?.ToString();
                    break;
                case PackageHeader.BILLIARDS_S2C_UPDATE_WHITE_BALL_POS:
                    var posData = BallPos.Parser.ParseFrom(packageData.Data);
                    OnSingleWhiteballUpdateCallback?.Invoke(posData.Pos.ToVector3());
                    dataResponse = posData?.ToString();
                    break;
                
                case PackageHeader.BILLIARDS_S2C_UPDATE_REMAIN_TIME_PLAYING:
                    var remainTimeData = RemainTimePlaying.Parser.ParseFrom(packageData.Data);
                    OnRemainTimeUpdateCallback?.Invoke(remainTimeData.RemainTime);
                    dataResponse = remainTimeData?.ToString();
                    break;
				case PackageHeader.ERROR:
                    var errorData = HcGames.GeneralError.Parser.ParseFrom(packageData.Data);
                    Debug.LogError("Receive Error FromServer : " + (GeneralError)errorData.ErrorCode);
                    dataResponse = errorData?.ToString();
                    break;
                case PackageHeader.BILLIARDS_C2S_BALL_COLLIDER:
                    Debug.LogWarning("========TRIGGER BALL=======");
                    break;
                case PackageHeader.CANCEL_MATCHING_RESPONSE:
                    Debug.Log("======== CANCEL_MATCHING_RESPONSE =======");
                    var cancelResponse = StatusCancel.Parser.ParseFrom(packageData.Data);
                    if (cancelResponse.StatusCancel_)
                    {
                        ScreenManagerHC.Instance.GoToScreenViewWithFull(() => {
                            ScreenManagerHC.Instance.ShowGameModeUI(GameType.Billard);
                        });
                    }
                    break;
            }
            string receiveData = $"Header: {packageData.Header}= Data: {dataResponse}";
            if (packageData.Header != (uint)PackageHeader.PING)
                Debug.Log("receive Data : " + receiveData);
            OnResponseTextDataCallback?.Invoke(receiveData);
        }
        public void SetMatchInformation(MatchInformation matchInfo)
        {
            MatchInformationData = matchInfo;
        }
        #endregion


        #region Ping

        private float pingInterval = 3.0f;

        uint pingIndex = 0;
        double pingAt = 0;
        public float rtt;
        float nextPing = 0f;
        bool shouldPing = true;

        public Action<float> OnRTTChangedCallback;
        private int currentMode;

        public void SendPing()
        {
            pingAt = GetUnixTime();

            Ping pingData = new Ping()
            {
                Index = pingIndex
            };
            SendByteString((uint)PackageHeader.PING, pingData.ToByteString());

            // Send($"{ACTION_PING}#{pingIndex}");
            //Debug.Log("Ping");
            shouldPing = false;
        }

        void OnGetPing(uint pingIndexReturn)
        {
            // var pingReturn = int.Parse(message);
            if (pingIndexReturn == pingIndex)
            {
                rtt = (float)(GetUnixTime() - pingAt);
                OnRTTChangedCallback?.Invoke(rtt);
                // OnRTT?.Invoke(rtt);
                // rttStatusTxt.text = "RTT: "+rtt.ToString("0.##")+"ms";
                pingIndex++;
                //Debug.Log($"RTT = {rtt}");
                shouldPing = true;
            }
        }

        int GetTimestamp()
        {
            return (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        double GetUnixTime()
        {
            return DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public void UpdatePing()
        {
            if (isConnected)
            {
                if (Time.time > nextPing && shouldPing)
                {
                    SendPing();
                    nextPing = Time.time + pingInterval;
                }
            }
        }

        #endregion
    }
}