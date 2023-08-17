using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ScaleOnClickAdvance : MonoBehaviour, IPointerClickHandler
{
    [Serializable]
    private class OnDelayedClickData
    {
        public float delayTime = 0.2f;
        public UnityEvent OnDelayedClick;
    }

    private Vector3 _baseScale;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private OnDelayedClickData[] OnDelayedClickCallbacks;


    // Start is called before the first frame update
    void Start()
    {
        _baseScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = _baseScale;
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), animationDuration);

        foreach(OnDelayedClickData data in OnDelayedClickCallbacks)
        {
            DOVirtual.DelayedCall(data.delayTime, () =>
            {
                data.OnDelayedClick?.Invoke();
            }, false);
        }
    }
}
