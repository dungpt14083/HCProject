using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemNotification : MonoBehaviour
{
	[SerializeField] TMP_Text title;
	[SerializeField] TMP_Text content;
	[SerializeField] TMP_Text time;
	TimeSpan timeSpan;
	
	public void Show(HcNotificationProto data)
	{
		gameObject.SetActive(true);
		title.text = data.Title;
		content.text = data.Content;
		
		if (data.SendTime != null) timeSpan = DateTime.UtcNow - data.SendTime.ToDateTime();
		Debug.Log("timeSpan " + timeSpan.ToString());
		if (timeSpan != null)
        {
			time.text = GetTime(timeSpan);
		}
        else
		{
			time.text = string.Empty;
		}
		//if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
		////Todo : iconReward 
		////Todo : iconMoneyReward 
		//title.text = data.Name;
		//content.text = $"data.Description ({data.InProgress}/{data.ToComplete})";
		//valueReward.text = data.Reward.ToString();
		//if (data.InProgress == data.ToComplete || data.ToComplete < 1)
		//{
		//	bg.sprite = bgComplete;
		//	bgProgess.sprite = progessComplete;
		//	bgProgess.fillAmount = 1;
		//}
		//else
		//{
		//	bg.sprite = bgNormal;
		//	bgProgess.sprite = progessNormal;
		//	bgProgess.fillAmount = data.InProgress / (float)data.ToComplete;
		//}
	}
    private void Update()
    {
        if(timeSpan != null)
        {
			timeSpan.Add(TimeSpan.FromSeconds(Time.deltaTime));
			time.text = GetTime(timeSpan);
		}
    }
    public string GetTime(TimeSpan timeSpan)
    {
		string result = string.Empty;
		result = $"{timeSpan.ToString(@"dd\.hh\:mm\:ss")} ago";
		return result;
    }
}
