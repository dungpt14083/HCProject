using Salday.EventBus;
using UnityEngine;
using Zenject;

public class WelcomeInstaller : MonoInstaller
{

	[SerializeField] SceneTransition sceneTransition;

	public override void InstallBindings()
	{
		Container.BindInterfacesTo<WelcomeController>().FromNew().AsTransient().NonLazy();
		Container.Bind<SceneTransition>().FromInstance(sceneTransition).AsSingle().NonLazy();
		Container.Bind<EventBus>().FromInstance(new EventBus()).AsTransient().NonLazy();
	}
}