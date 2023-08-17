using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using Google.Protobuf;

public class RankingPage : UIView<RankingPage>
{
    //private Color32 colorSelected = new Color32(255, 255, 255, 255);
    //private Color32 colorUnSelected = new Color32(255, 255, 255, 81);

    // public RankingItemTop top1;
    // public RankingItemTop top2;
    // public RankingItemTop top3;
    // public RectTransform top1RectTransform;
    // public RectTransform top2RectTransform;
    // public RectTransform top3RectTransform;

    //public RectTransform scrollData;

    //public RankingItemNormal myRank;
    // public RankingItemNormal rankNormalDefault;
    // public TMP_Text timeCount;
    //
    // private TimeSpan timeReset;
    // private MoneyType rankingType = MoneyType.Gold;
    //
    // public GameObject lightEffect;
    // public RectTransform characterEffect;
    // private Vector3 posDefaultCharacter = new Vector3(0, -500, 0);
    // private Vector3 posDefaultTop1 = new Vector3(0, -200, 0);
    // private Vector3 posDefaultTop2 = new Vector3(-300, -200, 0);
    // private Vector3 posDefaultTop3 = new Vector3(300, -200, 0);
    //
    // private int posYEndMoveTop1 = 30;
    // private int posYEndMoveTop2 = 45;
    // private int posYEndMoveTop3 = 45;
    // private int posYEndMovedata = -55;
    // public ScrollRect ScrollRect;
    // public Button btnScrollMyRank;
    //
    // private bool isMyrank0 = false;


    #region THANGREFACTORING

    [SerializeField] private List<UIButtonTab> listButtonTab;
    [SerializeField] private List<RankingItemTop> top123;

    [SerializeField] private RankingItemNormal prefabItemRanking;
    [SerializeField] private Transform holder;
    [SerializeField] private RankingItemNormal myRank;

    [SerializeField] private ScrollRect scrollView;

    private ListUserRankingProto _userRankingProto;

    private Coroutine _coroutineSendToTakeRanking;
    private bool _isShowTokenRanking = true;
    private bool _isShowWithMyRank = false;

    private UserRankingProto _myRankData;
    private List<UserRankingProto> _listUser123 = new List<UserRankingProto>();
    private List<UserRankingProto> _listUserRankingFrom4 = new List<UserRankingProto>();
    private List<UserRankingProto> _listRankingHaveOutTopFrom4 = new List<UserRankingProto>();

    protected override void Awake()
    {
        base.Awake();
        InitButton();
    }

    private void OnEnable()
    {
        _isShowTokenRanking = true;
        _isShowWithMyRank = false;
    }

    private void InitButton()
    {
        for (int i = 0; i < listButtonTab.Count; i++)
        {
            listButtonTab[i].OnShow(OnChangeFilterSelect);
        }
    }

    public void ShowViewFirst()
    {
        OnChangeFilterSelect(TypeButtonTab.Tab1);
    }

    private void OnChangeFilterSelect(TypeButtonTab type)
    {
        SetDefault();
        switch (type)
        {
            case TypeButtonTab.Tab1:
                ShowViewTokenRanking();
                break;
            case TypeButtonTab.Tab2:
                ShowViewGoldRanking();
                break;
            default:
                break;
        }

        foreach (var item in listButtonTab)
        {
            item.OnSelectFilter(type);
        }
    }


