using UnityEngine;
using System.Collections;
using System.Threading;
using InitScriptName;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using BubblesShot;
using Spine.Unity;

namespace BubblesShot
{
    public class ball : MonoBehaviour
    {
        public Sprite[] boosts;
        //public BoostType boosterType;
        public bool isTarget;
        public Vector3 target;
        Vector2 worldPos;
        public bool setTarget;
        public float startTime;
        public GameObject mesh;
        public bool findMesh;
        Vector3 dropTarget;
        public bool newBall;
        public Vector3 targetPosition;
        bool stopedBall;
        private bool destroyed;
        public bool NotSorting;
        ArrayList fireballArray = new ArrayList();
        public bool Destroyed
        {
            get { return destroyed; }
            set
            {
                if (value)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                    GetComponent<SpriteRenderer>().enabled = false;

                }
                destroyed = value;
            }
        }
        public ArrayList nearBalls = new ArrayList();
        //	private OTSpriteBatch spriteBatch = null;  
        GameObject Meshes;
        public int countNEarBalls;
        float bottomBorder;
        float topBorder;
        float leftBorder;
        float rightBorder;
        float gameOverBorder;
        bool gameOver;
        bool isPaused;
        public AudioClip swish;
        public AudioClip pops;
        public AudioClip join;
        Vector3 meshPos;
        public bool falling;
        private int HitBug;
        private bool fireBall;
        private static int fireworks;
        private int fireBallLimit = 10;
        private bool launched;
        private bool animStarted;
        private bool _readyShot;
        private DataGamePlay _dataGamePlay;
        private Camera mainCamera;
        private float _speed = 15.0f;
        public bool CollidingWithAnotherBubble;
        private Vector3 shootDir;
        private SpriteRenderer spriteRenderer;

