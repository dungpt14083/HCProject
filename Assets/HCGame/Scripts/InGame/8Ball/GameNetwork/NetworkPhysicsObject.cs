using HcGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPhysicsObject : MonoBehaviour
{
    Rigidbody rb;
    [HideInInspector]
    public GameObject body;
    public GameObject bodyPrefab;
    public PhysicMaterial potedMaterial;
    public PhysicMaterial normalMaterial;
    public bool useGlobalConfig = true;
    [SerializeField]
    private int delayFrames = 20;
    public int ballID;
    //public float potedRollForce = 1f;
    public Rigidbody Rigidbody { get { if (rb == null) rb = GetComponent<Rigidbody>(); return rb; } }
    Ball ball;
    public bool poted = false;
    public bool shouldUpdatePhysic = true;
    public bool shouldUpdateBody = true;
    public bool prePhaseUpdate = true;
    bool forceUpdatePos = false;
    int marker;
    int lastProcessFrame;
    public enum SOUND_FX
    {
        NONE,
        CUE_VS_BALL,
        BALL_VS_BALL
    }
    FakeShadowController shadowController;
    GameController gameController;
    public class ObjectInfo
    {
        public int currentFrame;
        public ObjectInfo(int frame, Vector3 position, Quaternion rotation)
        {
            currentFrame = frame;
            this.position = position;
            this.rotation = rotation;
            soundFX = SOUND_FX.NONE;
            soundVolume = 0f;
        }
        public Vector3 position;
        public Quaternion rotation;
        public SOUND_FX soundFX;
        public float soundVolume;
    }

    ObjectInfo[] qPos = new ObjectInfo[512];

    #region Trick collision dir
    //public Vector3 targetDir;
    //public int targetId;
    //public void SetTargetDirWhenCollision(Vector3 dir, int otherBallID)
    //{
    //    Debug.Log($"SHPT BALL = {ballID} SetTargetDirWhenCollision dir = {dir} - ball = {otherBallID}");
    //    targetDir = dir;
    //    targetId = otherBallID;
    //}
    #endregion
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball = GetComponent<Ball>();

        //body = Instantiate(bodyPrefab, rb.position, rb.rotation);
        //body.name = rb.name + "_body";
    }
    public void CreateBody(BallPhysicConfig config = null)
    {
        // init
        shouldUpdatePhysic = true;
        poted = false;
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (config != null && useGlobalConfig)
        {
            rb.mass = config.mass;
            rb.drag = config.drag;
            rb.angularDrag = config.angularDrag;
            rb.maxAngularVelocity = config.maxAngularVelocity;
            rb.collisionDetectionMode = config.collisionMode;
        }
        body = Instantiate(bodyPrefab, rb.position, rb.rotation);
        body.name = rb.name + "_body";
        shadowController = body.GetComponentInChildren<FakeShadowController>();
        gameController = GameController.Instance;
        gameController.physicController.OnFinishPrephase += FinishPrePhase;
        delayFrames = gameController.bodyDelayFrame;
    }
    private void OnDestroy()
    {
        if (gameController != null)
            gameController.physicController.OnFinishPrephase -= FinishPrePhase;
    }
    public void ForceUpdatePos(bool value)
    {
        forceUpdatePos = value;
        if (value == false)
            marker = Time.frameCount;
    }
    public void ResetRBState()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }
    public void ChangeLayer(string layerNme)
    {
        shadowController.gameObject.layer = LayerMask.NameToLayer(layerNme);
    }
    public void ToggleUpdateBody(bool shoudUpdate, bool clearQ = false)
    {
        if (clearQ)
            qPos = new ObjectInfo[512];
        shouldUpdateBody = shoudUpdate;
    }

    public void ResetState()
    {
        shouldUpdatePhysic = true;
        prePhaseUpdate = true;
        poted = false;
        rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        //rb.constraints = RigidbodyConstraints.FreezePositionZ;
        GetComponent<SphereCollider>().material = normalMaterial;
    }

    public void CreateBody(Texture texture, BallPhysicConfig config = null)
    {
        CreateBody(config);
        body.GetComponentInChildren<MeshRenderer>().material.mainTexture = texture;
    }

    void UpdateBody()
    {
        if (!shouldUpdateBody) return;
        if (forceUpdatePos)
        {
            body.transform.position = rb.position;
            body.transform.rotation = rb.rotation;
            return;
        }
        var physicsFrame = Time.frameCount - delayFrames;
        if (physicsFrame < marker || physicsFrame < lastProcessFrame)
            return;
        if (physicsFrame < 0) return;
        lastProcessFrame = physicsFrame;
        physicsFrame %= qPos.Length;
        var ballInfo = qPos[physicsFrame];
        if (ballInfo == null) return;
        //if (Vector3.Distance(body.transform.position, ballInfo.position) > 0.5f)
        //{
        //    Debug.Log($"UpdateBody ball ${transform.name}, frame = {ballInfo.currentFrame} - rb pos = {ballInfo.position} - transform pos = {body.transform.position}");
        //    Debug.Break();
        //}
        body.transform.position = ballInfo.position;
        body.transform.rotation = ballInfo.rotation;
        if (ballInfo.soundFX == SOUND_FX.BALL_VS_BALL)
        {
            ball.SoundBallVsBall(ballInfo.soundVolume);
        }
        if (ballInfo.soundFX == SOUND_FX.CUE_VS_BALL)
        {
            ball.SoundCueBall(ballInfo.soundVolume);
        }
        if (qPos[physicsFrame].soundFX != SOUND_FX.NONE)
        {
            qPos[physicsFrame].soundFX = SOUND_FX.NONE;
        }
    }

    public ObjectInfo GetObjectInfo()
    {
        var index = Time.frameCount % qPos.Length;
        return qPos[index];
    }


    public void UpdateQData(int frame, Vector3 position, Quaternion rot)
    {
        var index = frame % qPos.Length;
        if (qPos[index] == null) return;
        qPos[index].position = position;
        qPos[index].rotation = rot;

    }
    public void ForceSetPos(Vector3 posNew)
    {
        transform.position = posNew;
        body.transform.position = posNew;

    }
    // Update is called once per frame
    void Update()
    {
        if (rb == null) return;
        if (!shouldUpdateBody) return;
        if (gameController == null) return;
        //if (!gameController.physicController.IsPrePhase && gameController.physicController.OverrideId > 0 && Rigidbody.velocity != Vector3.zero && gameController.physicController.OverrideId == ballID)
        //{
        //    Debug.Log($"SHPT ballID = {ballID}, before vel = {Rigidbody.velocity}");
        //    var velMagnitude = Rigidbody.velocity.magnitude;
        //    Rigidbody.velocity = gameController.physicController.OverrideDir * velMagnitude;
        //    gameController.physicController.ClearOverride();
        //    Debug.Log($"SHPT otherballid = {ballID}, after vel = {Rigidbody.velocity}");
        //}
        var index = Time.frameCount % qPos.Length;
        if (gameController.physicController.IsPrePhase || Time.frameCount <= checkPointFrame + 5)
            return;
        if (qPos[index] == null)
            qPos[index] = new ObjectInfo(Time.frameCount, rb.transform.position, rb.transform.rotation);
        else
        {
            qPos[index].position = rb.transform.position;
            qPos[index].rotation = rb.transform.rotation;
        }
        //Debug.Log($"Update body ball = {transform.name} at frame = {Time.frameCount} - rb pos = {rb.transform.position}");
        UpdateBody();
    }

    public void ScheduleSoundFX(SOUND_FX sfx, float volume)
    {
        var index = Time.frameCount % qPos.Length;
        if (qPos[index] == null)
            qPos[index] = new ObjectInfo(Time.frameCount, rb.transform.position, rb.transform.rotation);
        qPos[index].soundFX = sfx;
        qPos[index].soundVolume = volume;
    }
    //bong vao lo
    void OnPoted(string potID)
    {
        GetComponent<SphereCollider>().material = potedMaterial;               
        rb.constraints = RigidbodyConstraints.None;
        var vel = rb.velocity;
        //vel.x = 0;
        //vel.y = 0;
        vel.z = 9f;
        rb.velocity = vel;
        shouldUpdatePhysic = false;
        if (gameController.physicController.IsPrePhase) return;
        if(gameController.tutorialController.ballIdOnpoted < 0) gameController.tutorialController.ballIdOnpoted = ballID;
        poted = true;
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
        {
            //if (ballID != EightBallGameSystem.WHITE_BALL_ID)
            //{
            //    GameController.Instance.tutorialController.ShowGoodLuck();
            //}
        }
        if (ballID == EightBallGameSystem.WHITE_BALL_ID)
        {
            WhiteBallPlacement();
        }
        else
            Invoke(nameof(MoveToRoller), 2f);

        //Debug.Log("shpt on poted id : " + potID);
        if (!string.IsNullOrEmpty(potID))
        {
            if (int.TryParse(potID, out var index))
            {
                index = index - 1;
                EightBallGameSystem.Instance.BallOnPot(ballID, index);
                Debug.Log("on ball poted : " + ballID + "===" + index);
            }
        }

    }
    public void ClearBall()
    {
        shadowController.ClearFakeShadow();
        Destroy(shadowController);
        Destroy(body);
        if(this.ballID != EightBallGameSystem.WHITE_BALL_ID)
        {
            Destroy(this.gameObject);
        }
    }
    void MoveToRoller()
    {
        rb.gameObject.SetActive(false);
        body.gameObject.SetActive(false);
        shadowController.DisableShadow();
        // rb.position = GameController.Instance.rollPotedAnchor.position;
        // var vel = Mathf.Clamp(rb.velocity.magnitude, 1f, 2.5f);
        // rb.velocity = Vector3.right * vel;
        // Debug.Log($"MoveToRoller vel = {rb.velocity.magnitude}");
    }

    void WhiteBallPlacement()
    {
        GameController.Instance.WhiteBallPlacement();
    }
    int checkPointFrame;
    void FinishPrePhase(int targetFrame, List<NetworkPhysicsMessage> physicsMessagesLocal)
    {
        checkPointFrame = targetFrame;
    }

    public bool pauseOnTouch;
    public int IgnoreSoundAtFrame;
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("ball collision with : " + ballID + "===" + collision.transform.name);
        if (gameController.physicController.IsPrePhase)
        {
            if (collision.transform.CompareTag("bumper") || collision.transform.CompareTag("Ball"))
            {
                int otherBallId = 0;
                if (collision.gameObject.TryGetComponent<NetworkPhysicsObject>(out var otherBall))
                {
                    otherBallId = otherBall.ballID;
                }

                gameController.physicController.ClearOverride(ballID, otherBallId);
            }
            return;
        }
        if (!poted)
        {
            if (collision.gameObject.TryGetComponent<NetworkPhysicsObject>(out var otherBall))
            {
#if UNITY_EDITOR
                if (this.ballID == EightBallGameSystem.WHITE_BALL_ID && pauseOnTouch)
                    UnityEditor.EditorApplication.isPaused = true;
#endif
                var vel = Mathf.Min(rb.velocity.sqrMagnitude, otherBall.Rigidbody.velocity.sqrMagnitude);
                vel *= 0.5f;
                if (IgnoreSoundAtFrame < Time.frameCount)
                {
                    ScheduleSoundFX(SOUND_FX.BALL_VS_BALL, vel);
                    otherBall.IgnoreSoundAtFrame = Time.frameCount;
                }
                EightBallGameSystem.Instance.BallCollision(this, otherBall);
                //if (targetId > 0 && this.ballID == 49 && otherBall.ballID == targetId)
                //{
                //    Debug.Log($"SHPT whiteball set dir = {targetDir} for ballid = {targetId}");
                //    otherBall.SetTargetDirWhenCollision(targetDir, targetId);
                //    targetDir = Vector3.zero;
                //    targetId = 0;
                //}
            }
            else
            {
                if (collision.transform.CompareTag("bumper"))
                {
                    EightBallGameSystem.Instance.BallCollision(this, null);
                }
            }
        }
        // if (!poted && collision.gameObject.TryGetComponent<NetworkPhysicsObject>(out var otherBall))
        // {
        //     var vel = Mathf.Min(rb.velocity.sqrMagnitude, otherBall.Rigidbody.velocity.sqrMagnitude);
        //     vel *= 0.5f;
        //     ScheduleSoundFX(SOUND_FX.BALL_VS_BALL, vel);
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rb.velocity.magnitude > 2f)
        {
            rb.velocity = rb.velocity.normalized;
        }

        if (!poted && other.tag.Contains("Pot"))
        {
            //var distance = Vector3.Distance(other.transform.position, this.transform.position);
            //Debug.Log("OnTriggerEnter Pot distance = " + distance);
            //if (distance >= other.GetComponent<SphereCollider>().radius) return;
            if (gameController.physicController.IsPrePhase)
            {
                gameController.physicController.ClearOverride(ballID, 0);
            }
            string indexStr = other.tag.Replace("Pot", "");
            OnPoted(indexStr);
        }
    }

}
