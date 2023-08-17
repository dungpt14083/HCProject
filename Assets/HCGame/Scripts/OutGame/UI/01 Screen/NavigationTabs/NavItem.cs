using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx;

public class NavItem : MonoBehaviour
{
	[SerializeField] MMF_Player onSelect, onDeSelect;
	[SerializeField] UnityEvent onClick;
	readonly CompositeDisposable disposables = new CompositeDisposable();
	private void Awake()
	{
		GetComponent<Button>().OnClickAsObservable()
			.Subscribe(_ =>
			{
				onClick?.Invoke();
			})
			.AddTo(disposables);
	}

	public void OnSelect()
	{
		onSelect?.PlayFeedbacks();
	}

	public void OnDeSelect()
	{
		onDeSelect?.PlayFeedbacks();
	}

	private void OnDestroy()
	{
		disposables.Clear();
	}


	public void OnClickPageName(HomePageEnum pageInput )
	{
		switch (pageInput)
		{
			case HomePageEnum.none:
				break;
			case HomePageEnum.home:
				break;
			case HomePageEnum.events:
				break;
			case HomePageEnum.ranking:
				break;
			case HomePageEnum.history:
				break;
		}
	}

	public enum HomePageEnum
	{
		none,
		home,
		events,
		ranking,
		history
	
	}

}