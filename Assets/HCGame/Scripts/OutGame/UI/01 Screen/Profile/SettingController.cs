using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingController : PageBase
{
    public Button btBack;
    public Button btUpdateProfile;
    public Button btLogout;
    public Button btDeleteAccount;
    public Toggle ToggleNotifily,ToggleMuteMusic,ToggleMuteSound;
    public Action actionShowUpdateProfile;
    public Action actionBack;
    public GameObject AboutHCApp, HowToGetHCtokens, HelpSupport, Privacypolicy, Reporstissues, Termconditions;

    private void Awake()
    {
        btBack.onClick.AddListener(Back);
        btLogout.onClick.AddListener(Logout);
        btDeleteAccount.onClick.AddListener(DeleteAccount);
        btUpdateProfile.onClick.AddListener(ShowUpdateProfile);
        ToggleMuteSound.isOn = !AudioManager.Instance.muted;
        ToggleMuteMusic.isOn = !AudioManager.Instance.mutedMS;
    }

    public void StopSound()
    {
        Debug.Log("click sound");
        AudioManager.Instance.StopSound();
        
    }
    public void StopMusic()
    {
        Debug.Log("click mussic");
        AudioManager.Instance.StopMusic();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        FadeIn();
    }
    public void Hide()
    {
        DisableDisplay();
    }
    public void Back()
    {
        Hide();
        actionBack?.Invoke();
    }
    public void Logout()
    {
        HcPopupManager.Instance.ShowLogoutPopup("Do you want to logout this account?");
    }
    public void DeleteAccount()
    {
        Debug.Log("DeleteAccount 1");
        HcPopupManager.Instance.ShowDeleteAccountPopup("<b>Note:</b> \n " +
            "1.User cannot log in the deleted account \n " +
            "2.User cannot recover deleted account \n " +
            "3.All data related to this account will be deleted and cannot be recoverd");
    }
    public void ShowUpdateProfile()
    {
        actionShowUpdateProfile?.Invoke();
    }

    public void showSupport()
    {
        Application.OpenURL("https://www.google.com/");
    }

    public void showAboutHCapp()
    {
        AboutHCApp.SetActive(true);
    }
    public void showhowtogetHCToken()
    {
        HowToGetHCtokens.SetActive(true);
    }

    public void showHelpSupport()
    {
        HelpSupport.SetActive(true);
    }

    public void showPrivacy()
    {
        Privacypolicy.SetActive(true);
    }

    public void showReportIsSues()
    {
        Reporstissues.SetActive(true);
    }

    public void showTermConditions()
    {
        Termconditions.SetActive(true);
    }
}
