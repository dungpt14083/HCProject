using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameModeUI : UIView<GameModeUI>
{
    public Image imgIconGame;
    public TMP_Text nameMiniGame;
    public ScrollRect scrollRect;
    public ContentSizeFitter verticalLayoutContent;
    public ItemGameMode itemHeadToHeadPrefab;
    public ItemGameMode itemOtherPrefab;

    public ContentSizeFitter groupHeadToHead;
    public ContentSizeFitter groupKnockOut;
    public ContentSizeFitter groupRoundRobin;
    public ContentSizeFitter groupOneVsMany;

    private List<ItemGameMode> listGameModeTypeHeadToHead = new List<ItemGameMode>();
    private List<ItemGameMode> listKnockOutRoundTour = new List<ItemGameMode>();
    private List<ItemGameMode> listRoundRobin = new List<ItemGameMode>();
    private List<ItemGameMode> listOneVsMany = new List<ItemGameMode>();

    [SerializeField] private Button btnHeadtoHead;
    [SerializeField] private Button btnKnokOutRoundTour;
    [SerializeField] private Button btnRoundRobin;
    [SerializeField] private Button btnOneVsMany;

    private bool isShowHead = true;
    private bool isKnockOut = true;
    private bool isRoundRobin = true;

    private bool isOneVsMany = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    protected override void Awake()
    {
        base.Awake();
        btnHeadtoHead.onClick.AddListener(btnHeadtoHeadvoid);
        btnKnokOutRoundTour.onClick.AddListener(btnKnockOutRoundTourvoid);
        btnOneVsMany.onClick.AddListener(btnOneVsManyvoid);
        btnRoundRobin.onClick.AddListener(btnRoundRobinvoid);
    }

    // Update is called once per frame
    void Update()
    {
        // btnHeadtoHead.onClick.AddListener(btnHideItemHeadtoHead(GameModeType.HeadToHead));
    }

    private void btnHeadtoHeadvoid()
    {
        StartCoroutine(btnHideItemHeadtoHead());
    }

    private void btnOneVsManyvoid()
    {
        StartCoroutine(btnHideItemOneVsMany());
    }

    private void btnRoundRobinvoid()
    {
        StartCoroutine(btnHideItemRoundRobin());
    }

    private void btnKnockOutRoundTourvoid()
    {
        StartCoroutine(btnHideItemKnockOutRoundTour());
    }

    private IEnumerator btnHideItemHeadtoHead()
    {
        List<ItemGameMode> itemGameLists = new List<ItemGameMode>();
        itemGameLists = listGameModeTypeHeadToHead;
        foreach (var item in itemGameLists)
        {
            if (isShowHead == true)
            {
                item.transform.gameObject.SetActive(false);
            }
            else
            {
                item.transform.gameObject.SetActive(true);
            }
        }

        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return null;
        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.verticalScrollbar.value = 1;
        //check bool stt
        isShowHead = !isShowHead;
    }

    public IEnumerator btnHideItemOneVsMany()
    {
        List<ItemGameMode> itemGameLists = new List<ItemGameMode>();
        itemGameLists = listOneVsMany;
        foreach (var item in itemGameLists)
        {
            if (isOneVsMany == true)
            {
                item.transform.gameObject.SetActive(false);
            }
            else
            {
                item.transform.gameObject.SetActive(true);
            }
        }

        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return null;
        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.verticalScrollbar.value = 1;
        isOneVsMany = !isOneVsMany;
    }

    public IEnumerator btnHideItemRoundRobin()
    {
        List<ItemGameMode> itemGameLists = new List<ItemGameMode>();
        itemGameLists = listRoundRobin;
        foreach (var item in itemGameLists)
        {
            if (isRoundRobin == true)
            {
                item.transform.gameObject.SetActive(false);
            }
            else
            {
                item.transform.gameObject.SetActive(true);
            }
        }

        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return null;
        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.verticalScrollbar.value = 1;
        isRoundRobin = !isRoundRobin;
    }

    public IEnumerator btnHideItemKnockOutRoundTour()
    {
        List<ItemGameMode> itemGameLists = new List<ItemGameMode>();
        itemGameLists = listKnockOutRoundTour;
        foreach (var item in itemGameLists)
        {
            if (isKnockOut == true)
            {
                item.transform.gameObject.SetActive(false);
            }
            else
            {
                item.transform.gameObject.SetActive(true);
            }
        }

        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return null;
        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.verticalScrollbar.value = 1;
        isKnockOut = !isKnockOut;
    }

    private Coroutine _coroutineShowView;

    public void ShowView(GameType gameType, List<DetailMiniGameProto> detailMiniGames, bool effect = false)
    {
        if (_coroutineShowView != null)
        {
            StopCoroutine(_coroutineShowView);
        }

        _coroutineShowView = StartCoroutine(Show(gameType, detailMiniGames, effect));
    }


    public IEnumerator Show(GameType gameType, List<DetailMiniGameProto> detailMiniGames, bool effect = false)
    {
        imgIconGame.sprite = ResourceManager.Instance.GetIconGame(gameType);
        nameMiniGame.text = ResourceManager.Instance.GetNameGame(gameType);

        ScreenManagerHC.Instance.groupBottomHomeController.SettingImageForButtonEvent(imgIconGame.sprite,
            nameMiniGame.text.ToString());
        SetDefault();
        /*
        if (effect) FadeIn();
        else myContents.SetActive(true);
        */
        foreach (var data in detailMiniGames)
        {
            GameModeType modeType = (GameModeType)data.ModeType;
            var group = GetGroupObj(modeType);
            var itemPrefab = GetItemPrefab(modeType);
            var item = Instantiate(itemPrefab, group.transform);
            item.gameObject.name = "itemGameMode" + data.MiniGameEventName;
            item.Show(data, JoinGameModeCallback);
            switch (modeType)
            {
                case GameModeType.HeadToHead:
                    listGameModeTypeHeadToHead.Add(item);
                    break;

                case GameModeType.RoundRobin:
                    listRoundRobin.Add(item);
                    break;

                case GameModeType.OneVsMany:
                    listOneVsMany.Add(item);
                    break;

                case GameModeType.KnockOutRoundTour:
                    listKnockOutRoundTour.Add(item);
                    break;
            }
        }

        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return null;
        verticalLayoutContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.verticalScrollbar.value = 1;
    }


    #region CHECKSTATUSTOURNAMENT

    private DetailMiniGameProto _currentDetailMiniGameProto;
    private Coroutine _sendToServer;

    private void JoinGameModeCallback(DetailMiniGameProto data)
    {
        _currentDetailMiniGameProto = data;
        if (_currentDetailMiniGameProto.ModeType == 1 || _currentDetailMiniGameProto.ModeType == 4)
        {
            ScreenManagerHC.Instance.ShowGameModeInfo(_currentDetailMiniGameProto);
        }
        else
        {
            ScreenManagerHC.Instance.CheckTournamentStatusAndShow(_currentDetailMiniGameProto);
        }
    }

    #endregion


    public IEnumerator LoadData(List<DetailMiniGameProto> detailMiniGames)
    {
        foreach (var data in detailMiniGames)
        {
            GameModeType modeType = (GameModeType)data.ModeType;
            var group = GetGroupObj(modeType);
            var itemPrefab = GetItemPrefab(modeType);
            var item = Instantiate(itemPrefab, group.transform);
            item.gameObject.name = "itemGameMode" + data.MiniGameEventName;
            item.Show(data, null);
        }

        this.verticalLayoutContent.gameObject.SetActive(false);
        yield return new WaitForSeconds(.1f);
        this.verticalLayoutContent.gameObject.SetActive(true);
    }

    public GameObject GetGroupObj(GameModeType modeType)
    {
        switch (modeType)
        {
            case GameModeType.HeadToHead:
                return groupHeadToHead.gameObject;
            case GameModeType.KnockOutRoundTour:
                return groupKnockOut.gameObject;
            case GameModeType.RoundRobin:
                return groupRoundRobin.gameObject;
            case GameModeType.OneVsMany:
                return groupOneVsMany.gameObject;
            default:
                return null;
        }
    }

    public ItemGameMode GetItemPrefab(GameModeType modeType)
    {
        switch (modeType)
        {
            case GameModeType.HeadToHead:
                return itemHeadToHeadPrefab;
            case GameModeType.KnockOutRoundTour:
                return itemOtherPrefab;
            case GameModeType.RoundRobin:
                return itemOtherPrefab;
            case GameModeType.OneVsMany:
                return itemOtherPrefab;
            default:
                return null;
        }
    }

    public void SetDefault()
    {
        listGameModeTypeHeadToHead = new List<ItemGameMode>();
        listKnockOutRoundTour = new List<ItemGameMode>();
        listRoundRobin = new List<ItemGameMode>();
        listOneVsMany = new List<ItemGameMode>();
        Clear(groupHeadToHead.transform);
        Clear(groupKnockOut.transform);
        Clear(groupRoundRobin.transform);
        Clear(groupOneVsMany.transform);
    }

    public void Clear(Transform parentClear)
    {
        for (int i = parentClear.childCount - 1; i >= 0; i--)
        {
            if (parentClear.GetChild(i).TryGetComponent(Type.GetType("ItemGameMode"), out Component component))
            {
                if (parentClear.GetChild(i).gameObject.activeSelf)
                {
                    BonusPool.DeSpawn(parentClear.GetChild(i));
                }
            }
        }
    }
}