using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FullScreenView : MonoBehaviour
{






    public static FullScreenView ins;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
        else
        {
            Debug.LogError("MULTI INS" + gameObject);
            Destroy(this);
        }

    }


    public GameObject bg, gameName, gameNameText, gameIcon, loading1, loading2, loading3, player1, player2, playerInfo1, playerInfo2;

    public Ease easeing;

    public float timeMove, delayPlayerMove;
    public CanvasGroup fadeGr;
    public GameObject rotateGr;
    public GameObject posPlayerLeft1, posPlayerLeft2, posPlayerRight1, posPlayerRight2, posPlayerLeftInfo1, posPlayerLeftInfo2, posPlayerRightInfo1, posPlayerRightInfo2, posGameNameStart, posGameNameEnd;
    [Button]
    public void PlayStartMatchingAnimation()
    {

        StartCoroutine(PlayStartMatchingAnimation_IE());

        IEnumerator PlayStartMatchingAnimation_IE()
        {
            
            fadeGr.gameObject.SetActive(true);
            fadeGr.blocksRaycasts = true;
            fadeGr.interactable = true;
            fadeGr.gameObject.SetActive(true);
            fadeGr.alpha = 0;
            var fadeTime = 1;
            var fadeTo = 1;
            DOTween.To(() => fadeGr.alpha, v => fadeGr.alpha = v, fadeTo, fadeTime).Play().OnComplete(() =>
            {
            });
            rotateGr.gameObject.SetActive(true);


            yield return new WaitForSeconds(.7f);
            player1.gameObject.SetActive(true);
            player1.transform.position = posPlayerLeft1.transform.position;
            player1.transform.DOMoveX(posPlayerLeft2.transform.position.x, timeMove).SetEase(Ease.OutBack);


            yield return new WaitForSeconds(delayPlayerMove);
            player2.gameObject.SetActive(true);
            player2.transform.position = posPlayerRight1.transform.position;
            player2.transform.DOMoveX(posPlayerRight2.transform.position.x, timeMove).SetEase(Ease.OutBack);


            yield return new WaitForSeconds(delayPlayerMove + .4f);
            playerInfo1.gameObject.SetActive(true);
            playerInfo1.transform.position = posPlayerLeftInfo1.transform.position;
            playerInfo1.transform.DOMoveX(posPlayerLeftInfo2.transform.position.x, timeMove);

            playerInfo2.gameObject.SetActive(true);
            playerInfo2.transform.position = new Vector3(posPlayerRightInfo1.transform.position.x,
                                                        playerInfo2.transform.position.y,
                                                        playerInfo2.transform.position.z);
            playerInfo2.gameObject.SetActive(true);
            playerInfo2.transform.DOMoveX(posPlayerRightInfo2.transform.position.x, timeMove);

            gameName.gameObject.SetActive(true);
            gameName.transform.position = new Vector3(gameName.transform.position.x,
                                                      posGameNameStart.transform.position.y,
                                                      gameName.transform.position.z);
            gameName.transform.DOMoveY(posGameNameEnd.transform.position.y, timeMove);



        }
    }

   

    [Button]
    public void Reset()
    {
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
        playerInfo1.gameObject.SetActive(false);
        playerInfo2.gameObject.SetActive(false);
        rotateGr.gameObject.SetActive(false);
        fadeGr.gameObject.SetActive(false);
        gameName.gameObject.SetActive(false);
    }





}
