using Salday.EventBus;

public class MyGameEvent : CancelableEventBase
{
	public GameEventType type { get; set; }
	public MyGameEvent(GameEventType type)
	{
		this.type = type;
	}

	
}


