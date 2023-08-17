using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour, IView, IAnimate
{
    public IAnimation animation;

    protected virtual void Awake()
    {
        animation = GetComponent<IAnimation>() ?? gameObject.AddComponent<UIDefaultAnimation>();
    }

    public void OpenView()
    {
        OnOpen();
    }

    public void CloseView()
    {
        OnClose();
    }

    #region IAnimate

    public virtual void OnOpen()
    {
        animation.OnStop();
        animation.OnStart();
    }

    public void OnStop()
    {
        animation.OnStop();
    }

    public virtual void OnClose(Action onComplete = null)
    {
        animation.OnStop();
        animation.OnReverse().SetReverseCompletedCallBack(() => { gameObject.Hide(); });
    }

    #endregion


    #region ForViewCreator

    public string ViewId { get; set; }
    public int Priority { get; set; }
    
    private GameObject _go;

    public GameObject GetGameObject()
    {
        if (!_go)
        {
            _go = gameObject;
        }

        return _go;
    }

    #endregion
}

public enum AnimationType
{
    Tween,
    UnityAnimation
}