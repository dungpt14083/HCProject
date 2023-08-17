using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BonusGame;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TournamentSumupPopup : UIView<TournamentSumupPopup>
{
    [SerializeField] GameObject bgTop;
    [SerializeField] List<TournamentSumupItem> listTournamentSumUpItem;
    [SerializeField] TournamentSumupItem prefabTournamentSumUpItem;
    [SerializeField] RectTransform holder;
    [SerializeField] Button claim;
    [SerializeField] TournamentSumupItem myRank;
    [SerializeField] Button btnNewMatch;
    [SerializeField] ScrollRect scrollRect;


    private ListUserTournament _listUserTournament;
    private Vector2 _positionClaim;


    private void OnEnable()
    {
        _positionClaim = claim.GetComponent<Transform>().position;
        bgTop.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        bgTop.gameObject.SetActive(false);
    }


    public void InitData(ListUserTournament data)
    {
        _listUserTournament = data;
        ShowView();
    }

    public void ShowView()
    {
        btnNewMatch.gameObject.SetActive(_listUserTournament.IsEnd == 0);
        //claim.gameObject.SetActive(_listUserTournament.IsEnd == 1 && _listUserTournament.IsClaim == 1);
        myRank.gameObject.SetActive(_listUserTournament.IsEnd == 1 && _listUserTournament.IsClaim == 1);

        if (_listUserTournament.IsEnd == 1 && _listUserTournament.IsClaim == 0)
        {
            claim.gameObject.SetActive(true);
            claim.interactable = true;
            claim.onClick.RemoveAllListeners();
            claim.onClick.AddListener((() => { SendToClaim(); }));
        }
        else
        {
            claim.gameObject.SetActive(false);
        }


        //SHOW RANK CỦA CHÍNH MÌNH KHI KHÔNG THỂ CLAIM
        if (_listUserTournament.IsEnd == 1 && _listUserTournament.IsClaim == 1)
        {
            var myData =
                _listUserTournament.ListUserTournament_.Where(s =>
                    s.UserCodeId == HCAppController.Instance.userInfo.UserCodeId).ToList();
            if (myData.Count > 0)
            {
                myRank.ShowView(myData[0], _listUserTournament.IsEnd == 0);
            }
        }

        //CÒN ĐÂY SẼ TẠO RA ĐỐNG ITEM VÀ INIT ITEM CỦA THẰNG 1 2 3
        ShowItemRanking(_listUserTournament.IsEnd == 0);
    }

    private void ShowItemRanking(bool is1VsMany)
    {
        _listUserTournament.ListUserTournament_.OrderBy(S => S.Rating).ToList();
        var index = 0;
        ShowItemRanking123(out index, is1VsMany);
        SetDefault();
        ShowItemRankFor4(index, is1VsMany);
    }

    private void ShowItemRanking123(out int index, bool is1VsMany)
    {
        var tmpIndex = 0;
        //TỪ MỘT TỚI 3
        for (int i = 0; i < listTournamentSumUpItem.Count() && i < _listUserTournament.ListUserTournament_.Count; i++)
        {
            listTournamentSumUpItem[i].ShowView(_listUserTournament.ListUserTournament_[i], is1VsMany, i);
            tmpIndex++;
        }

        if (tmpIndex < listTournamentSumUpItem.Count())
        {
            for (int j = tmpIndex; j < listTournamentSumUpItem.Count(); j++)
            {
                listTournamentSumUpItem[j].ShowView(null, is1VsMany, j);
                tmpIndex++;
            }
        }

        index = tmpIndex;
    }

    private void ShowItemRankFor4(int index, bool is1VsMany)
    {
        if (_listUserTournament.IsEnd == 1)
        {
            var tmpIndex = index;
            for (int i = index; i < _listUserTournament.ListUserTournament_.Count; i++)
            {
                var tmp = BonusPool.Spawn(prefabTournamentSumUpItem, holder);
                tmp.ShowView(_listUserTournament.ListUserTournament_[i], is1VsMany, i);
                tmpIndex++;
            }
        }
        else
        {
            var tmpIndex = index;
            for (int j = index; j < _listUserTournament.ListUserTournament_.Count; j++)
            {
                var tmp = BonusPool.Spawn(prefabTournamentSumUpItem, holder);
                tmp.ShowView(_listUserTournament.ListUserTournament_[j], is1VsMany, j);
                tmpIndex++;
            }

            if (tmpIndex < _listUserTournament.TotalRoom)
            {
                for (int i = tmpIndex; i < _listUserTournament.TotalRoom; i++)
                {
                    var tmp = BonusPool.Spawn(prefabTournamentSumUpItem, holder);
                    tmp.ShowView(null, is1VsMany, i);
                }
            }
        }
    }

    private Coroutine _sendToTakeDetailMiniGameProto;

    //MATCH THÌ CẦN miniGameEventId CÒN THẰNG CLAIM THÌ CẦN GROUPROOOMID
    public void NewMatch()
    {
        CloseView();
        if (HCAppController.Instance.currentDetailMiniGameProto == null)
        {
            var url = GameConfig.API_URL + GameConfig.API_TAIL_GETDETAILMINIGAMEPROTO + "id=" +
                      $"{_listUserTournament.MiniGameEventId}";
            if (_sendToTakeDetailMiniGameProto != null)
            {
                StopCoroutine(_sendToTakeDetailMiniGameProto);
            }

            _sendToTakeDetailMiniGameProto =
                StartCoroutine(APIUtils.RequestWebApiGetByte(url, SuccessDetailMiniGameProto, Error));
        }
        else
        {
            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.MiniGameEventId = (ulong)HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.Round = 1;

            GameType gameType = (GameType)HCAppController.Instance.currentDetailMiniGameProto.MiniGameId;
            long miniGameEventId = HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
            int modeType = HCAppController.Instance.currentDetailMiniGameProto.ModeType;
            int numberPlayer = HCAppController.Instance.currentDetailMiniGameProto.NumberInMiniGameEvent;
            var reward = HCAppController.Instance.currentDetailMiniGameProto.PrizePool;
            HCAppController.Instance.ConnectGameAndPlay(gameType, miniGameEventId, modeType, numberPlayer, _ccData,
                reward);
        }
    }

    private void SuccessDetailMiniGameProto(byte[] data)
    {
        var parser = new MessageParser<DetailMiniGameProto>(() => new DetailMiniGameProto());
        DetailMiniGameProto tmpDetailMiniGameProto = parser.ParseFrom(data);
        HCAppController.Instance.SetCurrentDetailMiniGame(tmpDetailMiniGameProto);
        if (HCAppController.Instance.currentDetailMiniGameProto != null)
        {
            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.MiniGameEventId = (ulong)HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.Round = 1;

            GameType gameType = (GameType)HCAppController.Instance.currentDetailMiniGameProto.MiniGameId;
            long miniGameEventId = HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
            int modeType = HCAppController.Instance.currentDetailMiniGameProto.ModeType;
            int numberPlayer = HCAppController.Instance.currentDetailMiniGameProto.NumberInMiniGameEvent;
            var reward = HCAppController.Instance.currentDetailMiniGameProto.PrizePool;
            HCAppController.Instance.ConnectGameAndPlay(gameType, miniGameEventId, modeType, numberPlayer, _ccData,
                reward);
        }
    }

    private void Error(string obj)
    {
        Toast.Show(obj);
    }

    public void SendToClaim()
    {
        if (_listUserTournament == null) return;
        claim.interactable = false;
        var tmpGroupId = _listUserTournament.GroupRoomId;
        string url = GameConfig.API_URL + GameConfig.API_TAIL_CLAIMTOURNAMENT +
                     $"groupRoomId={tmpGroupId}" + "&" + $"userId={HCAppController.Instance.userInfo.UserCodeId}" +
                     "&" + $"miniGameSettingId={_listUserTournament.MiniGameEventId}";
        StartCoroutine(APIUtils.RequestWebApiGetByte(url, ShowAnimationClaim, ShowClaimError));
    }

    private void ShowAnimationClaim(byte[] data)
    {
        //UPDATEMONEY
        var url = GameConfig.API_URL + GameConfig.API_TAIL_CLAIMBONUSGAME +
                  $"{HCAppController.Instance.userInfo.UserCodeId}?" +
                  $"access_token={HCAppController.Instance.GetTokenLogin()}" +
                  $"&deviceId={HCAppController.Instance.currentDeviceId}";
        StartCoroutine(APIUtils.RequestWebApiGetJson(url, SuccessRewardAddMoney, ErrorAddmoney));

        var parser = new MessageParser<ListReward>(() => new ListReward());
        ListReward listReward = parser.ParseFrom(data);
        var tmpPos = this.GetComponent<RectTransform>().InverseTransformPoint(_positionClaim);
        CoinFlyAnimation.Instance.SpawnListRewardClaim(listReward, tmpPos, null);
        StartCoroutine(GotoScreeDelay());
    }

    private void ErrorAddmoney(string obj)
    {
        //throw new NotImplementedException();
    }

    private void SuccessRewardAddMoney(JObject obj)
    {
        //throw new NotImplementedException();
    }

    private IEnumerator GotoScreeDelay()
    {
        yield return new WaitForSeconds(4.0f);
        ScreenManagerHC.Instance.GoToScreenViewWithFull(null);
    }


    private void ShowClaimError(string error)
    {
        //Toast.Show(error);
        ScreenManagerHC.Instance.GoToScreenViewWithFull(null);
    }

    private void SetDefault()
    {
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }


    private int GetPlayerRank()
    {
        if (_listUserTournament == null) return -1;
        int playerRank = -1;
        foreach (var item in _listUserTournament.ListUserTournament_)
        {
            playerRank++;
            if (item.UserCodeId == HCAppController.Instance.userInfo.UserCodeId)
            {
                break;
            }
        }

        if (playerRank == 1 || playerRank == 2 || playerRank == 3)
        {
            return -1;
        }

        return playerRank;
    }


    public void ScrollToPlayer()
    {
        int playerRank = GetPlayerRank();
        if (playerRank >= 0)
        {
            float position = 1 - (float)playerRank /
                (float)_listUserTournament.ListUserTournament_.Count();
            position = Mathf.Clamp01(position);
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = position;
            }
        }
    }
}