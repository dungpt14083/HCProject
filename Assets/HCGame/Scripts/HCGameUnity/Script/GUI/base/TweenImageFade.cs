using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenImageFade : MonoBehaviour
{
    [SerializeField] private float startAlpha;
    [SerializeField] private float endAlpha;
    [SerializeField] private float timeFade = 0.5f;

    private Image img;
    private List<Image> childImage;
    private Color color;

    private void Init()
    {
        img = GetComponent<Image>();
        childImage = new List<Image>(GetComponentsInChildren<Image>());
        color = img.color;
    }

    private void Awake()
    {
        if (null == img)
        {
            Init();
        }
        ResetFade();
    }

    public void ResetFade()
    {
        if(null == img)
        {
            Init();
        }
        color.a = startAlpha;
        img.color = color;
    }

    public void OnFadeToEndAlpha(Action OnFinishFading_Handler = null)
    {
        DOTween.Kill(this);
        ResetFade();
        img.DOFade(endAlpha, timeFade).onComplete += () => 
        {
            OnFinishFading_Handler?.Invoke();
        };
        foreach(Image childImg in childImage)
        {
            childImg.DOFade(endAlpha, timeFade);
        }
    }

    public void OnFadeToStartAlpha(Action OnFinishFading_Handler = null)
    {
        DOTween.Kill(this);
        img.DOFade(startAlpha, timeFade).onComplete += () =>
        {
            OnFinishFading_Handler?.Invoke();
        };
        foreach (Image childImg in childImage)
        {
            childImg.DOFade(startAlpha, timeFade);
        }
    }
}
