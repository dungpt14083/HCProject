using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Tweens.Plugins;

public class CueController : MonoBehaviour
{
    public GameObject cue;
    public GameObject posDetector;

    private Rigidbody myRigidbody;
    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;
    public ShotPowerScript ShotPowerIndicator;
    public GameObject targetLine;
    public GameObject circle;
    private RaycastHit hitInfo = new RaycastHit();
    private Ray ray = new Ray();
    public LayerMask layerMask;
    private Vector3 endPosition;
    [SerializeField]
    private float radius;
    [SerializeField]
    private bool displayTouched = false;
    [SerializeField]
    private bool isFirstTouch = false;
    public int steps = 0;
    private bool firstShotDone = false;
    private bool canCount;
    public bool ballsInMovement = false;
    private ArrayList multiplayerDataArray = new ArrayList();
    private bool startMovement;
    private int counterFixPositionMulti = 0;
    private Vector3 multiFirstShotPower;
    private Vector3 multiFirstShotDirection;
    private Vector3 circleShotPos = Vector3.zero;
    private Vector3 initShotPos = Vector3.zero;
    private bool firstShot = true;
    int updateCount = 0;
    private Vector3 shotDirection;
    private bool ballCollideFirst;
    public Vector3 trickShotAdd = Vector3.zero;
    public bool spinShowed;
    private bool raisedSixEvent = false;
    private bool movingWhiteBall = false;
    private bool canShowControllers = true;
    private AudioSource[] audioSources;
    public GameObject wrongBall;
    private Vector3 touchMousePos;
    public Renderer cueRenderer;
    // new property
    public float DefaultLineLength = 2f;
    [Range(0, 2)]
    public float BallChekRadiusPercent = 0.9f;
    //
    public bool IsShowLine { get; set; } = true;

    void Start()
    {
        //draw lines
        //circle = GameObject.Find("Circle");
        //targetLine = GameObject.Find("TargetLine");
        //lineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
        //lineRenderer2 = GameObject.Find("Line2").GetComponent<LineRenderer>();
        //lineRenderer3 = GameObject.Find("Line3").GetComponent<LineRenderer>();
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        //cueRenderer = cue.GetComponent<Renderer>();
        RefreshCuePos();
        //play audio
        // GameManager.Instance.audioSources[0].Play();
    }




    private void RefreshCuePos()
    {
        // Set cue position to ball position
        Vector3 cueInitialPos = transform.position;
        //cueInitialPos.x = cueInitialPos.x;
        cueInitialPos.z = cue.transform.position.z;
        cue.transform.position = cueInitialPos;
    }

    /// <summary>
    /// Callback sent to all game objects when the player pauses.
    /// </summary>
    /// <param name="pauseStatus">The pause state of the application.</param>
    void OnApplicationPause(bool pauseStatus)
    {
        // if (pauseStatus) {
        //     PhotonNetwork.RaiseEvent(151, 1, true, null);
        //
        //     PhotonNetwork.SendOutgoingCommands();
        //     Debug.Log("Application pause");
        // } else {
        //     PhotonNetwork.RaiseEvent(152, 1, true, null);
        //     PhotonNetwork.SendOutgoingCommands();
        //     Debug.Log("Application resume");
        // }
    }



    private float touchTime = 0.0f;
    void Update()
    {
        //Nếu click ugui thì line di chuyển hướng banh sẽ ko hoạt động
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        //Debug.Log("EightBallGameSystem.Instance.IsCanControl " + EightBallGameSystem.Instance.IsCanControl);
        if (!EightBallGameSystem.Instance.IsCanControl)
        {
            return;
        }
        if (GameController.Instance.isEndGame || !GameController.Instance.isStartGame) return;
        //Debug.Log("spinShowed "+ spinShowed);
        //Debug.Log("canCheckAnotherCueRotation " + canCheckAnotherCueRotation);
        if (!spinShowed && canCheckAnotherCueRotation)
        {
            //StartCoroutine(rotateCue());
            rotateCue();
        }
        
        drawTargetLines();

        if (Input.GetMouseButtonDown(0))
        {
            isFirstTouch = true;
            displayTouched = true;
            touchMousePos = Vector3.zero;
            canCheckAnotherCueRotation = true;
            Debug.Log("touch time GetMouseButtonDown ");
            if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
            {
                GameController.Instance.tutorialController.CompleteStep1();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("touch time GetMouseButtonUp spinShowed: " + spinShowed);
            if (spinShowed) return;

            if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice && !GameController.Instance.isEndGame)
            {
                GameController.Instance.tutorialController.ShowStep2();
            }

            displayTouched = false;
            Debug.Log("touch time GetMouseButtonUp: " + touchTime);
            if (touchTime <= 0.2f)
            {
                //touch to rotate
                touchToRotate();
            }

            touchTime = 0.0f;
            //touchToRotate();
        }

        if (displayTouched)
        {
            Debug.Log("displayTouched touchTime " + touchTime);
            Debug.Log("displayTouched Time.deltaTime " + Time.deltaTime);
            touchTime += Time.deltaTime;
            Debug.Log("displayTouched touchTime " + touchTime);
        }
    }

