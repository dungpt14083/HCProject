using UnityEngine;
using System.Collections;
using Salday.EventBus;
using System;

/// <summary>
/// Used as a base class for any other editor dragable event proxy
/// </summary>
public class UnityEventProxyBase : MonoBehaviour, IEventProxy
{
    public ISubscription Subscription { get; set; }

    #region Inspector Vars

    [SerializeField]
    protected bool ShouldBeDestroyedOnLoad = false;

    #endregion

    #region Mono Behavior Callbacks

    protected virtual void Awake()
    {
        if (!ShouldBeDestroyedOnLoad)
            DontDestroyOnLoad(this);
    }

    protected virtual void OnEnable()
    {
        if (Subscription != null)
            Subscription.Active = true;
    }

    protected virtual void OnDisable()
    {
        if (Subscription != null)
            Subscription.Active = false;
    }

    protected virtual void OnDestroy()
    {
        if (Subscription != null)
            Subscription.Dispose();
    }

    public void SetSubscription(ISubscription subscription)
    {
        this.Subscription = subscription;
    }

    #endregion
}
