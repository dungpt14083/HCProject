using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
public struct ClaimFreeGoldRequest
{
	public string userCodeId;
}
public struct ClaimFreeGoldResponse
{
	public bool isSuccess;
}
public class SlideMenu : MonoBehaviour
{
	[SerializeField] Button btDown;
	[SerializeField] Button btUp;
	[SerializeField] Button btDailyMission;
	[SerializeField] Button btGoldGift;
	[SerializeField] Button btDailyReward;
	[SerializeField] Button btNotification;
	[SerializeField] TMP_Text txtGoldGift;
	[SerializeField] ContentSizeFitter contentSizeFitter;

	private void Awake()
	{
		btDown.onClick.AddListener(ClickDown);
		btUp.onClick.AddListener(ClickUp);
		btDailyMission.onClick.AddListener(OnClickDailyMission);
		btGoldGift.onClick.AddListener(OnClickGoldGift);
		btDailyReward.onClick.AddListener(OnClickDailyReward);
		btNotification.onClick.AddListener(ShowNotification);
	}
    private void Start()
    {
		txtGoldGift.text = $"0m:00s";
	}
    private void Update()
    {
        if(HCAppController.Instance.timeGoldGift.TotalMilliseconds > 0)
        {
			HCAppController.Instance.timeGoldGift -= System.TimeSpan.FromSeconds(Time.deltaTime);
			if(HCAppController.Instance.timeGoldGift.TotalMilliseconds < 1)
            {
				txtGoldGift.text = $"0m:00s";
				if(btGoldGift.interactable == false) btGoldGift.interactable = true;
			}
            else
            {
				if(btGoldGift.interactable == true) btGoldGift.interactable = false;
				if (HCAppController.Instance.timeGoldGift.TotalSeconds >= 3600)
                {
					txtGoldGift.text = $"{HCAppController.Instance.timeGoldGift.Hours}h:{HCAppController.Instance.timeGoldGift.Minutes}m";
                }
                else
                {
					txtGoldGift.text = $"{HCAppController.Instance.timeGoldGift.Minutes}m:{HCAppController.Instance.timeGoldGift.Seconds}s";
				}
            }
		}
    }
    public void OnClickDailyMission()
	{
		HcPopupManager.Instance.DisplayDailyMission();
	}
    
    public void OnClickDailyReward()
    {
	    HcPopupManager.Instance.DisplayDailyReward();
    }

    public void OnClickGoldGift()
	{
		Debug.Log("OnClickGoldGift");
		HCAppController.Instance.ClaimFreeGold(ClaimFreeGoldSuccess);
	}
    public void ShowNotification()
    {
		HcPopupManager.Instance.DisplayNotification();
    }
	public void ClickUp()
    {
		btNotification.gameObject.SetActive(false);
		btDailyMission.gameObject.SetActive(true);
		btDown.gameObject.SetActive(true);
		btUp.gameObject.SetActive(false);
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
	}
	public void ClickDown()
    {
		btNotification.gameObject.SetActive(true);
		btDailyMission.gameObject.SetActive(false);
		btDown.gameObject.SetActive(false);
		btUp.gameObject.SetActive(true);
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
	}
	public void ClaimFreeGoldSuccess(string data)
    {
		var response = JsonUtility.FromJson<ClaimFreeGoldResponse>(data);
    }
}