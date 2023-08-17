using System.Collections;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialEventMediator : MonoBehaviour //, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private ItemEvent itemEvent;
    [SerializeField] private RectTransform holder;

    private ListMiniGameEventProto _listMiniGameEventProto;
    private DetailMiniGameProto _data;
    private Coroutine _sendToServer;


    public void ShowListMiniGameEvent()
    {
        if (HCAppController.Instance.ListMiniGameEventProto == null) return;
        SetDefault();
        for (int i = 0; i < HCAppController.Instance.ListMiniGameEventProto.ListMiniGame.Count; i++)
        {
            var tmp = BonusPool.Spawn(itemEvent, holder);
            tmp.ShowView(HCAppController.Instance.ListMiniGameEventProto.ListMiniGame[i], SendToServerCheckEvent, this);
        }
    }

    private void SetDefault()
    {
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }

    private void OnDisable()
    {
        _listMiniGameEventProto = null;
    }

    private void SendToServerCheckEvent(DetailMiniGameProto data)
    {
        Debug.LogError("xxxxxxxxxxxxxxxx");
        if (data.ModeType == 1 || data.ModeType == 4)
        {
            ScreenManagerHC.Instance.ShowGameModeInfo(data);
        }
        else
        {
            ScreenManagerHC.Instance.CheckTournamentStatusAndShow(data);
        }
    }

    #region VUOTLENNN

    [SerializeField] private float distanceToSlide;
    private bool _isDragging;
    private Vector2 _swipeStartPos;
    private bool _isOnSnap;
    public bool _isMoveDone = true;

    private void OnEnable()
    {
        _listMiniGameEventProto = null;
        _isMoveDone = true;
    }

    /*
    public void OnPointerDown(PointerEventData eventData)
    {
        _swipeStartPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;
        float distance = Vector2.Distance(_swipeStartPos, eventData.position);
        //Khi khoảng cách drag đủ thì mới cho snap true
        if (distance > distanceToSlide)
        {
            _isOnSnap = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            Debug.Log("Clicked on image!");
        }

        _isDragging = false;

        if (_isMoveDone && _isOnSnap)
        {
            _isMoveDone = false;
            OnEnableEventPageSignal.OnMoveSpecialEvent.Dispatch();
        }
    }
    */

    #endregion
}