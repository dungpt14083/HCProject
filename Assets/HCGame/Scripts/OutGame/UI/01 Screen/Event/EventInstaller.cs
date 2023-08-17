using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Salday.EventBus;

public class EventInstaller : MonoInstaller
{   

    [SerializeField] SceneTransition sceneTransition;
    [SerializeField] Button backButton;
    [SerializeField] Button joinButton;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<EventController>().FromNew().AsTransient().NonLazy();
		Container.Bind<SceneTransition>().FromInstance(sceneTransition).AsSingle().NonLazy();
		Container.Bind<EventBus>().FromInstance(new EventBus()).AsTransient().NonLazy();

        Container.Bind<Button>().WithId("Back").FromInstance(backButton).AsTransient().NonLazy();
        Container.Bind<Button>().WithId("Join").FromInstance(joinButton).AsTransient().NonLazy();
    }
}