using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIDefaultAnimation : MonoBehaviour, IAnimation
{
    protected Sequence sequence;

    [Button]
    void Play()
    {
        OnStart();
    }
    
    public virtual IAnimation OnStart()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        return this;
    }

    public virtual IAnimation OnReverse()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        return this;
    }

    public virtual IAnimation OnStop()
    {
        sequence?.Pause();
        return this;
    }

    public IAnimation SetStartCompletedCallback(Action action)
    {
        sequence.OnComplete(() =>
        {
            action?.Invoke();
        });
        return this;
    }

    public IAnimation SetReverseCompletedCallBack(Action action)
    {
        sequence.OnComplete(() =>
        {
            action?.Invoke();
        });
        return this;
    }
}
