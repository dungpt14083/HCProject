using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TabController : MonoBehaviour
{
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] GameObject navTabs;
	[SerializeField] int defaultPage = 0;
	[SerializeField] float scrollTime = 0.3f;
	private int currentPage = 0;
	private NavItem[] navItems;
	private NavPage[] navPages;


	private void Awake()
	{
		navItems = navTabs.GetComponentsInChildren<NavItem>();
		navPages = scrollRect.GetComponentsInChildren<NavPage>();
	}

	private void Start()
	{
		OnSelectPage(defaultPage);
	}

	public void OnSelectPage(int page)
	{
		navItems[currentPage].OnDeSelect();
		currentPage = page;
		navItems[currentPage].OnSelect();
		scrollRect.DOHorizontalNormalizedPos(1f / (navItems.Length - 1) * currentPage, scrollTime).SetEase(Ease.OutCubic);


		for (int i = 0; i < navPages.Length; i++)
		{
			if (i == currentPage)
			{
				navPages[i].OnSelect();
			}
			else
			{
				navPages[i].OnDeSelect();
			}
		}
	}

}