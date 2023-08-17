using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Toast : ManualSingletonMono<Toast>
{
    [SerializeField] private ToastItem prefabItem;
    [SerializeField] private RectTransform contentHolder;

    [Button]
    public static void Show(string str, float timeShow = 2f, float showPosY = 0)
    {
        var item = BonusPool.Spawn(Instance.prefabItem, Instance.contentHolder);
        item.SetTimeShow(timeShow).SetStartPosY(showPosY).SetText(str).ShowToast();
    }

    public static void Show(string str, bool isClearOther, float timeShow = 2f, float showPosY = 0)
    {
        if (isClearOther)
        {
            ToastItem.RemoveOtherToast();
        }

        var item = BonusPool.Spawn(Instance.prefabItem, Instance.contentHolder);
        item.SetTimeShow(timeShow).SetStartPosY(showPosY).SetText(str).ShowToast();
    }

    public static void Close()
    {
        ToastItem.RemoveOtherToast();
    }
}