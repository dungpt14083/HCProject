using Zenject;
using UniRx;
using Salday.EventBus;
using UnityEngine.UI;
using UnityEngine;

public class EventController : IInitializable, System.IDisposable
{
	[Inject] EventBus eventBus;
	[Inject] SceneTransition sceneTransition;
	[Inject(Id = "Back")] Button backButton;
	[Inject(Id = "Join")] Button joinButton;
	readonly CompositeDisposable _disposable = new CompositeDisposable();
	private ISubscription subscription;

	public void Initialize()
	{
		sceneTransition.FadeOut();
		subscription = eventBus.RegisterSubscription(this);
		subscription.AddTo(_disposable);

		ButtonListeners();
	}

	private void ButtonListeners()
	{
		backButton.onClick.AsObservable().Subscribe(_ =>
		{
			eventBus.Publish(new SceneChangeEvent("Home"));
		}).AddTo(_disposable);
		Debug.Log("joinButton "+joinButton.name);
		joinButton.onClick.AsObservable().Subscribe(_ =>
		{
			Debug.Log("ButtonListeners");
			eventBus.Publish(new SceneChangeEvent("Game Matching"));
		}).AddTo(_disposable);
	}

	public void Dispose()
	{
		_disposable.Dispose();
	}


	#region EventBus
	[Handler(HandlerPriority.High)]
	public void SceneChangeEventHandler(SceneChangeEvent ev)
	{
		sceneTransition.LoadScene(ev.code);
	}
	#endregion
}
