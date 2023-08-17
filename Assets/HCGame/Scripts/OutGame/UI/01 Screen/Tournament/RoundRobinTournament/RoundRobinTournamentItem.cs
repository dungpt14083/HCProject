using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoundRobinTournamentItem : MonoBehaviour
{
    [SerializeField] List<UserInfoRoundRobinItem> listUserInfoRoundRobinItem;

    [SerializeField] Image bgRoundRobinTournamentItem;
    [SerializeField] Button battleImageAndButton;

    [SerializeField] List<Sprite> listBattleImage;
    [SerializeField] List<Sprite> listBgRoundRobinTournamentItem;

    private RoomTournamentProto _roomTournamentProto;
    private TournamentProto _generalData;


    //Cần listItem Data truyền vào DATA CỦA ROOM VÀ MỘT SỐ FLAG ĐỂ TẠO TRẬN...
    public void ShowView(RoomTournamentProto data, TournamentProto generalData)
    {
        _roomTournamentProto = data;
        _generalData = generalData;
        ShowBgRoomAndButtonBattle();
        ShowUserItemInfo();
    }

    private void ShowBgRoomAndButtonBattle()
    {
        battleImageAndButton.onClick.RemoveAllListeners();
        //NẾU CÓ MÌNH Ở TRONG THÌ SẼ chạy hiển thị khsac
        var isMyRoom = false;
        isMyRoom = _roomTournamentProto.User.Where(s => s.UserCodeId == HCAppController.Instance.userInfo.UserCodeId)
            .ToList().Count > 0;

        if (isMyRoom)
        {
            bgRoundRobinTournamentItem.sprite = _roomTournamentProto.IsPlay == 2
                ? listBgRoundRobinTournamentItem[0]
                : listBgRoundRobinTournamentItem[1];

            if (_roomTournamentProto.IsPlay == 2)
            {
                battleImageAndButton.GetComponent<Image>().sprite = listBattleImage[0];
                battleImageAndButton.GetComponent<Image>().SetNativeSize();
            }
            else if (_roomTournamentProto.IsPlay == 3)
            {
                battleImageAndButton.GetComponent<Image>().sprite = listBattleImage[1];
                battleImageAndButton.GetComponent<Image>().SetNativeSize();

                battleImageAndButton.onClick.AddListener(SendToBattle);
            }
        }
        else
        {
            bgRoundRobinTournamentItem.sprite = listBgRoundRobinTournamentItem[2];
            battleImageAndButton.GetComponent<Image>().sprite = listBattleImage[0];
            battleImageAndButton.GetComponent<Image>().SetNativeSize();
        }

        bgRoundRobinTournamentItem.SetNativeSize();
    }

    private void ShowUserItemInfo()
    {
        var tmpIndex = 0;
        for (int i = 0; i < _roomTournamentProto.User.Count && i < listUserInfoRoundRobinItem.Count; i++)
        {
            listUserInfoRoundRobinItem[i].ShowView(_roomTournamentProto.User[i], _roomTournamentProto.Status);
            tmpIndex++;
        }

        if (tmpIndex < listUserInfoRoundRobinItem.Count)
        {
            for (int i = tmpIndex; i < listUserInfoRoundRobinItem.Count; i++)
            {
                listUserInfoRoundRobinItem[i].SetDefault();
            }
        }
    }


    #region NULLITEM

    public void ShowNullItem()
    {
        battleImageAndButton.onClick.RemoveAllListeners();
        battleImageAndButton.GetComponent<Image>().sprite = listBattleImage[0];
        battleImageAndButton.GetComponent<Image>().SetNativeSize();
        bgRoundRobinTournamentItem.sprite = listBgRoundRobinTournamentItem[2];
        for (int i = 0; i < listUserInfoRoundRobinItem.Count; i++)
        {
            listUserInfoRoundRobinItem[i].SetDefault();
        }
    }

    #endregion

    private Coroutine _sendToTakeDetailMiniGameProto;

    private void SendToBattle()
    {
        if (HCAppController.Instance.currentDetailMiniGameProto == null)
        {
            var url = GameConfig.API_URL + GameConfig.API_TAIL_GETDETAILMINIGAMEPROTO + "id=" +
                      $"{_generalData.MiniGameEventId}";
            if (_sendToTakeDetailMiniGameProto != null)
            {
                StopCoroutine(_sendToTakeDetailMiniGameProto);
            }

            _sendToTakeDetailMiniGameProto =
                StartCoroutine(APIUtils.RequestWebApiGetByte(url, SuccessDetailMiniGameProto, Error));
        }
        else
        {
            GameSignals.CloseAllPopup.Dispatch();
            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.MiniGameEventId = (ulong)HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.GroupRoomId = _generalData.GroupRoomId;
            _ccData.Round = 1;
            _ccData.RoomId = _roomTournamentProto.RoomId;

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
            GameSignals.CloseAllPopup.Dispatch();
            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.MiniGameEventId = (ulong)HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            _ccData.GroupRoomId = _generalData.GroupRoomId;
            _ccData.Round = 1;
            _ccData.RoomId = _roomTournamentProto.RoomId;

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
}