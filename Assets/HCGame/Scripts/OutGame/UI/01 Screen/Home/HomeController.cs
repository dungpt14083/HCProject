using System;
using Zenject;
using UniRx;
using Salday.EventBus;
using UnityEngine;
using MoreMountains.Feedbacks;

public class HomeController : IInitializable, IDisposable
{
	[Inject] EventBus eventBus;
	[Inject] SceneTransition sceneTransition;
	[Inject(Id = "ShowFeedback")] MMF_Player show;
	[Inject(Id = "HideFeedback")] MMF_Player hide;
	readonly CompositeDisposable _disposable = new CompositeDisposable();
	private ISubscription subscription;

	public void Initialize()
	{
		sceneTransition.FadeOut();
		subscription = eventBus.RegisterSubscription(this);
		subscription.AddTo(_disposable);
	}


	public void Dispose()
	{
		_disposable.Dispose();
	}


	#region EventBus
	[Handler(HandlerPriority.High)]
	public void SceneChangeEventHandler(SceneChangeEvent ev)
	{
		hide?.PlayFeedbacks();
		sceneTransition.LoadScene(ev.code);
	}
	#endregion
}
