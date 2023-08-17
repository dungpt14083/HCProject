using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UniRx;
public class NavPage : MonoBehaviour
{
	[SerializeField] MMF_Player onSelect, onDeSelect;

	readonly CompositeDisposable disposables = new CompositeDisposable();
	public float PageWidth { get { return GetComponent<LayoutElement>().preferredWidth; } }


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
}