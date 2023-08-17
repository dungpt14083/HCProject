using Salday.EventBus;

public class ShowPopupEvent : CancelableEventBase
{
	public string code { get; set; }
}

public class HidePopupEvent : CancelableEventBase
{
	public string code { get; set; }
}