using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCSettingGame : Singleton<HCSettingGame>
{
    private const string SETTING_MUSIC = "Setting_Music";
    private const string SETTING_SOUND = "Setting_Sound";
    private const string SETTING_VIBRATE = "Setting_Sound";

    public bool isMusic
    {
        get
        {
            return Convert.ToBoolean(LoadMusic());
        }
        set
        {
            SaveMusic(Convert.ToInt32(value));
        }
    }

    public bool isSound
    {
        get
        {
            return Convert.ToBoolean(LoadSound());
        }
        set
        {
            SaveSound(Convert.ToInt32(value));
        }
    }

    public bool isVibrate
    {
        get
        {
            return Convert.ToBoolean(LoadVibrate());
        }
        set
        {
            SaveVibrate(Convert.ToInt32(value));
        }
    }

    #region SAVE LOAD VOLUME

    private int LoadMusic()
    {
        return PlayerPrefs.GetInt(SETTING_MUSIC, 1);
    }

    private int LoadSound()
    {
        return PlayerPrefs.GetInt(SETTING_SOUND, 1);
    }

    private void SaveMusic(int isMusic)
    {
        PlayerPrefs.SetInt(SETTING_MUSIC, isMusic);
    }

    private void SaveSound(int isSound)
    {
        PlayerPrefs.SetInt(SETTING_SOUND, isSound);
    }

    #endregion

    #region SAVE LOAD VIBRATE

    private int LoadVibrate()
    {
        return PlayerPrefs.GetInt(SETTING_VIBRATE, 1);
    }

    private void SaveVibrate(int isVibrate)
    {
        PlayerPrefs.SetInt(SETTING_VIBRATE, isVibrate);
    }

    #endregion
}
