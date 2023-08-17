using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DailyReward : UIPopupView<DailyReward>
{
    #region THANGREFACTORING:::

    [SerializeField] private List<ItemDailyRewardDay1To6> listDay1To6;
    [SerializeField] private ItemDailyRewardDay7 itemDay7;

    private int _countCheckCurrent;
    private Coroutine _sendToClaim;


    public void ShowViewFirst()
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETDAILYREWARD +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}";
        Executors.Instance.StartCoroutine(APIUtils.RequestWebApiGetByte(url, TakeDataDailyRewardSuccess,
            TakeDataDailyRewardError));
    }

    private void TakeDataDailyRewardSuccess(byte[] data)
    {
        var parser = new MessageParser<ListHcDailyRewardProto>(() => new ListHcDailyRewardProto());
        ListHcDailyRewardProto listHcDailyRewardProto = parser.ParseFrom(data);
        if (listHcDailyRewardProto == null) return;
        for (int i = 0; i < listHcDailyRewardProto.ListHcDailyReward.Count && i < listDay1To6.Count; i++)
        {
            listDay1To6[i].ShowView(listHcDailyRewardProto.ListHcDailyReward[i], listHcDailyRewardProto.CountCheck,
                listHcDailyRewardProto.Received, SendToClaim);
        }

        itemDay7.ShowView(listHcDailyRewardProto.Date7, listHcDailyRewardProto.CountCheck,
            listHcDailyRewardProto.Received, SendToClaim);
    }

    private void TakeDataDailyRewardError(string error)
    {
        Toast.Show(error);
    }

    private void SendToClaim(int indexClaim)
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETREWARDDAILYREWARD;

        if (_sendToClaim != null)
        {
            StopCoroutine(_sendToClaim);
        }

        ClaimDailyRewardRequest request = new ClaimDailyRewardRequest
        {
            userCodeId = HCAppController.Instance.userInfo.UserCodeId,
            day = indexClaim
        };

        StartCoroutine(APIUtils.RequestWebApiPost(url, JsonUtility.ToJson(request),
            HCAppController.Instance.GetTokenLogin(), DailyRewardClaimSuccess, ErrorNotification));
    }
    
    private void DailyRewardClaimSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);

        var tmpDailyRewardResponse = new DailyRewardResponse(data);
        if (tmpDailyRewardResponse.isSuccess)
        {
            if (tmpDailyRewardResponse.listReward != null)
            {
                var parser = new MessageParser<ListReward>(() => new ListReward());
                ListReward reward = parser.ParseFrom(tmpDailyRewardResponse.listReward);
                CoinFlyAnimation.Instance.SpawnListRewardClaim(reward, new Vector2(0, 0), null);
            }

            ShowViewFirst();
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }
    }
    
    private void ErrorNotification(string error)
    {
        Toast.Show(error);
    }

    #endregion
}

public struct ClaimDailyRewardRequest
{
    public string userCodeId;
    public int day;
}

public class DailyRewardResponse
{
    public string message;
    public bool isSuccess;
    public byte[] listReward;

    public DailyRewardResponse(JObject data)
    {
        message = (string)data["message"];
        isSuccess = (bool)data["isSuccess"];
        listReward = (byte[])data["reward"];
    }
}