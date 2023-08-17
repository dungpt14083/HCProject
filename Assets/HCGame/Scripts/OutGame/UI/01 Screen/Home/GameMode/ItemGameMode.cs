using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemGameMode : MonoBehaviour
{
    public TextMeshProUGUI txtNameMode;
    public TextMeshProUGUI txtNumberPlayer;
    public TextMeshProUGUI txtMoney;
    public TextMeshProUGUI txtMoneyBag;
    public Image iconFee;
    public Image iconMoneyReward;
    public Image iconBagReward;
    public Button btPlay;
    public Button btItem;
    public GameObject objLock;
    public TMP_Text txtLevel;
    public TMP_Text txtNameTournament;


    private DetailMiniGameProto _detailMiniGameProto;
    private Action<DetailMiniGameProto> _invokeCallBackDetailMiniGameProto;


    public void Show(DetailMiniGameProto data, Action<DetailMiniGameProto> callback)
    {
        objLock.gameObject.SetActive(false);
        txtNameTournament.gameObject.SetActive(false);
        gameObject.SetActive(true);
        _invokeCallBackDetailMiniGameProto = callback;
        _detailMiniGameProto = data;
        LoadData();
    }

    private void LoadData()
    {
        if (_detailMiniGameProto == null) return;
        txtNameMode.text = _detailMiniGameProto.MiniGameEventName;
        txtNumberPlayer.text = _detailMiniGameProto.NumberInMiniGameEvent.ToString() + " players";

        var level = HCAppController.Instance.userInfo.Level;
        bool isLock = false;
        string strLevel = "";

        if (_detailMiniGameProto.OverLevel != null && level < _detailMiniGameProto.OverLevel)
        {
            isLock = true;
            strLevel = $"Level {_detailMiniGameProto.OverLevel}";
        }

        if (isLock)
        {
            objLock.gameObject.SetActive(true);
            txtLevel.text = strLevel;
            txtNameTournament.gameObject.SetActive(true);
            txtNameTournament.text = _detailMiniGameProto.MiniGameEventName;
        }

        if (_detailMiniGameProto.EntryFee != null)
        {
            txtMoney.text = _detailMiniGameProto.EntryFee.Fee_ > 0
                ? _detailMiniGameProto.EntryFee.Fee_.ToString()
                : "Free";

            var typeReward = _detailMiniGameProto.PrizePool.RewardType;
            var sprFeeMoney = ResourceManager.Instance.GetIconMoney((MoneyType)_detailMiniGameProto.EntryFee.FeeType);
            iconFee.sprite = sprFeeMoney;
            if (txtMoneyBag != null)
            {
                txtMoneyBag.text = _detailMiniGameProto.PrizePool.Reward_.ToString();
            }

            if (iconMoneyReward != null)
            {
                var sprMoneyReward =
                    ResourceManager.Instance.GetIconMoney((MoneyType)_detailMiniGameProto.PrizePool.RewardType);
                iconMoneyReward.sprite = sprMoneyReward;
            }
        }
        else
        {
            txtMoney.text = "";
            if (txtMoneyBag != null)
            {
                txtMoneyBag.text = "";
            }
        }


        btPlay.onClick.AddListener(() =>
        {
            _invokeCallBackDetailMiniGameProto?.Invoke(_detailMiniGameProto);
            //SendToServerReceivedInfoTournament();
            //UIManager.ins.ShowGameModeInfo(_detailMiniGameProto);
        });
        /*
        btItem.onClick.AddListener(() =>
        {
            _invokeCallBackDetailMiniGameProto?.Invoke(_detailMiniGameProto);
            //SendToServerReceivedInfoTournament();
            //UIManager.ins.ShowGameModeInfo(_detailMiniGameProto);
        });
        */
    }
}