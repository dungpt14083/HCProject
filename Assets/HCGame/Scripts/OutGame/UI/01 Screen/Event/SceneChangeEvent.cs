using Salday.EventBus;

public class SceneChangeEvent : CancelableEventBase
{
	public string code { get; set; }

	public SceneChangeEvent(string code)
	{
		this.code = code;
	}
}