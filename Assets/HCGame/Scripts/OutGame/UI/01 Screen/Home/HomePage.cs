using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using BonusGame;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;
using TMPro;

public class HomePage : UIView<HomePage>
{
    public TextMeshProUGUI txtJackPot;
    [SerializeField] private ItemGameList itemGameListPrefab;
    [SerializeField] private Transform holder;

    private Coroutine _coroutineGetTopJackPot;

    private void OnEnable()
    {
        Show();
        UISignals.OnUpdateJackPot.AddListener(UpdateJackPot);
        UISignals.OnUpdateListGameInHome.AddListener(LoadListGame);
        UISignals.WeeklyJackPotRefresh.AddListener(OnClickJackPot);
    }

    private void OnDisable()
    {
        UISignals.OnUpdateJackPot.RemoveListener(UpdateJackPot);
        UISignals.OnUpdateListGameInHome.RemoveListener(LoadListGame);
        UISignals.WeeklyJackPotRefresh.RemoveListener(OnClickJackPot);
    }

    private void Show()
    {
        UpdateJackPot();
        LoadListGame();
    }

    private void UpdateJackPot()
    {
        txtJackPot.text = HCAppController.Instance?.totalJackpot.ToString("N0");
    }

    private void LoadListGame()
    {
        ListMiniGameProto listMiniGame = HCAppController.Instance?.listMiniGame;
        ClearListGame();
        if (listMiniGame == null) return;
        foreach (var game in listMiniGame.ListMiniGame)
        {
            var item = BonusPool.Spawn(itemGameListPrefab, holder);
            item.gameObject.name = "itemGame" + game.Name;
            item.Show((GameType)(game.Id), game.Name);
        }
    }

    public void ClearListGame()
    {
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }
    
    #region SHOWJACKPOT

    public void OnClickJackPot()
    {
        if (_coroutineGetTopJackPot != null)
        {
            StopCoroutine(_coroutineGetTopJackPot);
        }

        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETTOPJACKPOT;
        _coroutineGetTopJackPot =
            StartCoroutine(APIUtils.RequestWebApiGetJson(url, SuccessShowTopJackPot, ErrorShowTopJackPot));
    }

    private void SuccessShowTopJackPot(JObject obj)
    {
        HcPopupManager.Instance.DisplayJackPot(new TopJackPotData(obj));
    }

    private void ErrorShowTopJackPot(string obj)
    {
        Toast.Show(obj);
    }

    #endregion
}

public class TopJackPotData
{
    public List<ItemTopJackPot> listAllTime = new List<ItemTopJackPot>();
    public List<ItemTopJackPot> listWeekly = new List<ItemTopJackPot>();

    public long timeRemain = 0;


    public TopJackPotData(JObject data)
    {
        var tmpDataAllTime = (JArray)data["topJackpotAllTime"];
        listAllTime.Clear();
        foreach (JObject tmp in tmpDataAllTime)
        {
            listAllTime.Add(new ItemTopJackPot(tmp));
        }

        var tmpDataWeekly = (JArray)data["topJackpotInWeek"];
        listWeekly.Clear();
        foreach (JObject tmp2 in tmpDataWeekly)
        {
            listWeekly.Add(new ItemTopJackPot(tmp2));
        }

        timeRemain = (long)data["getNextMonday"];
    }
}

public class ItemTopJackPot
{
    public string userId;
    public string avatar;
    public string username;
    public long reward;
    public int rank;

    public ItemTopJackPot(JObject data)
    {
        userId = (string)data["userCodeId"];
        avatar = (string)data["avatar"];
        username = (string)data["username"];
        reward = (long)data["totalReward"];
        rank = (int)data["rank"];
    }
}