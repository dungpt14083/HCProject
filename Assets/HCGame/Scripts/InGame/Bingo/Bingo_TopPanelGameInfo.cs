using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;

namespace Bingo
{
    public class Bingo_TopPanelGameInfo : MonoBehaviour
    {
        public static Bingo_TopPanelGameInfo instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }


        public void OnReceivedNewWebsocketMessage_UpdateInfo(BingoGameData bingoDataParam)
        {
            SetScore(bingoDataParam.currentScore);
        }


        [TitleGroup("___________  Reference  __________")]
        public GameObject scoreEff, newScorePos;

        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timeText, timeText_Shadow;
        public TextMeshProUGUI enemyScore;


        [TitleGroup("___________  Check var  __________")]
        public bool isCountinng = false;

        public int currentTime;

        [SerializeField] Bingo_PopupScoreEffect scoreEffect;


        [Button]
        public void SetScore(string newScoreText)
        {
            if (newScoreText == "" || newScoreText == null)
            {
                Debug.LogWarning(newScoreText + "__ NAN");
                return;
            }
            
            if (scoreText.text != newScoreText)
            {
                var newScoreEff = Instantiate(scoreEff, gameObject.transform, false);
                newScoreEff.transform.position = newScorePos.transform.position;


                var oldScore = int.Parse(scoreText.text);
                var newScore = int.Parse(newScoreText);
                newScoreEff.GetComponent<Bingo_ScoreEff>().PlayEffect(oldScore, newScore);
                ShowSocreEffect(newScore - oldScore);
                if (int.Parse(newScoreText) < 0)
                {
                    newScoreText = "0";
                }
                scoreText.text = newScoreText;
          
              


            }
            
        }

        [Button]
        public void StartCountdown(int totalTime)
        {
            if (totalTime - currentTime > 8)
            {
                currentTime += 10;
            }

            if (isCountinng)
            {
                return;
            }

            isCountinng = true;

            Bingo_GameManager.instance.SetCurrentGameState(GameState.playing);
            currentTime = totalTime;

            StartCoroutine(MinusSecond());

            IEnumerator MinusSecond()
            {
                while (currentTime >= 0)
                {
                    yield return new WaitForSeconds(1);

                    var minute = Mathf.FloorToInt(currentTime / 60);
                    var second = Mathf.FloorToInt(currentTime % 60);
                    timeText.text = string.Format("{0:00}:{1:00}", minute, second);
                    timeText_Shadow.text = timeText.text;


                    if (currentTime <= 0) // game end
                    {
                        isCountinng = false;
                        Bingo_GameManager.instance.SetCurrentGameState(GameState.displayResult);
                        yield break;
                    }

                    currentTime--;
                }
            }
        }


        public GameObject posPlayer2_In;
        public GameObject player2Gr;
        bool isPlayer2Playing = false;

        [Button]
        public void UpdateDataPlayer2(BingoGameDataPlayer2 data)
        {
            Debug.LogWarning("Bingo Update data player2 " + data.scores.player2_score + "__" +
                             data.scores.player2_name + "___" + data);
            enemyScore.text = "" + data.scores.player2_score;
        }

        public void ShowPlayer2()
        {
            if (isPlayer2Playing == false) player2Gr.transform.DOMoveX(posPlayer2_In.transform.position.x, .7f);
            isPlayer2Playing = true;
        }

        void ShowSocreEffect(int scoreDiff)
        {
            int id = -1;
            if(scoreDiff == 110)
            {
                id = 4;
            }
            else if (scoreDiff == 120)
            {
                id = 3;
            }
            else if (scoreDiff == 130)
            {
                id = 2;
            }
            else if (scoreDiff == 140)
            {
                id = 1;
            }
            else if (scoreDiff == 150)
            {
                id = 0;
            }

            Debug.Log("==================Score effect=======" + id + "=======" + scoreDiff);
            if(id >= 0)
            {
                scoreEffect.PlayScoreEffect(id);
            }
        }
    }
}