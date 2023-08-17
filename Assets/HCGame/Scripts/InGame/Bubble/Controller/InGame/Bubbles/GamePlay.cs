using UnityEngine;
using System.Collections;
using InitScriptName;
using BubblesShot;

namespace BubblesShot
{
    public enum GameState
    {
        None,
        Playing,
        Highscore,
        GameOver,
        Pause,
        Win,
        WaitForPopup,
        WaitAfterClose,
        BlockedGame,
        Tutorial
    }


    public class GamePlay : MonoBehaviour
    {
        public static GamePlay Instance { get; private set; }
        private GameState gameStatus;
        bool winStarted;
        private DataGamePlay _dataGamePlay;
        public GameState GameStatus
        {
            get { return GamePlay.Instance.gameStatus; }
            set
            {
                if (GamePlay.Instance.gameStatus != value)
                {
                    if (value == GameState.Win)
                    {
                        if (!winStarted)
                            StartCoroutine(WinAction());
                    }
                    else if (value == GameState.GameOver)
                    {
                        StartCoroutine(GameOverAction());
                    }
                    else if (value == GameState.Tutorial && gameStatus != GameState.Playing)
                    {
                        value = GameState.Playing;
                        gameStatus = value;
                        //  ShowTutorial();
                    }
                }
                if (value == GameState.WaitAfterClose)
                    StartCoroutine(WaitAfterClose());

                if (value == GameState.Tutorial)
                {
                    if (gameStatus != GameState.Playing)
                        GamePlay.Instance.gameStatus = value;
                }
                GamePlay.Instance.gameStatus = value;

                if (value == GameState.Playing)
                {
                    InGameUI.isStart = true;
                }
            }
        }

        // Use this for initialization
        void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            _dataGamePlay = Commons.GetDataGamePlay();
        }
        void Update()
        {
        }

        // Update is called once per frame
        IEnumerator WinAction()
        {
            winStarted = true;
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.winSound);
            yield return new WaitForSeconds(1f);

            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Ball"))
            {
                item.GetComponent<ball>().StartFall();

            }
            // StartCoroutine( PushRestBalls() );
            Transform b = GameObject.Find("Ball").transform;
            ball[] balls = GameObject.Find("Ball").GetComponentsInChildren<ball>();
            foreach (ball item in balls)
            {
                item.StartFall();
            }

            while (_dataGamePlay.moves > 0)
            {
                if (mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy != null)
                {
                    _dataGamePlay.moves--;
                    ball ball = mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>();
                    mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy = null;
                    ball.transform.parent = mainscript.Instance.Balls;
                    ball.tag = "Ball";
                    Commons.SaveDataGamePlay(_dataGamePlay);
                }
                yield return new WaitForEndOfFrame();
            }
            foreach (ball item in balls)
            {
                if (item != null)
                    item.StartFall();
            }
            yield return new WaitForSeconds(2f);
            while (GameObject.FindGameObjectsWithTag("Ball").Length > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.aplauds);
            if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", mainscript.Instance.currentLevel), 0) < mainscript.Instance.stars)
                PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", mainscript.Instance.currentLevel), mainscript.Instance.stars);


            if (PlayerPrefs.GetInt("Score" + mainscript.Instance.currentLevel) < _dataGamePlay.scores)
            {
                PlayerPrefs.SetInt("Score" + mainscript.Instance.currentLevel, _dataGamePlay.scores);

            }
            //GameObject.Find( "InGameUI" ).transform.Find( "LevelCleared" ).gameObject.SetActive( false );
            GameObject.Find("InGameUI").transform.Find("MenuComplete").gameObject.SetActive(true);
        }
        IEnumerator GameOverAction()
        {
            //
            yield return new WaitForSeconds(0.1f);
            //ScreenInfo screenInfo = new ScreenInfo();
            //screenInfo.Add("score", _dataGamePlay.scores.ToString());
            //UIManager.Instance.ShowPopup("Prefabs/GUI/FinishGame", screenInfo);
        }

        IEnumerator WaitAfterClose()
        {
            Debug.Log("WaitAfterClose");
            yield return new WaitForSeconds(5);
            GameStatus = GameState.Playing;
        }
    }

}