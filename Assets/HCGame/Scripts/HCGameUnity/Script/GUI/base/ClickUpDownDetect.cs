using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickUpDownDetect : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Vector3 _baseScale;
    [SerializeField] private Vector3 scaleOnClick = new Vector3(0.1f,0.1f,0.1f);

    [SerializeField] private UnityEvent<PointerEventData> onClickUp;
    [SerializeField] private UnityEvent<PointerEventData> onClickDown;

    void Start()
    {
        _baseScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = _baseScale;
        transform.DOScale(scaleOnClick, 0.3f);

        if (null != onClickDown)
        {
            onClickDown.Invoke(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = scaleOnClick;
        transform.DOScale(_baseScale, 0.3f);

        if (null != onClickUp)
        {
            onClickUp.Invoke(eventData);
        }
    }

}
