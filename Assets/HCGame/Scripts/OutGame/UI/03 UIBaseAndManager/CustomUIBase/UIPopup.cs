using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : UIBase
{
    public int priority = 10; //global::PopupData.DefaultPriority;
    public bool isOnPopupOpen;

    protected static readonly Dictionary<Type, UIPopup> Popups = new Dictionary<Type, UIPopup>();

    public override void OnOpen()
    {
        gameObject.Show();
        transform.SetAsLastSibling();
        base.OnOpen();
        isOnPopupOpen = true;
    }

    public override void OnClose(Action onComplete = null)
    {
        isOnPopupOpen = false;
        animation.OnReverse().SetReverseCompletedCallBack(() =>
        {
            gameObject.Hide();
            onComplete?.Invoke();
        });
    }

    public static void CloseAllPopup()
    {
        foreach (var popup in Popups)
        {
            popup.Value.CloseView();
        }
    }
}

public class UIPopupView<T> : UIPopup where T : UIPopup
{
    public static bool IsOnPopupOpen
    {
        get
        {
            var v = GetView();
            return v != null && v.isOnPopupOpen;
        }
    }

    public static void OpenPopup(Action<T> onLoadComplete = null)
    {
        HcPopupManager.OpenPopup<T>(onLoadComplete);
    }

    public static void ClosePopup()
    {
        HcPopupManager.ClosePopup<T>();
    }

    public static T GetView()
    {
        return HcPopupManager.GetPopup(typeof(T).Name) as T;
    }
}