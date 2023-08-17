using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.MatchThree.Scripts.Network
{
    public class Match3_Notice : MonoBehaviour
    {










        public GameObject endTheMatch, timesUp, gameover, ouOfMove;


        public void ShowEndTheMatch()
        {
            endTheMatch.gameObject.SetActive(true);
        }



        public void ShowOutOfMove()
        {
            ouOfMove.gameObject.SetActive(true);

        }




        public void ShowTimesUp()
        {
            timesUp.gameObject.SetActive(true);

        }




        public void ShowGameOver()
        {
            gameover.gameObject.SetActive(true);

        }


        public void OnClickClose()
        {
            MatchThreeNetworkManager.Instance.Disconnect();
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Home");
        }



    }
}
