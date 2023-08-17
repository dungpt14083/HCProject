using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Object = System.Object;

public static class HCHelper
{
    private static Dictionary<GameType, string> SceneGames = new Dictionary<GameType, string>
    {
        { GameType.Solitaire, "" },
        { GameType.Bubble, "" },
        { GameType.Bingo, "" },
        { GameType.Billard, "MenuScene" },
        { GameType.Puzzle, "" },
    };

    public static string GetSceneName(GameType gameType)
    {
        if (!SceneGames.ContainsKey(gameType)) return "";
        return SceneGames[gameType];
    }

    public static string GetDeviceId()
    {
        string deviceId;
        deviceId = SystemInfo.deviceUniqueIdentifier;
        return deviceId;
    }
    public static IEnumerator LoadAvatar(string url, Image avatar)
    {
        Debug.Log("Update avatar " + url);
        if (url == "")
        {
            if (avatar != null) avatar.sprite = null;
            yield return null;
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            
            if (www == null || www.texture == null)
            {
                Debug.Log("www == null || www.texture == null");
                if (avatar != null) avatar.sprite = null;
                yield return null;
            }
            else
            {
                Texture2D profilePic = www.texture;
                avatar.sprite = Sprite.Create(profilePic, new Rect(0, 0, profilePic.width, profilePic.height),Vector2.zero);
            }
        }
    }

    public static void DestroyChil(Transform parent, bool isAll = true, bool isActive = true)
    {
        foreach (Transform item in parent)
        {
            if (!isAll)
            {
                if (item.gameObject.activeSelf == isActive)
                {
                    UnityEngine.GameObject.Destroy(item.gameObject);
                }
            }
            else
            {
                UnityEngine.GameObject.Destroy(item.gameObject);
            }
        }
    }

    public static void NumberIncreasesEff(TMPro.TMP_Text text, long currentValues, long endValues,
        float timeCoinTween = 2.0f, bool isFormat = true)
    {
        if (currentValues == endValues) return;
        //if (text.gameObject.activeSelf)return;
        DOVirtual.Float(currentValues, endValues, timeCoinTween, v => { text.text = ((long)v).ToString(); })
            .OnComplete(() => {
                if (isFormat) text.text = StringUtils.FormatMoneyK(endValues);
                else text.text = endValues.ToString();
            });
    }

    /*
    private static Tween _moneyTween;

    private void UpdatePlayerInformation()
    {
        if (_player == null)
        {
            return;
        }

        long currentCoin = 0;
        if (_player != null)
        {
            userNameTxt.text = _player.displayName;
            if (_player.currency != null && _player.currency.Currencies != null)
            {
                // SDLogger.LogError("TestNull");
                currentCoin = _player.currency.GetCurrencyWithType(CurrencyType.Gold);
            }
        }

        var lastCoin = _lastUserCoin;
        _moneyTween?.Kill();
        _moneyTween = DOVirtual.Float(lastCoin, currentCoin, timeCoinTween, v => { coinTxt.text = ((long) v).FormatMoney(); })
            .OnComplete(() =>
            {
                coinTxt.text = StringUtils.FormatMoneyK(currentCoin);
                _lastUserCoin = currentCoin;
            });

        if (_player != null)
        {
            if (!_player.isPlayer)
            {
                statusText.PlayAnimation(PlayerStatusType.DangXem);
            }
        }
    }
    */
}