using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using Bingo;

public class Bingo_AutoClosePanelBosster : MonoBehaviour
{
    [SerializeField] int _timeClosed;
    [SerializeField] TextMeshProUGUI _txtTime;
    [SerializeField] bool _isShowTimeMinAndS;
    Action _endCloseCallback;

    Coroutine _coroutine;
    int _currentTime;
    // Start is called before the first frame update
    public void ShowPopup(Action endCloseCallback)
    {
        gameObject.SetActive(true);
        _endCloseCallback = endCloseCallback;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _currentTime = _timeClosed;
        SetTime(_currentTime);
        _coroutine = StartCoroutine(DelayClosePanelBosster());

    }

    public void HidePopup()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    IEnumerator DelayClosePanelBosster()
    {
        while (_currentTime >= 0)
        {
            yield return new WaitForSeconds(1);
            _currentTime -= 1;
            SetTime(_currentTime);
            if (_currentTime <= 0)
            {
                if (Bingo_Bot_Booster.instance._isPickABall == true)
                {
                    Bingo_NetworkManager.instance.SendMessageNoClickPickABall(0,0,7);
                }
                else
                {
                    Bingo_NetworkManager.instance.SendMessageClick(0,0);

                }
            }
        }
        _endCloseCallback.Invoke();
        //gửi lên resume
       
    }

    void SetTime(int time)
    {
        if (_isShowTimeMinAndS)
        {
            int m = time / 60;
            int s = time % 60;
            _txtTime.SetText("{0:00}:{1:00}", m, s);
        }
        else
        {
            _txtTime.SetText("{0}", time);
        }
    }
}
