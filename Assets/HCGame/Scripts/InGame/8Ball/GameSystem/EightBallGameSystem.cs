using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AssemblyCSharp.GameNetwork;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HcGames;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayMode
{
    Practice,
    Single,
    Online,
}
public class EightBallGameSystem : Singleton<EightBallGameSystem>
{
    
    public const int WHITE_BALL_ID = 49;
    public string UserCodeID { get; set; }
    public string TurnCodeID { get; set; }

    private PlayMode currentMode = PlayMode.Practice;
    private GameStatus currentStatus = GameStatus.Disconneced;


    public PlayMode CurrentMode => currentMode;

    private List<int> allBallIds = new List<int>();
    
    private bool isBroken = false;
    private BallGroup myBallGroup = BallGroup.BgtNone;
    private BallGroup otherBallGroup = BallGroup.BgtNone;

    public Action<EightBallShootStatus> OnEndBallCallback;
    public Action<int, bool> OnBallOnPotCallback;
    public Action<bool> OnEndGameCallback;

    public Action<BallGroup> OnGetBallGroupCallback;
    public Action<List<int>, List<int>> OnGetAllBallsGroupCallback;
    public Action<bool, float> OnTimeUpdateCallback;
    
    public Action OnBreaking;
    public Action<Vector3> OnOtherWhiteBallPosCallback;
    public Action OnOtherWhiteBallStartMoveCallback;
    public Action OnOtherWhiteBallEndMoveCallback;
    
    // === Single
    public Action<Dictionary<int, Vector3>> OnBallPosUpdateCallback;
   // public Action<List<int>,int> OnHolePointsUpdateCallback;
    public Action<int> OnRemainShootUpdateCallback;
    public Action<uint> OnSinglePointUpdateCallback;
    public Action<float> OnSingleRemainTimeUpdate;
    public Action<uint> OnSingleEndGameCallback;
    public Action<bool> OnSingleFailedPocketBallCallback;
    
    //Single Mode field
    private Dictionary<int, Vector3> initBallsPos = new Dictionary<int, Vector3>();
    public Dictionary<int, Vector3> InitBallPos => initBallsPos;
    private List<int> pointHoles = new List<int>();
    public List<int> PointHoles => pointHoles;
    private int remainShoot = 0;
    private uint singlePoint = 0;
    public Reward reward;

    public bool IsMyTurn => UserCodeID == TurnCodeID || currentMode == PlayMode.Practice;

    public bool IsShoot = false;
    public bool IsCanControl
    {
        get
        {
            return IsMyTurn && (!IsShoot || currentMode == PlayMode.Practice);
        }
    }
    public int FirstBallIDCollection;
    public int AllBallIDsCount => allBallIds.Count;
    public BallGroup MyBallGroup => myBallGroup;

    #region MATCHING 
    public void StartFindRoomNew(string gameUrl, string usercodeID, int mmr, CCData ccData, int userLevel)
    {
        Debug.Log("-----------Start Find Room EightBall-------> " + gameUrl);
        EightBallNetworkManager.Instance.OnConnectedSocketCallback = OnSocketConnectedEightBall;
        EightBallNetworkManager.Instance.OnDisconnectedSocketCallback = OnSocketDisconnectedEightBall;
        EightBallNetworkManager.Instance.OnOtherJoinRoomCallback = OnOtherJoinRoomEightBall;

        currentStatus = GameStatus.Connecting;
        EightBallNetworkManager.Instance.Connect(gameUrl,()=> {
            Debug.Log("8ball StartFindRoomNew");
            SceneManager.LoadScene("Game Matching");
            EightBallNetworkManager.Instance.SendFindRoomNew((int)GameType.Billard, userLevel, usercodeID, mmr: mmr, ccData: ccData);
        });
    }
    public void StartPractice(string gameUrl, string usercodeID)
    {
        Debug.Log("-----------Start Find Room EightBall-------> " + gameUrl);
        EightBallNetworkManager.Instance.OnConnectedSocketCallback = OnSocketConnectedEightBall;
        EightBallNetworkManager.Instance.OnDisconnectedSocketCallback = OnSocketDisconnectedEightBall;

        currentStatus = GameStatus.Connecting;
        EightBallNetworkManager.Instance.Connect(gameUrl, () => {
            Debug.Log("8ball StartPractice");
            EightBallNetworkManager.Instance.SendPractice((int)GameType.Billard, usercodeID);
        });
    }
    public async UniTask<bool> StartFindRoom(string gameUrl, string usercodeID, int mmr, CCData ccData, int userLevel)
    {
        Debug.Log("-----------Start Find Room EightBall-------> " + gameUrl);
        EightBallNetworkManager.Instance.OnConnectedSocketCallback = OnSocketConnectedEightBall;
        EightBallNetworkManager.Instance.OnDisconnectedSocketCallback = OnSocketDisconnectedEightBall;
        EightBallNetworkManager.Instance.OnOtherJoinRoomCallback = OnOtherJoinRoomEightBall;

        currentStatus = GameStatus.Connecting;
        var findRoomResult = await ConnectToMultiMode(gameUrl, usercodeID, mmr, ccData, userLevel);
        Debug.Log("find room result : " + findRoomResult);
               
        if (findRoomResult)
        {       
            //play multimode mode
            return true;
        }
        EndGame();
        await EightBallNetworkManager.Instance.Disconnect(true);
        // return false;
        //play single modeiu8888888
        await UniTask.Delay(1000);
        var result = await ConnectToSingleMode(gameUrl, usercodeID, mmr, ccData, userLevel);
        return result;     
    }

