using Salday.EventBus;
using UnityEngine;
using Zenject;
using UniRx;


public class EventDispatcher : MonoBehaviour
{
	[Inject] EventBus eventBus;
	private void Awake()
	{
		Observable.EveryApplicationPause().Subscribe(pauseStatus =>
		{
			if (pauseStatus)
			{
				// TODO cần bật lại khi build
				// eventBus.Publish(new MyGameEvent(GameEventType.PAUSE_GAME));
				// eventBus.Publish(new ShowPopupEvent() { code = "Options" });
			}
		}).AddTo(this);
	}

	public void DispatchEvent(GameEventType ev)
	{
		eventBus.Publish(new MyGameEvent(ev));
	}



}