    private void ShowViewTokenRanking()
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_TOKENRANKING +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}";
        SendToTakeRanking(url, true);
    }

    private void ShowViewGoldRanking()
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GOLDRANKING +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}";
        SendToTakeRanking(url, false);
    }

    private void SendToTakeRanking(string url, bool isTokenRanking)
    {
        _isShowTokenRanking = isTokenRanking;
        if (_coroutineSendToTakeRanking != null)
        {
            StopCoroutine(_coroutineSendToTakeRanking);
        }

        _coroutineSendToTakeRanking =
            StartCoroutine(APIUtils.RequestWebApiGetByte(url, ReceivedDataRanking, ErrorRanking));
    }

    private void ErrorRanking(string obj)
    {
        Toast.Show("Lỗi Ranking rồi!" + obj);
    }

    private void ReceivedDataRanking(byte[] data)
    {
        if (data == null)
        {
            Toast.Show("CHUA CO DU LIEU");
            return;
        }

        var parser = new MessageParser<ListUserRankingProto>(() => new ListUserRankingProto());
        ListUserRankingProto listUserRanking = parser.ParseFrom(data);
        _userRankingProto = listUserRanking;
        CalDataRanking();
        ShowViewRanking();
    }

    private void CalDataRanking()
    {
        if (_userRankingProto == null) return;
        _myRankData =
            _userRankingProto.ListUserRanking.First(s => s.UserId == HCAppController.Instance.userInfo.UserCodeId);
        if (_myRankData == null)
        {
            _myRankData =
                _userRankingProto.ListRankingOutTop.First(s =>
                    s.UserId == HCAppController.Instance.userInfo.UserCodeId);
        }

        _listUser123 = _userRankingProto.ListUserRanking.Take(3).ToList();
        _listUserRankingFrom4 = _userRankingProto.ListUserRanking.Skip(3).ToList();
        _listRankingHaveOutTopFrom4.AddRange(_listUserRankingFrom4);

        var tmpCount = _userRankingProto.ListUserRanking.Count;
        if (tmpCount > 0)
        {
            var tmpLastRankInTop50 = _userRankingProto.ListUserRanking[tmpCount - 1];
            for (int i = 0; i < _userRankingProto.ListRankingOutTop.Count(); i++)
            {
                if (_userRankingProto.ListRankingOutTop[i].Rank > tmpLastRankInTop50.Rank)
                {
                    _listRankingHaveOutTopFrom4.Add(_userRankingProto.ListRankingOutTop[i]);
                }
            }
        }
    }

    private void ShowViewRanking()
    {
        if (_userRankingProto == null) return;
        ShowItemRanking123();
        ShowItemRankFor4();
        ShowMyRank();
    }

    private void ShowItemRanking123()
    {
        var tmpIndex = 0;
        for (int i = 0; i < top123.Count() && i < _listUser123.Count; i++)
        {
            top123[i].ShowView(_listUser123[i], _isShowTokenRanking);
            tmpIndex++;
        }

        if (tmpIndex < top123.Count())
        {
            for (int j = tmpIndex; j < top123.Count(); j++)
            {
                top123[j].ShowDefault(_isShowTokenRanking);
                tmpIndex++;
            }
        }
    }


    private void ShowItemRankFor4()
    {
        SetDefaultScrollView();
        if (!_isShowWithMyRank)
        {
            for (int i = 0; i < _listUserRankingFrom4.Count; i++)
            {
                var tmp = BonusPool.Spawn(prefabItemRanking, holder);
                tmp.ShowView(_listUserRankingFrom4[i], _isShowTokenRanking);
            }
        }
        else
        {
            for (int i = 0; i < _listRankingHaveOutTopFrom4.Count; i++)
            {
                var tmp = BonusPool.Spawn(prefabItemRanking, holder);
                tmp.ShowView(_listRankingHaveOutTopFrom4[i], _isShowTokenRanking);
            }
        }
    }

    private void ShowMyRank()
    {
        //CÁI FUNC NÚT BẤM Ở ĐÂY LUÔN
        if (_myRankData != null)
        {
            myRank.ShowViewMyRank(_myRankData, _isShowTokenRanking);
            myRank.GetComponent<Button>().onClick.RemoveAllListeners();
            myRank.GetComponent<Button>().onClick.AddListener(ShowWithMyRank);
        }
        else
        {
            myRank.ShowViewMyRankNull(_isShowTokenRanking);
        }
    }

    private void ShowWithMyRank()
    {
        if (myRank == null) return;
        _isShowWithMyRank = true;
        ShowItemRankFor4();
        var playerRank = _myRankData.Rank;
        if (playerRank == 0)
        {
            playerRank = _isShowWithMyRank ? _listRankingHaveOutTopFrom4.Count : _listUserRankingFrom4.Count;
            if (!_isShowWithMyRank)
            {
                float position = 1 - ((float)playerRank) / (float)_listUserRankingFrom4.Count;
                position = Mathf.Clamp01(position);
                scrollView.verticalNormalizedPosition = position;
            }
            else
            {
                float position = 1 - ((float)playerRank) / (float)_listRankingHaveOutTopFrom4.Count;
                position = Mathf.Clamp01(position);
                scrollView.verticalNormalizedPosition = position;
            }
            return;
        }

        if (playerRank >= 4)
        {
            if (!_isShowWithMyRank)
            {
                float position = 1 - ((float)playerRank - 4) / (float)_listUserRankingFrom4.Count;
                position = Mathf.Clamp01(position);
                scrollView.verticalNormalizedPosition = position;
            }
            else
            {
                float position = 1 - ((float)playerRank - 4) / (float)_listRankingHaveOutTopFrom4.Count;
                position = Mathf.Clamp01(position);
                scrollView.verticalNormalizedPosition = position;
            }
        }
    }

    private void SetDefault()
    {
        _isShowWithMyRank = false;
        _listUser123.Clear();
        _listUserRankingFrom4.Clear();
        _listRankingHaveOutTopFrom4.Clear();
        ResetShowViewScroll();
        SetDefaultScrollView();
    }

    private void SetDefaultScrollView()
    {
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }

    private void ResetShowViewScroll()
    {
        scrollView.verticalNormalizedPosition = 1;
    }

    #endregion


    #region OLDANHNGOCANH

    public void Effect()
    {
        // top1RectTransform.anchoredPosition = posDefaultTop1;
        // top2RectTransform.anchoredPosition = posDefaultTop2;
        // top3RectTransform.anchoredPosition = posDefaultTop3;
        // //scrollData.anchoredPosition = postDefaultScroll;
        // characterEffect.anchoredPosition = posDefaultCharacter;
        //
        // lightEffect.transform.DOLocalRotate(new Vector3(0, 0, -360), 4, RotateMode.FastBeyond360).SetLoops(-1)
        //     .SetRelative(true).SetEase(Ease.Linear);
        // characterEffect.DOAnchorPosY(0, 2).SetEase(Ease.Linear);
        // top1RectTransform.DOAnchorPosY(posYEndMoveTop1, 2).SetEase(Ease.Linear);
        // top2RectTransform.DOAnchorPosY(posYEndMoveTop2, 2).SetEase(Ease.Linear);
        // top3RectTransform.DOAnchorPosY(posYEndMoveTop3, 2).SetEase(Ease.Linear);
        //scrollData.DOAnchorPosY(posYEndMovedata, 2).SetEase(Ease.Linear);
    }

    public override void OnClose(Action onComplete = null)
    {
        base.OnClose(onComplete);
        // DOTween.Kill(lightEffect.transform);
        // DOTween.Kill(characterEffect);
        // DOTween.Kill(top1RectTransform);
        // DOTween.Kill(top2RectTransform);
        // DOTween.Kill(top3RectTransform);
    }

    public void ShowTokenRanking()
    {
        // ScrollRect.verticalNormalizedPosition = 1;
        // underLineTabToken.SetActive(true);
        // txtTabToken.color = colorSelected;
        // underLineTabGold.SetActive(false);
        // txtTabGold.color = colorUnSelected;
        Effect();
        // return;
        /*
        ClearAll();
        if (HCAppController.Instance.listUserTokenRanking == null ||
            HCAppController.Instance.listUserTokenRanking.ListUserTokenRanking == null)
            return;
        timeReset = TimeSpan.FromSeconds(HCAppController.Instance.listUserTokenRanking.EndDate);
        var data = HCAppController.Instance.listUserTokenRanking.ListUserTokenRanking.OrderBy(x => x.Rank);
        var myData = data.FirstOrDefault(x => x.UserId == HCAppController.Instance.userInfo.UserCodeId);
        if (myData != null)
        {
            if (myData.Rank == 0)
            {
                isMyrank0 = true;
                myData.Rank = data.Max(x => x.Rank) + 1;
                data = data.OrderBy(x => x.Rank); // Reorder the data after updating the rank of the user with rank 0
            }
            else
            {
                isMyrank0 = false;
            }

            foreach (var item in data)
            {
                if (item.UserId == HCAppController.Instance.userInfo.UserCodeId)
                {
                    //show my rank data
                    myRank.Show(item.UserId, item.Avatar, item.HcUserName, item.Token, item.Reward, item.TypeReward,
                        item.Rank);
                }

                if (item.Rank == 1)
                {
                    top1.Show(item.UserId, item.Avatar, item.HcUserName, item.Token, item.Reward, item.TypeReward);
                }
                else if (item.Rank == 2)
                {
                    top2.Show(item.UserId, item.Avatar, item.HcUserName, item.Token, item.Reward, item.TypeReward);
                }
                else if (item.Rank == 3)
                {
                    top3.Show(item.UserId, item.Avatar, item.HcUserName, item.Token, item.Reward, item.TypeReward);
                }
                else
                {
                    var gobj = GameObject.Instantiate(rankNormalDefault, rankNormalDefault.transform.parent);
                    gobj.Show(item.UserId, item.Avatar, item.HcUserName, item.Token, item.Reward, item.TypeReward,
                        item.Rank);
                    Debug.Log("log type" + item.TypeReward);
                }
            }
        }*/
    }

    public void ShowGoldRanking()
    {
        // ScrollRect.verticalNormalizedPosition = 1;
        // underLineTabGold.SetActive(true);
        // txtTabGold.color = colorSelected;
        // underLineTabToken.SetActive(false);
        // txtTabToken.color = colorUnSelected;
        Effect();
        /*
        ClearAll();
        if (HCAppController.Instance.listUserGoldRanking == null ||
            HCAppController.Instance.listUserGoldRanking.ListUserGoldRanking == null)
            return;
        timeReset = TimeSpan.FromSeconds(HCAppController.Instance.listUserGoldRanking.EndDate);
        var data = HCAppController.Instance.listUserGoldRanking.ListUserGoldRanking.OrderBy(x => x.Rank);
        var myData = data.FirstOrDefault(x => x.UserId == HCAppController.Instance.userInfo.UserCodeId);
        if (myData != null)
        {
            if (myData.Rank == 0)
            {
                isMyrank0 = true;
                myData.Rank = data.Max(x => x.Rank) + 1;
                data = data.OrderBy(x => x.Rank);
            }
            else
            {
                isMyrank0 = false;
            }

            foreach (var item in data)
            {
                if (item.UserId == HCAppController.Instance.userInfo.UserCodeId)
                {
                    myRank.Show(item.UserId, item.Avatar, item.HcUserName, item.Gold, item.Reward, item.TypeReward,
                        item.Rank);
                }

                if (item.Rank == 1)
                {
                    top1.Show(item.UserId, item.Avatar, item.HcUserName, item.Gold, item.Reward, item.TypeReward);
                }
                else if (item.Rank == 2)
                {
                    top2.Show(item.UserId, item.Avatar, item.HcUserName, item.Gold, item.Reward, item.TypeReward);
                }
                else if (item.Rank == 3)
                {
                    top3.Show(item.UserId, item.Avatar, item.HcUserName, item.Gold, item.Reward, item.TypeReward);
                }
                else
                {
                    var gobj = GameObject.Instantiate(rankNormalDefault, rankNormalDefault.transform.parent);
                    gobj.Show(item.UserId, item.Avatar, item.HcUserName, item.Gold, item.Reward, item.TypeReward,
                        item.Rank);
                }
            }
        }*/
    }


    private int GetPlayerRank()
    {
        // if (HCAppController.Instance.listUserTokenRanking == null ||
        //     HCAppController.Instance.listUserTokenRanking.ListUserTokenRanking == null)
        //     return -1;

        int playerRank = -1;
        // foreach (var item in HCAppController.Instance.listUserTokenRanking.ListUserTokenRanking.OrderBy(x => x.Rank))
        // {
        //     playerRank++;
        //     if (item.UserId == HCAppController.Instance.userInfo.UserCodeId)
        //         break;
        // }

        return playerRank;
    }

    public void ScrollToPlayer()
    {
        int playerRank = GetPlayerRank();
        // if (playerRank >= 0)
        // {
        //     if (isMyrank0 == true)
        //     {
        //         float position = 1 - (float)playerRank +
        //                          1 / (float)HCAppController.Instance.listUserTokenRanking.ListUserTokenRanking.Count;
        //         position = Mathf.Clamp01(position);
        //         ScrollRect.verticalNormalizedPosition = position;
        //     }
        //     else if (isMyrank0 == false)
        //     {
        //         float position = 1 - (float)playerRank /
        //             (float)HCAppController.Instance.listUserTokenRanking.ListUserTokenRanking.Count;
        //         position = Mathf.Clamp01(position);
        //         ScrollRect.verticalNormalizedPosition = position;
        //     }
        // }
    }

    #endregion
}