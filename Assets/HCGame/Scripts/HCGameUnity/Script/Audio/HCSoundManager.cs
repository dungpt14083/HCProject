using Hellmade.Sound;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class HCSoundManager
{
    private const string SOUND_RESOURCES_PATH = "Sounds/";

    public float maxVolumeMusic = 1f;
    public float maxVolumeSound = 1f;

    private float environmentSoundInterval = 20.0f;
    private List<MusicName> _backgroundMusicList = new List<MusicName>() {
    MusicName.SunsetValley,
    MusicName.AdventureAdrift,
    };
    private bool _canPlayBackgroundMusic = false;
    public bool CanPlayBackgroundMusic
    {
        get
        {
            return _canPlayBackgroundMusic;
        }
        set
        {
            _canPlayBackgroundMusic = value;
            if(false == _canPlayBackgroundMusic)
            {
                StopMusicByName(MusicName.SunsetValley);
                StopMusicByName(MusicName.AdventureAdrift);
            }
            else
            {
                _lastPlayMusicBackground = MusicName.AdventureAdrift;
            }
        }
    }

    private MusicName _lastPlayMusicBackground = MusicName.AdventureAdrift;

    private List<SoundName> environmentSoundList = new List<SoundName>() {
        SoundName.Ambience_Water_Flowing_Fast_02_Loop,
        SoundName.Ambience_Wind_Intensity_Soft_With_Leaves_Loop,
    };
    private float lastPlayedEnvironmentSound = 0;

    private Dictionary<string, AudioClip> dicAudios = new Dictionary<string, AudioClip>();

    private Audio curmIDPlayer;

    private Dictionary<MusicName, int> _currentMusicPlayingDict = new Dictionary<MusicName, int>();

    public async UniTask LoadAllSound()
    {
        //Pre loading all sound to ease loading waiting time during game.
        //First play sound not play so play sound at lowest volume possible
        for(int i = 1; i < (int)SoundName.Last; i++)
        {
            SoundName soundName = (SoundName)i;
            await PlaySound(soundName, false, null, 0.001f);
        }

        for (int i = 1; i < (int)MusicName.Last; i++)
        {
            MusicName musicName = (MusicName)i;
            await PlayMusic(musicName,0.001f, false, 0.5f);
        }

        LGameData.Instance.OnUpdateMusicVolume += UpdateAllMusicVolume;
        LGameData.Instance.OnUpdateSoundVolume += UpdateAllSoundVolume;

        UpdateAllMusicVolume();
        UpdateAllSoundVolume();

        PlayBackgroundMusic();
        PlayEnviromentSound();
    }

    #region PLAY MUSIC

    private async UniTask UpdateCurrentMusicPlayingDic()
    {
        while(true)
        {
            if(null == _currentMusicPlayingDict || _currentMusicPlayingDict.Count == 0)
            {
                await UniTask.WaitForFixedUpdate();
                continue;
            }

            foreach(KeyValuePair<MusicName, int> musicData in _currentMusicPlayingDict)
            {
                Audio currentAudio = EazySoundManager.GetMusicAudio(musicData.Value);
                if(currentAudio.Paused)
                {
                    continue;
                }
                else if(currentAudio.Stopping || false == currentAudio.IsPlaying)
                {
                    _currentMusicPlayingDict.Remove(musicData.Key);
                    break;
                }
            }
        }
    }

    private bool IsMusicPlaying()
    {
        if (null == _currentMusicPlayingDict || _currentMusicPlayingDict.Count == 0)
        {
            return false;
        }
        foreach (KeyValuePair<MusicName, int> musicData in _currentMusicPlayingDict)
        {
            Audio currentAudio = EazySoundManager.GetMusicAudio(musicData.Value);
            if(currentAudio.IsPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public async UniTask<int> PlayMusic(MusicName musicName, float volume = default, bool isLoop = false, float stopAfterSecs = 0.0f)
    {
        // return 1;
        if (volume == default) volume = maxVolumeMusic;

        var clip = await LoadAudio(musicName.ToString());
        if (clip == null) return -1;

        int id = EazySoundManager.PlayMusic(clip, volume, isLoop, false, 1f, 1f);
        curmIDPlayer = EazySoundManager.GetSoundAudio(id);

        if (!HCSettingGame.Instance.isMusic)// check setting music
        {
            PauseAllMusic();
        }

        if(stopAfterSecs > 0)
        {
            DOVirtual.DelayedCall(stopAfterSecs, ()=> 
            {
                StopMusicByID(id);
                _currentMusicPlayingDict.Remove(musicName);
            }, false);
        }

        if (_currentMusicPlayingDict.ContainsKey(musicName))
        {
            _currentMusicPlayingDict[musicName] = id;
        }
        else
        {
            _currentMusicPlayingDict.Add(musicName, id);
        }

        return id;
    }

    public void StopAllMusic()
    {
        EazySoundManager.StopAllMusic();
    }

    public void PauseAllMusic()
    {
        EazySoundManager.PauseAllMusic();

    }

    public void ResumeAllMusic()
    {
        EazySoundManager.ResumeAllMusic();
    }

    public void StopMusicByName(MusicName musicName)
    {
        if(false == _currentMusicPlayingDict.ContainsKey(musicName))
        {
            return;
        }

        int idAudio = _currentMusicPlayingDict[musicName];
        StopMusicByID(idAudio);
        _currentMusicPlayingDict.Remove(musicName);
    }

    public void StopMusicByID(int idAudio)
    {
        var audio = EazySoundManager.GetMusicAudio(idAudio);
        if (audio != null) audio.Stop();
    }

    public void StopMusicByName_FadingVolume(MusicName musicName, float fadingTime = 2.0f)
    {
        if (false == _currentMusicPlayingDict.ContainsKey(musicName))
        {
            return;
        }

        int idAudio = _currentMusicPlayingDict[musicName];
        StopMusicByID_FadingVolume(idAudio, fadingTime);
        _currentMusicPlayingDict.Remove(musicName);
    }

    public void StopMusicByID_FadingVolume(int idAudio, float fadingTime = 2.0f)
    {
        var audio = EazySoundManager.GetMusicAudio(idAudio);
        if (audio != null)
        {
            DOVirtual.Float(audio.Volume, 0, fadingTime, (volumeValue) => 
            {
                audio.SetVolume(volumeValue);
                if(0 == audio.Volume)
                {
                    StopMusicByID(idAudio);
                }
            });
        }
    }

    public void PauseMusicByID(int idAudio)
    {
        var audio = EazySoundManager.GetMusicAudio(idAudio);
        if (audio != null) audio.Pause();
    }

    public void ResumeMusicByID(int idAudio)
    {
        var audio = EazySoundManager.GetMusicAudio(idAudio);
        if (audio != null) audio.Resume();
    }

    public void UpdateAllMusicVolume()
    {

        EazySoundManager.GlobalMusicVolume = LGameData.Instance.Music_Volume;
        //foreach(int musicID in _currentMusicPlayingDict.Values)
        //{
        //    Audio audio = EazySoundManager.GetMusicAudio(musicID);
        //    if(null != audio && true == audio.IsPlaying)
        //    {
        //        audio.SetVolume(LGameData.Instance.Music_Volume);
        //    }
        //}
    }

    #endregion

    #region PLAY SOUND

    public async UniTask<int> PlaySound(SoundName soundName, bool loop = false, Transform transform = null, float volume = default)
    {
        // return 1;
        if (volume == default) volume = maxVolumeSound;

        var clip = await LoadAudio(soundName.ToString());
        if (clip == null) return -1;

        int id = EazySoundManager.PlaySound(clip, volume, loop, transform);

        if (!HCSettingGame.Instance.isSound)
            StopSound(id);

        Debug.Log("PlaySound!!! " + soundName.ToString());
        return id;
    }

    public async UniTask<AudioClip> GetAudioClipByName(SoundName soundName)
    {
        var clip = await LoadAudio(soundName.ToString());
        if (clip == null) return null;

        return clip;
    }

    public void StopAllSound()
    {
        EazySoundManager.StopAllSounds();
    }

    public void PauseAllSound()
    {
        EazySoundManager.PauseAllSounds();
    }

    public void ResumeAllSound()
    {
        EazySoundManager.ResumeAllSounds();
    }

    public void StopSound(int idAudio)
    {
        var audio = EazySoundManager.GetSoundAudio(idAudio);
        if(audio != null) audio.Stop();
    }

    public void PauseSound(int idAudio)
    {
        var audio = EazySoundManager.GetSoundAudio(idAudio);
        if (audio != null) audio.Pause();
    }

    public void ResumeSound(int idAudio)
    {
        var audio = EazySoundManager.GetSoundAudio(idAudio);
        if (audio != null) audio.Resume();
    }

    #endregion

    private List<string> AudioPrefix = new List<string>{".wav", ".mp3"} ;
    
    
    private async UniTask<AudioClip> LoadAudio(string soundName)
    {
        if (dicAudios.ContainsKey(soundName)) return dicAudios[soundName];

        AudioClip audio = null;
        string path = soundName;
        for (int i = 0; i < AudioPrefix.Count; i++)
        {
            path = SOUND_RESOURCES_PATH + soundName + AudioPrefix[i];
            audio = await HCGameResource.LoadAssetAsync<AudioClip>(path);
            if (audio != null)
            {
                break;
            }
        }
        if (audio == null)
        {
            Debug.LogError("Could not load Audio at: " + path);
            return null;
        }

        dicAudios.Add(soundName, audio);

        return audio;
    }

    private void UpdateAllSoundVolume()
    {
        EazySoundManager.GlobalSoundsVolume = LGameData.Instance.Sound_Volume;
    }

    #region Enviroment sound

    //Only play if there is no other music playing
    private async UniTask PlayBackgroundMusic()
    {
        if(_backgroundMusicList.Count <= 0)
        {
            return;
        }

        while(true)
        {
            if(true == IsMusicPlaying() || false == _canPlayBackgroundMusic)
            {
                await UniTask.WaitForFixedUpdate();
                continue;
            }

            if(_lastPlayMusicBackground == MusicName.AdventureAdrift)
            {
                _lastPlayMusicBackground = MusicName.SunsetValley;
            }
            else if(_lastPlayMusicBackground == MusicName.SunsetValley)
            {
                _lastPlayMusicBackground = MusicName.AdventureAdrift;
            }
            PlayMusic(_lastPlayMusicBackground, default, true);
        }
    }

    private async UniTask PlayEnviromentSound()
    {
        if (environmentSoundList.Count <= 0)
        {
            return;
        }
        lastPlayedEnvironmentSound = Time.realtimeSinceStartup;

        while (true)
        {
            await UniTask.Delay(Mathf.RoundToInt(2000));
            if (false == HCGUIManager.Instance.IsOnlyShowingMainDialog())
            {
                continue;
            }

            if(Time.realtimeSinceStartup - lastPlayedEnvironmentSound <= environmentSoundInterval)
            {
                continue;
            }

            lastPlayedEnvironmentSound = Time.realtimeSinceStartup;

            int randomEnvironmentSoundIndex = UnityEngine.Random.Range(0, environmentSoundList.Count);

            float volume = default;
            if(environmentSoundList[randomEnvironmentSoundIndex] == SoundName.Ambience_Wind_Intensity_Soft_With_Leaves_Loop)
            {
                volume = 2;
            }
            await PlaySound(environmentSoundList[randomEnvironmentSoundIndex], false, null, volume);
        }
    }
    #endregion  Enviroment sound
}

public enum MusicName
{
    None,

    CosmicRay,
    SunnyVillage,
    Adventure,//Login music - no loop
    SunsetValley,//Background music - no loop
    AdventureAdrift,//Background music - no loop
    TheOcean_sLeap,//Wardrove music - loop
    SandyBeach,//Gacha music - loop

    Last,
}


public enum SoundName
{
    None,

    GenericNotification5,
    LittleSwoosh1a,
    LittleSwoosh5,
    Ambience_Place_Waterfall_Loop,
    Ambience_Water_Flowing_Fast_02_Loop,
    Ambience_Water_Fountain_Loop,
    Ambience_Wind_Intensity_Soft_With_Leaves_Loop,
    ClickyButton3b,
    GenericButton5,
    GenericButton14,
    OpenOrEnable3,
    Popup3,
    Popup4a,
    SnappyButton2,
    SnappyButton5,
    Success5,
    Success11,

    //Character_bombno,
    //Character_chay_1,
    //Character_chay_2,
    //Character_chay_3,

    //Character_lomo,
    //GameUI_cancle,
    //GameUI_confirm,
    //GameUI_invalid,
    //GameUI_openpopup,
    //GameUI_scrollelement,
    //GameUI_swipepages,
    //GameUI_unlockreward,

    Last,
}