    private UniTaskCompletionSource<bool> waitForConnectSocket = new UniTaskCompletionSource<bool>();
    private UniTaskCompletionSource<bool> waitForOtherJoinRoom = new UniTaskCompletionSource<bool>();


    private async UniTask<bool> ConnectToSingleMode(string initUrl, string userCodeID, int mmr, CCData ccData,
        int userLevel)
    {
        // connect to single room,
        waitForConnectSocket = new UniTaskCompletionSource<bool>();
        PlayGame(PlayMode.Single, userCodeID);
        EightBallNetworkManager.Instance.Connect(initUrl);
        var ct = new CancellationTokenSource(5000);
        ct.Token.Register(() =>
        {
            waitForConnectSocket.TrySetResult(false);
        });

        var result = await waitForConnectSocket.Task;
        
        
        if (result)
        {
            var createRoomResponse = await EightBallNetworkManager.Instance.SendCreateRoomData((int)GameType.Billard, userLevel, userCodeID, EightBallPlayType.GptSingle, mmr: mmr, ccData: ccData);
            currentStatus = GameStatus.Ready;
            return true;
        }
        return false;
    }

    private async UniTask<bool> ConnectToMultiMode(string initUrl, string userCodeID, int mmr, CCData ccData, int userLevel)
    {
        // connect to find room,
        waitForConnectSocket = new UniTaskCompletionSource<bool>();
        PlayGame(PlayMode.Online, userCodeID);
        EightBallNetworkManager.Instance.Connect(initUrl);
        var result = await waitForConnectSocket.Task;

        if (result)
        {
            SceneManager.LoadScene("Game Matching");
            // connect success
            var findRoomResponse = await EightBallNetworkManager.Instance.SendFindRoomData((int)GameType.Billard, userLevel, userCodeID, mmr: mmr, ccData: ccData);
            if (findRoomResponse != null && findRoomResponse.ToByteString() != ByteString.Empty)
            {
                return true;
            }
            else
            {
                // if have no room available, create new room and wait
                //create new room
                waitForOtherJoinRoom = new UniTaskCompletionSource<bool>();
                var createRoomResponse = await EightBallNetworkManager.Instance.SendCreateRoomData((int)GameType.Billard, userLevel, userCodeID,mmr: mmr, ccData: ccData);
                currentStatus = GameStatus.WaitingOther;
                Debug.Log("finished crate room, begin to waiting : " + DateTime.Now);
                return await waitForAction(5000, waitForOtherJoinRoom);
            }
        }
        return false;
    }
    
    private async UniTask<bool> waitForAction(int timeout, UniTaskCompletionSource<bool> task_comletion_source)
    {
        // UniTaskCompletionSource<bool> task_comletion_source = new UniTaskCompletionSource<bool>();
        var ct = new CancellationTokenSource(timeout);
        using (ct.Token.Register(() => {
                   // this callback will be executed when token is cancelled
                   // task_comletion_source.TrySetCanceled();
                   Debug.Log("action timeout : " +  DateTime.Now);
                   task_comletion_source.TrySetResult(false);
               })) {
            // ...
            return await task_comletion_source.Task;
        }
        
    }
    
