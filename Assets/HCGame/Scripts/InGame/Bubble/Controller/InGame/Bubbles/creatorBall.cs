using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = System.Random;

namespace BubblesShot
{
    public class creatorBall : MonoBehaviour
    {
        public static creatorBall Instance;
        public GameObject _meshes;
        public GameObject _balls;
        public GameObject _topBorder;
        public GameObject _ball;
        public GameObject _box;
        string[] ballsForCatapult = new string[11];
        string[] ballsForMatrix = new string[11];
        string[] bugs = new string[11];
        public static int columns = 11;
        public static int rows = 70;
        public static List<Vector2> grid = new List<Vector2>();
        int lastRow;
        float offsetStep = 0.33f;
        //private OTSpriteBatch spriteBatch = null;  
        GameObject Meshes;
        [HideInInspector]
        public List<GameObject> squares = new List<GameObject>();
        int[] map;
        private int maxCols = 11;
        private int maxRows;
        private int hidenLineNum;
        // Use this for initialization
        void Start()
        {
            Instance = this;
        }
        public void StartGame()
        {
            _box.transform.localScale = new Vector3(0.67f, 0.58f, 1);
            Meshes = _balls;
            //Gen ball when start game
            if (SessionPref.GetTutorial())
                ProcessGameDataFromString(AutoGenBalls(11, 20));
            else
            {
                // Call ball list from server to sync with other user in room
                ProcessGameDataFromString(AutoGenBalls(11, 20));
            }
            MoveLevelUp();
            createMesh();
            LoadMap(LevelData.map);
            Camera.main.GetComponent<mainscript>().connectNearBallsGlobal();
            StartCoroutine(getBallsForMesh());
        }
        //Auto gen ball
        public string AutoGenBalls(int colls, int rows)
        {
            var strBalls = string.Empty;
            int halfOfRows = rows / 2;
            int ballsTotal = halfOfRows * 10 + halfOfRows * 11;
            bool isEvenLine = false;
            int idx = 0;
            if (rows % 2 != 0)
                ballsTotal += 11;
            for (int i = 0; i < ballsTotal; i++)
            {
                idx++;
                Random r = new Random();
                int rInt = r.Next(1, 6);
                strBalls += rInt.ToString();
                if (isEvenLine && idx == colls - 1)
                {
                    isEvenLine = false;
                    idx = 0;
                    strBalls += "\n";
                }
                else if (!isEvenLine && idx == colls)
                {
                    isEvenLine = true;
                    idx = 0;
                    strBalls += "\n";
                }
                else
                {
                    strBalls += ",";
                }
            }
            return strBalls;
        }
        void ProcessGameDataFromString(string mapText)
        {
            string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            hidenLineNum = lines.Count() - 8;
            LevelData.colorsDict.Clear();
            int mapLine = 0;
            int key = 0;
            foreach (string line in lines)
            {
                //Maps
                //Split lines again to get map numbers
                string[] st = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    int value = int.Parse(st[i][0].ToString());
                    if (!LevelData.colorsDict.ContainsValue((BallColor)value) && value > 0 && value < (int)BallColor.random)
                    {
                        LevelData.colorsDict.Add(key, (BallColor)value);
                        key++;

                    }

                    LevelData.map[mapLine * maxCols + i] = int.Parse(st[i][0].ToString());
                }
                mapLine++;

            }
        }

