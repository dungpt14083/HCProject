using System;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class EventsPage : UIView<EventsPage>
{
    [SerializeField] private RectTransform onGoingEvents;
    [SerializeField] private RectTransform specialEvents;
    [SerializeField] private SpecialEventMediator specialEventMediator;
    [SerializeField] private Vector2 posGoingEvents = new Vector2(0, 1000);
    [SerializeField] private Vector2 posSpecialEvents = new Vector2(475, 315);
    [SerializeField] private float timeMove = 1.0f;

    private Vector2 _originGoingEvents;
    private Vector2 _originSpecialEvents;
    
    private float _currentValue;
    private float _currentValueGoingEvents;
    private Tween _floatTweenMoveSpecialEvents;
    private Tween _floatTweenMoveGoingEvents;
    
    protected override void Awake()
    {
        base.Awake();
        //OnEnableEventPageSignal.OnEnableEventContent.AddListener(ShowOnGoingAndContainerResize);
        _originGoingEvents = onGoingEvents.anchoredPosition;
        _originSpecialEvents = specialEvents.offsetMax;
        OnEnableEventPageSignal.OnMoveSpecialEvent.AddListener(MoveAnimationDrag);
    }

    private void OnDestroy()
    {
        //OnEnableEventPageSignal.OnEnableEventContent.RemoveListener(ShowOnGoingAndContainerResize);
        OnEnableEventPageSignal.OnMoveSpecialEvent.RemoveListener(MoveAnimationDrag);

    }

    private void OnEnable()
    {
        ShowOnGoingAndContainerResize();
    }

    [Button]
    private void ShowOnGoingAndContainerResize()
    {
        _floatTweenMoveSpecialEvents?.Kill();
        _floatTweenMoveGoingEvents?.Kill();
        specialEvents.offsetMax = _originSpecialEvents;
        onGoingEvents.anchoredPosition = _originGoingEvents;
        specialEventMediator.ShowListMiniGameEvent();
    }


    private void MoveAnimationDrag()
    {
        MoveSpecialEvents();
        MoveGoingEvents();
    }
    
    [Button]
    private void MoveSpecialEvents()
    {
        _currentValue = _originSpecialEvents.y;
        _floatTweenMoveSpecialEvents=DOTween.To(() => _currentValue, x => _currentValue = x, posSpecialEvents.y, timeMove).OnUpdate(() =>
        {
            float currentFloatValue = _currentValue;
            specialEvents.offsetMax = new Vector2(0, currentFloatValue);
        });
    }

    [Button]
    private void MoveGoingEvents()
    {
        _floatTweenMoveGoingEvents=onGoingEvents.DOAnchorPosY(posGoingEvents.y, timeMove);
    }
}