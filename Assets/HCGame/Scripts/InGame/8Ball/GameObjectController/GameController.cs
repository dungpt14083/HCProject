using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using TMPro;
using System.Text;
using AssemblyCSharp.GameNetwork;
using HcGames;
using Tweens.Plugins;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameController : SingletonMono<GameController>
{
    // NetworkPhysicsObject[] networkObjs;
    public Dictionary<int, NetworkPhysicsObject> networkObjDicts;


    public bool isTutorial = false;
    //public float force;
    public float minForce = 1f;
    public float maxForce = 5f;
    public int bodyDelayFrame = 15;
    //public Transform target;
    public ForceMode forceMode;
    public Vector3 offset;
    // public static GameController instance;
    // ACTION
    public Action<BallPhysicConfig> onSettingUpdate;
    public Action OnStartHandBall;
    public Action OnEndHandBall;
    public Action<bool> OnBlockShot;
    // --
    public BallPhysicConfig config;
    List<BallPosition> ballPositions = new List<BallPosition>();
    //public Camera sceneCamera;
    //public LayerMask tableLayer;
    //public TMP_Text status;

    [Header("Network")]
    public string url;
    public InputField inputUrl;
    public Button hitBtn;
    //bool stateChange = false;
    bool otherTurn = false;
    bool startNetworkSimulate = false;

    public PhysicController physicController;
    public BallSpawner ballSpawner;
    public WhiteBallPlacementController whiteBallPlacementController;

    [Header("Game Object")]
    public NetworkPhysicsObject whiteBall;
    // [SerializeField] private GameObject cueSpinObject;
    [SerializeField] private ShotPowerScript shotPowerIndicator;
    [SerializeField] private GameObject targetLine;
    [SerializeField] private GameObject cue;
    [SerializeField] private TMP_Text rttText;
    [SerializeField] private Image imgCountDown;
    [SerializeField] private GameObject table;
    [SerializeField] private EightQuitPanelController quitPanelController;

    public List<Pot> tablePots = new List<Pot>();

    public Transform rollPotedAnchor;
    public bool IsBallsMoving => physicController.IsMovement;
    [HideInInspector]
    public bool BallOverlap { private set; get; }
    Queue<NetworkPhysicsMessage> networkDataQueue = new Queue<NetworkPhysicsMessage>();
    public CueController cueController;
    float shootPower;
    public TutorialController tutorialController;
    public bool isStartGame = false;
    public bool isEndGame = false;
    private class BallPosition
    {
        public Vector3 position;
        public Quaternion rotation;
    }
    private void Awake()
    {
        // instance = this;
        // Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
    }

    public Vector3 GetWhiteBallInitPos()
    {
        if (EightBallGameSystem.Instance.InitBallPos.TryGetValue(EightBallGameSystem.WHITE_BALL_ID, out var pos) && EightBallGameSystem.Instance.CurrentMode == PlayMode.Single)
        {
            return pos;
        }
        return ballSpawner.whiteBallPos.position;
    }

    private void Start()
    {
        ScreenManagerHC.Instance.ShowUserDataAndNavigation(false);
        otherTurn = !EightBallGameSystem.Instance.IsMyTurn;
        physicController.OnFinishPrephase += OnFinishPrephase;
        physicController.OnStopSimulationCallback += StopSimulation;
        EightBallNetworkManager.Instance.OnRTTChangedCallback += OnPingRTTCallback;
        EightBallNetworkManager.Instance.OnCuePosChangedCallback += OnTargetChanged;
        EightBallNetworkManager.Instance.OnPlayerHitBallCallback += OnPlayerHitBall;
        
        EightBallGameSystem.Instance.OnEndBallCallback += OnChangeTurn;
        EightBallNetworkManager.Instance.OnDisconnectedSocketCallback += OnDisconnectedSocket;
        EightBallGameSystem.Instance.OnBallPosUpdateCallback += OnStartGameBallPosition;
        EightBallNetworkManager.Instance.OnUpdateBallPathCallback += OnUpdateBallPath;
        //EightBallGameSystem.Instance.OnHolePointsUpdateCallback += UpdatePointHoles;
        HideAllPoint();
        //whiteBallPlacementController = GetComponentInChildren<WhiteBallPlacementController>();
        //cueController = whiteBall.GetComponent<CueController>();
        cueController.HideCueController();
        //OnStartHandBall += whiteBallPlacementController.FindNewPosition;
        //OnEndHandBall += whiteBallPlacementController.EndHandBall;
        tutorialController.HideTutorial();
        HcPopupManager.Instance.ShowEightGameLoading(false);
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            EightBallNetworkManager.Instance.SendReady();
        }
        else
        {
            StartCoroutine(CountDown());
        }
        //else if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
        //{
        //    StartCoroutine(CountDown());
        //}
    }
    //public void RestartTutorial()
    //{
    //    StartTutorial(tutorialController.initBilliard);
    //}
    //public void StartTutorial(InitBilliard initBilliard)
    //{
    //    physicController.localPhysicSteps.Clear();
    //    networkDataQueue.Clear();
    //    isTutorial = true;
    //    isStartGame = true;
    //    isEndGame = false;
    //    var ballPosData = initBilliard.BallsPos;
    //    HideAllPoint();
    //    ClearAllBall();
    //    //whiteBall.Rigidbody.isKinematic = true;
    //    whiteBall.transform.rotation = Quaternion.identity;
    //    //random bong

    //    Dictionary<int, Vector3> ballsPos = new Dictionary<int, Vector3>();
    //    foreach (var ball in ballPosData.ListBallPos)
    //    {
    //        if (ball.Pos != null && !ballsPos.ContainsKey(ball.BallId))
    //        {
    //            ballsPos.Add(ball.BallId, ball.Pos.ToVector3());
    //        }
    //    }
        
    //    //Dictionary<int, Vector3> initBallsPosTutorial = new Dictionary<int, Vector3>();
    //    //initBallsPosTutorial.Add(49, new Vector3(0, 0, -0.24f));
    //    //initBallsPosTutorial.Add(1, new Vector3(-1.49f, 6.47f, -0.24f));
    //    //whiteBall.gameObject.SetActive(true);
    //    ballObjects = ballSpawner.SpawnByServer(whiteBall, ballsPos);
    //    tutorialController.StartTutorial(initBilliard, ballObjects);
    //    physicController.InitData(ballObjects);
    //    whiteBallPlacementController.StartTutorial();
    //    ShowAllControllers();
    //    refreshTurnUI();
    //}
    public IEnumerator CountDown()
    {
        int count = 2;
        imgCountDown.gameObject.SetActive(true);
        imgCountDown.sprite = ResourceManager.Instance.GetImgCountDown(count);
        imgCountDown.SetNativeSize();
        count -= 1;
        yield return new WaitForSeconds(1f);
        imgCountDown.sprite = ResourceManager.Instance.GetImgCountDown(count);
        imgCountDown.SetNativeSize();
        count -= 1;
        yield return new WaitForSeconds(1f);
        imgCountDown.sprite = ResourceManager.Instance.GetImgCountDown(count);
        imgCountDown.SetNativeSize();
        yield return new WaitForSeconds(1f);
        imgCountDown.gameObject.SetActive(false);
        EightBallNetworkManager.Instance.SendReady();
    }
    public void StartGame(InitBilliard initBilliard)
    {
        var ballPosData = initBilliard.BallsPos;
        NetworkPhysicsObject[] ballObjects = null;
        Dictionary<int, Vector3> ballsPos = new Dictionary<int, Vector3>();
        if (EightBallGameSystem.Instance.CurrentMode != PlayMode.Online  && ballPosData.ListBallPos != null)
        {
            Debug.Log("ballPosData.ListBallPos " + ballPosData.ListBallPos.Count);
            
            foreach (var ball in ballPosData.ListBallPos)
            {
                if (ball.Pos != null && !ballsPos.ContainsKey(ball.BallId))
                {
                    ballsPos.Add(ball.BallId, ball.Pos.ToVector3());
                }
            }
            EightBallGameSystem.Instance.OnBallInitPosUpdate(ballsPos);
            EightBallGameSystem.Instance.OnHolePointsUpdate(initBilliard.HolePoints.Points.ToList(),0);
            EightBallGameSystem.Instance.OnRemainShootUpdate(initBilliard.RemainShootErrorCount);
            EightBallGameSystem.Instance.OnRemainTimePlayingUpdate(initBilliard.RemainTimePlaying);
            
            ballObjects = ballSpawner.SpawnByServer(whiteBall, EightBallGameSystem.Instance.InitBallPos);
            //UpdatePointHoles(EightBallGameSystem.Instance.PointHoles);
        }
        else if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            HideAllPoint();
            ballObjects = ballSpawner.Spawn_V3(whiteBall, new int[15] { 1, 10, 15, 4, 8, 7, 6, 11, 12, 5, 13, 14, 3, 9, 2 });
        }
        
        if(ballObjects == null)
        {
            Debug.Log("ballObjects == null " + (ballObjects == null));
        }
        Debug.Log("whiteBallPlacementController == null " + (whiteBallPlacementController == null));
        Debug.Log("EightBallGameSystem.Instance.CurrentMode " + EightBallGameSystem.Instance.CurrentMode);
        physicController.InitData(ballObjects);

        
        networkObjDicts = new Dictionary<int, NetworkPhysicsObject>();
        foreach (var networkObj in ballObjects)
        {
            networkObjDicts.Add(networkObj.ballID, networkObj);
            Debug.Log("networkObj.ballID " + networkObj.ballID);
        }
        EightBallGameSystem.Instance.onChangeTurn(initBilliard.UpdateTurn.UserCodeId, (EightBallShootStatus)initBilliard.UpdateTurn.ErrorCode);
        EightBallUIController.Instance.StartGame();
        // EightBallNetworkManager.Instance.OnChangeTurnCallback += OnUpdateTurn;  
        //physicController.OnSample += OnPhysicSample;
        //physicController.BeforeProcess += BeforePhysicProcess;

        // whiteBallPlacement = true;
        //OnStartHandBall?.Invoke();
        //whiteBallPlacementController.FindNewPosition();
        whiteBallPlacementController.StartGame();
        WhiteBallPlacement();

        isStartGame = true;
        if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
        {
            tutorialController.StartTutorial(ballObjects, ballsPos);
            EightBallGameSystem.Instance.OnGetAllBallsGroupCallback.Invoke(ballsPos.Keys.Where(x=>x != 49).ToList(), new List<int>());
        }
    }
    public void ResetBall()
    {
        var ballsPos = tutorialController.ballsPos;
        foreach(var ball in networkObjDicts)
        {
            if (ball.Value.poted && ballsPos.ContainsKey(ball.Key) && tutorialController.ballIdTarget == tutorialController.ballIdOnpoted)
            {
                continue;
            }
            ball.Value.ForceSetPos(ballsPos[ball.Key]);
        }
    }
    public void UpdatePointHoles(List<int> pointHoles, int jum)
    {
        StartCoroutine(EffectJumPointHoles(pointHoles, jum));

        //for (int i = 0; i < pointHoles.Count; i++)
        //{
        //    tablePots[i].SetPoint(pointHoles[i]);
        //}
    }
    public IEnumerator EffectJumPointHoles(List<int> pointHoles,int jum)
    {
        List<Vector3> potCache = tablePots.Select(x=>x.transform.position).ToList();
        if(jum > 0)
        {
            for (int j = 0; j < jum; j++)
            {
                for (int i = 0; i < tablePots.Count; i++)
                {
                    int newPosIndex = (i + j + 1) % tablePots.Count;
                    var newPos = potCache[newPosIndex];
                    tablePots[i].transform.DOMove(newPos, 1);
                }
                yield return new WaitForSeconds(1);
            }
        }
        for (int i = 0; i < pointHoles.Count; i++)
        {
            tablePots[i].transform.position = potCache[i];
            tablePots[i].SetPoint(pointHoles[i]);
        }
    }
    public void HideAllPoint()
    {
        foreach(var pot in tablePots)
        {
            pot.HidePoint();
        }
    }
    void OnStartGameBallPosition(Dictionary<int, Vector3> ballPos)
    {
        foreach (var ball in ballPos)
        {
            Debug.Log($"ball id = {ball.Key} - post = {ball.Value}");
        }
    }

    private void OnDestroy()
    {
        physicController.OnFinishPrephase -= OnFinishPrephase;
        physicController.BeforeProcess -= BeforePhysicProcess;
        physicController.OnStopSimulationCallback -= StopSimulation;

        EightBallNetworkManager.Instance.OnRTTChangedCallback -= OnPingRTTCallback;
        EightBallNetworkManager.Instance.OnCuePosChangedCallback -= OnTargetChanged;
        EightBallNetworkManager.Instance.OnPlayerHitBallCallback -= OnPlayerHitBall;
        EightBallNetworkManager.Instance.OnUpdateBallCallback -= OnUpdateBallMessage;
        EightBallGameSystem.Instance.OnEndBallCallback -= OnChangeTurn;
        EightBallNetworkManager.Instance.OnDisconnectedSocketCallback -= OnDisconnectedSocket;
        EightBallGameSystem.Instance.OnBallPosUpdateCallback -= OnStartGameBallPosition;

        EightBallNetworkManager.Instance.OnUpdateBallPathCallback -= OnUpdateBallPath;
    }


    void OnTargetChanged(Quaternion quaternion)
    {
        //Debug.Log($"SHPT OnTargetChanged otherTurn = {otherTurn} -- IsMyTurn {EightBallGameSystem.Instance.IsMyTurn}");
        if (EightBallGameSystem.Instance.IsMyTurn) return;
        cueController.UpdateCueRot(quaternion);
        // target.position = pos;
    }

    void OnPlayerHitBall(float power, uint pathLength)
    {
        Debug.Log($"SHPT OnPlayerHitBall networkPhysicsMessage data count = {pathLength}");
        //Execute(false);
        //physicController.StartSimulate(networkPhysicsMessage);
        //networkPhysicsMessage.Clear();
        totalMessage = pathLength;
        networkData.Clear();
        //physicController.StartSimulate();
        whiteBallPlacement = false;
        whiteBallPlacementController.EndHandBall();
        HideAllControllers();

    }


    public void OnWhiteBallOverlap(bool isOverlap)
    {
        BallOverlap = isOverlap;
        OnBlockShot?.Invoke(isOverlap);
    }

    public void Shoot(float power, Vector3 shootPos, Vector3 trickShotAdd = new Vector3())
    {
        if (power <= 0.01f || BallOverlap || !EightBallGameSystem.Instance.IsMyTurn) return;
        if (whiteBallPlacementController.isError) return;
        shootPower = power;
        Debug.Log("Shoot my turn : " + power);
        EightBallGameSystem.Instance.IsShoot = true;
        whiteBallPlacement = false;
        //OnEndHandBall?.Invoke();
        whiteBallPlacementController.EndHandBall();
        //when shoot, hide all controller
        //HideAllControllers();
        whiteBall.Rigidbody.isKinematic = false;
        var rb = whiteBall.Rigidbody;
        Vector3 direction = whiteBall.transform.position - shootPos;
        direction.z = 0;
        direction = direction.normalized;
        
        var trickShot = rb.position;
        trickShot = trickShot + trickShotAdd;
        var shootForce = Mathf.Lerp(minForce, maxForce, power);
        Vector3 shotPower = direction * shootForce;
        Debug.Log($"power = {power}--- shootForce = {shootForce} --- trickShoot = {trickShot} -- real force = ${shotPower.magnitude}");
        CueController.PredictCollisionData collisionData = cueController.PredictHitInfo();
        //whiteBall.SetTargetDirWhenCollision(targetDir, ballId);
        HideAllControllers();
        physicController.SetOverrideData(collisionData);
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
        {
            //Physics.autoSimulation = true;
            //EightBallGameSystem.Instance.IsShoot = false;
        }
        rb.AddForceAtPosition(shotPower, trickShot, forceMode);
        whiteBall.ScheduleSoundFX(NetworkPhysicsObject.SOUND_FX.CUE_VS_BALL, power);
        //rb.AddTorque(shotPower, forceMode);
        
        physicController.StartSimulate();
        otherTurn = false;

        // var updateNetwork = true;
        // if (updateNetwork)
        // {
        // }
        // else
        // {
        //     otherTurn = true;
        // }
        networkData.Clear();
        //Debug.Break();
    }

    public void UpdateCueRotation(Quaternion quaternion)
    {
        EightBallNetworkManager.Instance.SendUpdateCueRotate(quaternion);

    }

    #region NETWORK EVENT CALLBACK

    private void OnPingRTTCallback(float rtt)
    {
        if (rttText != null)
        {
            rttText.text = $"RTT: {rtt}";
        }
    }

    #endregion


    #region PRIVATE
    
    public void EndTutorial()
    {
        isEndGame = true;
        tutorialController.EndTutorial();
    }
    public void OnChangeTurn(EightBallShootStatus shootStatus)
    {
        if (isEndGame) return;
        //whiteBallPlacement = true;
        if (shootStatus != EightBallShootStatus.Ok)
            whiteBallPlacement = true;
        Debug.Log("change turn with whiteball placement : " + whiteBallPlacement);
        if (whiteBallPlacement)
        {
            whiteBall.Rigidbody.isKinematic = true;
            //if (!EightBallGameSystem.Instance.IsMyTurn)
            //    EightBallUIController.Instance.ShowUserHandWhiteBall(false);
            //OnStartHandBall?.Invoke();
            whiteBallPlacementController.FindNewPosition();
            if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Single)
            {
                Debug.Log("On EndBall invoke change turn");
                //OnEndHandBall?.Invoke();
                whiteBallPlacementController.EndHandBall();
            }
        }
        
        otherTurn = !EightBallGameSystem.Instance.IsMyTurn;
        refreshTurnUI();
        
        //Single mode , update hole points
        //if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Single)
        //{
        //    UpdatePointHoles(EightBallGameSystem.Instance.PointHoles);
        //}
    }

    

    public void StopSimulation()
    {
        networkData.Clear();
        EightBallGameSystem.Instance.EndBall();
    }

    public bool isControlDisplay;
    [HideInInspector]
    public bool hideControl;
    public void ShowAllControllers()
    {
        Debug.Log($"Showing controllers at frame = {Time.frameCount}");
        cue.transform.position = new Vector3(whiteBall.transform.position.x, whiteBall.transform.position.y, cue.transform.position.z);
        cueController.ShowTargetLine();
        isControlDisplay = true;
        hideControl = false;
    }

    public void HideAllControllers()
    {
        Debug.Log($"Hide controllers at frame = {Time.frameCount}");
        targetLine.SetActive(false);
        cueController.cueRenderer.enabled = false;
        //cue.GetComponent<Renderer>().enabled = false;
        isControlDisplay = false;
    }

    public void ForceHideControl(bool value)
    {
        hideControl = value;
        if (value)
            HideAllControllers();
        else
            ShowAllControllers();
    }

    #endregion

    private void Update()
    {
        if (!isStartGame) return;
        if (isEndGame) return;
        //physicController.UpdatePhysics();
        physicController.UpdatePhysicsV3();
        //if (!physicController.IsMovement && stateChange && !otherTurn)
        //{
        //    //UpdateBallsPos();
        //    stateChange = false;
        //}
        HandlePhysicsQMessage();
        if ((physicController.IsMovement) && isControlDisplay)
        {
            //UnityEditor.EditorApplication.isPaused = true;
            HideAllControllers();
        }
        else if (!physicController.IsMovement && !isControlDisplay)
            if (!hideControl)
            {

                // delay show control for pre-simulate physic
                if (!physicController.IsPrePhase && physicController.EndMovementFrame + bodyDelayFrame <= Time.frameCount)
                {
                    ShowAllControllers();
                }
            }

        //update ping 
        EightBallNetworkManager.Instance.UpdatePing();

    }


    //TODO: temp using for show timeout
    private void FixedUpdate()
    {
        EightBallGameSystem.Instance.Tick();
    }
    public void QuitGame()
    {
        EightBallNetworkManager.Instance?.QuitGame();
        quitPanelController.ShowQuitPopup(false);
    }
    //public void Disconnect()
    //{
    //    EightBallNetworkManager.Instance.Disconnect(true);
    //    HCAppController.Instance.GotoHome();
    //    // if (inputUrl != null)
    //    //     url = inputUrl.text;
    //    // networkController.InitNetwork(url);
    //}

    #region Helpers
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    #endregion
    // Dictionary<int, NetworkPhysicsMessage> networkData = new Dictionary<int, NetworkPhysicsMessage>();
    Dictionary<int, NetworkPhysicsMessage> networkData = new Dictionary<int, NetworkPhysicsMessage>();

    void BeforePhysicProcess(int tick)
    {
        if (networkData.ContainsKey(tick))
        {
            UpdateLocalPhysicObject(networkData[tick].NetworkObjects.ToArray());
        }
    }

    void UpdateLocalPhysicObject(NetworkObjectPhysicData[] data)
    {
        foreach (var obj in data)
        {
            if (networkObjDicts.TryGetValue(obj.Id, out var localObj))
            {
                localObj.Rigidbody.position = obj.Position.ToVector3();
                localObj.Rigidbody.rotation = obj.Rotation.ToQuaternion();
                localObj.Rigidbody.velocity = obj.Velocity.ToVector3();
                localObj.Rigidbody.angularVelocity = obj.AngularVelocity.ToVector3();
            }
        }
    }

    //void OnPhysicSample(int physicTick, bool isFinal)
    //{
    //    Debug.Log($"OnPhysicSample physicTick = {physicTick} --- isFinal = {isFinal} -- otherTurn = {otherTurn}");
    //    // only send if this is our turn
    //    if (otherTurn)
    //    {
    //        if (isFinal)
    //        {
    //            startNetworkSimulate = false;
    //            Debug.Log($"Stop Handle Q message, msg count = {networkDataQueue.Count}");
    //            networkDataQueue.Clear();
    //        }
    //        return;
    //    }

    //    //var message = new NetworkPhysicsMessage();
    //    //message.tick = physicTick;
    //    List<NetworkObjectPhysicData> objects = new List<NetworkObjectPhysicData>();
    //    var listLocalObject = networkObjDicts.Values.ToList();
    //    for (int i = 0; i < listLocalObject.Count; i++)
    //    {
    //        var networkObj = listLocalObject[i];
    //        if (!isFinal && !physicController.IsObjectMoving(networkObj.Rigidbody)) continue;
    //        var nop = networkObj.ToNetworkPhysicData();
    //        // var nop = new NetworkObjectPhysicData()
    //        // {
    //        //     Id = networkObj.ballID,
    //        //     Position = networkObj.Rigidbody.position.ToHCVector3(),
    //        //     Rotation = networkObj.Rigidbody.rotation.ToHCQuaternion(),
    //        //     Velocity = networkObj.Rigidbody.velocity.ToHCVector3(),
    //        //     AngularVelocity = networkObj.Rigidbody.angularVelocity.ToHCVector3()
    //        // };
    //        objects.Add(nop);
    //    }

    //    var message = new NetworkPhysicsMessage();
    //    message.Tick = physicTick;
    //    message.NetworkObjects.AddRange(objects.ToArray());
    //    Debug.Log($"SHPT Send OnPhysicSample tick {message.Tick} -- ball count = {message.NetworkObjects.Count}");
    //    EightBallNetworkManager.Instance.SendUpdateBalls(message);
    //    // networkController.SendPhysicsData(JsonUtility.ToJson(message));
    //}

    void OnFinishPrephase(int frame, List<NetworkPhysicsMessage> messagesToSend)
    {       
        //Debug.Log($"SHPT meHitCount = {meHitCount}");
        //EightBallNetworkManager.Instance.SendUpdateBallPaths(messagesToSend.ToList());
        var count = 50;
        int batch = (int)(messagesToSend.Count / count) + 1;
        //EightBallNetworkManager.Instance.SendUpdateBallPaths(messagesToSend.GetRange(0, count));
        //EightBallNetworkManager.Instance.SendShoot(shootPower);
        //return;
        Debug.Log($"SHPT send tick count = {messagesToSend.Count}");
        EightBallNetworkManager.Instance.SendShoot(shootPower, (uint)messagesToSend.Count);
        for (int i = 0; i < batch; i++)
        {
            var start = i * count;
            var end = start + count;
            if (end >= messagesToSend.Count)
                count = messagesToSend.Count - start;
            //messagesToSend.GetRange(start, count);            
            var dataToSend = messagesToSend.GetRange(start, count);
            if (dataToSend.Count > 0)
            {
                var startTick = dataToSend[0].Tick;
                var endTick = dataToSend[dataToSend.Count - 1].Tick;
                //Debug.Log($"SHPT Send range = from tick = {startTick} to tick = {endTick}");
                EightBallNetworkManager.Instance.SendUpdateBallPaths(dataToSend);
            }

        }
        

    }
    void SimulatePhysicFromNetwork(NetworkPhysicsMessage msg)
    {
        //status.text = $"Update ball at network tick = {msg.tick}";
        var frameFromTick = physicController.GetFrameFromTick(msg.Tick);

        UpdateLocalPhysicObject(msg.NetworkObjects.ToArray());
        Physics.Simulate(Time.fixedDeltaTime);
        foreach (var obj in msg.NetworkObjects)
        {
            if (networkObjDicts.TryGetValue(obj.Id, out var localObj))
            {

                localObj.UpdateQData(frameFromTick, localObj.Rigidbody.position, localObj.Rigidbody.rotation);
            }
            else
            {
                Debug.LogError($"Can not find object at id {obj.Id}");
            }
            // var localObj = networkObjs[obj.Id];
            // if (localObj == null)
            // {
            //     Debug.LogError($"Can not find object at index {obj.Id}");
            //     continue;
            // }
            // localObj.UpdateQData(frameFromTick, localObj.Rigidbody.position, localObj.Rigidbody.rotation);
        }

    }

    void HandleNetworkPhysicsMessage(NetworkPhysicsMessage msg)
    {
        var networkTick = msg.Tick;
        Debug.Log($"HandleNetworkPhysicsMessage networkTick = {networkTick} --- current = {physicController.Tick}");
        if (networkTick >= physicController.Tick)
        {
            // store it for check later
            networkData.Add(networkTick, msg);
            Debug.Log("HandleNetworkPhysicsMessage =>" + networkTick.ToString());
        }
        else
        {
            while (networkTick < physicController.Tick)
            {
                SimulatePhysicFromNetwork(msg); // rewind 
                networkTick++;
            }
        }
    }

    public void refreshTurnUI()
    {
        Debug.Log("refresh Turn UI : " + EightBallGameSystem.Instance.UserCodeID + "===" + EightBallGameSystem.Instance.TurnCodeID);
        Debug.Log("refresh EightBallGameSystem.Instance.IsMyTurn : " + EightBallGameSystem.Instance.IsMyTurn);
        if (EightBallGameSystem.Instance.IsMyTurn)
        {
            //current turn
            if (shotPowerIndicator != null)
            {
                Debug.Log("refresh EightBallGameSystem.Instance.IsMyTurn : shotPowerIndicator");
                shotPowerIndicator.shotPowerCue.TweenMove.OnPlay();
                shotPowerIndicator.anim?.Play("MakeVisible");
            }
        }
        else
        {
            if (shotPowerIndicator != null)
            {
                Debug.Log("refresh EightBallGameSystem.Instance.IsMyTurn : shotPowerIndicator back");
                shotPowerIndicator.shotPowerCue.TweenMove.OnBack();
                shotPowerIndicator.anim?.Play("ShotPowerAnimation");

            }
        }
    }

    void OnUpdateBallMessage(NetworkPhysicsMessage msg)
    {
        // update ball base on other player
        if (!otherTurn) return;
        //status.text = $"Update ball at networkTick = {msg.tick} --- currentTick = {physicController.Tick}";
        networkDataQueue.Enqueue(msg);
        if (networkDataQueue.Count > 0)
            startNetworkSimulate = true;
    }
    uint totalMessage = 0;
    List<NetworkPhysicsMessage> networkPhysicsMessages = new List<NetworkPhysicsMessage>();
    void OnUpdateBallPath(NetworkPhysicsMessages message)
    {

        var listMessage = message.NetworkPhysicsMessage.ToList();
        if (listMessage.Count == 0)
        {
            Debug.Log($"SHPT OnUpdateBallPath msg count = 0");
            return;
        }        
        // Debug.Log($"shpt OnUpdateBallPath tick count = {message.NetworkPhysicsMessage.Count}, from  tick = {listMessage[0].Tick} to tick = {listMessage[listMessage.Count - 1].Tick}");
        networkPhysicsMessages.AddRange(listMessage);
        Debug.Log($"SHPT OnUpdateBallPath msg count = {networkPhysicsMessages.Count} ---- total = {totalMessage} ");
        if (networkPhysicsMessages.Count == totalMessage)
        {            
            physicController.StartSimulate(networkPhysicsMessages.ToList());
            totalMessage = 0;
            networkPhysicsMessages.Clear();
        }
        
        HideAllControllers();
    }
    void HandlePhysicsQMessage()
    {
        if (startNetworkSimulate && networkDataQueue.Count > 0)
        {
            var msg = networkDataQueue.Dequeue();
            HandleNetworkPhysicsMessage(msg);
        }
    }

    private void OnDisconnectedSocket(string errorMessage)
    {
        Debug.LogError("[GameController] : " + errorMessage);
        EightBallGameSystem.Instance.EndGame();
    }

    // public void Restart()
    // {
    //     EightBallNetworkManager.Instance.Disconnect();
    //     SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    // }
    
    #region Whiteball
    [HideInInspector]
    public bool whiteBallPlacement = true;
    public bool IsFirstShot = false;
    [SerializeField]
    private Bounds tableBound;

    public SpriteRenderer startBound;

    public void WhiteBallPlacement()
    {
        whiteBallPlacement = true;
        Debug.Log("WhiteBallPlacement()");
    }

    public (Vector3, Vector3) GetTableBound()
    {
        return (table.transform.position + tableBound.center, tableBound.size);
    }

    #endregion
    
    //public void ReSpawnBalls()  
    //{
    //    ballSpawner.ReSpawn();
    //    cueController.UpdateCuePos();
    //}

    private void OnDrawGizmos()
    {
        if (tableBound != null && table != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(table.transform.position + tableBound.center, tableBound.size);
        }
        if (startBound != null)
        {
            Gizmos.DrawWireCube(startBound.bounds.center, startBound.bounds.size);
        }

    }

}
