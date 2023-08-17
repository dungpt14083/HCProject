using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolicyPopupControler : MonoBehaviour
{
    public Image BackgrounDefult;
    public GameObject panelPolicy;
    public GameObject panelTermonUse;
    public Button btnConfirmDefult;
    public Button btnPricavcyPolicy;
    public Button btnTermOnUse;

    private void Awake()
    {
        btnPricavcyPolicy.onClick.AddListener(showPolicy);
        btnTermOnUse.onClick.AddListener(showTermonUse);
        btnConfirmDefult.onClick.AddListener(LoadSceneMain);
    }

    public void showPolicy()
    {
        //  btnConfirmDefult.gameObject.SetActive(false);
        BackgrounDefult.gameObject.SetActive(false);
        panelTermonUse.gameObject.SetActive(false);
        panelPolicy.gameObject.SetActive(true);
    }

    public void showTermonUse()
    {
        BackgrounDefult.gameObject.SetActive(false);
        panelPolicy.gameObject.SetActive(false);
        panelTermonUse.gameObject.SetActive(true);
    }

    public void LoadSceneMain()
    {
        HCAppController.Instance.LoadScene("Home", ShowLogin);
    }

    private void ShowLogin()
    {
        ScreenManagerHC.Instance.ShowLoginScreen();
    }
}