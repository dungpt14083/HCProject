using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBallPlacementController : MonoBehaviour
{
    [SerializeField]
    private GameObject ballHand;
    [SerializeField]
    private SpriteRenderer errorHand;
    public Camera mainCamera;
    public LayerMask noneCollisionBallLayer;
    public LayerMask ballLayer;
    [Header("Debug")]
    public bool findNew = false;
    public bool triggerCollides = true;
    private Vector3 newPos;
    NetworkPhysicsObject networkPhysicsObject;
    //GameController gameController;
    CueController cueController;
    float radius;
    Collider[] colliders = new Collider[5];
    int hits;
    public bool isError = false;
    public GameObject objIsError;
    public GameObject arrowTutorial;
    //IEnumerator FindGameController()
    //{
    //    //while (gameController == null)
    //    //{
    //    //    if (GameController.Instance != null)
    //    //    {
    //    //        gameController = GameController.Instance;
    //    //        gameController.OnStartHandBall += FindNewPosition;
    //    //        gameController.OnEndHandBall += EndHandBall;
    //    //        break;
    //    //    }
    //    //    yield return null;
    //    //}
    //}

    private void OnDestroy()
    {
        //if (gameController != null)
        //{
        //    gameController.OnStartHandBall -= FindNewPosition;
        //    gameController.OnEndHandBall -= EndHandBall;
        //}
        //GameController.Instance.OnStartHandBall -= FindNewPosition;
        //GameController.Instance.OnEndHandBall -= EndHandBall;
        EightBallGameSystem.Instance.OnBreaking -= OnBreaking;
        EightBallGameSystem.Instance.OnOtherWhiteBallPosCallback -= WhiteBallUpdate;
        EightBallGameSystem.Instance.OnOtherWhiteBallStartMoveCallback -= WhiteBallStartMove;
        EightBallGameSystem.Instance.OnOtherWhiteBallEndMoveCallback -= WhiteBallEndMove;
    }

    private void Start()
    {
        networkPhysicsObject = GetComponent<NetworkPhysicsObject>();
        cueController = GetComponent<CueController>();
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        //StartCoroutine(FindGameController());
        // Network

        EightBallGameSystem.Instance.OnBreaking += OnBreaking;
        EightBallGameSystem.Instance.OnOtherWhiteBallPosCallback += WhiteBallUpdate;
        EightBallGameSystem.Instance.OnOtherWhiteBallStartMoveCallback += WhiteBallStartMove;
        EightBallGameSystem.Instance.OnOtherWhiteBallEndMoveCallback += WhiteBallEndMove;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        //FindNewPosition();
        //if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Single)
        //{
        //    Debug.Log("End Hand ball callback on Start");
        //    EndHandBall();
        //}
    }
    public void StartGame()
    {
        FindNewPosition();
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Single)
        {
            Debug.Log("End Hand ball callback on Start");
            EndHandBall();
        }
    }
    void OnBreaking()
    {
        //gameController.startBound.gameObject.SetActive(false);
    }
    bool ValidateMove()
    {
        return GameController.Instance.whiteBallPlacement && EightBallGameSystem.Instance.IsMyTurn && EightBallGameSystem.Instance.CurrentMode == PlayMode.Online;
    }

    public void EndHandBall()
    {
        //Debug.Log("SHPT EndHandBall");
        ballHand.SetActive(false);
    }

    private void Update()
    {
        if (GameController.Instance != null && GameController.Instance.whiteBallPlacement)
        {
            hits = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, ballLayer);
            //Debug.Log($"SHPT OverlapSphereNonAlloc transform.position = {transform.position}");
            isError = false;
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    if (colliders[i].gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    {
                        isError = true;
                        objIsError = colliders[i].gameObject;
                        break;
                    }
                }
                if (isError)
                {
                    errorHand.gameObject.SetActive(true);

                }
                else
                {
                    objIsError = null;
                    errorHand.gameObject.SetActive(false);
                }
            }
            else
            {
                objIsError = null;
                errorHand.gameObject.SetActive(false);
            }
        }
    }
    public void StartTutorial()
    {
        networkPhysicsObject = GetComponent<NetworkPhysicsObject>();
        cueController = GetComponent<CueController>();
        newPos = GameController.Instance.GetWhiteBallInitPos();
        transform.position = newPos;
        networkPhysicsObject.ResetRBState();
        cueController.UpdateCuePos();
        cueController.targetLine.SetActive(true);
        cueController.cueRenderer.enabled = true;
        networkPhysicsObject.ResetState();
        var hitCheck = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, ballLayer);
        if (hitCheck == 0)
        {
            SetBallCollision(true);
        }
    }
    public void FindNewPosition()
    {
        Debug.Log("SHPT FInd new pos");
        newPos = GameController.Instance.GetWhiteBallInitPos();
        transform.position = newPos;
        Debug.Log("transform.position 444444444444444 " + transform.position);
        networkPhysicsObject.ResetRBState();
        cueController.UpdateCuePos();
        cueController.targetLine.SetActive(true);
        cueController.cueRenderer.enabled = true;
        networkPhysicsObject.ResetState();
        ballHand.SetActive(true);
        UpdateBallHandPos();
        // temp disable ball
        SetBallCollision(false);
        var hitCheck = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, ballLayer);
        if (hitCheck == 0)
        {
            SetBallCollision(true);
        }
    }

    private void UpdateBallHandPos()
    {
        var handPos = transform.position;
        handPos.z = -3f;
        ballHand.transform.position = handPos;
    }

    Bounds boundCheck;
    private void OnMouseDown()
    {
        if (!ValidateMove()) return;
        WhiteBallStartMove();
        if (EightBallGameSystem.Instance.IsBreakingShot())
        {
            Debug.Log($"Bound = Start bound");
            GameController.Instance.startBound.gameObject.SetActive(true);
            boundCheck = GameController.Instance.startBound.bounds;
        }
        else
        {
            Debug.Log($"Bound = table bound");
            (Vector3 center, Vector3 size) = GameController.Instance.GetTableBound();
            boundCheck.center = center;
            boundCheck.size = size;
        }
        
        EightBallGameSystem.Instance.StartWhiteBallMove(transform.position);
    }

    private void OnMouseDrag()
    {
        if (!ValidateMove()) return;
        float distance_to_screen = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 pos_move = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

        if (boundCheck.Contains(pos_move))
        {
            if (!ValidateMove()) return;
            WhiteBallUpdate(pos_move);
            EightBallGameSystem.Instance.UpdateWhiteBallPos(pos_move);
        }
    }

    private void OnMouseUp()
    {
        if (!ValidateMove()) return;
        if (objIsError != null)
        {
            //find new pos 8 ball
            var oldPos = transform.position;
            var posTop = boundCheck.center.y + boundCheck.size.y;
            var posDown = boundCheck.center.y - boundCheck.size.y;
            var posLeft = boundCheck.center.x - boundCheck.size.x;
            var posRight = boundCheck.center.x + boundCheck.size.x;
            var detaTop = Mathf.Abs(objIsError.transform.position.y - posTop);
            var detaDown = Mathf.Abs(objIsError.transform.position.y - posDown);
            var maxRadiusFindPos = Mathf.Max(detaTop, detaDown);
            float rang = 0.55f;
            while(rang < maxRadiusFindPos)
            {
                if (!EightBallGameSystem.Instance.IsMyTurn)
                    return;
                float ang = 0;
                while(ang <= 360)
                {
                    if (!EightBallGameSystem.Instance.IsMyTurn)
                        return;
                    var deltaX = rang * Mathf.Cos(ang * Mathf.Deg2Rad);
                    var deltaY = rang * Mathf.Sin(ang * Mathf.Deg2Rad);
                    var newPos = objIsError.transform.position + new Vector3(deltaX, deltaY, 0);
                    transform.position = newPos;
                    Debug.Log("transform.position 333333333333333 " + transform.position);
                    Debug.Log("objIsError new_pos " + transform.position);
                    if (boundCheck.Contains(transform.position))
                    {
                        hits = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, ballLayer);
                        //Debug.Log($"SHPT OverlapSphereNonAlloc hit = {hits}");
                        var findIsError = false;
                        if (hits > 0)
                        {
                            for (int i = 0; i < hits; i++)
                            {
                                if (colliders[i].gameObject.GetInstanceID() != gameObject.GetInstanceID())
                                {
                                    findIsError = true;
                                    break;
                                }
                            }
                            if (!findIsError)
                            {
                                //transform.position = new_pos;
                                objIsError = null;
                                isError = false;
                                ballHand.transform.position = transform.position;
                                WhiteBallEndMove();
                                EightBallGameSystem.Instance.EndWhiteBallMove(transform.position);
                                return;
                            }
                        }
                        else
                        {
                            objIsError = null;
                            isError = false;
                            ballHand.transform.position = transform.position;
                            WhiteBallEndMove();
                            EightBallGameSystem.Instance.EndWhiteBallMove(transform.position);
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("objIsError boundCheck fail ");
                    }
                    ang += 20;
                    Debug.Log("objIsError ang " + ang);
                }
                rang += 0.25f;
                Debug.Log("-----------------> objIsError rang " + rang);
            }
            transform.position = oldPos;
            Debug.Log("transform.position 22222222222 " + transform.position);
            objIsError = null;
            ballHand.transform.position = transform.position;
            WhiteBallEndMove();
            EightBallGameSystem.Instance.EndWhiteBallMove(transform.position);
        }
        else
        {
            WhiteBallEndMove();
            EightBallGameSystem.Instance.EndWhiteBallMove(transform.position);
        }
    }

    private void WhiteBallStartMove()
    {
        networkPhysicsObject.ForceUpdatePos(true);
        networkPhysicsObject.Rigidbody.isKinematic = true;
        SetBallCollision(false);
        GameController.Instance.ForceHideControl(true);
    }

    private void WhiteBallEndMove()
    {
        GameController.Instance.ForceHideControl(false);
        if (isError)
        {
            GameController.Instance.OnWhiteBallOverlap(true);
            return;
        }
        SetBallCollision(true);
        if (EightBallGameSystem.Instance.IsMyTurn)
            GameController.Instance.OnWhiteBallOverlap(false);
        networkPhysicsObject.ForceUpdatePos(false);
        
    }

    private void WhiteBallUpdate(Vector3 newPos)
    {
        SetBallCollision(false);
        transform.position = newPos;
        Debug.Log("transform.position WhiteBallUpdate " + transform.position);
        UpdateBallHandPos();
        cueController.UpdateCuePos();
    }
    int currentLayer;
    private void SetBallCollision(bool shouldCollision)
    {
        int targetLayer = 0;
        if (!shouldCollision)
        {
            targetLayer = noneCollisionBallLayer.value;
        }
        else
        {
            targetLayer = ballLayer.value;
        }
        if (targetLayer == currentLayer) return;
        // set layer
        gameObject.layer = (int)Mathf.Log(targetLayer, 2);
        currentLayer = targetLayer;
    }

}
