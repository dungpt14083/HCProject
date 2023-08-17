using System;
using Zenject;
using UniRx;
using Salday.EventBus;
using UnityEngine.UI;
using UnityEngine;

public class ProfileController : IInitializable, IDisposable
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
			eventBus.Publish(new SceneChangeEvent("Home"));
		}).AddTo(_disposable);

		GameObject.Find("Discord Group").GetComponent<Button>().onClick.AsObservable().Subscribe(_ =>
		{
			// open discord link in browser
			Application.OpenURL("https://discord.gg");
		}).AddTo(_disposable);

		GameObject.Find("Facebook").GetComponent<Button>().onClick.AsObservable().Subscribe(_ =>
		{
			// open twitter link in browser
			Application.OpenURL("https://www.facebook.com");
		}).AddTo(_disposable);
		GameObject.Find("FNCY").GetComponent<Button>().onClick.AsObservable().Subscribe(_ =>
		{
			// open twitter link in browser
			Application.OpenURL("https://google.com");
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
