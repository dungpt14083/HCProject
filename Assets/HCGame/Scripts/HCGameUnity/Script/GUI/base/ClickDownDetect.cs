using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickDownDetect : MonoBehaviour, IPointerDownHandler
{
    private Vector3 _baseScale;

    [SerializeField] private UnityEvent<PointerEventData> onClickDown;

    // Start is called before the first frame update
    void Start()
    {
        _baseScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = _baseScale;
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f);

        if(null != onClickDown)
        {
            onClickDown.Invoke(eventData);
        }
    }
}
