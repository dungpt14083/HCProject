using Salday.EventBus;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameMatchingInstaller : MonoInstaller
{
	[SerializeField] Button backButton;
	[SerializeField] SceneTransition sceneTransition;

	public override void InstallBindings()
	{
		Container.BindInterfacesTo<GameMatchingController>().FromNew().AsTransient().NonLazy();
		Container.Bind<SceneTransition>().FromInstance(sceneTransition).AsSingle().NonLazy();
		Container.Bind<EventBus>().FromInstance(new EventBus()).AsTransient().NonLazy();


		Container.Bind<Button>().FromInstance(backButton).AsTransient().NonLazy();
	}
}