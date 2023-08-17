using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InitScriptName;
using BubblesShot;
using Random = UnityEngine.Random;
using Spine.Unity;


namespace BubblesShot
{
    [RequireComponent(typeof(AudioSource))]
    public class mainscript : MonoBehaviour
    {
        public int currentLevel;

        public static mainscript Instance;
        public int bounceCounter = 0;
        GameObject[] fixedBalls;
        public Vector2[][] meshArray;
        public GameObject checkBall;
        public GameObject newBall;
        public static int stage = 1;
        public int arraycounter = 0;
        public ArrayList controlArray = new ArrayList();
        bool destringAloneBall;
        public bool dropingDown;
        public float dropDownTime = 0f;
        public bool isPaused;
        public bool noSound;
        public bool gameOver;
        public bool arcadeMode;
        public float bottomBorder;
        public float topBorder;
        public float leftBorder;
        public float rightBorder;
        public float gameOverBorder;
        public float ArcadedropDownTime;
        public bool hd;
        public GameObject boxCatapult;
        public GameObject boxFirst;
        public GameObject boxSecond;
        bool gameOverShown;
        public static bool StopControl;
        public creatorBall creatorBall;
        public GameObject TopBorder;
        public Transform Balls;
        public Hashtable animTable = new Hashtable();
        public static Vector3 lastBall;
        public static int doubleScore = 1;

        public int TotalTargets;

        public int countOfPreparedToDestroy;

        public int bugSounds;
        public int potSounds;

        public static Dictionary<int, BallColor> colorsDict = new Dictionary<int, BallColor>();

        private int _ComboCount;

