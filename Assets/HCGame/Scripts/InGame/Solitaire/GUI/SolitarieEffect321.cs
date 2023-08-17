using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SolitarieEffect321 : MonoBehaviour
{
    public static SolitarieEffect321 Ins;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public GameObject startGameGroup;
    public Image myImage;
    public Sprite Sprite_1, Sprite_2, Sprite_3,Sprite_Go;
    public float time1, time2, time3;
    public void PlayEffect321(Action complete)
    {
        startGameGroup.SetActive(true);
        StartCoroutine(PlayStartGameEFF(complete));

        IEnumerator PlayStartGameEFF(Action complete)
        {
            myImage.color = Color.clear;
            DisplaySprite(Sprite_1);
            yield return new WaitForSeconds(1f);
            DisplaySprite(Sprite_2);
            yield return new WaitForSeconds(1f);
            DisplaySprite(Sprite_3);
            yield return new WaitForSeconds(1f);
            DisplaySprite(Sprite_Go);
            yield return new WaitForSeconds(.5f);
            startGameGroup.gameObject.SetActive(false);
            complete.Invoke();
        }
       
        

    }
    public void DisplaySprite(Sprite sprite)
    {
        myImage.sprite = sprite;
        myImage.SetNativeSize();
        myImage.color = Color.white;
        myImage.transform.localScale = Vector3.zero;
        myImage.transform.DOScale(Vector3.one, time1).OnComplete(() =>
        {
            myImage.transform.DOScale(Vector3.one, time2).OnComplete(() =>
            {
                myImage.DOFade(0, time3);

            });// wait
        });





    }
}
