using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemRewardInvite : MonoBehaviour
{
    public TMP_Text txtIndex;
    public TMP_Text txtValue;
    public Image imgIcon;
    public Button btnGetReward;
    public int indexInvite;
    
    public void Show(int _index, long _value, RewardType rewardType)
    {
        gameObject.SetActive(true);
        btnGetReward.onClick.AddListener(_GetReward);
        indexInvite = _index;
        txtIndex.text = _index.ToString();
        txtValue.text = $"+{_value}";
        imgIcon.sprite = ResourceManager.Instance.GetIconReward(rewardType); 
    }

    private void _GetReward()
    {
        
        InviteRequest request = new InviteRequest
        {
            invited = indexInvite,
            userCodeId = HCAppController.Instance.userInfo.UserCodeId
        };
        string token = HCAppController.Instance.GetTokenLogin();
        StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlInvitedFriend(),
            JsonUtility.ToJson(request), token,
            (data) =>
            {
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                bool isSuccess = (bool)jsonObject.isSuccess;
                string message = (string)jsonObject.message; 
                if (isSuccess)
                {
                    HcPopupManager.Instance.ShowNotifyPopup(message.ToString(), "Notification");
                    _InviteClaimSuccess(data);
                }

                if (!isSuccess)
                {
                    HcPopupManager.Instance.ShowNotifyPopup(message.ToString(), "Notification");
                }
            }));
    }
    private void _InviteClaimSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);

        var tmpCoupon = new InviteRespone(data);
        if (tmpCoupon.isSuccess)
        {
            if (tmpCoupon.listReward != null)
            {
                var parser = new MessageParser<ListReward>(() => new ListReward());
                ListReward reward = parser.ParseFrom(tmpCoupon.listReward);
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

public class InviteRequest
{
    public string userCodeId;
    public int invited;
}

public class InviteRespone
{
    public string message;
    
    public bool isSuccess;
    public byte[] listReward;

    public InviteRespone(JObject data)
    {
        message = (string)data["message"];
        isSuccess = (bool)data["isSuccess"];
        listReward = (byte[])data["listReward"];
    }
}