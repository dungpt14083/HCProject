using HcGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicController : MonoBehaviour
{

    public int sampleFrameRate = 1;
    private float timer;
    private int[] tick2Frames = new int[1024];
    private bool shouldSimulate = false;
    public float MINIMUM_MOVEMENT_SQR = 0.005f;
    bool isMoving = false;
    public bool IsMovement => isMoving;

    public Action<int, bool> OnSample;
    public Action<int> BeforeProcess;
    public Action<int, List<NetworkPhysicsMessage>> OnFinishPrephase;

    bool stateChange;
    int tickFromHit = 0;
    public int Tick => tickFromHit;
    public int EndMovementFrame { get; private set; }
    public NetworkPhysicsObject[] objects;
    Dictionary<int, NetworkPhysicsObject> objectById = new Dictionary<int, NetworkPhysicsObject>();

    public Action OnStartSimulationCallback;
    public Action OnStopSimulationCallback;
    public bool IsPrePhase { get; private set; }
    public int EndPrePhaseFrame { get; private set; }
    public void InitData(NetworkPhysicsObject[] rbs)
    {
        this.objects = rbs;
        objectById.Clear();
        isMoving = false;
        timer = 0;
        foreach (var obj in objects)
        {
            objectById.Add(obj.ballID, obj);
        }
    }
    public void StartSimulate(List<NetworkPhysicsMessage> remoteSteps = null)
    {
        StartCoroutine(StartSimulateInternalSteps(remoteSteps));

    }
   
    IEnumerator StartSimulateInternalSteps(List<NetworkPhysicsMessage> remoteSteps = null)
    {
        IsPrePhase = true;
        isMoving = true;
        if (remoteSteps != null)
        {
            localPhysicSteps.Clear();
            foreach (var remoteStep in remoteSteps)
            {
                if (localPhysicSteps.ContainsKey(remoteStep.Tick))
                {
                    Debug.Log($"SHPT StartSimulateInternalSteps with duplicate key = {remoteStep.Tick}");
                    localPhysicSteps[remoteStep.Tick] = remoteStep;
                }
                else
                    localPhysicSteps.Add(remoteStep.Tick, remoteStep);
            }
            Debug.Log($"shpt Frame = {Time.frameCount} StartSimulateInternalSteps localPhysicSteps count = {localPhysicSteps.Count}");
        }
        else
        {
            yield return StartCoroutine(StartPreCalculateSteps());
        }
        shouldSimulate = true;
        IsPrePhase = false;
        OnStartSimulationCallback?.Invoke();
        //Debug.Break();
    }

    public void EndSimulate()
    {
        Debug.Log("end simulate");
        shouldSimulate = false;
        OnStopSimulationCallback?.Invoke();
        if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
        {
            GameController.Instance.tutorialController.EndShoot();
        }
    }

    public int GetFrameFromTick(int tick)
    {
        return tick2Frames[tick % tick2Frames.Length];
    }

    void Sample(int tick, bool isFinalStage = false)
    {
        OnSample?.Invoke(tick, isFinalStage);
    }

    public bool IsObjectMoving(Rigidbody rb)
    {
        return rb.velocity.sqrMagnitude > MINIMUM_MOVEMENT_SQR;
    }
    //public void UpdatePhysics()
    //{
    //    // Debug.Log("timer : " + timer +"===" + Physics.autoSimulation);

    //    if (Physics.autoSimulation)
    //        return; // do nothing if the automatic simulation is enabled
    //    timer += Time.deltaTime;
    //    // Catch up with the game time.
    //    // Advance the physics simulation in portions of Time.fixedDeltaTime
    //    // Note that generally, we don't want to pass variable delta to Simulate as that leads to unstable results.
    //    while (timer >= Time.fixedDeltaTime)
    //    {
    //        timer -= Time.fixedDeltaTime;
    //        if (shouldSimulate)
    //        {
    //            // Debug.Log("begin simulate : " + tickFromHit);
    //            BeforeProcess?.Invoke(tickFromHit);
    //        }
    //        Physics.Simulate(Time.fixedDeltaTime);
    //        bool moving = false;
    //        foreach (var obj in objects)
    //        {
    //            if (!obj.shouldUpdatePhysic)
    //                continue;
    //            if (obj.Rigidbody.velocity.sqrMagnitude > MINIMUM_MOVEMENT_SQR)
    //            {
    //                moving = true;
    //                break;
    //            }
    //            obj.Rigidbody.velocity = Vector3.zero;
    //            obj.Rigidbody.angularVelocity = Vector3.zero;
    //        }
    //        stateChange = moving != isMoving;
    //        //Debug.Log($"moving = {moving} --- ismoving = {isMoving} ---- statechanged = {stateChange}");
    //        isMoving = moving;
    //        if (isMoving)
    //        {
    //            if (tickFromHit % sampleFrameRate == 0)
    //            {
    //                Sample(tickFromHit);
    //            }
    //            tickFromHit++;
    //            tick2Frames[tickFromHit % tick2Frames.Length] = Time.frameCount;
    //        }
    //        if (stateChange && !moving)
    //        {
    //            Sample(tickFromHit, isFinalStage: true);
    //            Debug.Log($"tick from hit = {tickFromHit}");
    //            tickFromHit = 0;
    //            EndSimulate();
    //        }
    //    }
    //}

    //public void UpdatePhysicsV2()
    //{
    //    if (Physics.autoSimulation)
    //        return; // do nothing if the automatic simulation is enabled
    //    timer += Time.deltaTime;
    //    // Catch up with the game time.
    //    // Advance the physics simulation in portions of Time.fixedDeltaTime
    //    // Note that generally, we don't want to pass variable delta to Simulate as that leads to unstable results.
    //    while (timer >= Time.fixedDeltaTime)
    //    {
    //        timer -= Time.fixedDeltaTime;
    //        //if (shouldSimulate)
    //        //{
    //        //    // Debug.Log("begin simulate : " + tickFromHit);
    //        //    BeforeProcess?.Invoke(tickFromHit);
    //        //}
    //        if (IsPrePhase) return;
    //        foreach (var obj in objects)
    //        {
    //            if (OverrideId > 0 && obj.ballID == OverrideId)
    //            {
    //                var objVel = obj.Rigidbody.velocity.magnitude;
    //                if (objVel > MAGNITUDE_THRESHOLD)
    //                {
    //                    //Debug.Break();
    //                    obj.Rigidbody.velocity = OverrideDir * objVel;
    //                    OverrideId = -1;
    //                    Debug.Log($"SHPT prephase override for ball {obj.ballID} at tick = {tickFromHit}");
    //                }
    //            }
    //        }
    //        Physics.Simulate(Time.fixedDeltaTime);
    //        if (localPhysicSteps.TryGetValue(tickFromHit, out var data))
    //        {
    //            //Debug.Log($"UpdatePhysicsV2 localPhysicSteps at frame = {Time.frameCount} - tick = {tickFromHit}");
    //            foreach (var prePhaseObj in data.NetworkObjects)
    //            {
    //                if (objectById.TryGetValue(prePhaseObj.Id, out var localObject))
    //                {
    //                    //var prePhasePos = prePhaseObj.Position.ToVector3();
    //                    //var diffDistance = Vector3.Distance(localObject.Rigidbody.position, prePhasePos);
    //                    //if (diffDistance > 0.1f)
    //                    //{
    //                    //    Debug.Log($"SHPT Resync PrePhase {diffDistance} at tick = {tickFromHit} ball = {prePhaseObj.Id}");
    //                    //}
    //                    localObject.Rigidbody.position = prePhaseObj.Position.ToVector3();
    //                    localObject.Rigidbody.rotation = prePhaseObj.Rotation.ToQuaternion();
    //                    localObject.Rigidbody.angularVelocity = prePhaseObj.AngularVelocity.ToVector3();
    //                    localObject.Rigidbody.velocity = prePhaseObj.Velocity.ToVector3();

    //                }
    //            }
    //            //localPhysicSteps.Remove(tickFromHit);
    //        }
    //        bool moving = false;
    //        foreach (var obj in objects)
    //        {
    //            if (!obj.shouldUpdatePhysic)
    //                continue;
    //            if (obj.Rigidbody.velocity.sqrMagnitude > MINIMUM_MOVEMENT_SQR)
    //            {
    //                moving = true;
    //                break;
    //            }
    //            obj.Rigidbody.velocity = Vector3.zero;
    //            obj.Rigidbody.angularVelocity = Vector3.zero;
    //        }
    //        // Debug.Log($"UpdatePhysicsV2 after simulate at frame = {Time.frameCount}");
    //        stateChange = moving != isMoving;
    //        //Debug.Log($"moving = {moving} --- ismoving = {isMoving} ---- statechanged = {stateChange}");
    //        isMoving = moving;
    //        if (isMoving)
    //        {
    //            if (tickFromHit % sampleFrameRate == 0)
    //            {
    //                //Sample(tickFromHit);
    //            }
    //            tickFromHit++;
    //            tick2Frames[tickFromHit % tick2Frames.Length] = Time.frameCount;
    //        }
    //        if (stateChange && !moving)
    //        {
    //            //Sample(tickFromHit, isFinalStage: true);
    //            Debug.Log($"tick from hit = {tickFromHit}");
    //            tickFromHit = 0;
    //            localPhysicSteps.Clear();
    //            EndSimulate();
    //        }

    //    }
    //}
    int tickCountTemp = 0;
    public void UpdatePhysicsV3()
    {
        if (Physics.autoSimulation)
            return; // do nothing if the automatic simulation is enabled
        timer += Time.deltaTime;
        // Catch up with the game time.
        // Advance the physics simulation in portions of Time.fixedDeltaTime
        // Note that generally, we don't want to pass variable delta to Simulate as that leads to unstable results.
        while (timer >= Time.fixedDeltaTime)
        {
            timer -= Time.fixedDeltaTime;
            Physics.Simulate(Time.fixedDeltaTime);
            Debug.Log("localPhysicSteps.Count " + localPhysicSteps.Count);
            Debug.Log("IsPrePhase " + IsPrePhase);
            if (IsPrePhase || localPhysicSteps.Count == 0) return;
            if (localPhysicSteps.TryGetValue(tickFromHit, out var data))
            {
                tickCountTemp++;
                //Debug.Break();
                //Debug.Log($"shpt UpdatePhysicsV3 tick = {tickFromHit}");
                foreach (var prePhaseObj in data.NetworkObjects)
                {
                    if (objectById.TryGetValue(prePhaseObj.Id, out var localObject))
                    {
                        localObject.Rigidbody.position = prePhaseObj.Position.ToVector3();
                        localObject.Rigidbody.rotation = prePhaseObj.Rotation.ToQuaternion();
                        localObject.Rigidbody.angularVelocity = Vector3.zero;
                        localObject.Rigidbody.velocity = Vector3.zero;
                        //if (prePhaseObj.Id == 49)
                        //{
                        //    Debug.Log($"Whiteball id = {objectById[49].Rigidbody.GetInstanceID()} pos = {objectById[49].Rigidbody.position} tick = {tickFromHit}");
                        //}
                    }
                }
                //localPhysicSteps.Remove(tickFromHit);
                tickFromHit++;
                tick2Frames[tickFromHit % tick2Frames.Length] = Time.frameCount;
                if (tickFromHit == localPhysicSteps.Count)
                {
                    EndMovementFrame = Time.frameCount;
                    Debug.Log($"shpt Apply total tick = {tickCountTemp}");
                    tickCountTemp = 0;
                    // UnityEditor.EditorApplication.isPaused = true;
                    isMoving = false;
                    tickFromHit = 0;
                    localPhysicSteps.Clear();
                    EndSimulate();
                    //Debug.Log($"Whiteball id = {objectById[49].Rigidbody.GetInstanceID()} pos = {objectById[49].Rigidbody.position} end");
                    // Debug.Break();
                }
            }
            else
            {
                Debug.Log($"SHPT missing tick = {tickFromHit}");
            }
        }
    }


    public Dictionary<int, NetworkPhysicsMessage> localPhysicSteps = new Dictionary<int, NetworkPhysicsMessage>();
    Dictionary<int, CacheObjectData> cachedBallPrephaseData = new Dictionary<int, CacheObjectData>();
    struct CacheObjectData
    {
        public int ballId;
        public Vector3 position;
        public Quaternion rotation;
    }
    List<NetworkPhysicsMessage> networkPhysicSteps = new List<NetworkPhysicsMessage>();
    
    public int PrecalculateStepCount = 100;
    const float MAGNITUDE_THRESHOLD = 0.2f;
    IEnumerator StartPreCalculateSteps()
    {
        localPhysicSteps.Clear();
        networkPhysicSteps.Clear();
        bool shoudPreSimulate = true;
        int prePhaseTick = 0;
        var prePhaseOverrideId = OverrideId;

        foreach (var obj in objects)
        {
            cachedBallPrephaseData.Add(obj.ballID, new CacheObjectData() { ballId = obj.ballID, position = obj.Rigidbody.position, rotation = obj.Rigidbody.rotation });
            obj.ToggleUpdateBody(false);
        }
        while (shoudPreSimulate)
        {
            if (OverrideId > 0)
            {
                if (objectById.TryGetValue(OverrideId, out var ball))
                {
                    var objVel = ball.Rigidbody.velocity.magnitude;
                    if (objVel > MAGNITUDE_THRESHOLD)
                    {
                        //Debug.Log($"SHPT Apply Override for {obj.ballID}");
                        ball.Rigidbody.velocity = OverrideDir * objVel;
                        //prePhaseOverrideId = -1;
                    }
                }
                var distance = Vector3.Distance(startPoint, objects[0].Rigidbody.position);
                if (isPredictExecuted == false && distance - hitDistance >= 0.01f)
                {
                    //Debug.Break();
                    objects[0].Rigidbody.position = hitPosition;
                    Debug.Log($"SHPT Apply Override collision position ");
                    isPredictExecuted = true;
                }
            }
            Physics.Simulate(Time.fixedDeltaTime);
            bool moving = false;
            foreach (var obj in objects)
            {
                if (obj.Rigidbody.position.z >= 0.3f)
                    obj.prePhaseUpdate = false;
                if (!obj.shouldUpdatePhysic || !obj.prePhaseUpdate)
                    continue;
                if (obj.Rigidbody.velocity.sqrMagnitude > MINIMUM_MOVEMENT_SQR)
                {
                    moving = true;
                    break;
                }
                obj.Rigidbody.velocity = Vector3.zero;
                obj.Rigidbody.angularVelocity = Vector3.zero;
            }
            shoudPreSimulate = !(prePhaseTick > 0 && moving == false);
            if (prePhaseTick % sampleFrameRate == 0 || !shoudPreSimulate)
            {
                StoreTickData(prePhaseTick, !shoudPreSimulate);
            }
            prePhaseTick++;
            if (prePhaseTick % PrecalculateStepCount == 0)
                yield return null;

        }
        foreach (var cachedData in cachedBallPrephaseData)
        {
            if (objectById.TryGetValue(cachedData.Key, out var localObject))
            {
                localObject.Rigidbody.position = cachedData.Value.position;
                localObject.Rigidbody.rotation = cachedData.Value.rotation;
                localObject.Rigidbody.velocity = Vector3.zero;
                localObject.Rigidbody.angularVelocity = Vector3.zero;
                localObject.ToggleUpdateBody(true, true);
            }
        }
        yield return null;
        cachedBallPrephaseData.Clear();
        //Debug.Break();
        Debug.Log($"SHPT Prephase tick count = {localPhysicSteps.Count} end at frame = {Time.frameCount}");
        EndPrePhaseFrame = Time.frameCount;
        OnFinishPrephase?.Invoke(Time.frameCount, networkPhysicSteps);
    }
    Dictionary<int, NetworkObjectPhysicData> lastTickObjects = new Dictionary<int, NetworkObjectPhysicData>();
    void StoreTickData(int prePhaseTick, bool isFinal)
    {
        lastTickObjects.Clear();
        var lastTick = prePhaseTick - 1;
        if (lastTick >= 0)
        {
            foreach (var obj in localPhysicSteps[lastTick].NetworkObjects)
            {
                lastTickObjects.Add(obj.Id, obj);
            }
        }
        List<NetworkObjectPhysicData> physicData = new List<NetworkObjectPhysicData>();
        foreach (var obj in objects)
        {
            // ignore object poted 
            if (!obj.shouldUpdatePhysic || !obj.prePhaseUpdate)
                continue;
            // if final tick or first tick then always store data
            if (isFinal || lastTickObjects.Count == 0)
            {
                physicData.Add(obj.ToNetworkPhysicData());
            }
            else if (lastTickObjects.Count > 0)
            // check if position between last tick and current tick is diff then store data                
            {
                if (lastTickObjects.TryGetValue(obj.ballID, out var pData))
                {
                    if (pData.Position.ToVector3() != obj.Rigidbody.position)
                    {
                        physicData.Add(obj.ToNetworkPhysicData());
                    }
                }
                else
                {
                    physicData.Add(obj.ToNetworkPhysicData());
                }
            }
        }
        NetworkPhysicsMessage message = new NetworkPhysicsMessage();
        message.Tick = prePhaseTick;
        message.NetworkObjects.AddRange(physicData.ToArray());
        localPhysicSteps.Add(prePhaseTick, message);
        networkPhysicSteps.Add(message);
        //Debug.Log($"Sample at tick = {prePhaseTick}, frameCount = {Time.frameCount} ");
    }


    #region  override collision direction
    public Vector3 OverrideDir { get; private set; }
    public int OverrideId { get; private set; }
    Vector3 startPoint;
    float hitDistance;
    Vector3 hitPosition;
    bool isPredictExecuted = false;
    bool isWhiteBallCollisionWithTarget = false;
    public void SetOverrideData(CueController.PredictCollisionData data)
    {
        Debug.Log($"SHPT SetOverrideData for {data.targetBallId}");
        OverrideDir = data.targetDir;
        OverrideId = data.targetBallId;
        this.startPoint = data.statPosition;
        this.hitDistance = data.hitDistance;
        hitPosition = data.hitPosition;
        isPredictExecuted = false;
        isWhiteBallCollisionWithTarget = false;
    }

    public void ClearOverride(int ballId, int otherBallId, bool forceClear = false)
    {
        if (OverrideId == -1) return;
        if (!isWhiteBallCollisionWithTarget)
            isWhiteBallCollisionWithTarget = (ballId == EightBallGameSystem.WHITE_BALL_ID && otherBallId == OverrideId) || (ballId == OverrideId && otherBallId == EightBallGameSystem.WHITE_BALL_ID);
        // target ball collision with bumper or other ball (not white ball)
        if (ballId == OverrideId && otherBallId != EightBallGameSystem.WHITE_BALL_ID)
        {
            Debug.Log($"ClearOverride ballid = {ballId} -- otherball = {otherBallId}");
            // magic number :(
            OverrideId = -1;
            startPoint = Vector3.down;
            hitDistance = -999;  // magic number :(
            return;
        }
        // white ball collision with bumper or other ball (not target ball)
        if (!isWhiteBallCollisionWithTarget && ballId == EightBallGameSystem.WHITE_BALL_ID && otherBallId != OverrideId)
        {
            Debug.Log($"ClearOverride ballid = {ballId} -- otherball = {otherBallId}");
            OverrideId = -1;
            startPoint = Vector3.down;
            hitDistance = -999;
            return;
        }

    }
    #endregion
}
