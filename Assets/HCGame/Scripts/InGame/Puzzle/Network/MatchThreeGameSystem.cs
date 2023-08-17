using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using RoyalMatch;
using Google.Protobuf;
using RoyalMatch;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using BestHTTP.JSON;

namespace MiniGame.MatchThree.Scripts.Network
{
    public class MatchThreeGameSystem : MonoBehaviour
    {
        public static MatchThreeGameSystem Instance;
        private void Awake()
        {
            Instance = this;
        }
        public ulong UserID { get; private set; }

        public Action<RoyalMatch.DataGame> ActStartGame;

        public void PlayGame()
        {
            UserID = (ulong)DateTime.Now.Ticks;
        }


        public TextMeshProUGUI sc_text, cs_text;
        public int sc_count1;
        public int cs_count2
        {
            get { return ___cs_2; }
            set
            {
                ___cs_2 = value;
                cs_text.text = "CS: " + ___cs_2;
            }
        }
        [SerializeField] public int ___cs_2;
        public string jsonString, jsonString2;

        public DataGame _data;
        public bool isGameStarted;
        public RoyalMatch.DataGame originData;
        public void OnStartGame(RoyalMatch.DataGame data)
        {
            originData = data;
            StartCoroutine(OnStartGameIE());
            IEnumerator OnStartGameIE()
            {

                Debug.LogWarning(data.Grid);
                if (isGameStarted == false)
                {
                    isGameStarted = true;
                    yield return new WaitForSeconds(.001f);
                    MatchThreeNetworkManager.Instance.SendRequestAction(ROYAL_MATCH_TYPE.START_COUNT_TIME);
                }

                sc_count1++;

                sc_text.text = "SC: " + sc_count1;
                jsonString2 = data.ToString();




                _data = JsonUtility.FromJson<DataGame>(jsonString2);


                Application.targetFrameRate = 60;
                string receiveData = $"type: {(ROYAL_MATCH_STATUS_PLAY)data.Status} \n" +
                         $"status: {data.Status} \n" +
                         $"grid: {data.Grid} \n" +
                         $"nextGrid: {data.NextGrid} \n" +
                         $"canClear: {data.CanClear} \n" +
                         $"points: {data.Points} \n" +
                         $"needReset: {data.NeedReset} \n" +
                         $"numberSwap : {data.NumberSwap} \n";
                Debug.Log("receive Data : " + receiveData);
                //MatchThreeNetworkManager.Instance.OnDebugLog?.Invoke("receive Data : " + receiveData);
                ActStartGame?.Invoke(data);

                Debug.Log("___JSONDATA:____" + data.JsonData);


                _puzzleDataJson = JsonUtility.FromJson<PuzzleDataJson>(data.JsonData);



                //execute all new function
                if (_puzzleDataJson.specialTransform.Count > 0)
                {
                    NetWorkBoard.instance.SpawnSpecialTransform(_puzzleDataJson.specialTransform);
                    JObject jobj = JObject.Parse(data.JsonData.ToString());
                    Debug.Log("SpecialTransform: " + jobj["specialTransform"].ToString());
                }

                Match3_BotBar.Instance.SetShuffleNum(_puzzleDataJson);
                Match3_TopPanel.Instance.CountDownTime(_puzzleDataJson);
                Match3_TopPanel.Instance.SynchGameTarget(_puzzleDataJson);

                ParseFloorGrid();
                NetWorkBoard.instance.SynchFloorGrid();



                //JObject tmp = JObject.Parse(data.JsonData.ToString());
                //if (tmp["recomment"] != null)
                //{
                //    _recomment.actionType = int.Parse(tmp["recomment"]["actionType"].ToString());
                //    _recomment.posY = int.Parse(tmp["recomment"]["row"].ToString());
                //    _recomment.posX = int.Parse(tmp["recomment"]["col"].ToString());
                //}


                yield return null;
            }
        }

        public void WinResult()
        {
            Debug.Log("receive Data : WINNER");
            MatchThreeNetworkManager.Instance.OnDebugLog?.Invoke("receive Data : WINNER");
            //SceneManager.LoadScene("M3_WinResult");
        }


        public void LoseResult()
        {
            Debug.Log("receive Data : LOSER" + "ROYAL_MATCH_STATUS_PLAY.FINAL_ROUND");
            MatchThreeNetworkManager.Instance.OnDebugLog?.Invoke("receive Data : LOSER");

            var gameoverPuzzleData = new GameOverPuzzleData();
            gameoverPuzzleData.gameOverType = 3;
            gameoverPuzzleData.finalScore = Int32.Parse(Match3_TopPanel.Instance.user1ScoreText.text);
            GameOverPuzzle.instance.LoadData(gameoverPuzzleData);
        }

