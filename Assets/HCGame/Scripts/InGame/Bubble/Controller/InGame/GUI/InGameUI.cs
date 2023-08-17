using InitScriptName;
using System;
using System.Collections.Generic;
using BubblesShot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BubblesShot
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _score1;
        [SerializeField] private TextMeshProUGUI _score2;
        [SerializeField] private TextMeshProUGUI _playerName1;
        [SerializeField] private TextMeshProUGUI _playerName2;
        [SerializeField] private GameObject objPlayer2;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _itemBomb;
        [SerializeField] private TextMeshProUGUI _itemRocket;
        [SerializeField] private TextMeshProUGUI _itemColorful;
        [SerializeField] private TextMeshProUGUI _ready;
        [SerializeField] private TextMeshProUGUI _warning;
        [SerializeField] private GameObject _heart1;
        [SerializeField] private GameObject _heart2;
        [SerializeField] private GameObject _heart3;
        [SerializeField] private GameObject _menu;
        [SerializeField] private GameObject _endGame;
        [SerializeField] private GameObject _disconnect;
        [SerializeField] private GameObject _pauseGame;
        [SerializeField] private GameObject _timeout;
        [SerializeField] private GameObject _tutorial;
        [SerializeField] private GameObject _tutorialStep1;
        [SerializeField] private GameObject _tutorialStep2;
        [SerializeField] private GameObject _tutorialStep3;
        [SerializeField] private GameObject _tutorialStep4;
        [SerializeField] public GameObject gameover;

        public TMP_Text userName1;
        public Image userAvatar1;
        public TMP_Text userName2;
        public Image userAvatar2;

        public float timeRemain;
        int _heartNum;
        const int _heartMax = 3;
        public int newLineNum;
        float _timeCountdownReady = 4;
        float _timeCountdownDisconnect = 10;
        const int _totalTimeCoundown = 180;
        DateTime _timePauseStart = new DateTime();
        UserData _userData;
        DataGamePlay _dataGamePlay;
        public static bool isStart { get; set; }
        public static InGameUI Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            _userData = Commons.GetUserData();
        }
        // Start is called before the first frame update
        void Start()
        {
            _disconnect.SetActive(false);
            _dataGamePlay = Commons.GetDataGamePlay(true);
            _playerName1.text = "User1";
            _playerName2.text = "User2";
            _score2.text = "0";
            _menu.SetActive(false);
            _timeout.SetActive(false);
            gameover.SetActive(false);
            _endGame.SetActive(false);
            _pauseGame.SetActive(false);
            _warning.gameObject.SetActive(false);
            objPlayer2.SetActive(false);
            if (BubbesShotManager.Instance.currentMode == 2)
            {
                objPlayer2.SetActive(true);
            }
            HCAppController.Instance.LoadUserInfoInGame(HCAppController.Instance.findingRoomResponse, userName1, userAvatar1, userName2, userAvatar2);
        }
        public void StartGame()
        {
            timeRemain = _totalTimeCoundown;
            isStart = false;
            _heartNum = _heartMax;
            newLineNum = 0;
            UpdateHeartUI();
        }
        // Update is called once per frame
        void Update()
        {
            //Check connection
            if (!Commons.IsConnectionNetwork())
            {
                if (_timeCountdownDisconnect > 0)
                {
                    _disconnect.SetActive(true);
                    _timeCountdownDisconnect -= Time.deltaTime;
                    GamePlay.Instance.GameStatus = GameState.Pause;
                    return;
                }
                else
                {
                    _disconnect.SetActive(false);
                    GamePlay.Instance.GameStatus = GameState.GameOver;
                    int gameOverType = 2;
                    int score = _dataGamePlay.scores;
                    int timeBonus = 0;
                    int clearBoard = 0;
                    int finalScore = score + timeBonus + clearBoard;
                    BubbesShotManager.Instance.SendEndGame(gameOverType, score, clearBoard, timeBonus, finalScore);
                    SceneManager.LoadScene("Home");
                    return;
                }
            }
            else
            {
                if (_timeCountdownDisconnect != 10 && _timeCountdownReady < 0)
                {
                    _disconnect.SetActive(false);
                    _timeCountdownDisconnect = 10;
                    GamePlay.Instance.GameStatus = GameState.Playing;
                    _timeout.SetActive(false);
                }
            }

            if (GamePlay.Instance.GameStatus == GameState.None)
                return;
            UpdateCounterUI();



            //Time countdown
            _timeCountdownReady -= Time.deltaTime;
            if (_timeCountdownReady > 0)
            {
                _ready.text = ((int)_timeCountdownReady).ToString();
                GamePlay.Instance.GameStatus = GameState.Pause;
            }
            else if (_timeCountdownReady <= 0 && _timeCountdownReady >= -1)
            {
                _ready.text = "Ready!";
                if (SessionPref.GetTutorial())
                    ShowTutorial();
                else
                {
                    GamePlay.Instance.GameStatus = GameState.Playing;
                }

            }
            else
            {
                _ready.gameObject.SetActive(false);
                if (isStart)
                {
                    if (timeRemain <= _totalTimeCoundown && timeRemain >= 0 && GamePlay.Instance.GameStatus != GameState.GameOver)
                    {
                        timeRemain -= Time.deltaTime;
                        _time.text = Commons.ConvertIntToTimeStr(ConvertTimeType.mmss, timeRemain);
                        if (timeRemain < 10)
                            _time.color = Color.red;
                        if ((int)timeRemain == 0)
                        {
                            GamePlay.Instance.GameStatus = GameState.GameOver;
                            _timeout.SetActive(true);

                            int gameOverType = 2;
                            int score = _dataGamePlay.scores;
                            int timeBonus = 0;
                            int clearBoard = 0;
                            int finalScore = score + timeBonus + clearBoard;
                            BubbesShotManager.Instance.SendEndGame(gameOverType, score, clearBoard, timeBonus, finalScore);

                            //SimpleCoroutine.Instance.Delay(2f, () =>
                            //{
                            //    ScreenInfo screenInfo = new ScreenInfo();
                            //    screenInfo.Add("score", _dataGamePlay.scores);
                            //    screenInfo.Add("isClearBoard", false);
                            //    screenInfo.Add("timeRemain", 0);
                            //    screenInfo.Add("type", 2);
                            //    UIManager.Instance.ShowPopup("Prefabs/GUI/FinishGame", screenInfo);
                            //});
                        }
                    }
                }
            }
        }
        public void UpdateScorePlayer2(int scorePlayer2)
        {
            _score2.text = scorePlayer2.ToString();
        }
        public void UpdateCounterUI()
        {
            _dataGamePlay = Commons.GetDataGamePlay();
            //
            _score1.text = _dataGamePlay.scores.ToString();
            //WebSocket - Update score of Comertitor here
            //_score2.text = "0";
            //
            string itemBomb = GetPlus(_dataGamePlay.itemBomb);
            _itemBomb.text = itemBomb;
            if (_dataGamePlay.itemBomb > 0)
                _itemBomb.color = Color.white;
            else
                _itemBomb.color = Color.red;
            //
            string itemColorFull = GetPlus(_dataGamePlay.itemColorful);
            _itemColorful.text = itemColorFull;
            if (_dataGamePlay.itemColorful > 0)
                _itemColorful.color = Color.white;
            else
                _itemColorful.color = Color.red;
            //
            //string itemRocket = GetPlus(_userData.itemRocket);
            //_itemRocket.text = itemRocket;
        }
        string GetPlus(int boostCount)
        {
            if (boostCount > 0)
                return "" + boostCount;
            else
                return "-1000";
        }
        public void OnClickMenu()
        {
            GamePlay.Instance.GameStatus = GameState.WaitForPopup;
            _menu.SetActive(true);
        }
        public void OnClickTutorial()
        {
            GamePlay.Instance.GameStatus = GameState.WaitForPopup;
            UIManager.Instance.ShowPopup("Prefabs/GUI/Tutorials");
            _menu.SetActive(false);
        }
        public void OnClickEndGame()
        {
            GamePlay.Instance.GameStatus = GameState.WaitForPopup;
            _menu.SetActive(false);
            _endGame.SetActive(true);
        }
        public void OnClickEndNow()
        {
            _menu.SetActive(false);
            _endGame.SetActive(false);
            GamePlay.Instance.GameStatus = GameState.GameOver;

            int gameOverType = 0;
            int score = _dataGamePlay.scores;
            int timeBonus = 0;
            int clearBoard = 0;
            int finalScore = score + timeBonus + clearBoard;
            BubbesShotManager.Instance.SendEndGame(gameOverType, score, clearBoard, timeBonus, finalScore);

            //SceneManager.LoadScene("Home");
            //ScreenInfo screenInfo = new ScreenInfo();
            //screenInfo.Add("score", _dataGamePlay.scores);
            //screenInfo.Add("isClearBoard", false);
            //screenInfo.Add("timeRemain", 0);
            //screenInfo.Add("type", 0);
            //UIManager.Instance.ShowPopup("Prefabs/GUI/FinishGame", screenInfo);
        }
        public void OnClickCloseMenu()
        {
            _menu.SetActive(false);
            _endGame.SetActive(false);
            _pauseGame.SetActive(false);
            SimpleCoroutine.Instance.Delay(.7f, () =>
            {
                GamePlay.Instance.GameStatus = GameState.Playing;
            });
        }
        public void OnClickContinue()
        {
            OnClickCloseMenu();
        }

        public void OnClickItemBomb()
        {
            if (_dataGamePlay.itemBomb > 0)
            {
                SpendBoost(BoostType.ItemBomb);
            }
            else
            {
                if (_dataGamePlay.scores >= 1000)
                {
                    _dataGamePlay.scores -= 1000;
                    SpendBoost(BoostType.ItemBomb);
                }
                else
                {
                    _warning.text = "you need 1000 points to use this item";
                    _warning.gameObject.SetActive(true);
                    SimpleCoroutine.Instance.Delay(2f, () =>
                    {
                        _warning.gameObject.SetActive(false);
                    });
                }
            }
        }
        public void OnClickItemColorful()
        {
            if (_dataGamePlay.itemColorful > 0)
            {
                SpendBoost(BoostType.ItemColorful);
            }
            else
            {
                if (_dataGamePlay.scores >= 1000)
                {
                    _dataGamePlay.scores -= 1000;
                    SpendBoost(BoostType.ItemColorful);
                }
                else
                {
                    _warning.text = "you need 1000 points to use this item";
                    _warning.gameObject.SetActive(true);
                    SimpleCoroutine.Instance.Delay(2f, () =>
                    {
                        _warning.gameObject.SetActive(false);
                    });
                }
            }
        }
        public void OnClickItemRocket()
        {
            if (_dataGamePlay.itemRocket > 0)
            {
                SpendBoost(BoostType.ItemRocket);
            }
        }
        public void UpdateBoostItem(int itemBomb, int itemRocket, int itemColorfull)
        {
            _dataGamePlay.itemBomb += itemBomb;
            _dataGamePlay.itemColorful += itemColorfull;
            _dataGamePlay.itemRocket += itemRocket;
            Commons.SaveDataGamePlay(_dataGamePlay);
            UpdateCounterUI();
        }

        public void SpendBoost(BoostType boostType)
        {
            InitScript.Instance.BoostActivated = true;
            mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>().SetBoost(boostType);
            mainscript.boostType = boostType;
        }
        public void UpdateHeartUI(bool isRemove = false)
        {
            if (isRemove)
                _heartNum--;
            switch (_heartNum)
            {
                case 3:
                    _heart1.SetActive(true);
                    _heart2.SetActive(true);
                    _heart3.SetActive(true);
                    break;
                case 2:
                    _heart1.SetActive(true);
                    _heart2.SetActive(true);
                    _heart3.SetActive(false);
                    break;
                case 1:
                    _heart1.SetActive(true);
                    _heart2.SetActive(false);
                    _heart3.SetActive(false);
                    break;
                case 0:
                    _heart1.SetActive(true);
                    _heart2.SetActive(true);
                    _heart3.SetActive(true);
                    _heartNum = _heartMax;
                    newLineNum++;
                    //Update new line of bubble
                    creatorBall.Instance.MoveMeshDownOneLine();
                    //Show new line of ball
                    var balls = GameObject.Find("Balls").transform;
                    List<Transform> ballsHiden = new List<Transform>();
                    foreach (Transform ball in balls)
                    {
                        if (!ball.GetComponent<ColorBallScript>().isShowBall)
                            ballsHiden.Add(ball);
                    }
                    var newBallHidenNum = ballsHiden.Count - (10 + ((newLineNum - 1) % 2));
                    int idx = 0;
                    foreach (Transform ball in ballsHiden)
                    {
                        idx++;
                        if (idx > newBallHidenNum)
                            ball.GetComponent<ColorBallScript>().SetShowHide(true);
                    }
                    break;
            }
        }
        public void ShowTutorial()
        {
            //Tutorial
            if (SessionPref.GetTutorial())
            {
                _tutorial.SetActive(true);
                _tutorialStep1.SetActive(true);
                _tutorialStep2.SetActive(false);
                GamePlay.Instance.GameStatus = GameState.WaitForPopup;
            }
            else
                _tutorial.SetActive(false);
        }
        public void OnClickStep1()
        {
            _tutorialStep1.SetActive(false);
            _tutorialStep2.SetActive(true);
            _tutorialStep3.SetActive(false);
            _tutorialStep4.SetActive(false);
            GamePlay.Instance.GameStatus = GameState.Playing;
        }
        public void OnClickStep2()
        {
            _tutorialStep1.SetActive(false);
            _tutorialStep2.SetActive(false);
            _tutorialStep3.SetActive(true);
            _tutorialStep4.SetActive(false);
        }
        public void OnClickStep3()
        {
            _tutorialStep1.SetActive(false);
            _tutorialStep2.SetActive(false);
            _tutorialStep3.SetActive(false);
            _tutorialStep4.SetActive(true);
        }
        public void OnClickTryAgainTut()
        {
            Tutorials.Instance.OnClickTutorialInHomeMenu();
        }
        public void OnClickPlayGame()
        {
            SceneManager.LoadScene("Home");
        }

        //Check pause game
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _timePauseStart = DateTime.Now;
            }
            else
            {
                if (DateTime.Now >= _timePauseStart.AddMinutes(15))
                {
                    //Send end game
                    int gameOverType = 0;
                    int score = _dataGamePlay.scores;
                    int timeBonus = 0;
                    int clearBoard = 0;
                    int finalScore = score + timeBonus + clearBoard;
                    BubbesShotManager.Instance.SendEndGame(gameOverType, score, clearBoard, timeBonus, finalScore);
                }
                else
                {
                    //Return game
                    _pauseGame.SetActive(true);
                }
            }
        }
    }
}