        public int ComboCount
        {
            get { return _ComboCount; }
            set
            {
                _ComboCount = value;
                if (value > 0)
                {
                    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.combo[Mathf.Clamp(value - 1, 0, 5)]);
                    if (value >= 6)
                    {
                        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.combo[5]);
                        doubleScore = 2;
                    }
                }
                else
                {
                    doubleScore = 1;
                }
            }
        }

        public GameObject popupScore;

        private int TargetCounter;

        public int TargetCounter1
        {
            get { return TargetCounter; }
            set
            {
                TargetCounter = value;
            }
        }

        public GameObject[] starsObject;
        public int stars = 0;

        public GameObject perfect;

        public GameObject[] boosts;
        public GameObject[] locksBoosts;

        int stageTemp;
        public GameObject newBall2;
        private int limit;
        private int colorLimit;
        private int ballsRemain = 0;
        public static bool isMovingBubble;
        public static BoostType boostType;
        private DataGamePlay _dataGamePlay;
        [SerializeField] GameObject _winEffect;
        [SerializeField] SkeletonAnimation _laser;


        void Awake()
        {
            Instance = this;
            _dataGamePlay = Commons.GetDataGamePlay();
            //calculate screen scale 
            var deviceW = Screen.width;
            var deviceH = Screen.height;
            float ratioNomal = 1920f / 1080f;
            float ratioCurr = (float)deviceH / (float)deviceW;
            Camera cam = Camera.main;
            cam.orthographicSize = cam.orthographicSize * (float)(ratioCurr / ratioNomal);

            if (InitScript.Instance == null) gameObject.AddComponent<InitScript>();
            currentLevel = PlayerPrefs.GetInt("OpenLevel", 3);
            stage = 1;
            mainscript.StopControl = false;
            animTable.Clear();
            _winEffect.SetActive(false);
            creatorBall = GameObject.Find("Creator").GetComponent<creatorBall>();
            StartCoroutine(CheckColors());
        }

        IEnumerator CheckColors()
        {
            while (true)
            {
                GetColorsInGame();
                yield return new WaitForEndOfFrame();
                SetColorsForNewBall();
            }

        }

        public void PopupScore(int value, Vector3 pos)
        {
            _dataGamePlay.scores += value;
            BubbesShotManager.Instance.SendPoint(_dataGamePlay.scores);
            Transform parent = GameObject.Find("CanvasScore").transform;
            GameObject poptxt = Instantiate(popupScore, pos, Quaternion.identity) as GameObject;
            poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
            poptxt.transform.SetParent(parent);
            poptxt.transform.localScale = Vector3.one;
            Destroy(poptxt, 1);
        }

        void Start()
        {
            HcPopupManager.Instance.ShowEightGameLoading(false);
            GamePlay.Instance.GameStatus = GameState.None;
            boostType = BoostType.ItemNormal;
            stageTemp = 1;
            if (PlayerPrefs.GetInt("noSound") == 1) noSound = true;
            _laser.AnimationName = "laser idle";
            BubbesShotManager.Instance.SendReady();
        }
       
        public void StartGame()
        {
            GamePlay.Instance.GameStatus = GameState.BlockedGame;
            creatorBall.Instance.StartGame();
            InGameUI.Instance.StartGame();
        }
        // Update is called once per frame
        void Update()
        {
            if (noSound)
                GetComponent<AudioSource>().volume = 0;
            if (!noSound)
                GetComponent<AudioSource>().volume = 0.5f;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (gameOver && !gameOverShown)
            {
                gameOverShown = true;
            }
            Debug.Log("GamePlay.Instance.GameStatus " + GamePlay.Instance.GameStatus);
            if (GamePlay.Instance.GameStatus == GameState.None)
                return;
            //Check ball avaliable
            var balls = GameObject.Find("Balls").transform;
            List<Transform> ballsAvalible = new List<Transform>();
            foreach (Transform ball in balls)
            {
                if (ball.GetComponent<ColorBallScript>().isShowBall)
                    ballsAvalible.Add(ball);
            }
            ballsRemain = ballsAvalible.Count;
            if (ballsRemain == 0)
            {
                GamePlay.Instance.GameStatus = GameState.GameOver;
                int gameOverType = 1;
                int score = _dataGamePlay.scores;
                int timeBonus = 10 * (int)InGameUI.Instance.timeRemain;
                int clearBoard = 1000;
                int finalScore = score + timeBonus + clearBoard;
                BubbesShotManager.Instance.SendEndGame(gameOverType, score, clearBoard, timeBonus, finalScore);
                //_winEffect.SetActive(true);
                //SimpleCoroutine.Instance.Delay(3f, () =>
                //{
                //    ScreenInfo screenInfo = new ScreenInfo();
                //    screenInfo.Add("score", _dataGamePlay.scores);
                //    screenInfo.Add("isClearBoard", true);
                //    screenInfo.Add("timeRemain", (int)InGameUI.Instance.timeRemain);
                //    screenInfo.Add("type", 1);

                //    UIManager.Instance.ShowPopup("Prefabs/GUI/FinishGame", screenInfo);
                //});
            }

            if (checkBall != null && (GamePlay.Instance.GameStatus == GameState.Playing))
            {
                checkBall.GetComponent<ball>().checkNearestColor();
                Destroy(checkBall.GetComponent<Rigidbody>());
                _dataGamePlay.moves--;
                Commons.SaveDataGamePlay(_dataGamePlay);
                checkBall = null;
                //connectNearBallsGlobal();
                int missCount = 1;
                if (stage >= 3) missCount = 2;
                if (stage >= 9) missCount = 1;
                //Invoke("destroyAloneBall", 0.5f);
                StartCoroutine(destroyAloneBall());

                if (!arcadeMode)
                {
                    if (bounceCounter >= missCount)
                    {
                        bounceCounter = 0;
                        dropDownTime = Time.time + 0.5f;
                        //Invoke("dropUp", 0.1f);
                    }
                    else
                    {
                        if (!destringAloneBall && !dropingDown)
                        {
                            //connectNearBallsGlobal();
                            destringAloneBall = true;
                        }
                    }
                }
            }

            if (arcadeMode && Time.time > ArcadedropDownTime && GamePlay.Instance.GameStatus == GameState.Playing)
            {
                bounceCounter = 0;
                ArcadedropDownTime = Time.time + 10f;
                dropDownTime = Time.time + 0.2f;
                dropDown();
            }



            if (Time.time > dropDownTime && dropDownTime != 0f)
            {
                //	dropingDown = false;
                CheckLosing();
                dropDownTime = 0;
                StartCoroutine(getBallsForMesh());
            }
        }

        public void CheckLosing()
        {
            var balls = GameObject.Find("Balls").transform;
            foreach (Transform obj in balls)
            {
                if (obj != null)
                    if (obj.transform.position.y < -2f && obj.GetComponent<ColorBallScript>().isShowBall)
                    {
                        _laser.AnimationName = "laser attack";
                        GamePlay.Instance.GameStatus = GameState.GameOver;

                        int gameOverType = 0;
                        int score = _dataGamePlay.scores;
                        int timeBonus = 0;
                        int clearBoard = 0;
                        int finalScore = score + timeBonus + clearBoard;
                        BubbesShotManager.Instance.SendEndGame(gameOverType, score, clearBoard, timeBonus, finalScore);

                        //InGameUI.Instance.gameover.gameObject.SetActive(true);
                        //SimpleCoroutine.Instance.Delay(2f, () =>
                        //{
                        //    ScreenInfo screenInfo = new ScreenInfo();
                        //    screenInfo.Add("score", _dataGamePlay.scores);
                        //    screenInfo.Add("isClearBoard", false);
                        //    screenInfo.Add("timeRemain", 0);
                        //    screenInfo.Add("type", 0);
                        //    UIManager.Instance.ShowPopup("Prefabs/GUI/FinishGame", screenInfo);
                        //});
                    }
            }
        }

        IEnumerator getBallsForMesh()
        {
            GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
            foreach (GameObject obj1 in meshes)
            {
                Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(obj1.transform.position, 0.1f, 1 << LayerMask.NameToLayer("Ball"));  //balls
                foreach (Collider2D obj in fixedBalls)
                {
                    obj1.GetComponent<Grid>().Busy = obj.gameObject;
                    obj.GetComponent<bouncer>().offset = obj1.GetComponent<Grid>().offset;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }


        public GameObject createFirstBall(Vector3 vector3)
        {
            GameObject gm = GameObject.Find("Creator");
            return gm.GetComponent<creatorBall>().createBall(vector3, BallColor.random, true);
        }

        public void connectNearBallsGlobal()
        {
            ///connect near balls
            fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject obj in fixedBalls)
            {
                if (obj.layer == LayerMask.NameToLayer("Ball") && obj.name.Contains("ball"))
                    obj.GetComponent<ball>().connectNearBalls();
            }
        }

        public void dropUp()
        {
            if (!dropingDown)
            {
                creatorBall.AddMesh();
                dropingDown = true;
                GameObject Meshes = GameObject.Find("-Meshes");
                iTween.MoveAdd(Meshes, iTween.Hash("y", 0.5f, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "OnMoveFinished"));

            }

        }

        void OnMoveFinished()
        {
            dropingDown = false;
        }

        public void dropDown()
        {

            dropingDown = true;
            fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject obj in fixedBalls)
            {
                if (obj.layer == LayerMask.NameToLayer("Ball"))
                    obj.GetComponent<bouncer>().dropDown();
            }
            GameObject gm = GameObject.Find("Creator");
            gm.GetComponent<creatorBall>().createRow(0);
            //	Invoke("destroyAloneBall", 1f);
            //	destroyAloneBall();
        }

        public void explode(GameObject gameObject)
        {
            //gameObject.GetComponent<Detonator>().Explode();
        }

        public IEnumerator destroyAloneBall()
        {
            mainscript.Instance.newBall2 = null;

            //if( GamePlay.Instance.GameStatus == GameState.Playing )
            //    mainscript.Instance.newBall = null;
            yield return new WaitForSeconds(Mathf.Clamp((float)countOfPreparedToDestroy / 50, 0.6f, (float)countOfPreparedToDestroy / 50));
            //       yield return new WaitForSeconds( 0.6f );
            int i;
            //	while(true){
            connectNearBallsGlobal();
            i = 0;
            int willDestroy = 0;
            destringAloneBall = true;
            Camera.main.GetComponent<mainscript>().arraycounter = 0;
            GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];         // detect alone balls
            Camera.main.GetComponent<mainscript>().controlArray.Clear();
            foreach (GameObject obj in fixedBalls)
            {
                if (obj != null)
                {
                    if (obj.layer == LayerMask.NameToLayer("Ball"))
                    {

                        if (!findInArray(Camera.main.GetComponent<mainscript>().controlArray, obj.gameObject))
                        {
                            if (obj.GetComponent<ball>().nearBalls.Count < 7 && obj.GetComponent<ball>().nearBalls.Count > 0)
                            {
                                i++;
                                //	if(i>5){ i = 0; yield return new WaitForSeconds(0.5f); yield return new WaitForSeconds(0.5f);}
                                //		if(dropingDown) yield return new WaitForSeconds(1f);
                                yield return new WaitForEndOfFrame();
                                ArrayList b = new ArrayList();
                                obj.GetComponent<ball>().checkNearestBall(b);
                                if (b.Count > 0)
                                {
                                    willDestroy++;
                                    destroy(b);
                                }
                            }
                        }
                    }
                }
            }
            destringAloneBall = false;
            StartCoroutine(getBallsForMesh());
            dropingDown = false;

            yield return new WaitForSeconds(0.0f);
            GetColorsInGame();
            mainscript.Instance.newBall = null;
            SetColorsForNewBall();
        }

        public void SetColorsForNewBall()
        {
            GameObject ball = null;
            if (boxCatapult.GetComponent<Grid>().Busy != null && colorsDict.Count > 0)
            {
                ball = boxCatapult.GetComponent<Grid>().Busy;
                BallColor color = ball.GetComponent<ColorBallScript>().mainColor;
                if (!colorsDict.ContainsValue(color))
                {
                    ball.GetComponent<ColorBallScript>().SetColor((BallColor)mainscript.colorsDict[Random.Range(0, mainscript.colorsDict.Count)]);
                }
            }
        }

        public void GetColorsInGame()
        {
            int i = 0;
            colorsDict.Clear();
            foreach (Transform item in Balls)
            {
                if (item.tag == "empty" || item.tag == "Ball") continue;
                BallColor col = (BallColor)System.Enum.Parse(typeof(BallColor), item.tag);
                if (!colorsDict.ContainsValue(col)
                    && (int)col <= (int)BallColor.random
                    && item.GetComponent<ColorBallScript>().isShowBall)
                {
                    colorsDict.Add(i, col);
                    i++;
                }
            }
        }

        public bool findInArray(ArrayList b, GameObject destObj)
        {
            foreach (GameObject obj in b)
            {

                if (obj == destObj) return true;
            }
            return false;
        }

        public void destroy(ArrayList b)
        {
            Camera.main.GetComponent<mainscript>().bounceCounter = 0;
            int scoreCounter = 0;
            int rate = 0;

            foreach (GameObject obj in b)
            {
                //			obj.GetComponent<OTSprite>().collidable = false;
                if (obj.name.IndexOf("ball") == 0) obj.layer = 0;
                if (!obj.GetComponent<ball>().Destroyed)
                {
                    if (scoreCounter > 3)
                    {
                        rate += 3;
                        scoreCounter += rate;
                    }
                    scoreCounter++;
                    obj.GetComponent<ball>().StartFall();
                }
            }
        }

        public void ChangeBubble()
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);
            Grid.waitForAnim = true;
            GameObject ball1 = boxSecond.GetComponent<Grid>().Busy;
            boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>().newBall = false;
            iTween.MoveTo(boxSecond.GetComponent<Grid>().Busy, iTween.Hash("position", boxCatapult.transform.position, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
            iTween.MoveTo(boxCatapult.GetComponent<Grid>().Busy, iTween.Hash("position", boxSecond.transform.position, "time", 0.3, "easetype", iTween.EaseType.linear));
            boxSecond.GetComponent<Grid>().Busy = boxCatapult.GetComponent<Grid>().Busy;
            boxCatapult.GetComponent<Grid>().Busy = ball1;
        }
    }
}