        [Button]
        public void BackHC()
        {
            MatchThreeNetworkManager.Instance.Disconnect();
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Home");
        }

        [Button]
        public void SendRequestAction(ROYAL_MATCH_TYPE type, int row = 0, int col = 0)
        {
            Debug.Log("match 3 SendRequestAction() " + type);
            RoyalMatchAction action = new RoyalMatchAction()
            {
                Type = (int)type,
                Row = 0,
                Col = 0,
            };
            PackageData packageData = new PackageData();
            packageData.Header = (uint)PACKAGE_HEADER.ROYAL_MATCH_ACTION;
            packageData.Data = action.ToByteString();
            MatchThreeNetworkManager.Instance.SendBinaryData(packageData.ToByteArray());
        }

        public PuzzleDataJson _puzzleDataJson;



        //[Button]
        //public void LogGriaad(string data)
        //{

        //    var ob = JObject.Parse(_puzzleDataJson.ToString());
        //    Debug.LogError(floorGrid[0]);
        //    Debug.LogError(floorGrid[0][0]);

        //}



        [Button]
        public void ParseFloorGrid()
        {

            var ob = JObject.Parse(originData.JsonData.ToString());

            var a = JArray.Parse(ob["floorGrid"].ToString());

            floorGridCol0 = a[0].ToObject<List<int>>();
            floorGridCol1 = a[1].ToObject<List<int>>();
            floorGridCol2 = a[2].ToObject<List<int>>();
            floorGridCol3 = a[3].ToObject<List<int>>();
            floorGridCol4 = a[4].ToObject<List<int>>();
            floorGridCol5 = a[5].ToObject<List<int>>();
            floorGridCol6 = a[6].ToObject<List<int>>();
            floorGridCol7 = a[7].ToObject<List<int>>();
            floorGridCol8 = a[8].ToObject<List<int>>();

            //Debug.LogError(ob["floorGrid"][1]);
            //Debug.LogError(ob["floorGrid"][2]);
            //Debug.LogError(ob["floorGrid"][3]);
        }

        [Button]
        public void LogGriaad3(string data)
        {

            //Debug.LogError(ob["floorGrid"].Values);

            Debug.LogError(_puzzleDataJson.floorGrid[0][1]);
            Debug.LogError(_puzzleDataJson.tes[0, 1]);
        }


        public List<int> floorGridCol0, floorGridCol1, floorGridCol2, floorGridCol3, floorGridCol4, floorGridCol5, floorGridCol6, floorGridCol7, floorGridCol8, floorGridCol9, floorGridCol10;
        public Recomment _recomment;



        public float offsetScale;
        public float offsetScaleTime;
        [Button]
        public void ShowRecomment()
        {
            var pieceMove = NetWorkBoard.instance.GetPieceAt(_recomment.posX, _recomment.posY);
            pieceMove.ScaleHint();
            pieceMove.MoveHint(_recomment.actionType);
        }



        [Button]
        public void VerifyBoard(int x, int y)
        {
            var a = Check1(x, y);
            var b = Check2(x, y);

            if (a != b)
                Debug.LogError("ERRRRORRR:  ___  vry  x: " + x + "_y: " + y + "___:::" + a + "==" + b);


        }

        [Button]
        public int Check1(int x, int y)
        {
            var a = _data.grid[y].row[x];
            return a;

        }

        [Button]
        public int Check2(int x, int y)
        {

            var a = Int32.Parse(NetWorkBoard.instance.GetPieceAt(x, y).txtNumber.text);

            return a;
        }


        [Button]
        public void veryfiAll()
        {
            for (int x = 0; x <= 8; x++)
            {
                for (int y = 0; y <= 10; y++)
                {
                    VerifyBoard(x, y);

                }
            }
        }


    }





}
[Serializable]
public class PuzzleDataJson
{
    public int numberShuffle;
    public int timePlay;
    public int round;
    public List<ItemSpecial> specialSpawn;
    public List<Target> target;

    public List<specialTransform> specialTransform;



    public int[][] floorGrid;
    public int[,] tes;

}
[Serializable]
public class ItemSpecial
{
    public int x;
    public int y;
    public int value;
}
[Serializable]
public class Target
{
    public int type;
    public int valueTarget;
    public int amount;
}
[Serializable]

public class specialTransform
{
    public position position;
    public int value;
}
[Serializable]
public class position
{
    public int x;
    public int y;
}

[Serializable]
public class Recomment
{
    public int actionType;
    public int posX;
    public int posY;
}
