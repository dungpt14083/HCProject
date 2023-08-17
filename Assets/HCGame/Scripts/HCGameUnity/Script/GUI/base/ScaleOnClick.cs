using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Solitaire;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleOnClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static ScaleOnClick Ins;
    [Header("State")]
    public Color disableColor = new Color(120, 120, 120, 200);
    public bool deactive = false;
    [Header("Click handler")]
    private Vector3 _baseScale;
    [SerializeField] private Vector3 punchScale = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private float clickAnimationTime = 0.3f;
    [SerializeField] private float delayTime = 0.2f;
    [SerializeField] private SoundName clickSFX = SoundName.None;
    public UnityEvent OnDelayedClick;

    [Header("Pointer enter/leave handler")]
    [SerializeField] private bool scaleOnPointerEnter = false;
    [SerializeField] private Vector3 pointerHoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float hoverAnimation = 0.3f;
    private NetworkController _NetworkController;
    private bool _canClick = true;
    private float _lastClickTime = 0;

    private void Awake()
    {
        Ins = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _baseScale = transform.localScale;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Auto enable button if enough time have passed
        if (Time.realtimeSinceStartup >= _lastClickTime + clickAnimationTime)
        {
            _canClick = true;
        }

        if (deactive || false == _canClick)
        {
            return;
        }

        _canClick = false;
        _lastClickTime = Time.realtimeSinceStartup;

        transform.DOKill();
        //transform.localScale = _baseScale;
        transform.DOPunchScale(punchScale, clickAnimationTime).onComplete += () =>
        {
            _canClick = true;
        };

        if (SoundName.None != clickSFX)
        {
            HCGameManager.Instance.GetSoundManager.PlaySound(clickSFX);
        }
        OnDelayedClick?.Invoke();
        //TODO: Temp disable, exception not throw when using tweener callback
        // DOVirtual.DelayedCall(delayTime, () => 
        // {
        //     OnDelayedClick?.Invoke();
        // }, false);
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (deactive) return;
        if (false == scaleOnPointerEnter)
        {
            return;
        }
        transform.DOKill();
        transform.DOScale(pointerHoverScale, hoverAnimation);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (deactive) return;
        if (false == scaleOnPointerEnter)
        {
            return;
        }
        transform.DOKill();
        transform.DOScale(_baseScale, hoverAnimation);
    }
    #region State handle
    Image overlayDeactive;
    public void SetDeactive(bool deactive)
    {
        this.deactive = deactive;
        if (deactive)
        {
            if (overlayDeactive == null)
            {
                var go = new GameObject("DisableOverlay");
                go.transform.parent = transform;
                var rect = go.AddComponent<RectTransform>();
                rect.anchorMin = Vector3.zero;
                rect.anchorMax = Vector3.one;
                rect.anchoredPosition = Vector3.zero;
                rect.sizeDelta = Vector2.zero;
                rect.localScale = Vector3.one;
                Image currentSprite = transform.GetComponent<Image>();
                overlayDeactive = go.AddComponent<Image>();
                overlayDeactive.preserveAspect = currentSprite.preserveAspect;
                overlayDeactive.sprite = currentSprite.sprite;
                overlayDeactive.color = disableColor;
            }
            else
            {
                overlayDeactive.gameObject.SetActive(true);
            }
        }
        else
        {
            if (overlayDeactive != null)
                overlayDeactive.gameObject.SetActive(false);
        }
    }
    #endregion
}
