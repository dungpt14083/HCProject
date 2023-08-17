using System;
using Zenject;
using UniRx;
using Salday.EventBus;
using UnityEngine.UI;
using AssemblyCSharp.GameNetwork;

public class GameMatchingController : IInitializable, IDisposable
{
	[Inject] EventBus eventBus;
	[Inject] SceneTransition sceneTransition;
	[Inject] Button backButton;
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
			HCAppController.Instance.DisconnectAllMiniGame();
			eventBus.Publish(new SceneChangeEvent("Home"));
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