        public void LoadMap(int[] pMap)
        {
            map = pMap;
            int key = -1;
            int roww = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int mapValue = map[i * columns + j];
                    if (mapValue > 0)
                    {
                        roww = i;
                        bool isShowBall = roww >= hidenLineNum;
                        createBall(GetSquare(roww, j).transform.position, (BallColor)mapValue, false, i, isShowBall);
                    }
                }
            }
        }

        private void MoveLevelUp()
        {
            StartCoroutine(MoveUpDownCor());
        }

        IEnumerator MoveUpDownCor(bool inGameCheck = false)
        {
            yield return new WaitForSeconds(0.1f);
            if (!inGameCheck)
                GamePlay.Instance.GameStatus = GameState.BlockedGame;
            bool up = false;
            List<float> table = new List<float>();
            float lineY = -1.3f;
            Transform bubbles = _balls.transform;
            int i = 0;
            foreach (Transform item in bubbles)
            {
                if (!inGameCheck)
                {
                    if (item.position.y < lineY)
                    {
                        table.Add(item.position.y);
                    }
                }
                else if (!item.GetComponent<ball>().Destroyed)
                {
                    if (item.position.y > lineY && mainscript.Instance.TopBorder.transform.position.y > 5f)
                    {
                        table.Add(item.position.y);
                    }
                    else if (item.position.y < lineY + 1f)
                    {
                        table.Add(item.position.y);
                        up = true;
                    }
                }
                i++;
            }


            if (table.Count > 0)
            {
                if (up) AddMesh();

                float targetY = 0;
                table.Sort();
                if (!inGameCheck) targetY = lineY - table[0] + 2.3f;
                else targetY = lineY - table[0] + 1.5f;
                GameObject Meshes = _meshes;
                Vector3 targetPos = Meshes.transform.position + Vector3.up * targetY;
                float startTime = Time.time;
                Vector3 startPos = Meshes.transform.position;
                float speed = 0.5f;
                float distCovered = 0;
                while (distCovered < 1)
                {
                    speed += Time.deltaTime / 1.5f;
                    distCovered = (Time.time - startTime) / speed;
                    Meshes.transform.position = Vector3.Lerp(startPos, targetPos, distCovered);
                    yield return new WaitForEndOfFrame();
                    if (startPos.y > targetPos.y)
                    {
                        if (mainscript.Instance.TopBorder.transform.position.y <= 5 && inGameCheck) break;
                    }
                }
            }
        }

        public void MoveMeshDownOneLine()
        {
            StartCoroutine(MoveMeshDownOneLineCor());
        }
        IEnumerator MoveMeshDownOneLineCor()
        {
            yield return new WaitForSeconds(0.1f);

            GameObject Meshes = _meshes;
            Vector3 targetPos = Meshes.transform.position + Vector3.up * -.575f;
            float startTime = Time.time;
            Vector3 startPos = Meshes.transform.position;
            float speed = 0.5f;
            float distCovered = 0;
            while (distCovered < 1)
            {
                speed += Time.deltaTime / 1.5f;
                distCovered = (Time.time - startTime) / speed;
                Meshes.transform.position = Vector3.Lerp(startPos, targetPos, distCovered);
                yield return new WaitForEndOfFrame();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator getBallsForMesh()
        {
            GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
            foreach (GameObject obj1 in meshes)
            {
                Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(obj1.transform.position, 0.2f, 1 << LayerMask.NameToLayer("Ball"));  //balls
                foreach (Collider2D obj in fixedBalls)
                {
                    obj1.GetComponent<Grid>().Busy = obj.gameObject;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

        public void EnableGridColliders()
        {
            foreach (GameObject item in squares)
            {
                item.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
        public void OffGridColliders()
        {
            foreach (GameObject item in squares)
            {
                item.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        public void createRow(int j)
        {
            float offset = 0;
            GameObject gm = GameObject.Find("Creator");
            for (int i = 0; i < columns; i++)
            {
                if (j % 2 == 0) offset = 0; else offset = offsetStep;
                Vector3 v = new Vector3(transform.position.x + i * _box.transform.localScale.x + offset, transform.position.y - j * _box.transform.localScale.y, transform.position.z);
                createBall(v);
            }
        }

        public GameObject createBall(Vector3 vec, BallColor color = BallColor.random, bool newball = false, int row = 1, bool isShowBall = true)
        {
            GameObject b = null;
            List<BallColor> colors = new List<BallColor>();

            for (int i = 1; i < System.Enum.GetValues(typeof(BallColor)).Length; i++)
            {
                colors.Add((BallColor)i);
            }

            if (color == BallColor.random)
                color = (BallColor)LevelData.colorsDict[UnityEngine.Random.Range(0, LevelData.colorsDict.Count)];
            if (newball && mainscript.colorsDict.Count > 0)
            {
                if (GamePlay.Instance.GameStatus == GameState.Playing)
                {
                    mainscript.Instance.GetColorsInGame();
                    color = (BallColor)mainscript.colorsDict[UnityEngine.Random.Range(0, mainscript.colorsDict.Count)];
                }
                else
                    color = (BallColor)LevelData.colorsDict[UnityEngine.Random.Range(0, LevelData.colorsDict.Count)];

            }

            b = Instantiate(_ball, transform.position, transform.rotation) as GameObject;
            b.transform.position = new Vector3(vec.x, vec.y, _ball.transform.position.z);
            b.GetComponent<ColorBallScript>().SetColor(color);
            b.GetComponent<ColorBallScript>().SetShowHide(isShowBall);
            b.transform.parent = Meshes.transform;
            b.tag = "" + color;

            GameObject[] fixedBalls = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            b.name = b.name + fixedBalls.Length.ToString();
            if (newball)
            {

                b.gameObject.layer = LayerMask.NameToLayer("NewBall");
                b.transform.parent = Camera.main.transform;
                Rigidbody2D rig = b.AddComponent<Rigidbody2D>();
                b.GetComponent<CircleCollider2D>().enabled = false;
                rig.gravityScale = 0;
                if (GamePlay.Instance.GameStatus == GameState.Playing)
                    b.GetComponent<Animation>().Play();
            }
            else
            {
                b.GetComponent<ball>().enabled = false;
                //if (LevelData.mode == ModeGame.Vertical && row == 0)
                b.GetComponent<ball>().isTarget = true;
                b.GetComponent<BoxCollider2D>().offset = Vector2.zero;
                b.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
            }
            return b.gameObject;
        }

        public void createMesh()
        {
            GameObject Meshes = _meshes;
            float offset = 0;

            for (int j = 0; j < rows + 1; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    if (j % 2 == 0) offset = 0; else offset = offsetStep;
                    GameObject b = Instantiate(_box, transform.position, transform.rotation) as GameObject;
                    Vector3 v = new Vector3(transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z);
                    b.transform.parent = Meshes.transform;
                    b.transform.localPosition = v;
                    GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag("Mesh");
                    b.name = b.name + fixedBalls.Length.ToString();
                    b.GetComponent<Grid>().offset = offset;
                    squares.Add(b);
                    lastRow = j;
                }
            }
            creatorBall.Instance.OffGridColliders();
        }

        public void AddMesh()
        {
            GameObject Meshes = _meshes;
            float offset = 0;
            int j = lastRow + 1;
            for (int i = 0; i < columns; i++)
            {
                if (j % 2 == 0) offset = 0; else offset = offsetStep;
                GameObject b = Instantiate(_box, transform.position, transform.rotation) as GameObject;
                Vector3 v = new Vector3(transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z);
                b.transform.parent = Meshes.transform;
                b.transform.position = v;
                GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag("Mesh");
                b.name = b.name + fixedBalls.Length.ToString();
                b.GetComponent<Grid>().offset = offset;
                squares.Add(b);
            }
            lastRow = j;

        }

        public GameObject GetSquare(int row, int col)
        {
            return squares[row * columns + col];
        }
    }
}
