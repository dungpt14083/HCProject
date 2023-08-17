using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;


public class GameConst
{
    public const int ROI_BASE = 15;
    public const string TOKEN_KEY = "@#TOKEN";
}


public class HCGamePath
{

}


//HUB
public enum HUB_POOLER
{
    HubConfirm,
}


#region UI
public static class UIConstant
{
    public const string UI_SHOWANNOUNCEMENT = "ShowAnnouncement";
}

public static class IconPath
{
    public const string UI_Icon_Dialogue = "Icons/Dialogue/";
}
#endregion UI



public enum ESortType
{
    Recent_Up = 0,
    Recent_Down,
    Rarity_Up,
    Rarity_Down,
}
