using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDefine
{
    public enum Farm_SEVT
    {
        OnMainCharLoad,
        TurnOffCharMove,
        TurnOffCameraControl
    }

    public enum UI_Login
    {
        OnRegisterSuccess,
    }

    public enum UI_Dialogue
    {
        OnUpdateNameSucess,
        OnShowHighlight_HUD,
        OnShowHighlight_Minigame_Main,
        OnShowHighlight_Minigame_Lobby,
        OnShowHighlight_Minigame_Start,
        OnShowHighlight_Gacha_Main,
        OnShowHighlight_Gacha_Micoin,
        OnShowHighlight_Gacha_Roll,
        OnHighlightClicked,
    }
}
