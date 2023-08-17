using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Salday.EventBus;


public class UserFragmentUI : MonoBehaviour
{
    [SerializeField] Button avatarButton;
    [Inject] EventBus eventBus;
    readonly CompositeDisposable disposables = new CompositeDisposable();

    private void Awake()
    {
        avatarButton.onClick.AsObservable().Subscribe(_ =>
        {
            Debug.Log("UserFragmentUI: avatarButton.onClick");
            eventBus.Publish(new SceneChangeEvent("Profile"));
        }).AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }

}