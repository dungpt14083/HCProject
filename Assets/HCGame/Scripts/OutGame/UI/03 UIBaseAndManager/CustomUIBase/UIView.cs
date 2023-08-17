using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIView : UIBase
{
    public static readonly Dictionary<Type, UIView> Screens = new Dictionary<Type, UIView>();
    public static UIView CurrentScreen;
    public TransitionType transitionType;

    public override void OnOpen()
    {
        gameObject.Show();
        transform.SetAsLastSibling();
        base.OnOpen();
        CurrentScreen = this;
    }

    public override void OnClose(Action onComplete = null)
    {
        animation.OnStop();
        animation.OnReverse().SetReverseCompletedCallBack(() =>
        {
            if (gameObject.activeSelf)
            {
                gameObject.Hide();
            }

            onComplete?.Invoke();
        });
    }

    public void OpenFromInstance()
    {
        if (CurrentScreen == this)
        {
            return;
        }
        else
        {
            if (CurrentScreen)
            {
                CurrentScreen.OnClose(OpenView);
            }
            else
            {
                OpenView();
            }
        }
    }

    public virtual void BackScreen()
    {
        //ScreenManager.Instance.BackScreen();
    }
}

public class UIView<T> : UIView where T : UIView
{
    public static T Instance
    {
        get
        {
            if (Screens.ContainsKey(typeof(T)) && Screens[typeof(T)] != null)
            {
                var ins = Screens[typeof(T)];
                return ins as T;
            }
            else
            {
                var s = ScreenManagerHC.Instance.GetView<T>();

                if (Screens.ContainsKey(typeof(T)) && Screens[typeof(T)] == null)
                {
                    Screens.Remove(typeof(T));
                }

                Screens.Add(typeof(T), s);
                return s;
                return null;
            }
        }
    }


    private static readonly Func<bool> _IsOpened = () => ScreenManagerHC.Instance.IsOnScreen<T>();

    private static readonly List<TransitionType> _TransitionTypes = new List<TransitionType>
    {
        TransitionType.Wind,
        TransitionType.CurveWind,
        TransitionType.FishEye
    };

    public static void OpenScreen(bool isUseTransition = true)
    {
        Debug.LogError("UIView OpenScreen: " + typeof(T).ToString());
        if (isUseTransition)
        {
            TransitionFactorys.RunTransition(Instance.transitionType, OpenView, _IsOpened);
        }
        else
        {
            OpenView();
        }
    }

    private static void OpenView()
    {
        if (CurrentScreen is T)
        {
            Instance.gameObject.SetActive(true);
        }
        else
        {
            if (CurrentScreen)
            {
                CurrentScreen.OnClose(() => { Instance.OpenView(); });
            }
            else
            {
                Instance.OpenView();
            }
        }
    }

    public static void OpenViewWithCallBack(Action<T> onComplete = null)
    {
        _OpenViewWithCallBack(onComplete);
    }

    private static void _OpenViewWithCallBack(Action<T> onComplete = null)
    {
        UIView insTmp = null;
        if (Screens.ContainsKey(typeof(T)) && Screens[typeof(T)] != null)
        {
            insTmp = Screens[typeof(T)];
        }
        else
        {
            if (Screens.ContainsKey(typeof(T)) && Screens[typeof(T)] == null)
            {
                Screens.Remove(typeof(T));
            }

            insTmp = Instance;
        }

        if (insTmp != null)
        {
            OpenScreenWithInstance(insTmp);
            onComplete?.Invoke(insTmp.Cast<T>());
        }
    }

    private static void OpenScreenWithInstance(UIView ins)
    {
        if (CurrentScreen is T)
        {
            Instance.OpenView();
        }
        else
        {
            if (CurrentScreen)
            {
                CurrentScreen.OnClose();
            }

            ins.OpenView();
            //CurrentScreen = ins;
        }
    }
}