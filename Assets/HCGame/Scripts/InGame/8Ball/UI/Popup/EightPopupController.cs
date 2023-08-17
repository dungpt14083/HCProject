using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EightPopupController : MonoBehaviour
{
    [SerializeField] AutoHidePopupUI _popupPlayerBreaking,  _popupYourTurn, _successPopup, _gameOverPopup, _timeUpPopup;
    [SerializeField] EightPopupHitBall _popupHitBallGroup;
    [SerializeField] EightPopupShowMesageDialog _showMesageDialog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPopupPlayerBreaking(float delay = 0,Action hideSuccessed = null)
    {
        _popupPlayerBreaking.ShowPopup(delay, hideSuccessed);
    }

    public void ShowMesageDialog(string message, float delay = 0, Action hideSuccessed = null)
    {
        _showMesageDialog.ShowMesagePopup(message, delay, hideSuccessed);
    }

    public void ShowPopupHitBallAnotherGroup(bool isMain, float delay = 0,Action hideSuccessed = null)
    {
        _popupHitBallGroup.ShowHitBallGroup(isMain, delay, hideSuccessed);
    }

    public void ShowPopupYourTurn(float delay = 0, Action hideSuccessed = null)
    {
        _popupYourTurn.ShowPopup(delay, hideSuccessed);
    }

    public void ShowEndGamePopup(bool isWin, float delay = 0, Action hideSuccessed = null)
    {
        if(isWin)
            _successPopup.ShowPopup(delay, hideSuccessed);
        else
            _gameOverPopup.ShowPopup(delay, hideSuccessed);
    }

    public void ShowTimeupPopup(float delay = 0,Action hideSuccessed = null)
    {
        _timeUpPopup.ShowPopup(delay, hideSuccessed);
    }
  
}
