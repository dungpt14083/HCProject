using UnityEngine;
using Zenject;
using Salday.EventBus;

public class DemoInstaller : MonoInstaller
{   

    [SerializeField] SceneTransition sceneTransition;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<DemoController>().FromNew().AsTransient().NonLazy();
		Container.Bind<SceneTransition>().FromInstance(sceneTransition).AsSingle().NonLazy();
		Container.Bind<EventBus>().FromInstance(new EventBus()).AsTransient().NonLazy();
    }
}