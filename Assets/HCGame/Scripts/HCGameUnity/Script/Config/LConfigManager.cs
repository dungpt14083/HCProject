using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBCore;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;

public class LConfigManager : Singleton<LConfigManager>
{

    //=== App Game


    private const string BASE_DESTINATION_PATH = "Assets/FiveGameUnity/GameResources/";


    private bool hasLoadedLocalConfig = false;
    public bool HasLoadedLocalConfig
    {
        get { return hasLoadedLocalConfig; }
    }

    private bool hasLoadedRemoteConfig = false;
    public bool HasLoadedRemoteConfig
    {
        get { return hasLoadedRemoteConfig; }
    }

    public async UniTask Init()
    {
        //load serverConfig
        // SeedConfig = await loadConfig<SeedConfigData>("DataConfig/SeedConfig.bytes");

        //log test config
        // check config 
        // foreach (var data in ClothesItemTypeConfigData.GetAllItems())
        // {
        //     Debug.Log("cloth : " + data.ID + "=== " + data.Name +"===" + data.Description);
        // }
        hasLoadedLocalConfig = true;
    } // Init ()
    

    private async UniTask<T> loadConfig<T>(string path)
    {
        var bytes = (await HCGameResource.LoadAssetAsync<TextAsset>(path)).bytes;

        if (bytes != null)
        {
            T result = Utility.Deserialize<T>(bytes);
            Debug.Log("load config finish : " + path);
            return result;
        } // if
        return default(T);
    }

    #region Editor Build Config

#if UNITY_EDITOR
    [MenuItem("ConfigHelper/BuildConfig")]
    public static async void BuildConfig()
    {
        await Instance.LoadAndBuildConfig();
    }
    
    

    private async UniTask LoadAndBuildConfig()
    {
        //setup config in here
        // https://docs.google.com/spreadsheets/d/14dSyEyfY0Rfi2FoBgKTyKFszl5NgOsxq75ZYuTdd_kM/edit?usp=sharing
    }

    private async UniTask loadAndSaveConfig<T, V>(string loadPath, string savePath) where T : BaseConfigData<V>, new() where V : BaseRecord, new()
    {
        T configData = new T();

        var configValues = await GoogleSheetService.Instance.GetGoogleSheetValue(loadPath);
        configData.SetupData(configValues);
        var bytes = Utility.Serialize(configData);

        //string destinationPath = "Assets/LumiGame/Resources/DataConfig/StageConfig.bytes";
        //create File and Directory
        if (savePath.Contains("/"))
        {
            string destinationFolder = savePath.Remove(savePath.LastIndexOf("/"));
            if (!FileUtility.CreateDirectory(destinationFolder))
            {
                Debug.LogError(" can not create destination config folder : " + destinationFolder);
            } // if
        } // if
        File.WriteAllBytes(savePath, bytes);
        AssetDatabase.Refresh();
    }
    
    
    //====== public function
    public async UniTask<T> LoadConfig<T, V>(string loadPath)
        where T : BaseConfigData<V>, new() where V : BaseRecord, new()
    {
        T configData = new T();

        var configValues = await GoogleSheetService.Instance.GetGoogleSheetValue(loadPath);
        configData.SetupData(configValues);
        return configData;
    }
    
#endif

    #endregion
    
    #region UTILITY CONFIG

  
    #endregion

}
