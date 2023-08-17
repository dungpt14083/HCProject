using System;
using Zenject;
using UniRx;
using Salday.EventBus;

public class WelcomeController : IInitializable, IDisposable
{
	[Inject] EventBus eventBus;
	[Inject] SceneTransition sceneTransition;
	readonly CompositeDisposable _disposable = new CompositeDisposable();
	private ISubscription subscription;

	public void Initialize()
	{
		subscription = eventBus.RegisterSubscription(this);
		subscription.AddTo(_disposable);
	}


	public void Dispose()
	{
		_disposable.Dispose();
	}


	#region EventBus
	[Handler(HandlerPriority.High)]
	public void SignInEventHandler(SignInEvent ev)
	{
		sceneTransition.LoadScene("Home");
	}
	#endregion
}
