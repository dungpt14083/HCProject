using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenRectTransformLocalScale : MonoBehaviour, ITweenRectTransformInterface
{
    private enum ETweenType
    {
        Linear,
        Punch,
        Shake,
    }

    [SerializeField] private Vector2 showScale;
    [SerializeField] private Vector2 hideScale;
    [SerializeField] private float timeAnimation = 0.5f;
    [SerializeField] private ETweenType _tweenType;



    private void Awake()
    {
        Reset();
    }

    public void OnBeginShow()
    {
        DOTween.Kill(this);
        Reset();

        switch(_tweenType)
        {
            case ETweenType.Linear:
                transform.DOScale(showScale, timeAnimation);
                break;
            case ETweenType.Punch:
                transform.DOPunchScale(showScale, timeAnimation);
                break;
            case ETweenType.Shake:
                transform.DOShakeScale(timeAnimation);
                break;
        }
    }

    public void OnBeginHide()
    {
        DOTween.Kill(this);

        switch (_tweenType)
        {
            case ETweenType.Linear:
                transform.DOScale(hideScale, timeAnimation);
                break;
            case ETweenType.Punch:
                transform.DOPunchScale(hideScale, timeAnimation);
                break;
            case ETweenType.Shake:
                transform.DOShakeScale(timeAnimation);
                break;
        }
    }

    public void Reset()
    {
        transform.localScale = hideScale;
    }
}
