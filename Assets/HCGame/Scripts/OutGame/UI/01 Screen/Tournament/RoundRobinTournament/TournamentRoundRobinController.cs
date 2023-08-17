using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class TournamentRoundRobinController : UIView<TournamentRoundRobinController>
{
    [SerializeField] Button btnSeeTournamentResult;
    [SerializeField] List<Sprite> listImageButtonSee;
    
    [SerializeField] RectTransform holder;
    [SerializeField] RoundRobinTournamentItem prefabRoundRobinTournamentItem;

    [SerializeField] RectTransform rectView;
    [SerializeField] Vector2 positionOffsetView = new Vector2(475, 315);

    private TournamentProto _generalData;
    //private List<RoomTournamentProto> _listRoomTournamentProto = new List<RoomTournamentProto>();


    private Coroutine _sendToServerToNextRound;

    private Coroutine _sendToServerToShowResult;
    private List<RoundRobinTournamentItem> _listRoundRobinItem = new List<RoundRobinTournamentItem>();


    public void ShowView(TournamentProto data)
    {
        SetDefault();
        _generalData = data;
        SortMyData();
        ShowViewItemRoomRobin();
        ShowRectViewAndBtnSeeTournamentResult();
    }

    private void SortMyData()
    {
        /*
        if (_generalData == null) return;

        List<RoomWithUserCode> sortedRooms = _generalData.RoomTournament
            .SelectMany(room =>
                room.User.Select(user => new RoomWithUserCode { Room = room, UserCodeId = user.UserCodeId }))
            .OrderBy(roomWithUser => roomWithUser.UserCodeId == HCAppController.Instance.userInfo.UserCodeId ? 0 : 1)
            .ThenBy(roomWithUser => roomWithUser.UserCodeId)
            .ToList();

        var listNotDuplicate = new List<RoomWithUserCode>();
        foreach (var room in sortedRooms)
        {
            bool isDuplicate = listNotDuplicate.Any(r => r.Room.RoomId == room.Room.RoomId);
            if (!isDuplicate)
            {
                listNotDuplicate.Add(room);
            }
        }

        Dictionary<string, List<RoomTournamentProto>> groupedRooms = listNotDuplicate
            .GroupBy(roomWithUser => roomWithUser.UserCodeId)
            .ToDictionary(group => group.Key, group => group.Select(roomWithUser => roomWithUser.Room).ToList());

        List<RoomTournamentProto> currentUserRooms =
            groupedRooms.ContainsKey(HCAppController.Instance.userInfo.UserCodeId)
                ? groupedRooms[HCAppController.Instance.userInfo.UserCodeId]
                : new List<RoomTournamentProto>();
        groupedRooms.Remove(HCAppController.Instance.userInfo.UserCodeId);
        groupedRooms.Add(HCAppController.Instance.userInfo.UserCodeId, currentUserRooms);
        _listRoomTournamentProto = groupedRooms
            .SelectMany(kv => kv.Value.OrderBy(room => room.Position))
            .ToList();
            */
    }

    private void ShowViewItemRoomRobin()
    {
        var tmpIndex = 0;
        for (int i = 0; i < _generalData.RoomTournament.Count; i++)
        {
            var tmp = BonusPool.Spawn(prefabRoundRobinTournamentItem, holder);
            tmp.ShowView(_generalData.RoomTournament[i], _generalData);
            _listRoundRobinItem.Add(tmp);
            tmpIndex++;
        }

        if (tmpIndex < _generalData.TotalRoom)
        {
            for (int i = tmpIndex; i < _generalData.TotalRoom; i++)
            {
                var tmp = BonusPool.Spawn(prefabRoundRobinTournamentItem, holder);
                tmp.ShowNullItem();
            }
        }
    }

    private void ShowRectViewAndBtnSeeTournamentResult()
    {
        var isNotDone = false;
        //Check xem mình chơi hết trận có thể hay chưa:::
        var tmpMyRoom = _generalData.RoomTournament.Where(s =>
                s.User.Where(x => x.UserCodeId == HCAppController.Instance.userInfo.UserCodeId).ToList().Count > 0)
            .ToList();
        for (int i = 0; i < tmpMyRoom.Count; i++)
        {
            if (tmpMyRoom[i].Status != 3)
            {
                isNotDone = true;
                break;
            }
        }

        if (isNotDone)
        {
            //btnSeeTournamentResult.gameObject.SetActive(false);
            btnSeeTournamentResult.GetComponent<Image>().sprite = listImageButtonSee[1];
            //rectView.offsetMin = new Vector2(0, positionOffsetView.y);
        }
        else
        {
            //rectView.offsetMin = new Vector2(0, positionOffsetView.x);
            //btnSeeTournamentResult.gameObject.SetActive(true);
            btnSeeTournamentResult.GetComponent<Image>().sprite = listImageButtonSee[0];
            btnSeeTournamentResult.onClick.RemoveAllListeners();
            btnSeeTournamentResult.interactable = _generalData.IsEnd;
            if (_generalData.IsEnd)
            {
                btnSeeTournamentResult.onClick.AddListener(SendToShowTournamentRanking);
            }
        }

        btnSeeTournamentResult.GetComponent<Image>().SetNativeSize();
    }


    private void SetDefault()
    {
        btnSeeTournamentResult.onClick.RemoveAllListeners();
        //rectView.offsetMin = new Vector2(0, positionOffsetView.y);
        _listRoundRobinItem.Clear();
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }

    private void SendToShowTournamentRanking()
    {
        if (_sendToServerToShowResult != null)
        {
            StopCoroutine(_sendToServerToShowResult);
        }

        var url = GameConfig.API_URL + GameConfig.API_TAIL_TOURNAMENTRANKING +
                  $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" +
                  $"groupRoomId={_generalData.GroupRoomId}";
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
}

class RoomWithUserCode
{
    public RoomTournamentProto Room { get; set; }
    public string UserCodeId { get; set; }
}