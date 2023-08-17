using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
using DG.Tweening;
using Google.Protobuf;
using MoreMountains.Tools;
using Newtonsoft.Json.Linq;
using Tweens.Plugins;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DailyMission : UIPopupView<DailyMission>
{
    [SerializeField] private ItemDailyMission itemDailyMissionDefault;
    [SerializeField] private Transform holder;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        ShowView();
        UISignals.OnUpdateDailyMission.AddListener(ShowView);
    }

    private void OnDisable()
    {
        UISignals.OnUpdateDailyMission.RemoveListener(ShowView);
    }

    public void ShowView()
    {
        SetDefault();
        if (HCAppController.Instance.listHcDailyMission == null) return;
        var tmpListMission = HCAppController.Instance.listHcDailyMission.HcDailyMission.ToList();
        for (int i = 0; i < tmpListMission.Count; i++)
        {
            var tmpItemMission = BonusPool.Spawn(itemDailyMissionDefault, holder);
            tmpItemMission.ShowView(tmpListMission[i], ReceivedMission);
        }
    }


    private void ReceivedMission(long idMission)
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETREWARDDAILYMISSION;
        DailyMissionRequest request = new DailyMissionRequest
        {
            dailyMissionId = idMission,
            userCodeId = HCAppController.Instance.userInfo.UserCodeId
        };

        StartCoroutine(APIUtils.RequestWebApiPost(url, JsonUtility.ToJson(request),
            HCAppController.Instance.GetTokenLogin(), DailyMissionClaimSuccess, ErrorNotification));
    }

    private void DailyMissionClaimSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);

        var tmpDailyMissionResponse = new DailyMissionResponse(data);
        if (tmpDailyMissionResponse.isSuccess)
        {
            if (tmpDailyMissionResponse.reward != null)
            {
                var parser = new MessageParser<ListReward>(() => new ListReward());
                ListReward reward = parser.ParseFrom(tmpDailyMissionResponse.reward);
                CoinFlyAnimation.Instance.SpawnListRewardClaim(reward, new Vector2(0, 0), null);
            }
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


    private void SetDefault()
    {
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }
}