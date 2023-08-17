using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BonusGame;
using Bingo;
using Google.Protobuf;
using MiniGame.MatchThree;
using MiniGame.MatchThree.Scripts.Network;
using TMPro;
using UnityEngine.PlayerLoop;
using Sirenix.OdinInspector;

public class ModeInfo : UIView<ModeInfo>
{
    protected override void Awake()
    {
        base.Awake();
        btJoinToPlay.onClick.AddListener(JoinGame);
    }

    #region REFACTORINGTHANG::

    [SerializeField] private TMP_Text titleTournamentInfo;

    [SerializeField] private TMP_Text prizePool;
    [SerializeField] private Image iconPrizePool;


    [SerializeField] private TMP_Text roundCount;
    [SerializeField] private TMP_Text playersCount;

    [SerializeField] private RectTransform holder;
    [SerializeField] private TournamentInfoItem prefabTournamentInfoItem;

    [SerializeField] private Button btJoinToPlay;
    [SerializeField] private TMP_Text valuesFee;
    [SerializeField] private Image iconFee;

    [SerializeField] private GameObject infoTournament;
    [SerializeField] private GameObject warning;
    [SerializeField] private TMP_Text txtWarning;

    [SerializeField] private Image topCup123;

    [SerializeField] private Image iconGame;
    //[SerializeField] private TMP_Text topCup123;

    [SerializeField] private List<GameObject> listGuide;


    private DetailMiniGameProto _detailMiniGameProto;
    private InformationTournamentProto _informationTournamentProto;

    public void ShowView(DetailMiniGameProto data)
    {
        SetDefault();
        _detailMiniGameProto = data;
        iconGame.gameObject.SetActive(true);
        iconGame.sprite = ResourceManager.Instance.GetIconGame((GameType)_detailMiniGameProto.MiniGameId);
        ShowGuideText();
        SendToServerToShowInfoPrizePool();
        if (_detailMiniGameProto == null) return;
    }

    private void ShowGuideText()
    {
        if (_detailMiniGameProto != null)
        {
            if (_detailMiniGameProto.ModeType > 0 && _detailMiniGameProto.ModeType <= listGuide.Count)
            {
                listGuide[_detailMiniGameProto.ModeType-1].gameObject.SetActive(true);
            }
        }
    }


    private void ShowViewInfoTournament()
    {
        var typeReward = _informationTournamentProto.Fee[0].FeeType;

        Reward typePrizePool = _informationTournamentProto.PrizePool.First(s => s.RewardType == typeReward);

        long valuePrizePool = typePrizePool == null ? 0 : typePrizePool.Reward_;
        MoneyType moneyType = typePrizePool == null ? MoneyType.None : (MoneyType)typePrizePool.RewardType;


        int numberRound = _informationTournamentProto == null ? 0 : _informationTournamentProto.NumberRound;
        ShowViewTitle(_detailMiniGameProto.MiniGameEventName, valuePrizePool, moneyType, numberRound,
            _detailMiniGameProto.NumberInMiniGameEvent);
        ShowScrollPrize(moneyType);
        ShowViewButtonPlay(moneyType);
    }

