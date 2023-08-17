using UnityEngine;
using Salday.EventBus;
using System;

/// <summary>
/// Event bus that supports unity editor
/// </summary>
public class UnityAdoptedEventBus : MonoBehaviour, IEventBus
{
    #region Public stuff
    /// <summary>
    /// Is an event bus initialized and ready
    /// </summary>
    public bool WasInitialized { get { return wasInitialized; } }

    #endregion

    #region Inspector Vars

    /// <summary>
    /// An array of proxyes to register, when event bus is initialized
    /// </summary>
    [SerializeField]
    protected UnityEventProxyBase[] proxyes;

    /// <summary>
    /// Should event bus throw exceptions, when no handler can be found for particular type of event
    /// </summary>
    [Tooltip("Should event bus throw exceptions when no handler can be found for published message?")]
    [SerializeField]
    protected bool throwIfTypeNotRegistered = false;


    /// <summary>
    /// Should event bus be destroyed when the scene is switched
    /// </summary>
    [Tooltip("Should event bus be destroyed when the scene is switched")]
    [SerializeField]
    protected bool ShouldBeDestroyedOnLoad = false;

    #endregion

    #region Private/Protected Vars

    protected EventBus eventBus;
    protected bool wasInitialized = false;

    #endregion

    #region MonoBehavior Callbacks

    protected virtual void Awake()
    {
        //Initialize event bus
        eventBus = new EventBus(throwIfTypeNotRegistered);

        //Register proxyes
        foreach (var pr in proxyes)
        {
            RegisterSubscription(pr);
        }

        if (!ShouldBeDestroyedOnLoad)
            DontDestroyOnLoad(this);

        wasInitialized = true;
    }

    #endregion

    #region Subscription Management

    /// <summary>
    /// Registers proxy for event handling
    /// </summary>
    /// <param name="eventProxy">Proxy to register</param>
    /// <returns></returns>
    public ISubscription RegisterSubscription(IEventProxy eventProxy)
    {
        var sub = this.RegisterSubscription((object)eventProxy);
        eventProxy.Subscription = sub;
        return sub;
    }

    #endregion

    #region Interface implementation

    /// <summary>
    /// Publishes event argument to the event bus
    /// </summary>
    /// <typeparam name="TEvent">Type of event argument (used for event distribution)</typeparam>
    /// <param name="eventObject">Event argument to publish</param>
    public void Publish<TEvent>(TEvent eventObject) where TEvent : EventBase
    {
        eventBus.Publish(eventObject);
    }

    /// <summary>
    /// Registerers an object to event bus
    /// </summary>
    /// <param name="eventProxy">Object, which contains handlers marked with <cref = Salday.EventBus.HandlerAttribute></param>
    /// <returns></returns>
    public ISubscription RegisterSubscription(object eventProxy)
    {
        return eventBus.RegisterSubscription(eventProxy);
    }

    /// <summary>
    /// Deletes a subscription from event bus
    /// </summary>
    /// <param name="supscription"></param>
    public void RemoveSubscription(ISubscription supscription)
    {
        eventBus.RemoveSubscription(supscription);
    }

    /// <summary>
    /// Removes all handlers, that accept particular event type
    /// </summary>
    /// <param name="type"></param>
    public void RemoveType(Type type)
    {
        eventBus.RemoveType(type);
    }

    #endregion
}
