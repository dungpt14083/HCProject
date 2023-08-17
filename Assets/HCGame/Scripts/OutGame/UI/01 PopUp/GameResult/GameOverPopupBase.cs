using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopupBase : MonoBehaviour
{
    public Button btSubmit;
    //public TMP_Text txtSubmit;

    public Coroutine waitEndRoomShowButton;

    private void Awake()
    {
        btSubmit.onClick.AddListener(Submit);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        btSubmit.interactable = false;
        ScreenManagerHC.Instance.groupBottomHomeController.gameObject.SetActive(true);
        ClearData();
        if (waitEndRoomShowButton != null) StopCoroutine(waitEndRoomShowButton);
        waitEndRoomShowButton = StartCoroutine(ShowButtonSubmit());
    }

    private IEnumerator ShowButtonSubmit()
    {
        yield return new WaitForSeconds(3.0f);
        SetFuncForButtonSubmit();
    }

    private void SetFuncForButtonSubmit()
    {
        btSubmit.interactable = true;
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Submit()
    {
    }

    public virtual void ClearData()
    {
    }
}