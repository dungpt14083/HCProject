using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonBase<TData> : MonoBehaviour, ISelectItem<TData>
{
    [SerializeField] protected Button filterBtn;
    [SerializeField] protected RectTransform selectGo, disableGo;
    [SerializeField] protected TData _info;

    protected Action<TData> _onClick;

    protected Tween _twinScale;

    public TData Info => _info;
    
    private void Start()
    {
        filterBtn.onClick.AddListener(PressInvokeSelect);
    }

    private Coroutine _timeIntervalButton;
    private bool _isReadyShow = true;

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
    }

    public virtual void OnShow(Action<TData> onClick)
    {
        selectGo.gameObject.SetActive(false);
        disableGo.gameObject.SetActive(true);
        _onClick = onClick;
        _isReadyShow = true;
    }

    public virtual void OnSelectFilter(TData info)
    {
        /*
        if (this._info==info)
        {
            selectGo.gameObject.SetActive(true);
            disableGo.gameObject.SetActive(false);
        }
        else
        {
            disableGo.gameObject.SetActive(true);
            selectGo.gameObject.SetActive(false);
        }
        */
    }
}