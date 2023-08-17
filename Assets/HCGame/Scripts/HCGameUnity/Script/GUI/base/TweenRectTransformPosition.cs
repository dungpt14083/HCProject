
using UnityEngine;
using DG.Tweening;

public class TweenRectTransformPosition : MonoBehaviour, ITweenRectTransformInterface
{
    [SerializeField] private Vector2 showPosition;
    [SerializeField] private Vector2 hidePosition;
    [SerializeField] float timeMove = 0.5f;

    public Vector2 ShowPosition
    {
        set
        {
            showPosition = value;
        }
        get
        {
            return showPosition;
        }
    }

    public Vector2 HidePosition
    {
        set
        {
            hidePosition = value;
        }
        get
        {
            return hidePosition;
        }
    }

    private RectTransform rectTransform;
    private void Awake()
    {
        if(null == rectTransform)
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
        rectTransform.anchoredPosition = hidePosition;
    }

    public void OnBeginShow()
    {
        DOTween.Kill(this);
        Reset();
        rectTransform.DOAnchorPos(showPosition, timeMove, true);
    }

    public void OnBeginHide()
    {
        DOTween.Kill(this);
        rectTransform.DOAnchorPos(hidePosition, timeMove, true);
    }
}
