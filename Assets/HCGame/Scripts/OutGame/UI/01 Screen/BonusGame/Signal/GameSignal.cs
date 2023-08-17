using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public static class GameSignals
{
    public static readonly Signal<bool> ClaimRewardDone = new Signal<bool>();
    public static readonly Signal CloseAllPopup = new Signal();
}

public static class ScratchSignals
{
    public static readonly Signal InitScratch = new Signal();
    public static readonly Signal<bool> ShowHideResult = new Signal<bool>();
    public static readonly Signal<bool> ShowHideScratch = new Signal<bool>();
}

public static class RandomBoxSignals
{
    public static readonly Signal InitRandomBox = new Signal();
    public static readonly Signal ResetAnimationRandomBoxToIdle = new Signal();
}

public static class WheelSignals
{
    public static readonly Signal InitWheel = new Signal();
}

public static class RefreshTopUserSignals
{
    public static readonly Signal RefreshTop = new Signal();
}

public static class BonusGameSignals
{
    public static readonly Signal ClosePopupReward = new Signal();
}

public static class TournamentDataSignals
{
    public static readonly Signal<ListUserTournament> ReceivedTournamentRankData = new Signal<ListUserTournament>();
}

public static class UpdateCurrencySignals
{
    public static readonly Signal<UpdateMoney, UpdateMoney> UpdateMoney = new Signal<UpdateMoney, UpdateMoney>();
    public static readonly Signal<UpdateMoney> UpdateMoneyToUserInfo = new Signal<UpdateMoney>();
}

public static class OnEnableEventPageSignal
{
    public static readonly Signal OnEnableEventContent = new Signal();
    public static readonly Signal OnMoveSpecialEvent = new Signal();
}

public static class UISignals
{
    #region POPUP

    public static readonly Signal OnUpdateDailyMission = new Signal();
    public static readonly Signal OnUpdateNotification = new Signal();
    public static readonly Signal OnUpdateJackPot = new Signal();
    public static readonly Signal<int> OnUpdateDailyReward = new Signal<int>();
    
    public static readonly Signal LoginSuccess = new Signal();

    #endregion

    #region SCREEN

    public static readonly Signal<MoneyType> OnUpdateRanking = new Signal<MoneyType>();
    
    public static readonly Signal OnUpdateListGameInHome = new Signal();
    public static readonly Signal WeeklyJackPotRefresh = new Signal();



    #endregion
}

public static class DataSocketSignals
{
    public static readonly Signal<MoneyType> OnUpdate = new Signal<MoneyType>();

}
