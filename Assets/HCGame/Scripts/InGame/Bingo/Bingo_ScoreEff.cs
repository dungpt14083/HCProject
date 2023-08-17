using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Bingo
{
    public class Bingo_ScoreEff : MonoBehaviour
    {
      

        [TitleGroup("___________  Reference  __________")]
        public float distanceMove;
        public float timeMove, timeStay, timeFade;
        public TextMeshProUGUI myText;
        public CanvasGroup myCanvas;

        [Button]
        public void PlayEffect(int oldScore, int newScore)
        {
            var scoreDiff = newScore - oldScore;
            if (scoreDiff > 0)
            {
                //myText.color = Color.green;
                myText.text = "+" + scoreDiff;
                Bingo_SoundManager.instance.PlaySound_EffMoreScore();
            }
            else
            {
                //myText.color = Color.red;
                myText.text = "" + scoreDiff;
                Bingo_SoundManager.instance.PlaySound_EffLoseScore();

            }

            myCanvas.transform.DOMoveY(transform.position.y + distanceMove, timeMove).OnComplete(() =>//move up
            {
                myCanvas.transform.DOMoveY(transform.position.y, timeStay).OnComplete(() => //wait
                {
                    myCanvas.DOFade(0, timeFade).OnComplete(()=> {//fade
                        Destroy(gameObject);
                    });
                });
            });


        }


    }

}
