using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : SingletonMonoAwake<AudioManager>
{
    [Header("Main Settings:")]
    [Range(0, 1)]
    public float musicVolume;

    [Range(0, 1)]
    public float sfxVolume;
    public AudioSource musicAus;
    public AudioSource sfxAus;
    
    

    [Header("Game Sounds And Musics:")]
    public AudioClip[] bgmusics;

    [Header("Game Sounds solitarie:")]
    public AudioClip anTien;
    public AudioClip soundUi;
    public AudioClip soundEndTime;
    public AudioClip gameOver;
    public AudioClip succesandTimeUp;
    public AudioClip xaoBai;
    public AudioClip xepBai1;
    public AudioClip xepBai2;
    
    [Header("Settings: sound")]
    public bool muted = false;
    public bool mutedMS = false;
    
      public void Start()
    {
        if (!PlayerPrefs.HasKey("mutedMS"))
        {
            PlayerPrefs.SetInt("mutedMS", 0);
            LoadSettingMS();
        }
        else
        {
            LoadSettingMS();
        }
        //2cai
        if (!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            LoadSetting();
        }
        else
        {
            LoadSetting();
        }
        sfxAus.mute = muted;
        musicAus.mute = mutedMS;
    }
   
    public void PlaySound(AudioClip sound, AudioSource aus = null)
    {
        if (!aus)
        {
            aus = sfxAus;
        }
        if (aus)
        {
            aus.PlayOneShot(sound, sfxVolume);
            // Handheld.Vibrate();
        }
    }

    public void PlaySound(AudioClip[] sounds, AudioSource aus = null)
    {
        if (!aus)
        {
            aus = sfxAus;
        }
        if (aus)
        {
            int ranIdx = Random.Range(0, sounds.Length);
            if (sounds[ranIdx] != null)
            {
                aus.PlayOneShot(sounds[ranIdx], sfxVolume);
            }
        }
    }

    public void PlayMusic(AudioClip music, bool loop = true)
    {
        if (musicAus)
        {
            musicAus.clip = music;
            musicAus.loop = loop;
            musicAus.volume = musicVolume;
            musicAus.Play();
        }
    }

    public void PlayMusic(AudioClip[] musics, bool loop = true)
    {
        if (musicAus)
        {
            int ranIdx = Random.Range(0, musics.Length);
            if (musics[ranIdx] != null)
            {
                musicAus.clip = musics[ranIdx];
                musicAus.loop = loop;
                musicAus.volume = musicVolume;
                musicAus.Play();
            }
        }
    }
    public void PlayBG()
    {
        PlayMusic(bgmusics);
    }

    public void StopMusic()
    {
        if (mutedMS == false)
        {
            mutedMS = true;

            musicAus.mute = !musicAus.mute;
        }
        else
        {
            mutedMS = false;
            musicAus.mute = !musicAus.mute;
        }
        saveSettingMS();
    }

    public void StopSound()
    {
        if (muted == false)
        {
            muted = true;
            //   musicAus.volume = 0f;
            sfxAus.mute = !sfxAus.mute;
        }
        else
        {
            muted = false;
            //  musicAus.volume = 100f;
            sfxAus.mute = !sfxAus.mute;
        }
        saveSetting();
    }

    private void LoadSetting()
    {
        muted = PlayerPrefs.GetInt("muted") == 1;
    }

    private void saveSetting()
    {
        PlayerPrefs.SetInt("muted", muted ? 1 : 0);
    }

    ///mute2
    private void LoadSettingMS()
    {
        mutedMS = PlayerPrefs.GetInt("mutedMS") == 1;
    }

    private void saveSettingMS()
    {
        PlayerPrefs.SetInt("mutedMS", mutedMS ? 1 : 0);
    }
    
}
