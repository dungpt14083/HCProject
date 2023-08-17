using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReferraPopup : UIPopupView<ReferraPopup>
{
    public Button btComfirm;
    public Button btclose;
    public Button btCloseBg;
    public TMP_Text valueReward;
    public Image iconReward;
    public TMP_InputField inputCode;

    protected override void Awake()
    {
        base.Awake();
        btComfirm.onClick.AddListener(Comfirm);
        btclose.onClick.AddListener(CloseView);
        btCloseBg.onClick.AddListener(CloseView);
    }

    public void Show(Reward reward)
    {
        if (reward != null)
        {
            valueReward.gameObject.SetActive(true);
            iconReward.gameObject.SetActive(true);
            valueReward.text = reward?.Reward_.ToString();
            iconReward.sprite = ResourceManager.Instance.GetIconReward((RewardType)reward.RewardType);
        }
        else
        {
            valueReward.gameObject.SetActive(false);
            iconReward.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (inputCode.text.ToString().Length <= 0)
        {
            btComfirm.interactable = false;
        }

        if (inputCode.text.ToString().Length > 0)
        {
            btComfirm.interactable = true;
        }
    }

    public string GetTitle()
    {
        return "Referral";
    }

    public void Comfirm()
    {
        string input = inputCode.text.Trim();
        ReferenceCodeRequest request = new ReferenceCodeRequest
        {
            code = input,
            userCodeId = HCAppController.Instance.userInfo.UserCodeId
        };
        string token = HCAppController.Instance.GetTokenLogin();
        StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlReferenceCode(),
            JsonUtility.ToJson(request), token,
            (data) =>
            {
                var isCouponData = (string)data;
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                String message = jsonObject.message;

                HcPopupManager.Instance.ShowNotifyPopup(message.ToString(), GetTitle());
                inputCode.text = "";
                RefeClaimSuccess(data);
                Debug.Log("RequestWebApiPost ReferenceCodeRequest data " + data);
            }));
    }

    private void RefeClaimSuccess(string dataResponse)
    {

        JObject data = JObject.Parse(dataResponse);

        var tmpReference = new ReferenceResspone(data);
        if (tmpReference.isSuccess)
        {
            if (tmpReference.reward != null)
            {
                var parser = new MessageParser<Reward>(() => new Reward());
                Reward reward = parser.ParseFrom(tmpReference.reward);
                CoinFlyAnimation.Instance.SpawnRewardClaim(reward, new Vector2(0, 0), null);
                Debug.Log("đã bay");
            }
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }
    }

 
}
public class ReferenceResspone
{
    public string message;
    
    public bool isSuccess;
    public byte[] reward;

    public ReferenceResspone(JObject data)
    {
        message = (string)data["message"];
        isSuccess = (bool)data["isSuccess"];
        reward = (byte[])data["reward"];
    }
}
