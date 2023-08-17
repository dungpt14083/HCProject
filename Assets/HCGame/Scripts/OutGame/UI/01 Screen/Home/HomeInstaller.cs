using MoreMountains.Feedbacks;
using Salday.EventBus;
using UnityEngine;
using Zenject;

public class HomeInstaller : MonoInstaller
{

	[SerializeField] SceneTransition sceneTransition;
	[SerializeField] MMF_Player show, hide;

	public override void InstallBindings()
	{
		Container.BindInterfacesTo<HomeController>().FromNew().AsTransient().NonLazy();
		Container.Bind<SceneTransition>().FromInstance(sceneTransition).AsSingle().NonLazy();
		Container.Bind<EventBus>().FromInstance(new EventBus()).AsTransient().NonLazy();
		Container.Bind<MMF_Player>().WithId("ShowFeedback").FromInstance(show).AsTransient().NonLazy();
		Container.Bind<MMF_Player>().WithId("HideFeedback").FromInstance(hide).AsTransient().NonLazy();
	}

}