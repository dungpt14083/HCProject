using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInfo : PageBase
{
    
    public ProfileDetail profileDetail;
    public Image avatar;
    public Image backgroundUser;
    public Image boderBackground;
    public TextMeshProUGUI txtUserName;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txUserId;
    public TextMeshProUGUI txtExp;
    public Image progessbarExp;

    public Button btUpdateProfile;
    public Button btInviteFriend;
    public Button btReferralCode;
    public Button btCoupon;
    public Button btDiscord;
    public Button btFacebook;
    public Button btFNCY;

    private void Awake()
    {
        btUpdateProfile.onClick.AddListener(UpdateProfile);
        btInviteFriend.onClick.AddListener(InviteFriend);
        btReferralCode.onClick.AddListener(ReferralCode);
        btCoupon.onClick.AddListener(Coupon);
        btDiscord.onClick.AddListener(Discord);
        btFacebook.onClick.AddListener(Facebook);
        btFNCY.onClick.AddListener(FNCY);
    }

    private void OnEnable()
    {
        backgroundUser.sprite = HCAppController.Instance?.myBackground;
        StartCoroutine(LoadBackGround(HCAppController.Instance.myurlbackround, backgroundUser));
        StartCoroutine(LoadBoder(HCAppController.Instance.myUrlBoder, boderBackground));
    }

    public void Show()
    {
        UserDataProto userInfo = HCAppController.Instance?.userInfo;
        var level = userInfo == null ? 0 : userInfo.Level;
        long currentExp = userInfo.Exp;
        long nextLevelExp = userInfo.ExpToNextLevel;
        avatar.sprite = HCAppController.Instance?.myAvatar;
        boderBackground.sprite = HCAppController.Instance?.myBoder;
        backgroundUser.sprite = HCAppController.Instance?.myBackground;
        txtUserName.text = userInfo?.UserName;
        string usercodeid = HCAppController.Instance?.userInfo.UserCodeId;
        txUserId.text = usercodeid.Substring(0, 9);
        txtLevel.text = $"Level {level} :";
        txtExp.text = $"( {currentExp} / {nextLevelExp})";
        if (nextLevelExp > 0) progessbarExp.fillAmount = currentExp / (float)nextLevelExp;
        else progessbarExp.fillAmount = 1;
        
        FadeIn();
    }

    public IEnumerator LoadBoder(string url, Image Boder)
    {
        yield return new WaitForSeconds(0);

        if (url == "")
        {
            if (Boder != null) Boder.sprite = null;
            yield return null;
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            Texture2D profilePic = www.texture;
            if (www == null || www.texture == null)
            {
                Debug.Log("www == null || www.texture == null");
                if (Boder != null) Boder.sprite = null;
                yield return null;
            }
            else
            {

                var spr = Sprite.Create(profilePic, new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0, 0));
                if (Boder != null) Boder.sprite = spr;
                boderBackground.sprite = Boder.sprite;
                Debug.Log("đã load sprte");
                HCAppController.Instance.myBoder = Boder.sprite;
            }
        }
    }

    public  IEnumerator LoadBackGround(string url, Image background)
    {
        yield return new WaitForSeconds(0);
        
        if (url == "")
        {
            if (background != null) background.sprite = null;
            yield return null;
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            Texture2D profilePic = www.texture;
            if (www == null || www.texture == null)
            {
                Debug.Log("www == null || www.texture == null");
                if (background != null) background.sprite = null;
                yield return null;
            }
            else
            {
                
                var spr = Sprite.Create(profilePic, new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0, 0));
                if (background != null) background.sprite = spr;
                backgroundUser.sprite = background.sprite;
                Debug.Log("đã load sprte");
                HCAppController.Instance.myBackground = background.sprite;
            }
        }
      
    }
    public void Hide()
    {
        FadeOut();
    }
    public void UpdateProfile()
    {
        profileDetail.ShowUpdateProfile();
    }
    public void InviteFriend()
    {
        profileDetail.ShowInvite();
    }
    public void ReferralCode()
    {
        profileDetail.ShowReferraPopupp();
    }
    public void Coupon()
    {
        profileDetail.ShowCouponPopup();
    }
    public void Discord()
    {
        Application.OpenURL("https://discord.gg");
    }
    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com");
    }
    public void FNCY()
    {
        Application.OpenURL("https://google.com");
    }
    public string GetTitle()
    {
        return "Profile";
    }
}
