using Salday.EventBus;

/// <summary>
/// It is necessary to implement this interface for editor configurable proxies
/// If not using them, this can be ommited
/// </summary>
public interface IEventProxy
{
    ISubscription Subscription { get; set; }
}

