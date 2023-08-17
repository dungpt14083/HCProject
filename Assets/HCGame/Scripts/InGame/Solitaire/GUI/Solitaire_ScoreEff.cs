using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Solitaire_ScoreEff : MonoBehaviour
{
   
    public float distanceMove;
    public float timeMove, timeStay, timeFade;
    public TMP_Text myText;
    public CanvasGroup myCanvas;

    
    public void PlayEffect(int oldScore, int newScore)
    {
        var scoreDiff = newScore - oldScore;
        if (scoreDiff > 0)
        {
            myText.color = Color.green;
            myText.text = "+" + scoreDiff;
           
        }
        else
        {
            myText.color = Color.red;
            myText.text = "" + scoreDiff;
          

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
