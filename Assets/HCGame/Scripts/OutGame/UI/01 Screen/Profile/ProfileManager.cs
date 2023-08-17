using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : UIView<ProfileManager>
{
    public ProfileDetail profileDetail;
    public UpdateProfile updateProfile;
    public SettingController settingController;

    public void ShowProfile()
    {
        DisableAll();
        profileDetail.actionShowUpdateProfile = ShowUpdateProfileFromProfile;
        profileDetail.actionShowSetting = ShowSetting;
        profileDetail.Show();
    }
    public void ShowSetting()
    {
        DisableAll();
        settingController.actionShowUpdateProfile = ShowUpdateProfileFromSetting;
        settingController.actionBack = ShowProfile;
        settingController.Show();
    }
    public void Hide()
    {
        profileDetail.Hide();
    }
    public void ShowUpdateProfileFromProfile()
    {
        DisableAll();
        updateProfile.actionBack = ShowProfile;
        updateProfile.Show();
    }
    public void ShowUpdateProfileFromSetting()
    {
        DisableAll();
        updateProfile.actionBack = ShowSetting;
        updateProfile.Show();
    }
    public void DisableAll()
    {
        profileDetail.Hide();
        settingController.Hide();
        updateProfile.Hide();
    }
}
