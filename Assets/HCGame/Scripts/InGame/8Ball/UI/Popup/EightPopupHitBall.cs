using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightPopupHitBall : MonoBehaviour
{
    [SerializeField] AutoHidePopupUI _autoHidePopupUI;
    [SerializeField] GameObject _solidBall, _stripedBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHitBallGroup(bool isMain, float delay = 0, Action hideSuccessed = null)
    {
        _autoHidePopupUI.ShowPopup(delay, hideSuccessed);
        _solidBall.SetActive(isMain);
        _stripedBall.SetActive(!isMain);
    }
}