    void FixedUpdate()
    {
        checkFirstCollision();
    }


    //TODO: check collision later
    private void checkFirstCollision()
    {
        //have we moved more than our minimum extent? 

        if (ballCollideFirst && gameObject.layer == 11)
        {
            RaycastHit hitInfo;


            if (Physics.SphereCast(initShotPos, radius, shotDirection, out hitInfo, Vector3.Distance(initShotPos, transform.position), layerMask.value))
            {

                if (!hitInfo.transform.tag.Equals(transform.tag))
                {

                    if (!hitInfo.collider)
                        return;

                    if (hitInfo.collider.isTrigger)
                    {
                        //hitInfo.collider.SendMessage ("OnTriggerEnter", myCollider);
                    }

                    if (!hitInfo.collider.isTrigger)
                    {
                        Debug.Log("fix pos");
                        Vector3 fixedPos = circleShotPos;
                        fixedPos.z = transform.position.z;
                        myRigidbody.transform.position = fixedPos;
                        gameObject.layer = 8;
                    }

                }
            }
        }
    }

    void OnDestroy()
    {
    }

    List<String> collides = new List<String>();
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag.Contains("Ball"))
            collides.Add(other.tag);
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Ball"))
            collides.Remove(other.tag);
    }

    
    public void UpdateCuePos()
    {
        cue.transform.position = new Vector3(GameController.Instance.whiteBall.transform.position.x,
            GameController.Instance.whiteBall.transform.position.y, cue.transform.position.z);
        drawTargetLines();
    }



    private bool canCheckAnotherCueRotation = true;


    private void touchToRotate()
    {
        //touch to rotate
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("touchToRotate 777777777777");
        if (Physics.Raycast(pos, Vector3.forward))
        {
            Debug.Log("touchToRotate 88888888888");
            pos.z = transform.position.z;
            var dir = pos - transform.position;
            cue.transform.right = -dir;
            if (lastRot != cue.transform.rotation)
            {
                lastRot = cue.transform.rotation;
                if(EightBallGameSystem.Instance.CurrentMode != PlayMode.Practice) GameController.Instance.UpdateCueRotation(lastRot);
                //if (EightBallGameSystem.Instance.IsMyTurn) GameController.Instance.UpdateCueRotation(lastRot);
                Debug.Log("touchToRotate 9999999999999999");
            }
            
            Debug.Log("touch to rotate : EightBallGameSystem.Instance.IsMyTurn " + pos);
            Debug.Log("EightBallGameSystem.Instance.IsMyTurn " + EightBallGameSystem.Instance.IsMyTurn);

        }
    }

    private Quaternion lastRot;
    private /*IEnumerator*/ void rotateCue()
    {
        //ROTATE CUE
        canCheckAnotherCueRotation = false;
        //Debug.Log("CueController 11111111111");
        if (!movingWhiteBall && !ballsInMovement)
        {
            float ang = 0;
            //Quaternion initRot = cue.transform.rotation;
            //Debug.Log("CueController 222222222222");
            Quaternion rot = cue.transform.rotation;
            if (!ShotPowerIndicator.mouseDown && displayTouched & !isFirstTouch)
            {
                //Debug.Log("CueController 3333333333333333");
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Mouse Y")))
                {
                    ang = Input.GetAxis("Mouse X");
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > transform.position.y)
                    {
                        ang = -ang;
                    }
                }
                else
                {
                    ang = Input.GetAxis("Mouse Y");
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
                    {
                        ang = -ang;
                    }
                }

                float multAng = Vector2.Distance(touchMousePos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                multAng *= 300.0f;

                if (multAng < 1.5f) multAng = 1.5f;


                multAng = multAng * 0.05f;


                if (multAng > 6.0f) multAng = 6.0f;

                ang *= multAng;
                touchMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                isFirstTouch = false;
               
            }

            //yield return new WaitForSeconds(0.01f);
            // Debug.Log("rotate cue : " + rot);
            //TODO: temp check rotation
            if (displayTouched && EightBallGameSystem.Instance.IsMyTurn)
            {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z + ang);
                cue.transform.rotation = rot;
                //Debug.Log("CueController 444444444444");
                //temp 
                // cue.transform.LookAt(touchMousePos);

                if (lastRot != rot)
                {
                    GameController.Instance.UpdateCueRotation(rot);
                    lastRot = rot;
                }
            }
            canCheckAnotherCueRotation = true;
            //drawTargetLines();
        }
        else
        {
            canCheckAnotherCueRotation = true;
        }
    }

    public void UpdateCueRot(Quaternion quaternion)
    {
        Debug.Log("CueController 5555555555555");
        cue.transform.rotation = quaternion;
        drawTargetLines();
    }


    // DRAW TARGET LINES
    private void drawTargetLines()
    {
        Debug.Log("drawTargetLines " + GameController.Instance.IsBallsMoving);
        if (GameController.Instance.IsBallsMoving)
        {
            return;
        }
        //cue.GetComponent<Renderer>().enabled = true;
        //if(!targetLine.activeSelf) targetLine.SetActive(true);
        Vector3 dir = transform.position - posDetector.transform.position;
        dir.z = 0;
        dir = dir.normalized;

        Vector3 linePos = transform.position;
        linePos.z = -3;

        lineRenderer.SetPosition(0, linePos);

        ray.origin = transform.position;
        ray.direction = dir;

        if (Physics.SphereCast(ray, radius * BallChekRadiusPercent, out hitInfo, 10, layerMask))
        {
            endPosition = ray.origin + (ray.direction.normalized * hitInfo.distance);
            endPosition.z = -3;
            circle.transform.position = endPosition;
            Vector3 linePos1 = endPosition;
            linePos1.z = -3;
            lineRenderer.SetPosition(1, linePos1);
            if (hitInfo.transform.TryGetComponent<NetworkPhysicsObject>(out var ball))
            {
                if (!EightBallGameSystem.Instance.IsValidBall(ball.ballID))
                {
                    wrongBall.SetActive(true);
                    lineRenderer2.enabled = false;
                    lineRenderer3.enabled = false;
                }
                else
                {
                    //if()
                    wrongBall.SetActive(false);
                    drawLine(linePos);
                }
            }
            else
            {
                wrongBall.SetActive(false);
                lineRenderer2.enabled = false;
                lineRenderer3.enabled = false;
            }
        }

    }
    void CalculateHitFactor(ref float factor)
    {
        var myPos = transform.position;
        myPos.z = -2f;
        var ballGetHit = hitInfo.transform.position;
        ballGetHit.z = -2f;
        var ball2BallDir = (myPos - ballGetHit).normalized;

        var hitPos = endPosition;
        hitPos.z = -2f;
        var ball2HitPos = (hitPos - ballGetHit).normalized;
        factor = Vector3.Dot(ball2BallDir, ball2HitPos);
        //Debug.DrawRay(myPos, ball2BallDir * 2f, Color.red);
        //Debug.DrawRay(myPos, ball2HitPos * 2f, Color.green);
        //Debug.Log($"CalculateHitFactor = {factor}");
    }
    Ray r2;
    public void drawLine(Vector3 linePos)
    {
        wrongBall.SetActive(false);
        circle.GetComponent<LineRenderer>().enabled = true;
        Vector3 hitBallPosition = hitInfo.transform.position;
        hitBallPosition.z = -3;
        lineRenderer3.SetPosition(0, hitBallPosition);

        Vector3 r2dir = (hitBallPosition - endPosition).normalized;
        r2 = new Ray(hitBallPosition, r2dir);
        //var ball2balldir = (transform.position - hitBallPosition);
        //ball2balldir.z = -3;
        //ball2balldir.Normalize();
        //var hitdir = -(r2dir);
        //hitdir.z = -3;
        //hitdir.Normalize();
        //var dotFactor = Vector3.Dot(ball2balldir, hitdir);
        float dotFactor = 1f;
        CalculateHitFactor(ref dotFactor);
        Vector3 pos3 = r2.origin + DefaultLineLength * dotFactor * r2dir;
        // Debug.Log($"SHPT dot = {Vector3.Dot(ball2balldir, hitdir)}");
        //Debug.Log(dotFactor);
        pos3.z = -3;
        lineRenderer3.SetPosition(1, pos3);

        Vector3 l = DefaultLineLength * (1 - dotFactor) * r2dir;
        l = Quaternion.Euler(0, 0, -90) * l + endPosition;
        l.z = -3;

        float angleBeetwen = AngleBetweenThreePoints(linePos, endPosition, l);

        if (angleBeetwen < 90.0f || angleBeetwen > 270.0f)
        {
            l = DefaultLineLength * (1 - dotFactor) * r2dir;
            l = Quaternion.Euler(0, 0, 90) * l + endPosition;
            l.z = -3;
        }
        lineRenderer2.SetPosition(0, endPosition);
        lineRenderer2.SetPosition(1, l);
        ballCollideFirst = true;


        if (IsShowLine)
        {
            lineRenderer2.enabled = true;
            lineRenderer3.enabled = true;
        }
    }
    public void ShowTargetLine()
    {
        drawTargetLines();
        targetLine.SetActive(true);
        cueRenderer.enabled = true;
    }
    public void HideCueController()
    {
        targetLine.SetActive(false);
        cueRenderer.enabled = false;
    }
    public float AngleBetweenThreePoints(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float a = pointB.x - pointA.x;
        float b = pointB.y - pointA.y;
        float c = pointB.x - pointC.x;
        float d = pointB.y - pointC.y;

        float atanA = Mathf.Atan2(a, b) * Mathf.Rad2Deg;
        float atanB = Mathf.Atan2(c, d) * Mathf.Rad2Deg;

        float output = atanB - atanA;
        output = Mathf.Abs(output);
        return output;
    }

    public struct PredictCollisionData
    {
        public Vector3 targetDir;
        public int targetBallId;
        public Vector3 statPosition;
        public float hitDistance;
        public Vector3 hitPosition;
    }
    public PredictCollisionData PredictHitInfo()
    {
        Vector3 targetDir = Vector3.zero;
        int ballId = 0;
        Vector3 dir = transform.position - posDetector.transform.position;
        dir.z = 0;
        dir = dir.normalized;
        Ray predictRay = new Ray();
        predictRay.origin = transform.position;
        predictRay.direction = dir;
        PredictCollisionData collisionData = new PredictCollisionData();

        if (Physics.SphereCast(predictRay, radius * BallChekRadiusPercent, out var hitInfo, 10, layerMask))
        {
            if (hitInfo.transform.TryGetComponent<NetworkPhysicsObject>(out var ball))
            {
                var hitPos = predictRay.origin + (predictRay.direction.normalized * hitInfo.distance);
                hitPos.z = hitInfo.transform.position.z;

                ballId = ball.ballID;
                targetDir = (hitInfo.transform.position - hitPos).normalized;
                collisionData.hitDistance = hitInfo.distance;
                collisionData.hitPosition = hitPos;
            }

        }
        collisionData.statPosition = transform.position;
        collisionData.targetDir = targetDir;
        collisionData.targetBallId = ballId;
        return collisionData;
        //return (targetDir, ballId, transform.position, hitInfo.distance);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //var pos = endPosition;
        //pos.z = transform.position.z;
        //Gizmos.DrawWireSphere(pos, radius * BallChekRadiusPercent);
        //Gizmos.color = Color.yellow;
        //var newdir = capture_r2.origin;
        //newdir.z = -1f;
        //Gizmos.DrawLine(newdir, newdir + capture_r2.direction * rayLength);
        //Gizmos.color = Color.cyan;
        //var hitPos = endPosition;
        //hitPos.z = -1f;
        //Gizmos.DrawLine(wb2hit.origin, hitPos);
    }


}
