using DG.Tweening;
using UnityEngine;

public class UIScaleAnimation : UIBaseAnimation
{
    public float startSize = 0.7f, middleSize = 1.05f, endSize = 1;
    public float firstTime = 0.2f, secondTime = 0.1f;
    public override IAnimation OnStart()
    {
        base.OnStart();
        content.localScale = Vector3.one * startSize;
        canvasGroup.alpha = 0;
        sequence.Append(content.DOScale(middleSize, firstTime))
            .Join(canvasGroup.DOFade(1, firstTime))
            .Append(content.DOScale(endSize, secondTime));
        sequence.Restart();
        return this;
    }

    public override IAnimation OnReverse()
    {
        base.OnReverse();
        content.localScale = Vector3.one * endSize;
        canvasGroup.alpha = 1;
        sequence.Append(content.DOScale(middleSize, secondTime))
            .Append(content.DOScale(startSize + 0.1f, firstTime))
            .Join(canvasGroup.DOFade(0, firstTime));
        return this;
    }
}
