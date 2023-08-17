using System;
using Zenject;
using UniRx;
using Salday.EventBus;
using UnityEngine;

public class DemoController : IInitializable, IDisposable
{

    [Inject] SceneTransition sceneTransition;
    [Inject] private EventBus eventBus;
    
	readonly CompositeDisposable _disposable = new CompositeDisposable();
    private ISubscription subscription;

	public void Initialize()
	{
	}

	public void Dispose()
	{
        _disposable.Dispose();
	}
}
