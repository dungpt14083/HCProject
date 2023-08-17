using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BonusGame;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HcPopupManager : SingletonMonoAwake<HcPopupManager>
{
    private static PopupData _PopupData;

    public static PopupData PopupData
    {
        get
        {
            if (!_PopupData)
            {
                _PopupData = Resources.Load<PopupData>("DataUI/PopupPriorityData");
            }

            return _PopupData;
        }
    }

    [SerializeField] private Transform holderPopup;

    private static Coroutine popupCt;


    private Dictionary<string, UIPopup> Prefabs = new Dictionary<string, UIPopup>();
    private List<UIPopup> _cachePopups = new List<UIPopup>();

    //ĐÓNG TẤT CẢ POPUP ĐANG MỞ
    public static void CloseAllPopup()
    {
        foreach (var popup in Instance._cachePopups)
        {
            popup.CloseView();
        }
    }

    //Đóng popup theo tên
    public static void ClosePopup(string popupName)
    {
        var p = Instance._cachePopups.Find(s => s.ViewId == popupName);
        if (p)
        {
            if (p.GetGameObject().activeInHierarchy)
            {
                p.CloseView();
            }
        }
    }

    public static void ClosePopup<T>() where T : UIBase
    {
        ClosePopup(typeof(T).Name);
    }

    //kểm tra 1 popup đang sống trên hierarchy hay không
    public static bool IsActiveInHierarchy(string popupName)
    {
        var p = Instance._cachePopups.Find(s => s.ViewId == popupName);
        if (p)
        {
            if (p.GetGameObject().activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    public static UIPopup GetPopup(string popupName)
    {
        return Instance._cachePopups.Find(s => s.ViewId == popupName);
    }

    public static bool IsHasPopupOpenNow()
    {
        return Instance._cachePopups.Any(s => s.isOnPopupOpen);
    }

    public static void OpenPopup<T>(Action<T> onLoadComplete = null, bool isWithoutParent = false,
        Transform parent = null) where T : UIBase
    {
        if (popupCt != null)
        {
            Executors.Instance.StopCoroutine(popupCt);
        }

        popupCt = Executors.Instance.StartCoroutine(IOpenPopup(typeof(T).Name, OnLoadSuccess, isWithoutParent, parent));

        void OnLoadSuccess(UIBase view)
        {
            onLoadComplete?.Invoke(view.Cast<T>());
        }
    }


    public static IEnumerator IOpenPopup(string popupName, Action<UIBase> onLoadComplete = null,
        bool isWithoutParent = false, Transform parent = null)
    {
        var priority = PopupData.GetPriority(popupName);

        if (!Instance.Prefabs.ContainsKey(popupName))
        {
            foreach (var p in Instance._cachePopups)
            {
                if (p != null)
                {
                    if (p.GetGameObject().activeInHierarchy)
                    {
                        if (p.Priority <= priority)
                        {
                            p.CloseView();
                        }
                    }
                }
                else
                {
                    Instance._cachePopups.Remove(p);
                    Destroy(p);
                }
            }

            var v = PopupData.popups.Find(s => s.namePopup == popupName);
            Instance.Prefabs.Add(popupName, v.popup);
            var m = BonusPool.Spawn(Instance.Prefabs[popupName],
                isWithoutParent ? null : !parent ? Instance.holderPopup : parent);
            m.ViewId = popupName;
            m.Priority = priority;
            Instance._cachePopups.Add(m);
            m.OpenView();
            onLoadComplete?.Invoke(m);
        }
        else
        {
            UIBase cache = null;
            var isLoadYet = false;
            foreach (var view in Instance._cachePopups)
            {
                if (view != null)
                {
                    if (view.ViewId == popupName)
                    {
                        isLoadYet = true;
                    }

                    if (view.GetGameObject().activeSelf)
                    {
                        if (view.ViewId == popupName)
                        {
                            cache = view;
                        }
                        else
                        {
                            if (view.Priority <= priority)
                            {
                                view.CloseView();
                            }
                        }
                    }
                }
                else
                {
                    Instance._cachePopups.Remove(view);
                    Destroy(view);
                }
            }

            if (cache)
            {
                onLoadComplete?.Invoke(cache);
                yield break;
            }

            var m = BonusPool.Spawn(Instance.Prefabs[popupName],
                isWithoutParent ? null : (!parent ? Instance.holderPopup : parent));
            m.ViewId = popupName;
            m.OpenView();
            if (!isLoadYet)
            {
                Instance._cachePopups.Add(m);
            }

            onLoadComplete?.Invoke(m);
        }
    }


    /// <summary>
    /// ///
    /// </summary>

    #region TODOSHOWTEMPRORYPOPUP

    //SHOWPOPUP TĂNG LEVEL
    public void ShowLevelup()
    {
        if (HCAppController.Instance.listLevelUp != null && HCAppController.Instance.listLevelUp.Count > 0)
        {
            LevelUpPopup.OpenPopup(controller => controller.Show());
        }
    }

    public void ShowLogoutPopup(string content = "")
    {
        LogoutPopup.OpenPopup(popup => popup.Show(content));
    }

    public void ShowDeleteAccountPopup(string content = "")
    {
        DeleteAccountPopup.OpenPopup(popup => popup.Show(content));
    }

    public void ShowNotifyPopup(string content, string title = "Notify")
    {
#if !Test
        return;
#endif
        NotifyPopup.OpenPopup(popup => popup.Show(content, title));
    }

    public void ShowEditYourProfile()
    {
        StartCoroutine(WaitHaveData());
    }

    private IEnumerator WaitHaveData()
    {
        yield return new WaitUntil(() => HCAppController.Instance.userInfo != null);
        EditYourProfilePopup.OpenPopup(popup => popup.Show(HCAppController.Instance.userInfo.UserName,
            HCAppController.Instance.userInfo.UserAvatar));
    }


    public void DisplayDailyReward()
    {
        DailyReward.OpenPopup(reward => reward.ShowViewFirst());
    }


    public void DisplayDailyMission()
    {
        DailyMission.OpenPopup();
    }

    public void UpdateDailyMission()
    {
        if (SceneManager.GetActiveScene().name == "Home")
        {
            UISignals.OnUpdateDailyMission.Dispatch();
        }
    }


    public void DisplayJackPot(TopJackPotData data)
    {
        JackPot.OpenPopup(pot => pot.ShowView(data));
    }


    public void DisplayNotification()
    {
        Notification.OpenPopup(notification => notification.Show(HCAppController.Instance.listHcNotification));
    }

    public void UpdateNotification()
    {
        UISignals.OnUpdateNotification.Dispatch();
    }

    [Button]
    public void ShowCouponPopup()
    {
        CouponPopup.OpenPopup(popup => popup.Show());
    }

    public void ShowReferraPopup(Reward data)
    {
        ReferraPopup.OpenPopup(popup => popup.Show(data));
    }

    public void ShowInviteFrendPoup()
    {
        Invite.OpenPopup(popup=>popup.Show());
    }


    public void ShowBigJackPot(int value, Action firstCallback, Action callback, bool isAutoSpin)
    {
        BonusGame_Popup.OpenPopup(popup => popup.ShowBigJackPot(value, firstCallback, callback, isAutoSpin));
    }

    public void ShowRewardWheelAndScratch(wheelRewardType rewardType, int value, Action firstCallback,
        Action callback, bool isAutoSpin)
    {
        BonusGame_Popup.OpenPopup(popup =>
            popup.ShowRewardWheelAndScratch(rewardType, value, firstCallback, callback, isAutoSpin));
    }

    public void ShowMsgRandomBox(WheelItemData[] data, Action callback)
    {
        BonusGame_Popup.OpenPopup(popup => popup.ShowMsgRandomBox(data, callback));
    }

    #endregion


    #region Tournament

    public void ShowGameResultEndUser(EndUserWebsocketProto endUser)
    {
        GameResultPopup.OpenPopup(popup => popup.ShowViewWithEndUserData(endUser));
    }

    #endregion

    #region GAMEOVER

    #region TODOGAMEOVERANDSHOWRESULT

    public void ShowGameOverEndRoom(EndRoomWebsocketProto endRoom)
    {
        if (GameResultPopup.InstanceResult != null && GameResultPopup.InstanceResult.isShowFrameResult)
        {
            ShowGameResultEndRoom(endRoom);
        }
        else
        {
            GameOverProcessPopup.OpenPopup(popup => popup.ShowViewEndRoom(endRoom));
        }
    }

    public void ShowGameOverEndUser(EndUserWebsocketProto endUser)
    {
        GameOverProcessPopup.OpenPopup(popup => popup.ShowViewEndUser(endUser));
    }


    public void ShowGameResultEndRoom(EndRoomWebsocketProto endRoom)
    {
        if (GameResultPopup.InstanceResult != null && GameResultPopup.InstanceResult.gameObject.activeSelf)
        {
            GameResultPopup.InstanceResult.ShowViewWithEndRoomData(endRoom);
        }
        else
        {
            GameResultPopup.OpenPopup(popup => popup.ShowViewWithEndRoomData(endRoom));
        }
    }

    #endregion

    #endregion

    #region loadingmatchingforMachingANDNOTIFY

    public EightLoadingPanelController eightBallLoadPanelCtrl;

    public void ShowEightGameLoading(bool isShow, GameType gameType = GameType.None)
    {
        eightBallLoadPanelCtrl.gameObject.SetActive(isShow);
        if (isShow)
        {
            eightBallLoadPanelCtrl.LoadScene(gameType);
        }
    }

    public void ShowNotifyPopup(string msg)
    {
        NotifyPopup.OpenPopup(popup => popup.Show(msg));
    }

    #endregion

    public void ShowTestLoginPopup()
    {
        TestLoginPopup.OpenPopup(popup => popup.Show());
    }
}

public class ListRewardAndPosition
{
    public Vector3 position;
    public Reward reward;
}