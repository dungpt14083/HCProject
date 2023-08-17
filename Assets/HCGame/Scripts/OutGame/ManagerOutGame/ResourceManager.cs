using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMonoAwake<ResourceManager>
{
    public Sprite[] sprMiniGameIcons;
    public Sprite[] sprMoneyIcons;
    public Sprite[] sprRewardIcons;
    public Sprite[] sprRewardLevelUpIcons;
    public Sprite[] sprHomeBonusGameIcons;
    public Sprite[] sprTypeTournamentIcons;
    public Sprite[] sprCountDown;
    public Sprite sprHomeIcon;
    public Sprite sprEventIcon;
    public Sprite avatarDefault;
    public Sprite[] sprRewardMission;
    public Sprite[] sprRewardIconsBiggest;

    public Sprite[] sprRewardIconsSmallBonusGame;


    public List<string> listTypeModeTournament = new List<string>()
    {
        "Head to Head",
        "Knock Out",
        "Round Robin",
        "One To Many",
    };

    public Sprite GetIconGame(GameType _gameType)
    {
        if (sprMiniGameIcons == null || sprMiniGameIcons.Length == 0) return null;
        int index = (int)_gameType;
        if (index > sprMiniGameIcons.Length || index - 1 < 0)
            return null;
        return sprMiniGameIcons[index - 1];
    }

    public Sprite GetIconMoney(MoneyType type)
    {
        if (sprMoneyIcons == null || sprMoneyIcons.Length == 0) return null;
        int index = (int)type;
        if (index > sprMoneyIcons.Length || index - 1 < 0)
            return null;
        return sprMoneyIcons[index - 1];
    }

    public Sprite GetIconReward(RewardType type)
    {
        if (sprRewardIcons == null || sprRewardIcons.Length == 0) return null;
        int index = (int)type;
        if (type == RewardType.X2XP) index = 4;
        if (index > sprRewardIcons.Length || index - 1 < 0)
            return null;
        return sprRewardIcons[index - 1];
    }

    public Sprite GetIconRewardMission(RewardType type)
    {
        if (sprRewardMission == null || sprRewardMission.Length == 0) return null;
        int index = (int)type;
        if (type == RewardType.X2XP) index = 4;
        if (index > sprRewardMission.Length || index - 1 < 0)
            return null;
        return sprRewardMission[index - 1];
    }

    public Sprite GetIconRewardLevelUp(RewardType type)
    {
        if (sprRewardLevelUpIcons == null || sprRewardLevelUpIcons.Length == 0) return null;
        int index = (int)type;
        if (index > sprRewardLevelUpIcons.Length || index - 1 < 0)
            return null;
        return sprRewardLevelUpIcons[index - 1];
    }

    public Sprite GetIconBonusGame(BonusGameType type)
    {
        if (sprHomeBonusGameIcons == null || sprHomeBonusGameIcons.Length == 0) return null;
        int index = (int)type;
        if (index > sprHomeBonusGameIcons.Length || index - 1 < 0)
            return null;
        return sprHomeBonusGameIcons[index - 1];
    }

    public Sprite GetImgCountDown(int index)
    {
        if (sprCountDown == null || sprCountDown.Length == 0) return null;
        if (index > sprCountDown.Length)
            return null;
        return sprCountDown[index];
    }

    public Sprite GetIconTypeTournament(TypeTournament type)
    {
        if (sprTypeTournamentIcons == null || sprTypeTournamentIcons.Length == 0) return null;
        int index = (int)type;
        if (index > sprTypeTournamentIcons.Length || index - 1 < 0)
            return null;
        return sprTypeTournamentIcons[index - 1];
    }

    public Sprite GetIconGameMode(GameModeType modeType)
    {
        if (sprTypeTournamentIcons == null || sprTypeTournamentIcons.Length == 0) return null;
        int index = (int)modeType;
        if (index > sprTypeTournamentIcons.Length || index - 1 < 0)
            return null;
        return sprTypeTournamentIcons[index - 1];
    }

    public Sprite GetAvatarDefault()
    {
        return avatarDefault;
    }

    public string GetNameTypeModeTournament(int typeMode)
    {
        return typeMode > listTypeModeTournament.Count || typeMode < 0
            ? listTypeModeTournament[0]
            : listTypeModeTournament[typeMode - 1];
    }

    public Sprite GetIconFeeMoney(MoneyType type)
    {
        if (sprRewardMission == null || sprRewardMission.Length == 0) return null;
        int index = (int)type;
        if (index > sprRewardMission.Length || index - 1 < 0)
            return null;
        return sprRewardMission[index - 1];
    }

    public string GetNameGame(GameType gameType)
    {
        string name = "";
        switch (gameType)
        {
            case GameType.Billard:
                name = "8 Ball";
                break;
            case GameType.Bingo:
                name = "Bingo";
                break;
            case GameType.Bubble:
                name = "Bubble";
                break;
            case GameType.Puzzle:
                name = "Puzzle";
                break;
            case GameType.Solitaire:
                name = "Solitaire";
                break;
        }

        return name;
    }

    public Sprite GetIconRewardBiggest(RewardType type)
    {
        if (sprRewardIconsBiggest == null || sprRewardIconsBiggest.Length == 0) return null;
        int index = (int)type;
        if (type == RewardType.X2XP) index = 4;
        if (index > sprRewardIconsBiggest.Length || index - 1 < 0)
            return null;
        return sprRewardIconsBiggest[index - 1];
    }

    public Sprite GetIconRewardBonusGameSmall(RewardType type)
    {
        if (sprRewardIconsSmallBonusGame == null || sprRewardIconsSmallBonusGame.Length == 0) return null;
        int index = (int)type;
        if (index > sprRewardIconsSmallBonusGame.Length || index - 1 < 0)
            return null;
        return sprRewardIconsSmallBonusGame[index - 1];
    }
    
    
}

public enum TypeTournament
{
    None,
    HeadToHead,
    KnockOut,
    RoundRobin,
    OneToMany
}