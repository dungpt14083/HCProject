using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIBaseAnimation : UIDefaultAnimation
{
    public RectTransform content;
    [SerializeField] protected CanvasGroup canvasGroup;

    private void OnValidate()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override IAnimation OnStart()
    {
        sequence?.Kill();
        canvasGroup.blocksRaycasts = true;
        sequence = DOTween.Sequence();
        return this;
    }

    public override IAnimation OnReverse()
    {
        canvasGroup.blocksRaycasts = false;
        sequence?.Kill();
        sequence = DOTween.Sequence();
        return this;
    }

    public override IAnimation OnStop()
    {
        sequence?.Pause();
        return this;
    }
}