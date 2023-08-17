using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenData", menuName = "HCGame/Data/OutGame/ScreenData", order = 0)]
public class ScreenData : ScriptableObject
{
    public UserDataUI userDataUI;
    public GroupBottomHomecController groupBottomHomeController;

    public List<ScreenInfo> screenInfos = new List<ScreenInfo>();
}

[Serializable]
public class ScreenInfo
{
    public UIView uiView;
}