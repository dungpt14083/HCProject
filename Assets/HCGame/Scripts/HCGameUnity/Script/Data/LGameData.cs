using NBCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

public class LGameData : Singleton<LGameData>
{
    
    // public ulong PlayerID { get; set; }
    // public ulong PlayerTurnID { get; set; }
    
    public string CachedToken
    {
        get
        {
            return PlayerPrefs.GetString(GameConst.TOKEN_KEY, "");
        }
        set
        {
            PlayerPrefs.SetString(GameConst.TOKEN_KEY, value);
        }
    }

    public string Username
    {
        get; set;
    }

    //Only use once for register new account
    public string TempPassword
    {
        get; set;
    }

    public bool IsCreateNewAcc
    {
        get; set;
    }

    public bool IsSuperAdmin
    {
        get;set;
    }

    public Vector3 UnlockableBlockPosition
    {
        get;set;
    }

    #region Music - Sound
    public Action OnUpdateMusicVolume = null;
    public Action OnUpdateSoundVolume = null;
    public int Music_IsEnable
    {
        get
        {
            if (PlayerPrefs.HasKey("Music_IsMuted"))
            {
                return PlayerPrefs.GetInt("Music_IsMuted");
            }
            else
            {
                PlayerPrefs.SetInt("Music_IsMuted", 1);
                return 1;
            }
        }
        set
        {
            PlayerPrefs.SetInt("Music_IsMuted", value);
            OnUpdateMusicVolume?.Invoke();
        }
    }

    public float Music_Volume
    {
        get
        {
            if (PlayerPrefs.HasKey("Music_Volume"))
            {
                return PlayerPrefs.GetFloat("Music_Volume") * Music_IsEnable;
            }
            else
            {
                PlayerPrefs.SetFloat("Music_Volume", 1.0f);
                return 1.0f * Music_IsEnable;
            }
        }
        set
        {
            PlayerPrefs.SetFloat("Music_Volume", value);
            OnUpdateMusicVolume?.Invoke();
        }
    }

    public int Sound_IsEnable
    {
        get
        {
            if (PlayerPrefs.HasKey("Sound_IsEnable"))
            {
                return PlayerPrefs.GetInt("Sound_IsEnable");
            }
            else
            {
                PlayerPrefs.SetInt("Sound_IsEnable", 1);
                return 1;
            }
        }
        set
        {
            PlayerPrefs.SetInt("Sound_IsEnable", value);
            OnUpdateSoundVolume?.Invoke();
        }
    }

    public float Sound_Volume
    {
        get
        {
            if (PlayerPrefs.HasKey("Sound_Volume"))
            {
                return PlayerPrefs.GetFloat("Sound_Volume") * Sound_IsEnable;
            }
            else
            {
                PlayerPrefs.SetFloat("Sound_Volume", 1.0f);
                return 1.0f * Sound_IsEnable;
            }
        }
        set
        {
            PlayerPrefs.SetFloat("Sound_Volume", value);
            OnUpdateSoundVolume?.Invoke();
        }
    }
    #endregion Music - Sound
}
