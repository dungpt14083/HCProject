using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using MiniGame.MatchThree.Scripts.Network;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameResultPopup : UIPopupView<GameResultPopup>
{
    public static GameResultPopup InstanceResult;

    [SerializeField] private Image avatarFirstUser;
    [SerializeField] private TMP_Text userNameFirst;
    [SerializeField] private TMP_Text statusFirst;
    [SerializeField] private TMP_Text pointFirst;

    [SerializeField] private Image avatarSecondUser;
    [SerializeField] private TMP_Text userNameSecond;
    [SerializeField] private TMP_Text statusSecond;
    [SerializeField] private TMP_Text pointSecond;

    [SerializeField] private TMP_Text expBonus;

    [SerializeField] private Image firstBgStatus;
    [SerializeField] private List<Sprite> firstSpriteStatus;
    [SerializeField] private Image secondBgStatus;
    [SerializeField] private List<Sprite> secondSpriteStatus;


    public Button btClose;

    private EndUserWebsocketProto _endUserWebsocketProto;
    private EndRoomWebsocketProto _endRoomWebsocketProto;

    private bool _isEndRoom;
    private string _urlAvatar1;
    private string _urlAvatar2;

    public bool isShowFrameResult = false;


    protected override void Awake()
    {
        base.Awake();
        InstanceResult = this;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        SetDefault();
    }

    #region ENDUSERSHOW

    public void ShowViewWithEndUserData(EndUserWebsocketProto data)
    {
        SetValueShow();
        if (data == null) return;
        _endUserWebsocketProto = data;
        _isEndRoom = false;
        ShowViewEndUser();
        ShowBtn();
    }

    private void ShowViewEndUser()
    {
        var firstData = _endUserWebsocketProto.User.Where(s => s.Type == 1).ToList();
        var secondData = _endUserWebsocketProto.User.Where(s => s.Type == 2).ToList();

        avatarFirstUser.sprite = HCAppController.Instance.myAvatar;
        userNameFirst.text = firstData.Count > 0 ? firstData[0].UserName : "Name99";

        statusFirst.text = "Completed";
        firstBgStatus.sprite = firstSpriteStatus[0];

        //TỨC LÀ ĐỐI THỦ ĐANG CHƠI THÌ CẦN HIỂN THỊ CÁC THÔNG TIN CỦA NÓ
        if (_endUserWebsocketProto.StatusSecondUser == 1 && secondData.Count > 0)
        {
            if (HCAppController.Instance.userInfo.UserCodeId != secondData[0].UserId &&
                secondData[0].Avatar != _urlAvatar2)
            {
                _urlAvatar2 = secondData[0].Avatar;
                StartCoroutine(HCHelper.LoadAvatar(_urlAvatar2, avatarSecondUser));
            }
            else avatarSecondUser.sprite = HCAppController.Instance.myAvatar;

            userNameSecond.text = secondData[0].UserName;
        }
        else
        {
            avatarSecondUser.sprite = null;
            userNameSecond.text = "";
        }

        switch (_endUserWebsocketProto.StatusSecondUser)
        {
            case 1:
                statusSecond.text = "Playing Now";
                break;
            case 2:
                statusSecond.text = "Tobe determined";
                break;
            case 3:
                statusSecond.text = "Completed";
                break;
            default:
                break;
        }

        secondBgStatus.sprite = secondSpriteStatus[1];
        pointFirst.gameObject.SetActive(false);
        pointSecond.gameObject.SetActive(false);
        expBonus.gameObject.SetActive(false);
    }

    #endregion


    #region ENDROOMSHOW

    public void ShowViewWithEndRoomData(EndRoomWebsocketProto data)
    {
        SetValueShow();
        if (data == null) return;
        _isEndRoom = true;
        _endRoomWebsocketProto = data;
        ShowViewEndRoom();
        ShowBtn();
    }

    private void ShowViewEndRoom()
    {
        if (HCAppController.Instance.userInfo.UserCodeId != _endRoomWebsocketProto.FirstUserCodeId &&
            _endRoomWebsocketProto.AvatarFirstUser != _urlAvatar1)
        {
            _urlAvatar1 = _endRoomWebsocketProto.AvatarFirstUser;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar1, avatarFirstUser));
        }
        else avatarFirstUser.sprite = HCAppController.Instance.myAvatar;

        if (HCAppController.Instance.userInfo.UserCodeId != _endRoomWebsocketProto.SecondUserCodeId &&
            _endRoomWebsocketProto.AvatarSecondUser != _urlAvatar2)
        {
            _urlAvatar2 = _endRoomWebsocketProto.AvatarFirstUser;
            StartCoroutine(HCHelper.LoadAvatar(_urlAvatar2, avatarSecondUser));
        }
        else avatarSecondUser.sprite = HCAppController.Instance.myAvatar;

        //Name
        userNameFirst.text = _endRoomWebsocketProto.UserNameFirstUser;
        userNameSecond.text = _endRoomWebsocketProto.UserNameSecondUser;

        //WIN LOST//1 là 
        statusFirst.text = _endRoomWebsocketProto.Result == 2 ? "YOU WON" : "LOST";
        statusSecond.text = _endRoomWebsocketProto.Result == 2 ? "LOST" : "YOU WON";


        firstBgStatus.sprite = _endRoomWebsocketProto.Result == 2 ? firstSpriteStatus[0] : firstSpriteStatus[1];
        secondBgStatus.sprite = _endRoomWebsocketProto.Result == 2 ? secondSpriteStatus[1] : secondSpriteStatus[0];

        //ĐIỂM
        pointFirst.gameObject.SetActive(true);
        pointSecond.gameObject.SetActive(true);
        pointFirst.text = _endRoomWebsocketProto.PointFirstUser.ToString();
        pointSecond.text = _endRoomWebsocketProto.PointSecondUser.ToString();

        expBonus.gameObject.SetActive(true);
        expBonus.text = "+" + _endRoomWebsocketProto.Exp.ToString() + " EXP";
    }

    #endregion


    #region GENERALSHOW

    public Button btAcceptRematch;
    public Button btRematch;
    public Button btNewMatch;

    private Coroutine _sendToServerToShowTournament;

    private void ShowBtn()
    {
        SetDefaultButton();

        //Lấy modeType
        var modeType = _isEndRoom ? _endRoomWebsocketProto.ModeType : _endUserWebsocketProto.ModeType;

        switch (modeType)
        {
            //HEAD TO HEAD
            //ĐÂY LÀ TRƯỜNG HỢP ĐẶC BIỆT CHO HIÊN THỊ NÚT CÓ NEWMATCH VÀ THẰNG X THÌ VỀ MÀN HOME OR MÀN CHỌN GÌ ĐÓ ĐI
            case 1:
                ShowResultWithTournament();
                break;
            //GỬI LÊN MỞ TOURNAMENT LÀ TOURNAMENT BRACKETGUIWRI ĐỂ NÚT CLOSE THOI
            case 2:
                SendToOpenTournament();
                break;

            //GỬI LÊN MỞ TOURNAMENT LÀ ROUNDROBIN ĐỂ NÚT CLOSE THOI
            case 3:
                SendToOpenTournament();
                break;
            //LÀ LOẠI 1 VS MANY TỰ ẨN NÓ ĐI VÌ NÓ K CÓ HIỂN THỊ CSAI NÀY// BỎ BỎ B BỎ
            case 4:
                CloseView();
                LoadSceneHome();
                break;
        }
    }


    private void SendToOpenTournament()
    {
        btClose.onClick.AddListener((() =>
        {
            CloseView();
            ShowTournament();
        }));
    }


    private void LoadSceneHome()
    {
        HCAppController.Instance.GotoHome();
    }


    #region HEADTOHEADSOLO

    //ENDROOM MỚI MỚI GỌI REMATCH
    private void ShowResultWithTournament()
    {
        btNewMatch.gameObject.SetActive(true);
        btNewMatch.onClick.AddListener(SendForNewMatch);

        //Ở ĐÂY SẼ K CÓ FUNC CHO THẰNG NÚT CLOSE
        if (_isEndRoom)
        {
            ShowRematch();
        }

        btClose.onClick.AddListener((() =>
        {
            CloseView();
            ScreenManagerHC.Instance.GoToScreenViewWithFull(null);
        }));
    }

    #endregion


    private void ShowTournament()
    {
        if (_sendToServerToShowTournament != null)
        {
            Executors.Instance.StopCoroutine(_sendToServerToShowTournament);
        }

        var tmpGroupId = _isEndRoom ? _endRoomWebsocketProto.GroupRoomId : _endUserWebsocketProto.GroupRoomId;
        string url = GameConfig.API_URL + GameConfig.API_TAIL_SHOWTOURNAMENT +
                     $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" + $"groupRoomId={tmpGroupId}";
        _sendToServerToShowTournament =
            Executors.Instance.StartCoroutine(APIUtils.RequestWebApiGetByte(url, ShowTournamentPopup,
                ShowTournamentError));
    }


    private void ShowTournamentPopup(byte[] data)
    {
        ShowTournamentSameTopPlayerPopup(data);
    }

    private void ShowTournamentError(string error)
    {
        //Toast.Show(error);
        ScreenManagerHC.Instance.GotoHomeByDelay(error);
    }

    #endregion


    #region TODOLOADSCENEWITHTOURNAMENT

    private byte[] _tmpData;

    private void ShowTournamentSameTopPlayerPopup(byte[] data)
    {
        _tmpData = data;
        HCAppController.Instance.LoadScene("Home", CallBackLoadScene);
    }

    private void CallBackLoadScene()
    {
        Executors.Instance.StartCoroutine(CallBackShowTournament(_tmpData));
    }

    private IEnumerator CallBackShowTournament(byte[] data)
    {
        yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => UIMainScene.ins != null);
        //yield return new WaitUntil(() => UIMainScene.ins.isInitDone);
        var parser = new MessageParser<TournamentProto>(() => new TournamentProto());
        TournamentProto tournamentProto = parser.ParseFrom(data);
        if (tournamentProto == null) yield break;
        switch (tournamentProto.TypeMode)
        {
            //KNOCKOUT
            case 2:
                ScreenManagerHC.Instance.ShowViewWithTypeDiagram(tournamentProto);
                break;
            //ROBIN
            case 3:
                ScreenManagerHC.Instance.ShowViewWithTypeRoundRobin(tournamentProto);
                break;
            default:
                //HALLO
                break;
        }
    }

    #endregion


    private void ShowTournamentError()
    {
        HCAppController.Instance.GotoHome();
    }

    private void ShowRematch()
    {
        btRematch.gameObject.SetActive(true);
        btAcceptRematch.gameObject.SetActive(false);
        btRematch.onClick.AddListener(SendForReMatch);
    }

    private void ShowAcceptRematch()
    {
        btAcceptRematch.gameObject.SetActive(true);
        btRematch.gameObject.SetActive(false);
        btRematch.onClick.AddListener(SendForAcceptReMatch);
    }


    private void SendForNewMatch()
    {
        CloseView();
        HcGames.CCData _ccData = new HcGames.CCData();
        _ccData.MiniGameEventId = (ulong)HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
        _ccData.Token = HCAppController.Instance.GetTokenLogin();
        GameType gameType = (GameType)HCAppController.Instance.currentDetailMiniGameProto.MiniGameId;
        long miniGameEventId = HCAppController.Instance.currentDetailMiniGameProto.MiniGameEventId;
        int modeType = HCAppController.Instance.currentDetailMiniGameProto.ModeType;
        int numberPlayer = HCAppController.Instance.currentDetailMiniGameProto.NumberInMiniGameEvent;
        var reward = HCAppController.Instance.currentDetailMiniGameProto.PrizePool;
        HCAppController.Instance.ConnectGameAndPlay(gameType, miniGameEventId, modeType, numberPlayer, _ccData, reward);
    }

    private void SendForReMatch()
    {
        //Bên server chưa là
    }

    private void SendForAcceptReMatch()
    {
        //Bên server chưa là
    }


    private void SetDefaultButton()
    {
        btClose.onClick.RemoveAllListeners();
        btAcceptRematch.onClick.RemoveAllListeners();
        btRematch.onClick.RemoveAllListeners();
        btNewMatch.onClick.RemoveAllListeners();
        btAcceptRematch.gameObject.SetActive(false);
        btRematch.gameObject.SetActive(false);
        btNewMatch.gameObject.SetActive(false);
    }

    private void SetValueShow()
    {
        isShowFrameResult = true;
    }

    private void SetDefault()
    {
        isShowFrameResult = false;
    }
}