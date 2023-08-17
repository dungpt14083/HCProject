using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenRectTransformSize : MonoBehaviour, ITweenRectTransformInterface
{
    [SerializeField] private Vector2 showSize;
    [SerializeField] private Vector2 hideSize;
    [SerializeField] float timeAnimation = 0.5f;

    private RectTransform rectTransform;
    private void Awake()
    {
        if (null == rectTransform)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        Reset();
    }

    public void Reset()
    {
        if (null == rectTransform)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        rectTransform.sizeDelta = hideSize;
    }

    public void OnBeginShow()
    {
        DOTween.Kill(this);
        Reset();
        rectTransform.DOSizeDelta(showSize, timeAnimation, true);
    }

    public void OnBeginHide()
    {
        DOTween.Kill(this);
        rectTransform.DOSizeDelta(hideSize, timeAnimation, true);
    }
}
