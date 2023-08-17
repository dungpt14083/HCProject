using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HcGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TournamentTypeDiagramController : UIView<TournamentTypeDiagramController>
{
    [SerializeField] RoomTournament prefabRoomTournament;
    [SerializeField] TMP_Text txtTimeRemain;
    [SerializeField] TMP_Text txtName;

    private TournamentProto _tournamentProto;

    private void OnEnable()
    {
        overlay.gameObject.SetActive(false);
    }

    public void ShowView(TournamentProto data = null)
    {
        _tournamentProto = data;
        Debug.Log(_tournamentProto.ToString());
        txtName.text = _tournamentProto.NameTournament;
        SetDefault();
        var tmpTotalRound = CalTotalRound();
        MakeContainerRoundGameObject(tmpTotalRound);
        MakeDataRoomTournament();
        MakeRoomTournamentView();
        MakeLineRenderTournament();
        MakeSizeBackGroundRectTransform();
        ShowAndSettingButton();
        ShowTimeRemain();
    }

    #region SHOWTIMEREMAIN

    private long _timeCount = 0;
    private Coroutine _coroutineTime;

    private void ShowTimeRemain()
    {
        _timeCount = _tournamentProto.EndTime;
        if (_coroutineTime != null)
        {
            StopCoroutine(_coroutineTime);
        }

        _coroutineTime = StartCoroutine(ShowTime());
    }

    private IEnumerator ShowTime()
    {
        var tmpFlag = false;
        while (true)
        {
            _timeCount -= 1;
            if (_timeCount < 0)
            {
                //Sẽ gọi lại lên server để lấy sơ đồ mới nhất::
                txtTimeRemain.gameObject.SetActive(false);
                if (tmpFlag)
                {
                    //Gọi lên server để init lại sơ đồ:::
                }

                tmpFlag = false;
                yield break;
            }
            else
            {
                if (!txtTimeRemain.gameObject.activeSelf)
                {
                    txtTimeRemain.gameObject.SetActive(true);
                }

                tmpFlag = true;
            }

            yield return new WaitForSeconds(1.0f);
            var tmp = TimeSpan.FromSeconds(_timeCount);
            txtTimeRemain.text = $"Time left : {tmp.Days}d {tmp.Hours:00}h {tmp.Minutes:00}m";
        }
    }

    #endregion


    private int CalTotalRound()
    {
        var tmpIndex = 0;

        if (_tournamentProto != null)
        {
            tmpIndex = (int)Math.Ceiling(Math.Log(_tournamentProto.TotalRoom, 2)) + 1;
            _tournamentProto.TotalRound = tmpIndex;
        }

        return tmpIndex;
    }


    #region POOLPARENTANDROOOMTOURMENT

    private void SetDefault()
    {
        playNextRound.gameObject.SetActive(false);
        seeTournamentResult.gameObject.SetActive(false);
        rewardTopTournamentPopup.gameObject.SetActive(false);
        for (int i = holderLine.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holderLine.GetChild(i));
        }

        for (int i = 0; i < _listRoundTransforms.Count; i++)
        {
            for (int j = _listRoundTransforms[i].childCount - 1; j >= 0; j--)
            {
                BonusPool.DeSpawn(_listRoundTransforms[i].GetChild(j));
            }
        }
    }

    #endregion


    #region MakeTranformParent

    [SerializeField] RectTransform originSampleForTakePosition;
    [SerializeField] RectTransform parenOfOrigin;

    [SerializeField] float spacingHorizontalRoomsTournament = 60.9f;
    [SerializeField] float spacingVerticalRoomsTournament = 65;

    private Vector3 _sizeRoomTournament = new Vector3(616, 294, 0);
    private List<Transform> _listRoundTransforms = new List<Transform>();

    [Button]
    private void MakeContainerRoundGameObject(int roundCount = 0)
    {
        _listRoundTransforms.Clear();
        var origin = new GameObject("OriginGame");
        origin.transform.SetParent(parenOfOrigin.transform);
        origin.transform.localScale = new Vector3(1, 1, 1);
        origin.transform.position = originSampleForTakePosition.transform.position;

        for (int i = 0; i < roundCount; i++)
        {
            var tmp = new GameObject($"ParentContainerRound{i}");
            tmp.transform.SetParent(origin.transform);
            tmp.transform.localScale = new Vector3(1, 1, 1);
            tmp.transform.SetAsLastSibling();
            tmp.transform.localPosition =
                new Vector3(((i * _sizeRoomTournament.x) + (i * spacingHorizontalRoomsTournament)), 0.0f, 0.0f);
            _listRoundTransforms.Add(tmp.transform);
        }
    }

    #endregion


    #region ToDoMakeDataForRoom

    private List<RoomTournamentProto> _dataAllRoomTournament = new List<RoomTournamentProto>();

    private List<List<RoomTournamentProto>> _dataRoomTournamentByRound = new List<List<RoomTournamentProto>>();
    private List<List<Vector3>> _listTransformRoomTourment = new List<List<Vector3>>();
    private List<List<Vector3>> _listPositionRoom = new List<List<Vector3>>();


    private void MakeDataRoomTournament()
    {
        if (_tournamentProto == null) return;

        _dataAllRoomTournament.Clear();
        foreach (var o in _tournamentProto.RoomTournament)
        {
            _dataAllRoomTournament.Add(o);
        }

        _dataRoomTournamentByRound.Clear();
        for (int i = 0; i < _tournamentProto.TotalRound; i++)
        {
            var tmp = _tournamentProto.RoomTournament.Where(s => s.Round == (i + 1)).ToList();
            tmp = tmp.OrderBy(s => s.Position).ToList();

            var numOfRoomsInRound = (int)Math.Ceiling((double)(_tournamentProto.TotalRoom) / Math.Pow(2, i));
            if (tmp.Count < numOfRoomsInRound)
            {
                for (int j = tmp.Count; j < numOfRoomsInRound; j++)
                {
                    tmp.Add(null);
                }
            }

            _dataRoomTournamentByRound.Add(tmp);
        }

        Debug.Log(_dataRoomTournamentByRound);
    }

    #endregion

    #region MakeRoomAndView

    private void MakeRoomTournamentView()
    {
        _listTransformRoomTourment.Clear();
        _listPositionRoom.Clear();
        for (int i = 0; i < _dataRoomTournamentByRound.Count; i++)
        {
            var tmpPosition = new List<Vector3>();
            var tmpRoomPosition = new List<Vector3>();

            for (int j = 0; j < _dataRoomTournamentByRound[i].Count; j++)
            {
                var tmp = BonusPool.Spawn(prefabRoomTournament, _listRoundTransforms[i]);
                var pos = tmp.transform;
                //pos.SetParent(_listRoundTransforms[i]);
                pos.localScale = new Vector3(1, 1, 1);
                pos.localPosition = new Vector3(0, -(j * _sizeRoomTournament.y + (j * spacingVerticalRoomsTournament)),
                    0);
                pos.localPosition = i == 0
                    ? new Vector3(0, -(j * _sizeRoomTournament.y + (j * spacingVerticalRoomsTournament)), 0)
                    : new Vector3(0,
                        ((_listTransformRoomTourment[i - 1][j * 2].y + _listTransformRoomTourment[i - 1][j * 2 + 1].y) /
                         2.0f), 0);

                tmp.ShowView(_dataRoomTournamentByRound[i][j] == null ? null : _dataRoomTournamentByRound[i][j]);


                var tmpRoomPositionConvert = holderLine.transform.InverseTransformPoint(tmp.CenterPosition);
                tmpRoomPosition.Add(tmpRoomPositionConvert);
                tmpPosition.Add(pos.localPosition);
            }

            _listTransformRoomTourment.Add(tmpPosition);
            _listPositionRoom.Add(tmpRoomPosition);
        }
    }

    #endregion


    #region DRAWLINE::

    [SerializeField] public BracketLine prefabBracketLine;
    [SerializeField] public RectTransform holderLine;

    public Color lineColor;
    public float lineWidth;

    private void MakeLineRenderTournament()
    {
        for (int i = 0; i < _listPositionRoom.Count - 1; i++)
        {
            List<Vector3> currentRound = _listPositionRoom[i];
            List<Vector3> nextRound = _listPositionRoom[i + 1];

            for (int j = 0; j < currentRound.Count; j += 1)
            {
                Vector3 start = currentRound[j];
                Vector3 end = nextRound[j / 2];
                var tmp = BonusPool.Spawn(prefabBracketLine, holderLine);
                tmp.ShowLineRender(start, end, lineColor, lineWidth);
            }
        }
    }

    #endregion


    #region RectTranformBackGround

    [SerializeField] RectTransform rectView;
    [SerializeField] RectTransform rectBackGround;
    [SerializeField] RectTransform contentSize;

    private void MakeSizeBackGroundRectTransform()
    {
        var width = 1060.0f;
        var height = 1300.0f;
        if (_listTransformRoomTourment.Count > 0)
        {
            width = Math.Abs(originSampleForTakePosition.anchoredPosition.x) +
                    (_tournamentProto.TotalRound) * _sizeRoomTournament.x +
                    spacingHorizontalRoomsTournament * (_tournamentProto.TotalRound);
            height = Math.Abs(originSampleForTakePosition.anchoredPosition.y) +
                     _listTransformRoomTourment[0].Count * _sizeRoomTournament.y +
                     spacingVerticalRoomsTournament * _listTransformRoomTourment[0].Count;
        }

        var tmp = new Vector2(width > rectView.rect.width ? width : rectView.rect.width,
            height > rectView.rect.height ? height : rectView.rect.height);
        rectBackGround.sizeDelta = tmp;
        contentSize.sizeDelta = tmp;
    }

    #endregion


    #region TODOSHOWTOPREWARD

    [SerializeField] RewardTopTournamentPopup rewardTopTournamentPopup;
    [SerializeField] GameObject overlay;

    public void ShowTopRewardTournament()
    {
        rewardTopTournamentPopup.gameObject.SetActive(true);
        rewardTopTournamentPopup.ShowView(_tournamentProto.Reward);
        overlay.gameObject.SetActive(true);
    }

    public void CloseTopRewardTournament()
    {
        rewardTopTournamentPopup.gameObject.SetActive(false);
        overlay.gameObject.SetActive(false);
    }

    #endregion


    #region FUNCPLAYNEXTROUNDORTOURNAMENTRESULTBUTTON

    [SerializeField] Button playNextRound;
    [SerializeField] Button seeTournamentResult;
    [SerializeField] TournamentSumupPopup seeTournamentResultPopup;
    [SerializeField] List<Sprite> spriteSeeTournamentResult;


    private string RouletteURL = "";

    private Coroutine _sendToServerToShowResult;

    //Nhận value từ server để hiện nút mở func sau tournament
    private void ShowAndSettingButton()
    {
        playNextRound.onClick.RemoveAllListeners();

        if (_tournamentProto.IsEnd)
        {
            playNextRound.gameObject.SetActive(false);
            seeTournamentResult.gameObject.SetActive(true);
            seeTournamentResult.interactable = true;
            seeTournamentResult.onClick.AddListener(SendToTakeTopPlayer);
            seeTournamentResult.GetComponent<Image>().sprite = spriteSeeTournamentResult[0];
            seeTournamentResult.GetComponent<Image>().SetNativeSize();
        }
        else
        {
            if (_tournamentProto.Result == 2)
            {
                playNextRound.gameObject.SetActive(false);
                seeTournamentResult.gameObject.SetActive(true);
                seeTournamentResult.interactable = false;
                seeTournamentResult.GetComponent<Image>().sprite = spriteSeeTournamentResult[1];
                seeTournamentResult.GetComponent<Image>().SetNativeSize();
            }
            else
            {
                playNextRound.gameObject.SetActive(true);
                seeTournamentResult.gameObject.SetActive(false);
                if (_tournamentProto.IsEndTime == 1)
                {
                    playNextRound.onClick.AddListener(SendToServerToNextRound);
                    playNextRound.interactable = true;
                }
                else
                {
                    playNextRound.interactable = false;
                }
            }

            if (_tournamentProto.Round == _tournamentProto.TotalRound)
            {
                playNextRound.gameObject.SetActive(false);
                seeTournamentResult.gameObject.SetActive(true);
                seeTournamentResult.interactable = false;
            }
        }
    }

    private Coroutine _sendToTakeDetailMiniGameProto;

    private void SendToServerToNextRound()
    {
        if (HCAppController.Instance.currentDetailMiniGameProto == null)
        {
            var url = GameConfig.API_URL + GameConfig.API_TAIL_GETDETAILMINIGAMEPROTO + "id=" +
                      $"{_tournamentProto.MiniGameEventId}";
            if (_sendToTakeDetailMiniGameProto != null)
            {
                StopCoroutine(_sendToTakeDetailMiniGameProto);
            }

            _sendToTakeDetailMiniGameProto =
                StartCoroutine(APIUtils.RequestWebApiGetByte(url, SuccessDetailMiniGameProto, Error));
        }
        else
        {
            //ĐÂY LÀ FINDROOOM HEADER:::
            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.MiniGameEventId = (ulong)_tournamentProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.Round = _tournamentProto.Round + 1;
            _ccData.GroupRoomId = _tournamentProto.GroupRoomId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
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
            _ccData.MiniGameEventId = (ulong)_tournamentProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.Round = _tournamentProto.Round + 1;
            _ccData.GroupRoomId = _tournamentProto.GroupRoomId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
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


    private void SendToTakeTopPlayer()
    {
        if (_sendToServerToShowResult != null)
        {
            StopCoroutine(_sendToServerToShowResult);
        }

        var url = GameConfig.API_URL + GameConfig.API_TAIL_TOURNAMENTRANKING +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" +
                  $"groupRoomId={_tournamentProto.GroupRoomId}";
        _sendToServerToShowResult =
            StartCoroutine(APIUtils.RequestWebApiGetByte(url, ShowTournamentRanking, ShowTournamentRankingError));
    }

    private void ShowTournamentRanking(byte[] data)
    {
        var parser = new MessageParser<ListUserTournament>(() => new ListUserTournament());
        ListUserTournament listUserTournament = parser.ParseFrom(data);
        TournamentDataSignals.ReceivedTournamentRankData.Dispatch(listUserTournament);
    }

    private void ShowTournamentRankingError(string error)
    {
        Toast.Show(error);
        ScreenManagerHC.Instance.GotoHomeByDelay("Claim");
    }

    #endregion
}