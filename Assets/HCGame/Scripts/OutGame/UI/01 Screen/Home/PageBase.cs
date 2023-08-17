using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using BonusGame;
using System;
using UnityEngine.Events;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PageBase : MonoBehaviour
{

    [TitleGroup("___________  Reference  __________")]
    [SerializeField]
    private CanvasGroup myCanvasGroup;
    [SerializeField]
    public GameObject myContents;



    [TitleGroup("___________  Config  __________")]
    [SerializeField]
    private float fadeTime = .3f;
    [SerializeField]
    private Vector3 fadeInStartScale;

    [SerializeField]
    private Vector3 fadeOutEndScale;

    [Button]
    public void FadeOut(Action callbackCompleted = null)
    {
        DOTween.To(() => myCanvasGroup.alpha, v => myCanvasGroup.alpha = v, 0, fadeTime).Play().OnComplete(() =>
        {
            DisableDisplay();
            callbackCompleted?.Invoke();
        });

        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.DOScale(fadeOutEndScale, fadeTime);

    }
    [Button]
    public void FadeIn(Action callbackCompleted = null)
    {
        myCanvasGroup.blocksRaycasts = true;
        myCanvasGroup.interactable = true;
        myContents.gameObject.SetActive(true);
        myCanvasGroup.alpha = 0;
        DOTween.To(() => myCanvasGroup.alpha, v => myCanvasGroup.alpha = v, 1, fadeTime).Play().OnComplete(() =>
        {
            callbackCompleted?.Invoke();
        });

        gameObject.transform.localScale = fadeInStartScale;
        gameObject.transform.DOScale(Vector3.one, fadeTime);
    }


    public void DisableDisplay()
    {
        myContents.gameObject.SetActive(false);
        myCanvasGroup.alpha = 0;
        myCanvasGroup.interactable = false;
        myCanvasGroup.blocksRaycasts = false;
    }
    
}