    private void OnSocketConnectedEightBall()
    {
        Debug.Log("onSocketConnectedEightBall");
        currentStatus = GameStatus.FindingRoom;
        if (waitForConnectSocket != null)
        {
            waitForConnectSocket.TrySetResult(true);
            waitForConnectSocket = null;
        }
    }
    
    private void OnOtherJoinRoomEightBall(string nickName)
    {
        Debug.Log("Other join room : " + nickName);
        waitForOtherJoinRoom?.TrySetResult(true);
    }
    
    private void OnSocketDisconnectedEightBall(string msg)
    {
        currentStatus = GameStatus.Disconneced;
    }

    public void SetCurrentMode(PlayMode mode)
    {
        currentMode = mode;
    }

    #endregion
    public void PlayGame(PlayMode mode, string userCodeID)
    {
        Debug.Log($"Play game with : {mode}=={userCodeID}");
        isBroken = false;
        allBallIds = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 };
        myBallGroup = BallGroup.BgtNone;
        otherBallGroup = BallGroup.BgtNone;
        currentMode = mode;

        
        if (mode == PlayMode.Online || mode == PlayMode.Single || mode == PlayMode.Practice)
        {
            UserCodeID = userCodeID;
            // UserID = (ulong)DateTime.Now.Ticks;
            //subribe network 
            EightBallNetworkManager.Instance.OnChangeTurnCallback += onChangeTurn;
            EightBallNetworkManager.Instance.OnEndGameCallback += onEndGame;
            EightBallNetworkManager.Instance.OnGetBallGroupType += onGetBallGroup;
            EightBallNetworkManager.Instance.OnWhiteBallUpdateCallback += OnWhiteBallPosUpdate;
            EightBallNetworkManager.Instance.OnWhiteBallMoveStartCallback += OnWhiteBallStartMove;
            EightBallNetworkManager.Instance.OnWhiteBallMoveEndCallback += OnWhiteBallEndMove;
            EightBallNetworkManager.Instance.OnFinalBallCallback += OnFinalBall;
            EightBallNetworkManager.Instance.OnRemainTimeUpdateCallback += OnRemainTimePlayingUpdate;

            //SingleMode
            if (mode != PlayMode.Online)
            {
                singlePoint = 0;
                remainShoot = 3;
                UserCodeID = userCodeID;
                initBallsPos.Clear();
                pointHoles.Clear();
                EightBallNetworkManager.Instance.OnInitBallCallback += OnBallInitPosUpdate;
                EightBallNetworkManager.Instance.OnPointHolesUpdateCallback += OnHolePointsUpdate;
                EightBallNetworkManager.Instance.OnRemainShootUpdateCallback += OnRemainShootUpdate;
                EightBallNetworkManager.Instance.OnSinglePointUpdateCallback += OnSinglePointUpdate;
                EightBallNetworkManager.Instance.OnSingleWhiteballUpdateCallback += OnSingleWhiteBallUpdate;
            }
        }
    }
    
    public void EndGame()
    {
        GameController.Instance.isEndGame = true;
        GameController.Instance.isStartGame = false;
        EightBallNetworkManager.Instance.OnChangeTurnCallback -= onChangeTurn;
        EightBallNetworkManager.Instance.OnEndGameCallback -= onEndGame;
        EightBallNetworkManager.Instance.OnGetBallGroupType -= onGetBallGroup;
        EightBallNetworkManager.Instance.OnWhiteBallUpdateCallback -= OnWhiteBallPosUpdate;
        EightBallNetworkManager.Instance.OnWhiteBallMoveStartCallback -= OnWhiteBallStartMove;
        EightBallNetworkManager.Instance.OnWhiteBallMoveEndCallback -= OnWhiteBallEndMove;
        EightBallNetworkManager.Instance.OnFinalBallCallback -= OnFinalBall;

        EightBallNetworkManager.Instance.OnRemainTimeUpdateCallback -= OnRemainTimePlayingUpdate;

        //SingleMode
        if (currentMode!= PlayMode.Online)
        {
            initBallsPos.Clear();
            pointHoles.Clear();
            EightBallNetworkManager.Instance.OnInitBallCallback -= OnBallInitPosUpdate;
            EightBallNetworkManager.Instance.OnPointHolesUpdateCallback -= OnHolePointsUpdate;
            EightBallNetworkManager.Instance.OnRemainShootUpdateCallback -= OnRemainShootUpdate;
            EightBallNetworkManager.Instance.OnSinglePointUpdateCallback -= OnSinglePointUpdate;
            EightBallNetworkManager.Instance.OnSingleWhiteballUpdateCallback -= OnSingleWhiteBallUpdate;
            if(currentMode == PlayMode.Practice)
            {
                GameController.Instance?.EndTutorial();
            }
        }
        currentMode = PlayMode.Practice;
        isTimeUpdate = false;
    }


    public void BallOnPot(int ballID, int potIndex)
    {

        if(allBallIds.Contains(ballID))
            allBallIds.Remove(ballID);

        if(EightBallGameSystem.Instance.currentMode == PlayMode.Online)
        {
            if (ballID != WHITE_BALL_ID && ballID != 8 && myBallGroup == BallGroup.BgtNone)
            {
                BallGroup ballGroup = BallGroup.BgtNone;
                if (IsMyTurn)
                {
                    ballGroup = ballID < 8 ? BallGroup.Bgt17 : BallGroup.Bgt915;
                }
                else
                {
                    ballGroup = ballID < 8 ? BallGroup.Bgt915 : BallGroup.Bgt17;
                }
                if (ballGroup != BallGroup.BgtNone) onGetBallGroup(ballGroup);
            }
        }
        
        // var isMyBall = myBallGroup != BallGroup.BgtNone && IsValidBall(ballID);
        Debug.Log("Balls Count:" +allBallIds.Count);
        Debug.Log("check is my ball : " + ballID +"===" + IsMyBall(ballID));
        OnBallOnPotCallback?.Invoke(ballID, IsMyBall(ballID));
        
        if (IsMyTurn)
        {
            EightBallNetworkManager.Instance.SendBallOnPot(ballID, potIndex);
        }
    }

    public void EndBall()
    {
        //if (currentMode != PlayMode.Practice)
        //{
        //    EightBallNetworkManager.Instance.SendEndBall();
        //}
        //else
        //{
        //    //local logic
        //    OnEndBallCallback?.Invoke(EightBallShootStatus.Ok);
        //}
        EightBallNetworkManager.Instance.SendEndBall();
    }

    public void BallCollision(NetworkPhysicsObject ball, NetworkPhysicsObject colliderBall)
    {
        if (isBroken == false &&  colliderBall != null)
        {
            Debug.Log($"collision {ball.gameObject.name} -- {colliderBall.gameObject.name}");
            Debug.Log("OnBreaking");
            OnBreaking?.Invoke();
            isBroken = true;
        }
        if (!IsMyTurn)
            return;
        if (colliderBall != null)
        {
            if (FirstBallIDCollection <= 0)
            {
                FirstBallIDCollection = ball.ballID;
            }
            if (ball.ballID == EightBallGameSystem.WHITE_BALL_ID)
            {
                Debug.Log("collider with ball");
                //collider with ball
                EightBallNetworkManager.Instance.SendBallCollider(ball.ToNetworkPhysicData(), colliderBall.ToNetworkPhysicData());
            }
        }
        else
        {
            Debug.Log("collider with bumper");
            // collider with bumper
            EightBallNetworkManager.Instance.SendBumperCollider(ball.ToNetworkPhysicData());
        }
        //if (currentMode != PlayMode.Practice)
        //{
        //    if(!IsMyTurn)
        //        return;
        //    if (colliderBall != null)
        //    {
        //        if(FirstBallIDCollection <= 0)
        //        {
        //            FirstBallIDCollection = ball.ballID;
        //        }
        //        if (ball.ballID == EightBallGameSystem.WHITE_BALL_ID)
        //        {
        //            Debug.Log("collider with ball");
        //            //collider with ball
        //            EightBallNetworkManager.Instance.SendBallCollider(ball.ToNetworkPhysicData(), colliderBall.ToNetworkPhysicData());

        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("collider with bumper");
        //        // collider with bumper
        //        EightBallNetworkManager.Instance.SendBumperCollider(ball.ToNetworkPhysicData());
        //    }
        //}
        //else
        //{
        //    // local logic
        //}       
    }

    public void UpdateWhiteBallPos(Vector3 pos)
    {
        if (IsMyTurn)
        {
            EightBallNetworkManager.Instance.SendUpdateWhiteBall(pos);
        }
    }

    public void StartWhiteBallMove(Vector3 pos)
    {
        if (IsMyTurn)
        {
            EightBallNetworkManager.Instance.SendStartWhiteBallMove(pos);
        }
    }

    public void EndWhiteBallMove(Vector3 pos)
    {
        if (IsMyTurn)
        {
            EightBallNetworkManager.Instance.SendEndWhiteBallMove(pos);
        }
    }

    public bool IsValidBall(int ballId)
    {

        if (currentMode == PlayMode.Single | CurrentMode == PlayMode.Practice)
        {
            return true;
        }
        
        if (myBallGroup == BallGroup.BgtNone)
        {
            return ballId != 8;
        }
        if ((myBallGroup == BallGroup.Bgt8 && IsMyTurn) ||(!IsMyTurn && otherBallGroup == BallGroup.Bgt8) )
        {
            return ballId == 8;
        }
        var checkGroup = IsMyTurn ? myBallGroup : otherBallGroup;
        Debug.Log($"checkGroup = {checkGroup} IsMyTurn = {IsMyTurn}");
        Debug.Log($"checkGroup == BallGroup.Bgt17 ? ballId <= 7 : ballId > 8 = "+(checkGroup == BallGroup.Bgt17 ? ballId <= 7 : ballId > 8));
        return checkGroup == BallGroup.Bgt17 ? ballId <= 7 : ballId > 8;
    }

    private bool IsMyBall(int ballID)
    {
        if (myBallGroup == BallGroup.BgtNone)
        {
            //not have ball group, all is not my ball
            return false;
        }
        return myBallGroup == BallGroup.Bgt17 ? ballID <= 7 : ballID > 8;
    }
    
    public bool IsBreakingShot()
    {
        Debug.Log($"SHPT isBroken = {isBroken} IsBreakingShot = {!isBroken}");
        return !isBroken;
    }
    
    #region NetworkEvent

    public void onChangeTurn(string nextCodeID, EightBallShootStatus status)
    {
        IsShoot = false;
        if (currentMode == PlayMode.Single)
        {
            if (status == EightBallShootStatus.Ok && isRemainShootChanged)
            {
                OnSingleFailedPocketBallCallback?.Invoke(FirstBallIDCollection <= 0);
                isRemainShootChanged = false;
            }
        }
        
        Debug.Log("change turn with : " + status + "===" + nextCodeID + "==== usercode id "+ UserCodeID); 
        
        TurnCodeID = nextCodeID;
        if(!IsMyTurn)
        {
            EightBallUIController.Instance.ShowPopupEndShot(status);
        }
        OnEndBallCallback?.Invoke(status);
        FirstBallIDCollection = -1;
        // _remainTime = TURN_TIME;
    }

    private void onEndGame(string winnerCodeID)
    {
        Debug.Log("on End Game : " + winnerCodeID +"===" + UserCodeID);
        //if (currentMode == PlayMode.Online)
        //{
        //    OnEndGameCallback?.Invoke(winnerCodeID == UserCodeID);
        //} else if (currentMode == PlayMode.Single)
        //{
        //    OnSingleEndGameCallback?.Invoke(singlePoint);
        //}
        EndGame();
        //EightBallNetworkManager.Instance.Disconnect();
        // SceneManager.LoadScene("Home");
    }

    public void onGetBallGroup(BallGroup ballGroup)
    {
        myBallGroup = ballGroup;
        otherBallGroup = myBallGroup == BallGroup.Bgt17 ? BallGroup.Bgt915 : BallGroup.Bgt17;
        var lowerBall = new List<int>();
        var higherBall = new List<int>();

        foreach (var ball in allBallIds)
        {
            if (ball >7)
            {
                higherBall.Add(ball);
            }
            else
            {
                lowerBall.Add(ball);
            }
        }

        if (myBallGroup == BallGroup.Bgt17)
        {
            OnGetAllBallsGroupCallback?.Invoke(lowerBall, higherBall);
        }
        else
        {
            OnGetAllBallsGroupCallback?.Invoke(higherBall, lowerBall);
        }
        
        OnGetBallGroupCallback?.Invoke(myBallGroup);
        
    }

    private void OnWhiteBallPosUpdate(Vector3 newPos)
    {
        if (!IsMyTurn)
        {
            OnOtherWhiteBallPosCallback?.Invoke(newPos);
        }
    }

    private void OnWhiteBallStartMove(Vector3 newPos)
    {
        if (!IsMyTurn)
        {
            OnOtherWhiteBallPosCallback?.Invoke(newPos);
            OnOtherWhiteBallStartMoveCallback?.Invoke();
        }
    }
    
    private void OnWhiteBallEndMove(Vector3 newPos)
    {
        if (!IsMyTurn)
        {
            OnOtherWhiteBallPosCallback?.Invoke(newPos);
            OnOtherWhiteBallEndMoveCallback?.Invoke();
        }
    }

    private void OnFinalBall(string userCodeID)
    {
        if (userCodeID == UserCodeID)
        {
            myBallGroup = BallGroup.Bgt8;
        }
        else
        {
            otherBallGroup = BallGroup.Bgt8;
        }
    }


    public const float TURN_TIME = 30.0f;
    private void OnTimeUpdate(float remainTime)
    {
        // Debug.Log("ontime update : " + remainTime +"===" + TURN_TIME +"==" + currentMode);
        if (remainTime < 0)
        {
            return;
        }
        if (currentMode == PlayMode.Single)
        {
            OnSingleRemainTimeUpdate?.Invoke(remainTime);
            return;
        }
        var percent = remainTime / TURN_TIME;
        OnTimeUpdateCallback?.Invoke(IsMyTurn, percent);
    }
    
    // for Single Mode

    public void OnBallInitPosUpdate(Dictionary<int, Vector3> ballinitPos)
    {
        Debug.Log("OnBallInitPosUpdate count "+ ballinitPos.Count);
        initBallsPos = ballinitPos;
        OnBallPosUpdateCallback?.Invoke(initBallsPos);
    }

    private void OnSingleWhiteBallUpdate(Vector3 pos)
    {
        initBallsPos[EightBallGameSystem.WHITE_BALL_ID] = pos;
        Debug.Log("update white ball init pos: " + pos);
    }

    public void OnHolePointsUpdate(List<int> pointHoles, int jum)
    {
        // await UniTask.Delay(1000);
        Debug.Log($"OnHolePointsUpdate pointHoles {pointHoles.Count} jum = {jum}");
        this.pointHoles = pointHoles;
        GameController.Instance.UpdatePointHoles(pointHoles, jum);
    }

    private bool isRemainShootChanged = false;
    public void OnRemainShootUpdate(int shootCount)
    {

        if (shootCount < remainShoot)
        {
            isRemainShootChanged = true;
        }
        Debug.Log("on Remain ball shoot Update : " + isRemainShootChanged + "==" + shootCount +"==" + remainShoot);

        remainShoot = shootCount;

        OnRemainShootUpdateCallback?.Invoke(shootCount);
    }

    private void OnSinglePointUpdate(uint point)
    {
        Debug.Log("on SinglePoint Update");
        singlePoint = point;
        OnSinglePointUpdateCallback?.Invoke(point);
    }

    public void OnRemainTimePlayingUpdate(uint time)
    {
        _remainTime = time;
        isTimeUpdate = true;
        Debug.Log("on Remain Time Update : " + _remainTime);
    }

    public void StartTick(bool isStart)
    {
        isTimeUpdate = isStart;
    }

    #endregion

    private bool isTimeUpdate = false;
    private float _remainTime = TURN_TIME;
    public void Tick()
    {
        if (!isTimeUpdate)
            return;
        _remainTime -= Time.deltaTime;
        OnTimeUpdate(_remainTime);
    }
    public void CancelMatching()
    {
        EightBallNetworkManager.Instance.SendCancelMatching();
    }
}