    private void SendToServerToShowInfoPrizePool()
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETINFOTOURNAMENT + "?miniGameSettingId=" +
                  $"{_detailMiniGameProto.MiniGameEventId}";
        StartCoroutine(APIUtils.RequestWebApiGetByte(url, SuccessInfoTournament, ErrorInfoTournament));
    }

    private void ErrorInfoTournament(string obj)
    {
        Toast.Show(obj);
    }

    private void SuccessInfoTournament(byte[] data)
    {
        var parser = new MessageParser<InformationTournamentProto>(() => new InformationTournamentProto());
        _informationTournamentProto = parser.ParseFrom(data);
        ShowViewInfoTournament();
    }


    private void ShowViewTitle(string nameTournament, long valuePrizePool, MoneyType moneyType, int numberRound,
        int _playersCount)
    {
        titleTournamentInfo.text = nameTournament;
        prizePool.text = valuePrizePool.ToString();
        iconPrizePool.sprite = ResourceManager.Instance.GetIconMoney(moneyType);
        roundCount.text = _informationTournamentProto.NumberRound.ToString() + " Rounds";
        playersCount.text = _informationTournamentProto.NumberPlayer.ToString() + " Players";
    }


    private void ShowScrollPrize(MoneyType moneyType)
    {
        if (_informationTournamentProto == null) return;
        if (_informationTournamentProto.RewardRanking == null) return;

        var listInformationTournamentProto =
            _informationTournamentProto.RewardRanking.Where(
                s => s.Reward.Any(s => (MoneyType)s.RewardType == moneyType)).ToList();
        for (int i = 0; i < listInformationTournamentProto.Count; i++)
        {
            var tmp = BonusPool.Spawn(prefabTournamentInfoItem, holder);
            tmp.ShowView(moneyType, listInformationTournamentProto[i]);
        }
    }


    private void SetDefault()
    {
        iconGame.gameObject.SetActive(false);
        warning.gameObject.SetActive(false);
        _detailMiniGameProto = null;
        _informationTournamentProto = null;

        for (int i = 0; i < listGuide.Count; i++)
        {
            listGuide[i].gameObject.SetActive(false);
        }

        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }

    private void ShowViewButtonPlay(MoneyType moneyType)
    {
        valuesFee.text = _informationTournamentProto.Fee[0].Fee_.ToString();
        iconFee.sprite = ResourceManager.Instance.GetIconMoney(moneyType);
    }

    private void JoinGame()
    {
        if (_detailMiniGameProto == null) return;
        var isEnough = CheckEnoughFee();
        if (isEnough)
        {
            var gameType = (GameType)_detailMiniGameProto.MiniGameId;
            
            var mmr = HCAppController.Instance.GetMmrByGameType(gameType);
            switch (gameType)
            {
                case GameType.Billard:
                    HCAppController.Instance.InitNetworkEightBallNew(this._detailMiniGameProto.MiniGameEventId,
                        this._detailMiniGameProto.ModeType, this._detailMiniGameProto.NumberInMiniGameEvent, mmr, null, _detailMiniGameProto.PrizePool);
                    break;
                case GameType.Solitaire:
                    HCAppController.Instance.InitNetworkSoliraite(this._detailMiniGameProto.MiniGameEventId,
                        this._detailMiniGameProto.ModeType, this._detailMiniGameProto.NumberInMiniGameEvent, mmr, null);
                    break;

                case GameType.Bingo: //mmr get from HCAppController.Instance.userInfo;
                    Bingo_NetworkManager.instance.SendRequestFindRoomFromHCGameList(
                        this._detailMiniGameProto.MiniGameEventId,
                        this._detailMiniGameProto.ModeType,
                        this._detailMiniGameProto.NumberInMiniGameEvent,
                        HCAppController.Instance.GetBingoWs(), null);
                    break;

                case GameType.Puzzle: //mmr get from HCAppController.Instance.userInfo;
                    MatchThreeNetworkManager.Instance.SendRequestFindRoomFromHCGameList(
                        this._detailMiniGameProto.MiniGameEventId, this._detailMiniGameProto.ModeType,
                        this._detailMiniGameProto.NumberInMiniGameEvent, HCAppController.Instance.GetMatch3Ws(), null);
                    break;
                case GameType.Bubble:
                    BubbesShotManager.Instance.StartFindRoom(
                        this._detailMiniGameProto.MiniGameEventId, this._detailMiniGameProto.ModeType,
                        this._detailMiniGameProto.NumberInMiniGameEvent, HCAppController.Instance.GetUrlBubblesShot(),
                        null);
                    break;
            }

            HCAppController.Instance.SetCurrentDetailMiniGame(this._detailMiniGameProto);
        }
        else
        {
            warning.gameObject.SetActive(true);
            txtWarning.text =
                $"Your don't gave enough {StringUtils.ConvertRewardTypeToString((RewardType)_informationTournamentProto.Fee[0].FeeType)} to join this match";
        }
    }


    [Button]
    public void FakeJoinBingo()
    {
        Bingo_NetworkManager.instance.SendRequestFindRoomFromHCGameList(62, 1, 1, HCAppController.Instance.GetBingoWs(),
            null);
    }

    [Button]
    public void FakeJoinPuzzle()
    {
        MatchThreeNetworkManager.Instance.SendRequestFindRoomFromHCGameList(62, this._detailMiniGameProto.ModeType,
            this._detailMiniGameProto.NumberInMiniGameEvent, HCAppController.Instance.GetMatch3Ws(), null);
    }


    public void CloseWarningAndShowInfoTournament()
    {
        warning.gameObject.SetActive(false);
        infoTournament.gameObject.SetActive(true);
    }

    private bool CheckEnoughFee()
    {
        return true;
        var feePlay = _informationTournamentProto.Fee;
    }

    #endregion
}