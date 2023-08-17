using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemEvent : MonoBehaviour
{
    [SerializeField] private Button btnJoinEvent;
    [SerializeField] private TMP_Text txtPrizePool;
    [SerializeField] private Image iconPrizePool;
    [SerializeField] private TMP_Text txtNumberPlayer;
    [SerializeField] private TMP_Text feeEntry;
    [SerializeField] private Image iconFeeEntry;
    [SerializeField] private Image iconTypeGame;
    [SerializeField] private Image bgModeType;
    [SerializeField] private List<Sprite> listSpriteModeType;
    [SerializeField] private TMP_Text nameTournament;
    [SerializeField] private Image iconTypeTournament;

    private Action<DetailMiniGameProto> _onClick;
    private DetailMiniGameProto _miniGameEventProto;
    private SpecialEventMediator _parent;

    private void Start()
    {
        btnJoinEvent.onClick.AddListener(() =>
        {
            if (_miniGameEventProto != null)
            {
                _onClick?.Invoke(_miniGameEventProto);
            }
        });
    }

    public void ShowView(DetailMiniGameProto info, Action<DetailMiniGameProto> onClick, SpecialEventMediator parent)
    {
        _parent = parent;
        _onClick = onClick;
        _miniGameEventProto = info;
        ShowItem();
    }

    private void ShowItem()
    {
        switch (_miniGameEventProto.ModeType)
        {
            case 1:
                iconTypeTournament.sprite = ResourceManager.Instance.GetIconTypeTournament(TypeTournament.HeadToHead);
                bgModeType.sprite = listSpriteModeType[0];
                break;
            case 2:
                iconTypeTournament.sprite = ResourceManager.Instance.GetIconTypeTournament(TypeTournament.KnockOut);
                bgModeType.sprite = listSpriteModeType[1];
                break;
            case 3:
                iconTypeTournament.sprite = ResourceManager.Instance.GetIconTypeTournament(TypeTournament.RoundRobin);
                bgModeType.sprite = listSpriteModeType[2];
                break;
            case 4:
                iconTypeTournament.sprite = ResourceManager.Instance.GetIconTypeTournament(TypeTournament.OneToMany);
                bgModeType.sprite = listSpriteModeType[3];
                break;
            default:
                bgModeType.sprite = listSpriteModeType[0];
                break;
        }

        iconTypeTournament.SetNativeSize();
        nameTournament.text = _miniGameEventProto.MiniGameEventName;
        bgModeType.SetNativeSize();


        txtPrizePool.text = _miniGameEventProto.PrizePool.Reward_.ToString();
        iconPrizePool.sprite =
            ResourceManager.Instance.GetIconReward((RewardType)_miniGameEventProto.PrizePool.RewardType);

        txtNumberPlayer.text = _miniGameEventProto.NumberInMiniGameEvent.ToString() + "Players";


        if (_miniGameEventProto.EntryFee != null & _miniGameEventProto.EntryFee.Fee_ >= 0)
        {
            feeEntry.text = _miniGameEventProto.EntryFee.Fee_.ToString();
            iconFeeEntry.sprite =
                ResourceManager.Instance.GetIconReward((RewardType)_miniGameEventProto.EntryFee.FeeType);
        }

        iconTypeGame.sprite = ResourceManager.Instance.GetIconGame((GameType)_miniGameEventProto.MiniGameId);
    }
}