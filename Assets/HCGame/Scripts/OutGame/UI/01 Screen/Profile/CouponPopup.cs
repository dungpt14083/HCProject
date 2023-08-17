using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CouponPopup : UIPopupView<CouponPopup>
{
    public Button btSubmit;
    public Button closeButton;
    public Button closeBg;
    public TMP_InputField input;
    private NotifyPopup _NotifyPopup;

    protected override void Awake()
    {
        base.Awake();
        btSubmit.onClick.AddListener(Submit);
        closeButton.onClick.AddListener(CloseView);
        closeBg.onClick.AddListener(CloseView);

    }
    

    private void Update()
    {
        if (input.text.ToString().Length <= 0)
        {
            btSubmit.interactable = false;
        }

        if (input.text.ToString().Length > 0)
        {
            btSubmit.interactable = true;
        }
    }

    public void Show()
    {
        Debug.Log("token: " + HCAppController.Instance.GetTokenLogin());
        Debug.Log("username: " + HCAppController.Instance.userInfo.UserCodeId);
    }

    public string GetTitle()
    {
        return "Coupon";
    }

    private void Submit()
    {
        var value = input.text.Trim();

        CouponRequest request = new CouponRequest
        {
            code = value,
            userCodeId = HCAppController.Instance.userInfo.UserCodeId
        };
        string token = HCAppController.Instance.GetTokenLogin();
        StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlCouponCode(),
            JsonUtility.ToJson(request), token,
            (data) =>
            {
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                String message = jsonObject.message;
                bool isSucces = (bool)jsonObject.isSuccess;
                if (isSucces)
                {
                    HcPopupManager.Instance.ShowNotifyPopup("Coupon code success", GetTitle());
                    CouponClaimSuccess(data);
                }

                if (!isSucces)
                {
                    HcPopupManager.Instance.ShowNotifyPopup("Coupon code not success", GetTitle());
                }

                input.text = "";
            }));
    }
    private void CouponClaimSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);

        var tmpCoupon = new CouponResspone(data);
        if (tmpCoupon.isSuccess)
        {
            if (tmpCoupon.reward != null)
            {
                var parser = new MessageParser<ListReward>(() => new ListReward());
                ListReward reward = parser.ParseFrom(tmpCoupon.reward);
                CoinFlyAnimation.Instance.SpawnListRewardClaim(reward, new Vector2(0, 0), null);
                Debug.Log("đã bay");
            }
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }
    }
    
    
}

public class CouponResspone
{
    public string message;
    
    public bool isSuccess;
    public byte[] reward;

    public CouponResspone(JObject data)
    {
        message = (string)data["message"];
        isSuccess = (bool)data["isSuccess"];
        reward = (byte[])data["reward"];
    }
}
