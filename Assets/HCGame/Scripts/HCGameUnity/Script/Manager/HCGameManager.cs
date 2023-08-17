using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public partial class HCGameManager : SingletonMono<HCGameManager>
{
    [SerializeField]
    public Camera mainCamera;

    public ShaderVariantCollection androidVariants;
    public ShaderVariantCollection webglVariants;
    [SerializeField] private Transform sceneAnchor;

    private HCSoundManager soundManager;

    public HCSoundManager GetSoundManager
    {
        get
        {
            if (null == soundManager)
            {
                soundManager = new HCSoundManager();
            }
            return soundManager;
        }
    }

    #region System Custom   
    private void Awake()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 30;
#else
    Application.targetFrameRate = 60;
#endif



#if PROD && !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
#if UNITY_ANDROID
        if (androidVariants != null)
            androidVariants.WarmUp();
#endif
#if UNITY_WEBGL
        if (webglVariants != null)
            webglVariants.WarmUp();
#endif
        DontDestroyOnLoad(this);
    }

    async void Start()
    {
        try
        {
            //TODO: temp disable
            if (mainCamera == null)
                mainCamera = Camera.main;
            var url = "";
            HCNetworkManager.Instance.SetURL(url);
            await GetSoundManager.LoadAllSound();
            // Scene init 
        }
        catch (Exception ex)
        {
            ErrorHandlerHelper.HandleException(ex);
            //BGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, ex.ToString()));
        }
        finally
        {
            await HCGUIManager.Instance.ShowWaiting(false);
        }

    } // Start ()

    public async void SignoutAndShowLogin()
    {
        try
        {
            await HCGUIManager.Instance.ShowWaiting(true);
            HCNetworkManager.Instance.SetToken(string.Empty);
            LGameData.Instance.CachedToken = string.Empty;
            // await mapHandler.LoadMap(new List<LandModels>());
            // LGUIManager.HideMainDialog();
        }
        finally
        {
            await HCGUIManager.Instance.ShowWaiting(false);

        }
    }

    public async void BeginLoadingConfigResource()
    {
        // var test = await Addressables.CleanBundleCache().ToUniTask();
        //TEMP : clear addressable loader
        List<DataLoader> dataLoader = new List<DataLoader>()
        {
            new AddressAssetLoader(Addressables.DownloadDependenciesAsync("Config"), "Config"),
            new AddressAssetLoader(Addressables.DownloadDependenciesAsync("GameResource"), "Game Resource"),
            // new AddressAssetLoader(Addressables.DownloadDependenciesAsync("GUIData"), "Gui Data"),
            // new AddressAssetLoader(Addressables.DownloadDependenciesAsync("Character"), "Character"),
            // new AddressAssetLoader(Addressables.DownloadDependenciesAsync("Icons"), "Icons"),
            // new AddressAssetLoader(Addressables.DownloadDependenciesAsync("Map"), "Map"),
            // new AddressAssetLoader(Addressables.DownloadDependenciesAsync("Sounds"), "Sound"),
            // new LocalConfigLoader(),
        };

        HCGUIManager.Instance.ShowLoading(dataLoader, 0, async (error) =>
        {
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("load Asset error");
            }
            else
            {
                try
                {
                    // LGameData.Instance.MiCoin = 100.0f;
                }
                catch (Exception e)
                {
                    ErrorHandlerHelper.HandleException(e);
                }
                finally
                {
                    HCGUIManager.Instance.HideLoading();
                    await LConfigManager.Instance.Init();
                    HCGUIManager.Instance.ShowGUI(HCGUIManager.Instance.loginDialog);
                }

            }
        });
    }



    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //WebGLCopyAndPasteAPI.ApplicationQuit();
