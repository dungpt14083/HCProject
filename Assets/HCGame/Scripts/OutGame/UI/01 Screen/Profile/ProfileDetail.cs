using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProfileDetail : MonoBehaviour
{
    public GameObject topUI;
    public TextMeshProUGUI txtTitle;
    public ProfileInfo profileInfo;
    public Invite invite;
    public Button btBack;
    public Button btShowSetting;

    public Action actionShowUpdateProfile;
    public Action actionShowSetting;

    private void Awake()
    {
        btBack.onClick.AddListener(Back);
        btShowSetting.onClick.AddListener(ShowSetting);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        topUI.SetActive(true);
        DisableAll();
        profileInfo.Show();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Back()
    {
        Hide();
       ScreenManagerHC.Instance.ReceivedOnClickNavigation(TypeButtonHome.Home);
    }
    public void ShowSetting()
    {
        DisableAll();
        actionShowSetting?.Invoke();
    }
    public void DisableAll()
    {
        profileInfo.DisableDisplay();
//        invite.CloseView();
    }
    public void ShowProfileInfo()
    {
        DisableAll();
        txtTitle.text = profileInfo.GetTitle();
        btBack.onClick.RemoveAllListeners();
        btBack.onClick.AddListener(Back);

        profileInfo.Show();
    }
    public void ShowUpdateProfile()
    {
        actionShowUpdateProfile?.Invoke();
    }
    public void ShowCouponPopup()
    {
        txtTitle.text = "Coupon";
        btBack.onClick.RemoveAllListeners();
        btBack.onClick.AddListener(ShowProfileInfo);
        HcPopupManager.Instance.ShowCouponPopup();
    }
    public void ShowReferraPopupp()
    {
        txtTitle.text = "Referral";
        btBack.onClick.RemoveAllListeners();
        btBack.onClick.AddListener(ShowProfileInfo);
        HcPopupManager.Instance.ShowReferraPopup(HCAppController.Instance.userInfo.RewardReferralCode);
    }
    public void ShowInvite()
    {
//        txtTitle.text = invite.GetTitle();
        btBack.onClick.RemoveAllListeners();
        btBack.onClick.AddListener(ShowProfileInfo);
        HcPopupManager.Instance.ShowInviteFrendPoup();
    }
}
