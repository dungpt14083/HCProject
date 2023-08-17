using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ActivityPage : UIView<ActivityPage>
{
    #region THANGREFACTORING

    [SerializeField] private List<UIButtonTab> listButtonTab;
    [SerializeField] private ItemActivity itemPrefab;
    [SerializeField] private Transform holder;

    [SerializeField] private List<Sprite> listBg;
    [SerializeField] private Image title;
    [SerializeField] private TMP_Text txtTitle;

    [SerializeField] private Button claimAll;

    private Coroutine _coroutineSendToTakeHistory;
    private bool _isShowHistoryInProgress = true;
    private int _pageNumber = 0;

    protected override void Awake()
    {
        base.Awake();
        InitButton();
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
                title.sprite = listBg[0];
                txtTitle.text = "In Progress";
                ShowViewHistoryInProgress();
                break;
            case TypeButtonTab.Tab2:
                title.sprite = listBg[1];
                txtTitle.text = "Completed";
                ShowViewCompleted();
                break;
            default:
                break;
        }

        foreach (var item in listButtonTab)
        {
            item.OnSelectFilter(type);
        }
    }

    private void ShowViewHistoryInProgress(int pageNumber = 0)
    {
        //SetDefault();
        RefreshViewInprogress(pageNumber);
    }

    private void ShowViewCompleted(int pageNumber = 0)
    {
        //SetDefault();
        RefreshViewCompleted(pageNumber);
    }

    private void RefreshViewInprogress(int pageNumber = 0)
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETHISTORYINPROGRESS +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" + $"pageNumber={pageNumber}" + "&" +
                  "sizePage=20";
        SendToTakeHistory(url, true, pageNumber);
    }

    private void RefreshViewCompleted(int pageNumber = 0)
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETHISTORYCOMPLETED +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" + $"pageNumber={pageNumber}" + "&" +
                  "sizePage=20";
        SendToTakeHistory(url, false, pageNumber);
    }

    private void SendToTakeHistory(string url, bool isHistoryInProgress, int pageNumber)
    {
        _isShowHistoryInProgress = isHistoryInProgress;
        if (_coroutineSendToTakeHistory != null)
        {
            StopCoroutine(_coroutineSendToTakeHistory);
        }

        _tmpPageNumber = pageNumber;

        _coroutineSendToTakeHistory =
            StartCoroutine(APIUtils.RequestWebApiGetByte(url, ShowViewHistory, ErrorHistory));
    }

    private int _tmpPageNumber = 0;

    private void ErrorHistory(string obj)
    {
        Toast.Show("Lỗi History rồi!");
        StartCoroutine(ResetAgain());
    }

    private void ShowViewHistory(byte[] data)
    {
        if (data == null)
        {
            Toast.Show("CHUA CO DU LIEU");
            StartCoroutine(ResetAgain());
            return;
        }

        _pageNumber = _tmpPageNumber;
        if (_pageNumber < 0)
        {
            _pageNumber = 0;
        }
        Debug.LogError("wwwwwwwwwwwwwww" + _pageNumber);

        SetDefaultRefresh();
        var parser = new MessageParser<ListHcPlayHistoryProto>(() => new ListHcPlayHistoryProto());
        ListHcPlayHistoryProto listHcPlayHistory = parser.ParseFrom(data);

        for (int i = 0; i < listHcPlayHistory.ListHcPlayHistoryProto_.Count; i++)
        {
            var tmpItem = BonusPool.Spawn(itemPrefab, holder);
            tmpItem.ShowView(listHcPlayHistory.ListHcPlayHistoryProto_[i], _isShowHistoryInProgress, RefreshViewPage);
        }

        var haveItemNotClaim =
            listHcPlayHistory.ListHcPlayHistoryProto_.Any(s => s.Reward.Reward.Count > 0 && s.IsClaim == 0);
        if (haveItemNotClaim && !_isShowHistoryInProgress)
        {
            claimAll.gameObject.SetActive(true);
            claimAll.onClick.AddListener(ClamAll);
        }
        else
        {
            claimAll.gameObject.SetActive(false);
        }

        _lastValueChange = 0.5f;
        StartCoroutine(ResetAgain());
    }

    private IEnumerator ResetAgain()
    {
        yield return new WaitForSeconds(0.5f);
        _isLoading = false;
    }


    private void RefreshViewPage()
    {
        if (_isShowHistoryInProgress)
        {
            ShowViewHistoryInProgress(_pageNumber);
        }
        else
        {
            ShowViewCompleted(_pageNumber);
        }
    }

    private void ResetShowViewScroll()
    {
        scrollView.verticalNormalizedPosition = 1;
    }


    private Coroutine _sendToClaim;

    private void ClamAll()
    {
        if (_sendToClaim != null)
        {
            StopCoroutine(_sendToClaim);
        }

        var url = GameConfig.API_URL + GameConfig.API_TAIL_CLAIMALLHISTORY;
        ActivityHistoryClaimAllRequest request = new ActivityHistoryClaimAllRequest
        {
            userCodeId = HCAppController.Instance.userInfo.UserCodeId,
            accessToken = HCAppController.Instance.GetTokenLogin()
        };

        _sendToClaim = StartCoroutine(APIUtils.RequestWebApiPost(url, JsonUtility.ToJson(request),
            HCAppController.Instance.GetTokenLogin(), ClaimSuccess, ClaimError));
    }

    private void ClaimSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);
        var tmpActivityHistoryClaimResponse = new ActivityHistoryClaimResponse(data);
        if (tmpActivityHistoryClaimResponse.isSuccess)
        {
            if (tmpActivityHistoryClaimResponse.listReward != null)
            {
                var parser = new MessageParser<ListReward>(() => new ListReward());
                ListReward listReward = parser.ParseFrom(tmpActivityHistoryClaimResponse.listReward);
                CoinFlyAnimation.Instance.SpawnListRewardClaim(listReward, new Vector2(0, 0), null);
            }

            RefreshViewPage();
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }
    }

    private void ClaimError(string error)
    {
        Toast.Show(error);
    }


    private void SetDefault()
    {
        ResetShowViewScroll();
        _pageNumber = 0;
        _tmpPageNumber = 0;
        _isLoading = false;
        //_lastValueChange = 0.5f;
        claimAll.gameObject.Hide();
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }

    private void SetDefaultRefresh()
    {
        claimAll.onClick.RemoveAllListeners();
        ResetShowViewScroll();
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }

    private void OnDisable()
    {
        _isShowHistoryInProgress = true;
        _pageNumber = 0;
        _tmpPageNumber = 0;
        //_lastValueChange = 0.5f;
        if (_sendToClaim != null)
        {
            StopCoroutine(_sendToClaim);
        }
    }


    #region CHOVIEW KEO SCROSSVIEW DE VIEW THEM AJAX

    [SerializeField] private ScrollRect scrollView;
    private bool _isLoading = false;
    private float _lastValueChange = 1;

    void Start()
    {
        scrollView.onValueChanged.AddListener(OnScroll);
    }


    void OnScroll(Vector2 position)
    {
        var tmpValueChange = position.y;

        if (tmpValueChange == 1 && _pageNumber == 0) return;
        if (tmpValueChange == 0 && _pageNumber != 0) return;

        if (!_isLoading && ((tmpValueChange >= 1 && _lastValueChange <= 1.01)))
        {
            var tmp = _pageNumber - 1;
            if (tmp < 0) return;
            LoadData(tmp);
        }

        else if (!_isLoading && (tmpValueChange < 0 && _lastValueChange >= 0.01))
        {
            var tmp = _pageNumber + 1;
            LoadData(tmp);
        }

        _lastValueChange = tmpValueChange;
    }

    private void LoadData(int indexPage)
    {
        _isLoading = true;
        if (_isShowHistoryInProgress)
        {
            ShowViewHistoryInProgress(indexPage);
        }
        else
        {
            ShowViewCompleted(indexPage);
        }
    }

    #endregion

    #endregion
}

public struct ActivityHistoryClaimAllRequest
{
    public string userCodeId;
    public string accessToken;
}