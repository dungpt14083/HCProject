using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.IO;
using System.Net;
using BestHTTP;
using BonusGame;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class UserDataUI : MonoBehaviour
{
    public Button btProfile;
    [SerializeField] private Image myAvatar;
    [SerializeField] private Image myBoderAvatar;
    [SerializeField] private TextMeshProUGUI ticketVaueText, gemValueText, goldValueText;
    [SerializeField] private RectTransform gold, token, ticket;

    public static Vector3 GoldPos = new Vector3(-219f, 854f, 0);
    public static Vector3 TokenPos = new Vector3(37f, 854f, 0);
    public static Vector3 TicketPos = new Vector3(292.7f, 850f, 0);


    private void Start()
    {
        GoldPos = gold.anchoredPosition;
        TokenPos = token.anchoredPosition;
        TicketPos = ticket.anchoredPosition;
        Debug.Log("-----------UserDataUI");
        btProfile.onClick.AddListener(ShowProfile);
    }

    #region UPDATEANIMTION

    private void OnEnable()
    {
        UpdateData(HCAppController.Instance.userInfo);
        UpdateCurrencySignals.UpdateMoney.AddListener(RunAnimationAddMoney);
    }

    private void OnDisable()
    {
        UpdateCurrencySignals.UpdateMoney.RemoveListener(RunAnimationAddMoney);
    }


    private void RunAnimationAddMoney(UpdateMoney current, UpdateMoney newData)
    {
        if (newData.Token > 0)
        {
            HCHelper.NumberIncreasesEff(gemValueText, current.Token, newData.Token);
        }

        if (newData.Gold > 0)
        {
            HCHelper.NumberIncreasesEff(goldValueText, current.Gold, newData.Gold);
        }

        if (newData.Token > 0)
        {
            HCHelper.NumberIncreasesEff(ticketVaueText, current.Ticket, newData.Ticket);
        }
    }

    #endregion


    //HÀM NÀY ĐANG CẬP NHẬT TIỀN KHI START HOẶC LÀ CÓ UPDATE USERINFO THÌ MỚI UPDATE
    public void UpdateData(UserDataProto userInfo)
    {
        if (userInfo == null) return;
        ticketVaueText.text = StringUtils.FormatMoneyK(userInfo.UserTicket);
        gemValueText.text = StringUtils.FormatMoneyK(userInfo.UserToken);
        goldValueText.text = StringUtils.FormatMoneyK(userInfo.UserGold);
        Executors.Instance.StartCoroutine(LoadAvatar(userInfo.UserAvatar, myAvatar));
        Executors.Instance.StartCoroutine(LoadBoder(userInfo.Frame, myBoderAvatar));
    }


    public IEnumerator LoadBoder(string url, Image Boder)
    {
        yield return new WaitUntil(() => this.gameObject.activeSelf);
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
                myBoderAvatar.sprite = Boder.sprite;
                Debug.Log("đã load sprte");
                HCAppController.Instance.myBoder = Boder.sprite;
            }
        }
    }

    public IEnumerator LoadAvatar(string url, Image avatar)
    {
        yield return new WaitUntil(() => this.gameObject.activeSelf);
        Debug.Log("Update avatar " + url);
        if (url == "")
        {
            if (avatar != null) avatar.sprite = null;
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
                if (avatar != null) avatar.sprite = null;
                yield return null;
            }
            else
            {
                var spr = Sprite.Create(profilePic, new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0, 0));
                if (avatar != null) avatar.sprite = spr;
                HCAppController.Instance.myAvatar = avatar.sprite;
            }
        }
    }

    public void ShowProfile()
    {
        if (BonusGame_Manager.Instance != null && BonusGame_Manager.Instance.gameObject.activeSelf &&
            BonusGame_Manager.Instance.IsRunning)
        {
            return;
        }

        GameSignals.CloseAllPopup.Dispatch();
        ScreenManagerHC.Instance.ShowProfile();
    }
}