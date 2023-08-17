using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = System.Random;

public class EditYourProfilePopup : UIPopupView<EditYourProfilePopup>
{
    public Button btClose;
    public Button btApply;
    public Button btEditAvatar;
    public Button btRandomUserName;
    public Image avatar;
    public TMP_InputField inputUserName;
    protected override void Awake()
    {
        base.Awake();
        btClose.onClick.AddListener(CloseView);
        btApply.onClick.AddListener(Apply);
        btEditAvatar.onClick.AddListener(EditAvatar);
        btRandomUserName.onClick.AddListener(GenerateUsername);
    }

    public void Show(string userName, string urlAvatar = "")
    {
        inputUserName.text = userName;
        if (urlAvatar != string.Empty) StartCoroutine(HCHelper.LoadAvatar(urlAvatar, avatar));
    }

    public void EditAvatar()
    {
        ScreenManagerHC.Instance.ShowUpdateProfile();
        CloseView();
    }

    public void Apply()
    {
        var newUserName = inputUserName.text.Trim();
        HCAppController.Instance.ApplyEditYourProfile(newUserName);
    }
    public static string GenerateRandomUsername()
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789_";
        var rng = new Random();
        var chars = new char[12];
        for (int i = 0; i < 12; i++)
        {
            chars[i] = allowedChars[rng.Next(0, allowedChars.Length)];
        }
        return new string(chars);
    }
    
    public void GenerateUsername()
    {
        string randomUsername = GenerateRandomUsername();
        inputUserName.text = randomUsername;
    }

}