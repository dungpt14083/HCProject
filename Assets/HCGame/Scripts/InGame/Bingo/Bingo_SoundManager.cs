using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bingo
{
    public class Bingo_SoundManager : MonoBehaviour
    {

        public static Bingo_SoundManager instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }
        public AudioSource _soundEff, _soundMusic,_soundEff_Number;
        public AudioClip loseScore, moreScore, tapButton;

        private void Start()
        {
            startSettingSound();
        }

        #region MuteSoundAndMusic
        [Header("Settings: sound")]
        private bool muted = false;
        private bool mutedMS = false;

        private void startSettingSound()
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
            _soundEff.mute = muted;
            _soundMusic.mute = mutedMS;
        }
        public void StopMusic()
        {
            if (mutedMS == false)
            {
                mutedMS = true;

                _soundMusic.mute = !_soundMusic.mute;
            }
            else
            {
                mutedMS = false;
                _soundMusic.mute = !_soundMusic.mute;
            }
            saveSettingMS();
        }

        public void StopSound()
        {
            if (muted == false)
            {
                muted = true;
                //   musicAus.volume = 0f;
                _soundEff.mute = !_soundEff.mute;
            }
            else
            {
                muted = false;
                //  musicAus.volume = 100f;
                _soundEff.mute = !_soundEff.mute;
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
        

        #endregion


        [Button]

        public void PlaySound_EffLoseScore()
        {
            _soundEff.clip = loseScore;
            _soundEff.Play();
        }
        [Button]

        public void PlaySound_EffMoreScore()
        {
            _soundEff.clip = moreScore;
            _soundEff.Play();
        }


        public string pathMusicFolder = "BingoSound";
        [Button]
        public void PlaySound_NewTarget(string targetName)
        {
            var finalPath = pathMusicFolder + "/" + "sound eff " + targetName;


            var soundClip = Resources.Load(finalPath) as AudioClip;
            _soundEff_Number.clip = soundClip;
            _soundEff_Number.Play();
        }



        [Button]
        public void PlaySound_TapButton()
        {

            _soundEff.clip = tapButton;
            _soundEff.Play();
        }

        public AudioClip ting;
        public void PlaySound_Ting()
        {

            _soundEff.clip = ting;
            _soundEff.Play();
        }
        public AudioClip go;
        public void PlaySound_Go()
        {

            _soundEff.clip = go;
            _soundEff.Play();
        }
}
}