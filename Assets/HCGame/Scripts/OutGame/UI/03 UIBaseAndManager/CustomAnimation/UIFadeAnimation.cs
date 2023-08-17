using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeAnimation : UIDefaultAnimation
{
    public float fadeTime = 0.25f;
    [SerializeField]
    protected CanvasGroup canvasGroup;

    private void OnValidate()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override IAnimation OnStart()
    {
        base.OnStart();
        canvasGroup.alpha = 0;
        sequence.Append(canvasGroup.DOFade(1, fadeTime));
        sequence.Restart();
        return this;
    }

    public override IAnimation OnReverse()
    {
        base.OnReverse();
        canvasGroup.alpha = 1;
        sequence.Append(canvasGroup.DOFade(0, fadeTime));
        sequence.Restart();
        return this;
    }
}
