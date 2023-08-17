using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupPriorityData", menuName = "HCGame/Data/OutGame", order = 0)]
public class PopupData : ScriptableObject
{
    public const int DefaultPriority = 10;
    public int defaultPriority = 10;
    public List<PopupInfo> popups = new List<PopupInfo>();

    public int GetPriority(string viewId)
    {
        foreach (var layer in popups.Where(layer => layer.namePopup == viewId))
        {
            return layer.priority;
        }

        return DefaultPriority;
    }
    
}

[Serializable]
public class PopupInfo
{
    public string namePopup;
    public int priority = 10;
    public UIPopup popup;
}