        public int HitBug1
        {
            get { return HitBug; }
            set
            {
                if (value < 3)
                    HitBug = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            meshPos = new Vector3(-1000, -1000, -10);
            dropTarget = transform.position;
            Meshes = GameObject.Find("Balls");
            mainCamera = Camera.main;
            spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && DrawLine.waitingTime <= 0)
            {
                _readyShot = true;
            }
            if (Input.GetMouseButtonUp(0) && _readyShot)
            {
                _readyShot = false;
                _dataGamePlay = Commons.GetDataGamePlay();
                //if (_dataGamePlay.moves > 0)
                //{
                GameObject ball = gameObject;
                if (!launched && !ball.GetComponent<ball>().setTarget && mainscript.Instance.newBall2 == null && newBall
                    && !Camera.main.GetComponent<mainscript>().gameOver
                    && GamePlay.Instance.GameStatus == GameState.Playing
                    && GamePlay.Instance.GameStatus != GameState.WaitForPopup
                    && GamePlay.Instance.GameStatus != GameState.Pause)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    worldPos = pos;
                    if (worldPos.y > -3f && pos.y < 5f && !mainscript.StopControl)
                    {
                        launched = true;
                        if (!fireBall)
                            GetComponent<CircleCollider2D>().enabled = true;
                        target = worldPos;

                        setTarget = true;
                        startTime = Time.time;
                        dropTarget = transform.position;
                        InitScript.Instance.BoostActivated = false;
                        mainscript.Instance.newBall = gameObject;
                        mainscript.Instance.newBall2 = gameObject;
                        Vector2 force = target - dropTarget;
                        shootDir = force.normalized;
                        GetComponent<Rigidbody2D>().AddForce(force);

                        switch (mainscript.boostType)
                        {
                            case BoostType.ItemBomb:
                                InGameUI.Instance.UpdateBoostItem(-1, 0, 0);
                                mainscript.boostType = BoostType.ItemNormal;
                                break;
                            case BoostType.ItemColorful:
                                InGameUI.Instance.UpdateBoostItem(0, 0, -1);
                                mainscript.boostType = BoostType.ItemNormal;
                                break;
                        }
                    }
                }
                //}
            }
            if (transform.position != target && setTarget && !stopedBall && !isPaused && Camera.main.GetComponent<mainscript>().dropDownTime < Time.time)
            {
                transform.position += shootDir * _speed * Time.deltaTime;
                var leftEdge = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
                var rightEdge = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
                if (transform.position.x - spriteRenderer.bounds.size.x / 2 <= leftEdge.x ||
                    transform.position.x + spriteRenderer.bounds.size.x / 2 >= rightEdge.x)
                {
                    ReverseDirection();
                }
            }

        }

        public void SetBoost(BoostType boostType)
        {
            GetComponent<SpriteRenderer>().sprite = boosts[(int)boostType];
            if (boostType == BoostType.ItemBomb)
            {
                tag = "Ball";
            }
            if (boostType == BoostType.ItemColorful)
            {
                tag = "colorful";
            }
            if (boostType == BoostType.ItemRocket)
            {
                tag = "Ball";
                GetComponent<SpriteRenderer>().sortingOrder = 10;
                GetComponent<CircleCollider2D>().enabled = false;
                fireBall = true;
                fireballArray.Add(gameObject);
            }
        }

        void FixedUpdate()
        {
            if (Camera.main.GetComponent<mainscript>().gameOver) return;

            if (stopedBall)
            {
                transform.position = meshPos;
                stopedBall = false;
                if (newBall)
                {
                    newBall = false;
                    gameObject.layer = LayerMask.NameToLayer("Ball");
                    Camera.main.GetComponent<mainscript>().checkBall = gameObject;
                    this.enabled = false;
                }
            }

        }

        public void ReverseDirection()
        {
            shootDir.x *= -1;
        }

        public GameObject findInArrayGameObject(ArrayList b, GameObject destObj)
        {
            foreach (GameObject obj in b)
            {

                if (obj == destObj) return obj;
            }
            return null;
        }


        public bool findInArray(ArrayList b, GameObject destObj)
        {
            foreach (GameObject obj in b)
            {

                if (obj == destObj) return true;
            }
            return false;
        }

        public ArrayList addFrom(ArrayList b, ArrayList b2)
        {
            foreach (GameObject obj in b)
            {
                if (!findInArray(b2, obj))
                {
                    b2.Add(obj);
                }
            }
            return b2;
        }

        public void changeNearestColor()
        {
            GameObject gm = GameObject.Find("Creator");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, 1 << LayerMask.NameToLayer("Ball"));
            foreach (Collider2D obj in fixedBalls)
            {
                gm.GetComponent<creatorBall>().createBall(obj.transform.position);
                Destroy(obj.gameObject);
            }

        }


        public void checkNextNearestColor(ArrayList b, int counter)
        {
            //		Debug.Log(b.Count);
            Vector3 distEtalon = transform.localScale;
            //		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
            //		foreach(GameObject obj in meshes) {
            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1.0f, layerMask);
            foreach (Collider2D obj1 in meshes)
            {
                if (obj1.GetComponent<ColorBallScript>().isShowBall)
                    if (obj1.gameObject.tag == tag)
                    {
                        GameObject obj = obj1.gameObject;
                        float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                        if (distTemp <= 1.0f)
                        {
                            if (!findInArray(b, obj))
                            {
                                counter++;
                                b.Add(obj);
                                obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                            }
                        }
                    }
            }
        }

        public void checkNearestColor()
        {
            int counter = 0;
            ArrayList balls = new ArrayList();
            var fixedBalls = GameObject.Find("Balls").transform;
            // change color tag of the rainbow
            foreach (Transform obj in fixedBalls)
            {
                if (obj != null)
                {
                    if (obj.GetComponent<ColorBallScript>().isShowBall)
                    {
                        balls.Add(obj.gameObject);
                        if (obj.gameObject.layer == LayerMask.NameToLayer("Ball") && (obj.name.IndexOf("Rainbow") > -1))
                        {
                            obj.tag = tag;
                        }
                    }
                }
            }
            ArrayList b = new ArrayList();
            b.Add(gameObject);

            if (tag == "colorful")
            {
                foreach (GameObject obj in balls)
                {
                    // detect the same color balls
                    float distTemp = Vector3.Distance(transform.position, obj.transform.position);

                    if (obj.GetComponent<ColorBallScript>().isShowBall)
                        if (distTemp <= 0.9f && distTemp > 0)
                        {
                            b.Add(obj);
                            obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                        }
                }
            }
            else
            {
                GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in meshes)
                {
                    if (obj.GetComponent<ColorBallScript>().isShowBall)
                    {
                        // detect the same color balls
                        float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                        if (distTemp <= 0.9f && distTemp > 0)
                        {
                            b.Add(obj);
                            obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                        }
                    }
                }
            }
            b.Distinct();
            mainscript.Instance.countOfPreparedToDestroy = b.Count;
            if (b.Count >= 3)
            {
                mainscript.Instance.ComboCount++;
                destroy(b, 0.00001f);
            }
            if (b.Count < 3)
            {
                Camera.main.GetComponent<mainscript>().bounceCounter++;
                mainscript.Instance.ComboCount = 0;
                if (tag != "Ball" && tag != "colorful")
                    InGameUI.Instance.UpdateHeartUI(true);
            }

            b.Clear();
            Camera.main.GetComponent<mainscript>().dropingDown = false;
            FindLight(gameObject);

        }

        public void StartFall()
        {
            enabled = false;

            if (mesh != null)
                mesh.GetComponent<Grid>().Busy = null;
            if (gameObject == null) return;
            setTarget = false;
            transform.SetParent(null);
            gameObject.layer = LayerMask.NameToLayer("FallingBall");
            gameObject.tag = "Ball";
            if (gameObject.GetComponent<Rigidbody2D>() == null) gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            gameObject.GetComponent<Rigidbody2D>().fixedAngle = false;
            gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity + new Vector2(Random.Range(-2, 2), 0);
            gameObject.GetComponent<CircleCollider2D>().enabled = true;
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
            gameObject.GetComponent<CircleCollider2D>().radius = 0.3f;
            GetComponent<ball>().falling = true;

        }

        public bool checkNearestBall(ArrayList b)
        {
            if ((mainscript.Instance.TopBorder.transform.position.y - transform.position.y <= 0.5f))
            {
                Camera.main.GetComponent<mainscript>().controlArray = addFrom(b, Camera.main.GetComponent<mainscript>().controlArray);
                b.Clear();
                return true;    /// don't destroy
            }
            if (findInArray(Camera.main.GetComponent<mainscript>().controlArray, gameObject)) { b.Clear(); return true; } /// don't destroy
            b.Add(gameObject);
            foreach (GameObject obj in nearBalls)
            {
                if (obj != gameObject && obj != null)
                {
                    if (obj.gameObject.layer == LayerMask.NameToLayer("Ball"))
                    {
                        float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                        if (distTemp <= 0.9f && distTemp > 0)
                        {
                            if (!findInArray(b, obj.gameObject))
                            {
                                Camera.main.GetComponent<mainscript>().arraycounter++;
                                if (obj.GetComponent<ball>().checkNearestBall(b))
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;

        }

        public void connectNearBalls()
        {
            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, .6f, layerMask);
            nearBalls.Clear();

            foreach (Collider2D obj in fixedBalls)
            {
                if (nearBalls.Count <= 10)
                {
                    nearBalls.Add(obj.gameObject);
                }
            }
            countNEarBalls = nearBalls.Count;
        }

        IEnumerator pullToMesh(Transform otherBall = null)
        {
            //	AudioSource.PlayClipAtPoint(join, new Vector3(5, 1, 2));
            GameObject busyMesh = null;
            float searchRadius = 0.2f;
            while (findMesh)
            {
                Vector3 centerPoint = transform.position;
                Collider2D[] fixedBalls1 = Physics2D.OverlapCircleAll(centerPoint, 0.1f, 1 << LayerMask.NameToLayer("Mesh"));  //meshes

                foreach (Collider2D obj1 in fixedBalls1)
                {
                    if (obj1.gameObject.GetComponent<Grid>() == null) DestroySingle(gameObject, 0.00001f);
                    else if (obj1.gameObject.GetComponent<Grid>().Busy == null)
                    {
                        findMesh = false;
                        stopedBall = true;
                        if (meshPos.y <= obj1.gameObject.transform.position.y)
                        {
                            meshPos = obj1.gameObject.transform.position;
                            busyMesh = obj1.gameObject;
                        }
                    }
                }
                if (findMesh)
                {
                    Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(centerPoint, searchRadius, 1 << LayerMask.NameToLayer("Mesh"));  //meshes
                    foreach (Collider2D obj in fixedBalls)
                    {
                        if (obj.gameObject.GetComponent<Grid>() == null) DestroySingle(gameObject, 0.00001f);
                        else if (obj.gameObject.GetComponent<Grid>().Busy == null)
                        {
                            findMesh = false;
                            stopedBall = true;

                            if (meshPos.y <= obj.gameObject.transform.position.y)
                            {
                                meshPos = obj.gameObject.transform.position;
                                busyMesh = obj.gameObject;
                            }
                        }
                    }
                }
                if (busyMesh != null)
                {
                    busyMesh.GetComponent<Grid>().Busy = gameObject;
                    gameObject.GetComponent<bouncer>().offset = busyMesh.GetComponent<Grid>().offset;

                }
                transform.parent = Meshes.transform;
                Destroy(GetComponent<Rigidbody2D>());
                yield return new WaitForFixedUpdate();
                dropTarget = transform.position;

                if (findMesh) searchRadius += 0.2f;

                yield return new WaitForFixedUpdate();

            }
            mainscript.Instance.connectNearBallsGlobal();

            if (busyMesh != null)
            {
                Hashtable animTable = mainscript.Instance.animTable;
                animTable.Clear();
                PlayHitAnim(transform.position, animTable);
            }
            creatorBall.Instance.OffGridColliders();

            yield return new WaitForSeconds(0.5f);
        }

        public void PlayHitAnim(Vector3 newBallPos, Hashtable animTable)
        {

            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
            float force = 0.15f;
            foreach (Collider2D obj in fixedBalls)
            {
                if (!animTable.ContainsKey(obj.gameObject) && obj.gameObject != gameObject && animTable.Count < 50)
                    obj.GetComponent<ball>().PlayHitAnimCorStart(newBallPos, force, animTable);
            }
            if (fixedBalls.Length > 0 && !animTable.ContainsKey(gameObject))
                PlayHitAnimCorStart(fixedBalls[0].gameObject.transform.position, 0, animTable);
        }

        public void PlayHitAnimCorStart(Vector3 newBallPos, float force, Hashtable animTable)
        {
            if (!animStarted)
            {
                StartCoroutine(PlayHitAnimCor(newBallPos, force, animTable));
                PlayHitAnim(newBallPos, animTable);
            }
        }

        public IEnumerator PlayHitAnimCor(Vector3 newBallPos, float force, Hashtable animTable)
        {
            animStarted = true;
            animTable.Add(gameObject, gameObject);
            yield return new WaitForFixedUpdate();
            float dist = Vector3.Distance(transform.position, newBallPos);
            force = 1 / dist + force;
            newBallPos = transform.position - newBallPos;
            if (transform.parent == null)
            {
                animStarted = false;
                yield break;
            }
            newBallPos = Quaternion.AngleAxis(transform.parent.parent.rotation.eulerAngles.z, Vector3.back) * newBallPos;
            newBallPos = newBallPos.normalized;
            newBallPos = transform.localPosition + (newBallPos * force / 10);

            float startTime = Time.time;
            Vector3 startPos = transform.localPosition;
            float speed = force * 5;
            float distCovered = 0;
            while (distCovered < 1 && !float.IsNaN(newBallPos.x))
            {
                distCovered = (Time.time - startTime) * speed;
                if (this == null) yield break;
                if (falling)
                {
                    yield break;
                }
                transform.localPosition = Vector3.Lerp(startPos, newBallPos, distCovered);
                yield return new WaitForEndOfFrame();
            }
            Vector3 lastPos = transform.localPosition;
            startTime = Time.time;
            distCovered = 0;
            while (distCovered < 1 && !float.IsNaN(newBallPos.x))
            {
                distCovered = (Time.time - startTime) * speed;
                if (this == null) yield break;
                if (falling)
                {
                    yield break;
                }
                transform.localPosition = Vector3.Lerp(lastPos, startPos, distCovered);
                yield return new WaitForEndOfFrame();
            }
            transform.localPosition = startPos;
            animStarted = false;
        }
        void OnTriggerStay2D(Collider2D other)
        {
            if (findMesh && other.gameObject.layer == LayerMask.NameToLayer("Ball"))
            {
                //	StartCoroutine(pullToMesh());
            }
        }

        public void FindLight(GameObject activatedByBall)
        {
            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
            int i = 0;
            foreach (Collider2D obj in fixedBalls)
            {
                i++;
                if (i <= 10)
                {
                    if ((obj.gameObject.tag == "light") && GamePlay.Instance.GameStatus == GameState.Playing)
                    {
                        DestroySingle(obj.gameObject);
                        DestroySingle(activatedByBall);
                    }
                    else if ((obj.gameObject.tag == "cloud") && GamePlay.Instance.GameStatus == GameState.Playing)
                    {
                        obj.GetComponent<ColorBallScript>().ChangeRandomColor();
                    }

                }
            }
        }


        void OnCollisionEnter2D(Collision2D coll)
        {
            OnTriggerEnter2D(coll.collider);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // stop
            if (other.gameObject.name.Contains("ball") && setTarget && name.IndexOf("bug") < 0)
            {
                if (!other.gameObject.GetComponent<ball>().enabled)
                {
                    if ((other.gameObject.tag == "black_hole") && GamePlay.Instance.GameStatus == GameState.Playing)
                    {
                        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.black_hole);
                        DestroySingle(gameObject);
                    }
                    StopBall(true, other.transform);
                    //if (!fireBall)
                    //    StopBall(true, other.transform);
                    //else
                    //{
                    //    fireBallLimit--;
                    //    if (fireBallLimit > 0)
                    //        DestroySingle(other.gameObject, 0.000000000001f);
                    //    else
                    //    {
                    //        StopBall();
                    //        destroy(fireballArray, 0.000000000001f);
                    //    }
                    //}
                }
            }
            else if (other.gameObject.name.IndexOf("ball") == 0 && setTarget)
            {
                if (other.gameObject.tag == gameObject.tag)
                {
                    Destroy(other.gameObject);
                }
            }

            else if (other.gameObject.name == "TopBorder" && setTarget)
            {
                if (!findMesh)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    StopBall();

                    if (fireBall)
                    {
                        destroy(fireballArray, 0.000000000001f);
                    }
                }
            }
        }

        void StopBall(bool pulltoMesh = true, Transform otherBall = null)
        {
            launched = true;
            mainscript.lastBall = gameObject.transform.position;
            creatorBall.Instance.EnableGridColliders();
            target = Vector2.zero;
            setTarget = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            findMesh = true;
            GetComponent<BoxCollider2D>().offset = Vector2.zero;
            GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);

            if (GetComponent<SpriteRenderer>().sprite == boosts[0])  //color ball boost
            {
                DestroyAround();
            }
            if (pulltoMesh)
                StartCoroutine(pullToMesh(otherBall));

        }


        void DestroyAround()
        {
            ArrayList b = new ArrayList();
            b.Add(gameObject);
            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1f, layerMask);
            foreach (Collider2D obj1 in meshes)
            {
                if (obj1.gameObject.GetComponent<ColorBallScript>().isShowBall)
                {
                    GameObject obj = obj1.gameObject;
                    if (!findInArray(b, obj))
                    {
                        b.Add(obj);
                    }
                }
            }
            if (b.Count >= 0)
            {
                mainscript.Instance.ComboCount++;
                destroy(b, 0.001f);
            }

        }

        void DestroyLine()
        {

            ArrayList b = new ArrayList();
            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            RaycastHit2D[] fixedBalls = Physics2D.LinecastAll(transform.position + Vector3.left * 10, transform.position + Vector3.right * 10, layerMask);
            int i = 0;
            foreach (RaycastHit2D item in fixedBalls)
            {
                if (!findInArray(b, item.collider.gameObject))
                {
                    b.Add(item.collider.gameObject);
                }
            }

            if (b.Count >= 0)
            {
                mainscript.Instance.ComboCount++;
                mainscript.Instance.destroy(b);
            }

            mainscript.Instance.StartCoroutine(mainscript.Instance.destroyAloneBall());
        }


        public void CheckBallCrossedBorder()
        {
            if (Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Border")) != null || Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("NewBall")) != null)
            {
                DestroySingle(gameObject, 0.00001f);
            }

        }

        public void destroy(ArrayList b, float speed = 0.1f)
        {
            StartCoroutine(DestroyCor(b, speed));
        }

        IEnumerator DestroyCor(ArrayList b, float speed = 0.1f)
        {
            ArrayList l = new ArrayList();
            foreach (GameObject obj in b)
            {
                if (!l.Contains(obj))
                    l.Add(obj);
            }
            l.Distinct();
            Camera.main.GetComponent<mainscript>().bounceCounter = 0;
            int scoreCounter = 0;
            //if (l.Count > 3)
            //{
            //    scoreCounter += ((10 * (l.Count - 3)) + 10);
            //}
            for (int i = 1; i <= l.Count; i++)
            {
                if (i <= 3)
                {
                    scoreCounter += 10;
                }
                else if (i > 3 && i < 12)
                {
                    scoreCounter += (10 * (i - 3)) + 10;
                }
                else
                {
                    scoreCounter += 100;
                }
            }
            int soundPool = 0;
            foreach (GameObject obj in l)
            {
                if (obj == null) continue;
                if (obj.name.IndexOf("ball") == 0) obj.layer = 0;
                obj.GetComponent<ball>().growUp();
                soundPool++;
                GetComponent<Collider2D>().enabled = false;
                if (b.Count > 10 && Random.Range(0, 10) > 5) mainscript.Instance.perfect.SetActive(true);
                obj.GetComponent<ball>().Destroyed = true;
                if (b.Count < 10 || soundPool % 20 == 0)
                    yield return new WaitForSeconds(speed);
            }
            mainscript.Instance.PopupScore(scoreCounter, transform.position);

        }

        void DestroySingle(GameObject obj, float speed = 0.1f)
        {
            //Camera.main.GetComponent<mainscript>().bounceCounter = 0;
            //int scoreCounter = 0;
            //if (obj.name.IndexOf("ball") == 0) obj.layer = 0;
            //obj.GetComponent<ball>().growUp();
            //
            //if (obj.tag == "light")
            //{
            //    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.spark);
            //    obj.GetComponent<ball>().DestroyLine();
            //}
            //
            //scoreCounter += 10;
            //
            //obj.GetComponent<ball>().Destroyed = true;
            //mainscript.Instance.PopupScore(scoreCounter, transform.position);
            ////Send ball counter be destroy on ball cluster to server

        }

        public void SplashDestroy()
        {
            if (setTarget) mainscript.Instance.newBall2 = null;
            Destroy(gameObject);
        }

        public void destroy()
        {
            growUpPlaySound();
            destroy(gameObject);
        }

        public void destroy(GameObject obj)
        {
            if (obj.name.IndexOf("ball") == 0) obj.layer = 0;

            Camera.main.GetComponent<mainscript>().bounceCounter = 0;
            //	collider.enabled = false;
            obj.GetComponent<ball>().destroyed = true;
            //	Destroy(obj);
            //obj.GetComponent<ball>().growUpPlaySound();
            obj.GetComponent<ball>().growUp();
            //	Invoke("playPop",1/(float)Random.Range(2,10));
            Camera.main.GetComponent<mainscript>().explode(obj.gameObject);
        }

        public void growUp()
        {
            StartCoroutine(explode());
        }

        public void growUpPlaySound()
        {
            Invoke("growUpDelayed", 1 / (float)Random.Range(2, 10));
        }

        public void growUpDelayed()
        {
            StartCoroutine(explode());
        }

        IEnumerator explode()
        {
            float startTime = Time.time;
            float endTime = Time.time + 0.1f;
            Vector3 tempPosition = transform.localScale;
            Vector3 targetPrepare = transform.localScale * 1.2f;

            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;


            while (!isPaused && endTime > Time.time)
            {
                transform.localScale = Vector3.Lerp(tempPosition, targetPrepare, (Time.time - startTime) * 10);
                yield return new WaitForEndOfFrame();
            }
            GameObject prefab = Resources.Load("Particles/BubbleExplosion") as GameObject;
            //GameObject prefab = Resources.Load("Particles/BubbleBroken") as GameObject;

            GameObject explosion = (GameObject)Instantiate(prefab, gameObject.transform.position + Vector3.back * 20f, Quaternion.identity);
            //Get bubble color
            //var bubbleColor = GetComponent<SpriteRenderer>().sprite.texture.GetPixelBilinear(0.55f, 0.55f);
            //explosion.GetComponent<SpineAnim>().ChangeSkinColor(bubbleColor);
            if (mesh != null)
                explosion.transform.parent = mesh.transform;

            Destroy(gameObject, 1);
        }

        public void ShowFirework()
        {
            fireworks++;
            if (fireworks <= 2)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.hit);

        }

        void OnMouseDown()
        {
            if (transform.parent != null)
                if ((transform.parent.name == "boxCatapult" || transform.parent.name == "boxSecond") && GamePlay.Instance.GameStatus == GameState.Playing && !mainscript.isMovingBubble)
                {
                    mainscript.Instance.ChangeBubble();
                }
        }
    }
}