#endif

        HCNetworkManager.Instance.OnQuit();
    } // OnApplicationQuit

    //private void handleUnityLog(string logString, string stackStrace, LogType type)
    //{
    //    if (type == LogType.Exception)
    //    {
    //        Debug.LogError("exeption error : " + stackStrace);
    //        //            GUIMessageDialog.Show((r) => {
    //        //                #if UNITY_WEBGL && !UNITY_EDITOR
    //        //                Application.OpenURL("");
    //        //                #else
    //        //                Application.Quit();
    //        //                #endif
    //        //                return true;
    //        //            }, FHLocalization.instance.GetString(FHStringConst.GAME_ERROR), FHLocalization.instance.GetString(FHStringConst.LABEL_ERROR));
    //    } // if
    //    if (type != LogType.Log && type != LogType.Warning)
    //    {
    //        if (type == LogType.Exception)
    //        {
    //            logString += "==" + stackStrace;
    //        } // if
    //        Log.Instance.WriteLog(logString, type == LogType.Exception ? BFLogType.Exception : BFLogType.Error);
    //    } // if
    //} // handleUnityLog ()

    // Check NETWORK Status
    const float UPDATE_TIME = 2.0f;
    bool isCheckingWifi = false;
    float timeUpdate = 0.0f;

    //     private void checkingNetworkStatus()
    //     {
    // #if !UNITY_WEBGL
    //         if (!isCheckingWifi)
    //         {
    //             timeUpdate += Time.deltaTime;
    //             if (timeUpdate >= UPDATE_TIME)
    //             {
    //                 isCheckingWifi = true;
    //                 //                Debug.LogError(" check wifi status");
    //                 timeUpdate = 0;
    //                 if (Application.internetReachability == NetworkReachability.NotReachable)
    //                 {
    //                     Debug.LogError("wifi problem notReachable: " );
    //                     //showWifiProblem(true);
    //                 }
    //                 else
    //                 {
    //                     pingToCheckConnection();
    //                 } // else
    //             } // if
    //         }
    // #endif
    //     } // checkingNetworkStatus ()

    private void showWifiProblem(bool isProblem)
    {
        if (isProblem)
        {
            HCGUIManager.Instance.ShowWifiAlert(isProblem, () =>
            {

            });
        } // if

    }


    #endregion

    #region Main Call

    private void LoadBasicResources(Action onLoadComplete)
    {
        List<DataLoader> dataLoader = new List<DataLoader>() {
            new FakeDataLoader(HCGUIManager.Instance.FakeLoadingTime),
        };

        DOVirtual.DelayedCall(0.5f, () =>
        {
            // LoadCharacter();
            HCGUIManager.Instance.ShowLoading(dataLoader, 0, async (error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError("load Asset error");
                }
                else
                {
                    try
                    {
                        HCGUIManager.Instance.HideLoading();

                        onLoadComplete?.Invoke();
                    }
                    catch (Exception e)
                    {
                        ErrorHandlerHelper.HandleException(e);
                    }
                    finally
                    {
                        HCGUIManager.Instance.HideLoading();
                        GetSoundManager.StopMusicByName_FadingVolume(MusicName.Adventure);
                    }

                }
            });
        }, false);
    }

    public async void LoadFarmScene()
    {
        LoadBasicResources(async () =>
        {
            HCGUIManager.HideAllGUI();

            HCGUIManager.Instance.ShowGUI(HCGUIManager.Instance.userInfoDialog);
            HCGUIManager.Instance.ShowGUI(HCGUIManager.Instance.mainDialog);
            if (false == PlayerPrefs.HasKey(UIConstant.UI_SHOWANNOUNCEMENT) || DateTime.Now.DayOfYear != PlayerPrefs.GetInt(UIConstant.UI_SHOWANNOUNCEMENT))
            {
            }
            await UniTask.Delay(1000);
        });
    }


    public void LoadHomeScene()
    {
        List<DataLoader> dataLoader = new List<DataLoader>() {
            //new FakeDataLoader(LGUIManager.Instance.FakeLoadingTime),            
                new LocalConfigLoader(),
                };

        DOVirtual.DelayedCall(0.5f, () =>
        {
            HCGUIManager.Instance.ShowLoading(dataLoader, 0, (error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError("load Asset error");
                }
                else
                {
                    HCGUIManager.Instance.HideGUI(HCGUIManager.Instance.mainDialog);
                    HCGUIManager.Instance.ShowGUI(HCGUIManager.Instance.homeDialog);
                    HCGUIManager.Instance.ShowGUI(HCGUIManager.Instance.userInfoDialog);
                    HCGUIManager.Instance.HideLoading();
                }
            });
        }, false);
    }

  

    #endregion
    



}
