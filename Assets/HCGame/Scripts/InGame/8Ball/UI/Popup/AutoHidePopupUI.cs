using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AutoHidePopupUI : MonoBehaviour
{
    Action _hideSuccessCallback;
    [SerializeField] GameObject _content;
    [SerializeField] float timeToHide;
    bool _isHidingContent;
    float _timeShow;
    // Start is called before the first frame update
    private void OnDestroy()
    {
        _hideSuccessCallback = null;
    }

    private void Update()
    {
        if(_isHidingContent)
        {
            if(_timeShow>0)
            {
                _timeShow -= Time.deltaTime;
            }
            else
            {
                _isHidingContent = false;
                _hideSuccessCallback?.Invoke();
                _content.SetActive(false);
            }
        }
    }
    public void ShowPopup(float delay = 0, Action action = null)
    {        
        _timeShow = timeToHide;
        _hideSuccessCallback = action;
        try
        {
            Invoke(nameof(DelayShowContent), delay);
        }catch(Exception e)
        {
            Debug.LogError("8 ball ShowPopup Error " + e.StackTrace);
        }
    }
    
    void DelayShowContent()
    {
        _isHidingContent = true;
        _content.SetActive(true);
    }
    
}
