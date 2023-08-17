using System.Collections;
using System.Collections.Generic;
using BonusGame;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusGame_Screen : MonoBehaviour
{
    public GameObject bgImage;

    [SerializeField] protected NoticeManagement noticeManagement;
    [SerializeField] protected Button openButton;
    [SerializeField] protected TMP_Text txtPrice;
    [SerializeField] protected Image imgPrice;

    [SerializeField] protected int feeTicket = 10;
    [SerializeField] protected int feeHcGem = 10;

    protected bool _isUseTicket = true;

    protected virtual bool IsUseTicket
    {
        get { return _isUseTicket; }
        set { _isUseTicket = value; }
    }

    #region BUTTONOPEN

    protected virtual void TakeUserCurrencyAndCheck()
    {
        var userTicket = HCAppController.Instance.userInfo.UserTicket;
        var userToken = HCAppController.Instance.userInfo.UserToken;
        openButton.gameObject.SetActive(true);
        //Sau này bổ sung thằng lớn hơn hoặc bằng ở đây để xét
        if (_isUseTicket)
        {
            SettingButtonPlay(true, userTicket >= feeTicket ? true : false);
        }
        else
        {
            SettingButtonPlay(false, userToken >= feeHcGem ? true : false);
        }
    }

    protected virtual void SettingButtonPlay(bool isUseTicket, bool isEnoughCurrency)
    {
        txtPrice.gameObject.SetActive(true);
        if (isUseTicket)
        {
            txtPrice.text = feeTicket.ToString();
            txtPrice.color = isEnoughCurrency ? Color.white : Color.red;
            BonusGame_Manager.Instance.SetImageReward(imgPrice, wheelRewardType.ticket);
        }
        else
        {
            txtPrice.text = feeHcGem.ToString();
            txtPrice.color = isEnoughCurrency ? Color.white : Color.red;
            BonusGame_Manager.Instance.SetImageReward(imgPrice, wheelRewardType.hc_token);
        }
    }

    #endregion

    #region ErrorNotice

    [Button]
    protected virtual void ShowError(int indexError, BonusGameType type)
    {
        switch (indexError)
        {
            case 5:
                ShowNoticeCanNotOpenByTicket(type);
                break;
            case 6:
                ShowNoticeCanNotOpenByHcGem(type);
                break;
            default:
                break;
        }
    }

    protected virtual void ShowNoticeCanNotOpenByHcGem(BonusGameType type)
    {
        BonusGame_Manager.Instance.ShowOpacity();
        noticeManagement.ShowNoticeNotEnoughGems(() =>
        {
            //Reset Button Show
            openButton.interactable = true;
            IsUseTicket = false;
            TakeUserCurrencyAndCheck();
            RandomBoxSignals.ResetAnimationRandomBoxToIdle.Dispatch();
            BonusGame_Manager.Instance.HideOpacity(NoThingToDo);
        }, type);
    }

    protected virtual void ShowNoticeCanNotOpenByTicket(BonusGameType type)
    {
        BonusGame_Manager.Instance.ShowOpacity();
        noticeManagement.ShowNoticeGuideUseGem(feeHcGem, () =>
        {
            //Reset Button Show
            openButton.interactable = true;
            IsUseTicket = false;
            TakeUserCurrencyAndCheck();
            RandomBoxSignals.ResetAnimationRandomBoxToIdle.Dispatch();
            BonusGame_Manager.Instance.HideOpacity(NoThingToDo);
        }, type);
    }

    protected virtual void NoThingToDo()
    {
    }

    #endregion


    public virtual void ShowView()
    {
        gameObject.SetActive(true);
        bgImage.gameObject.SetActive(false);
    }

    public void CloseView()
    {
        gameObject.SetActive(false);
        bgImage.gameObject.SetActive(true);
    }
}