using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBoxData
{
    public string mTitle = "";
    public string mContent = "";
    public Action onOkClick;
    public bool isHide = true;

    public MessageBoxData(string title, string content, Action _onOkClick = null, bool isHide = true)
    {
        if (string.IsNullOrEmpty(title))
        {
            mTitle = "Error";
        }
        else
        {
            mTitle = title;
        }
        mContent = content;
        onOkClick = _onOkClick;
        this.isHide = isHide;
    }
}

public class MessageDialogHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtContent, txtTitle;
    [HideInInspector] private MessageBoxData data;
    [HideInInspector] public bool IsShowing = false;

    private Action _onCallbackHandler = null;

    public void OnShow(MessageBoxData messageBoxData, Action callback = null)
    {
        data = messageBoxData;
        txtTitle?.SetText(data.mTitle);
        txtContent?.SetText(data.mContent);
        gameObject.SetActive(true);
        IsShowing = true;
        _onCallbackHandler = callback;
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
        IsShowing = false;
    }

    public void OnBtnOkClicked()
    {
        data.onOkClick?.Invoke();
        _onCallbackHandler?.Invoke();
        if (data.isHide)
        {
            OnHide();
        }
    }
}
