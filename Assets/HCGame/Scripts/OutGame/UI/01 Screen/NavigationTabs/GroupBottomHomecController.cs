using System;
using System.Collections;
using System.Collections.Generic;
using BonusGame;
using UnityEngine;

public class GroupBottomHomecController : MonoBehaviour
{
    [SerializeField] private List<UIButtonHome> listButton;
    private TypeButtonHome _currentTypeButton = TypeButtonHome.None;

    public Action<TypeButtonHome> OnClickNavigationInvoke;

    protected void Awake()
    {
        //base.Awake();
        InitButton();
    }

    private void InitButton()
    {
        for (int i = 0; i < listButton.Count; i++)
        {
            listButton[i].OnShow(OnChangeFilterSelect);
        }
    }

    public void OnChangeFilterSelect(TypeButtonHome info)
    {
        if (ScreenManagerHC.Instance.IsOnScreen<BonusGame_Manager>() && (BonusGame_Manager.Instance.gameObject.activeSelf &&
                                                                         BonusGame_Manager.Instance.IsRunning)) return;

        SelectBottomIconAndShowScreen(info);
    }

    public void SelectBottomIconAndShowScreen(TypeButtonHome info)
    {
        SelectBottomIcon(info);
        ShowScreenHome(info);
    }

    public void SelectBottomIcon(TypeButtonHome info)
    {
        foreach (var item in listButton)
        {
            item.OnSelectFilter(info);
        }
    }

    private void ShowScreenHome(TypeButtonHome info)
    {
        OnClickNavigationInvoke?.Invoke(info);
        _currentTypeButton = info;
    }


    public void SettingImageForButtonEvent(Sprite spriteEvent, string lable)
    {
        var tmp = listButton.Find(s => s.Info == TypeButtonHome.Home);
        if (tmp != null)
        {
            tmp.ChangeEventIcon(spriteEvent);
            tmp.ChangeEventLable(lable);
        }
    }
}