using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHome : MonoBehaviour, ISelectItem<TypeButtonHome>
{
    [SerializeField] private Button filterBtn;
    [SerializeField] private RectTransform selectGo, disableGo;
    [SerializeField] private RectTransform scaleButton;
    [SerializeField] private RectTransform scaleIcon;
    [SerializeField] private RectTransform scaleLable;
    [SerializeField] private float scale = 1.2f;
    [SerializeField] private float scaleinIcon = 1.5f;
    [SerializeField] private float scaleinLabe = 1.5f;
    [SerializeField]private TypeButtonHome _info;
    
    [SerializeField] private Image iconButton;
    [SerializeField] private TMP_Text textEvent;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private string defaultText;
    
    private Action<TypeButtonHome> _onClick;
    
    private Tween _twinScale;
    private Vector3 _originScale = new Vector3(1, 1, 1);

    public TypeButtonHome Info => _info;
    
    
    private void Start()
    {
        filterBtn.onClick.AddListener(PressInvokeSelect);
        _originScale = scaleButton.localScale;
        _originScale = scaleIcon.localScale;
        _originScale = scaleLable.localScale;
    }

    private Coroutine _timeIntervalButton;
    private bool _isReadyShow = true;

    //NGĂN CHẶN BẤM NHANH QUÁ
    private void PressInvokeSelect()
    {
        if (_isReadyShow)
        {
            _onClick?.Invoke(_info);
            _isReadyShow = false;
            _timeIntervalButton = StartCoroutine(IsReadyInvokeButton());
        }
        else
        {
            Debug.LogError("Thao tác chậm thôi nào !");
        }
    }

    private IEnumerator IsReadyInvokeButton()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            _isReadyShow = true;
            break;
        }
    }


    private void OnDisable()
    {
        _isReadyShow = true;
        StopAllCoroutines();
        _twinScale?.Kill();
    }

    public void OnShow(Action<TypeButtonHome> onClick)
    {
        selectGo.gameObject.SetActive(false);
        disableGo.gameObject.SetActive(true);
        _onClick = onClick;
        _isReadyShow = true;
        scaleButton.localScale = _originScale;
        scaleIcon.localScale = _originScale;
        scaleLable.localScale = _originScale;
    }

    public void OnSelectFilter(TypeButtonHome info)
    {
        _twinScale?.Kill();
        ChangeEventDefault(defaultSprite);
        textEvent.text = defaultText;
        if (this._info == info)
        {
            selectGo.gameObject.SetActive(true);
            disableGo.gameObject.SetActive(false);
            _twinScale = scaleButton.DOScale(_originScale * scale, 0.2f);
            _twinScale = scaleIcon.DOScale(_originScale * scaleinIcon, 0.2f);
            _twinScale = scaleLable.DOScale(_originScale * scaleinLabe, 0.2f);
        }
        else
        {
            disableGo.gameObject.SetActive(true);
            selectGo.gameObject.SetActive(false);
            _twinScale = scaleButton.DOScale(_originScale, 0.2f);
            _twinScale = scaleIcon.DOScale(_originScale, 0.2f);
            _twinScale = scaleLable.DOScale(_originScale, 0.2f);
        }
    }



    public void ChangeEventIcon(Sprite spriteEvent)
    {
        iconButton.sprite = spriteEvent;
        
    }
    
    public void ChangeEventLable(String lableEvent)
    {
        textEvent.text = lableEvent;
        
    }
    public void ChangeEventDefault(Sprite spriteEvent)
    {
        iconButton.sprite = defaultSprite;
    }
}

public enum TypeButtonHome
{
    Home,
    Event,
    Ranking,
    History,
    None
}