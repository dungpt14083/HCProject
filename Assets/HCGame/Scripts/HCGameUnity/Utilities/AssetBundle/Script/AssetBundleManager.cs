
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NBCore;
using System.IO;
using System;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class LoadedAssetBundle
{
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public LoadedAssetBundle(AssetBundle assetBundle)
    {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 1;
    }
}

public class AssetBundleManager : SingletonMono<AssetBundleManager> {

    public float PercentLoading{ get; set;}
    public string LoadingFileName{ get; set;}

    static string m_BaseDownloadingURL = "";

    #if UNITY_EDITOR    
    static int m_SimulateAssetBundleInEditor = -1;
    const string kSimulateAssetBundles = "SimulateAssetBundles";
    #endif


//    static Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW> ();
//    static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();
////    static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation> ();
//    static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();


    #region new
    //can use
     public static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle> ();
    //new
    static Dictionary<string, DataLoader> m_DataLoaders = new Dictionary<string, DataLoader>();
    static Dictionary<string, UnityWebRequestAsyncOperation> m_loadingBundleOperations = new Dictionary<string, UnityWebRequestAsyncOperation>();

    private static AssetBundleManifest m_assetBundleManifest;


    public static string BaseDownloadingURL {get{ return m_BaseDownloadingURL;}}
    public static AssetBundleManifest BundleManifest {get{return m_assetBundleManifest;}}
    #endregion

    #region Public call

    private void setupAssetBundlePath () {
        m_BaseDownloadingURL = AssetBundleUtility.GetAssetBundleURL();
    } // SetupAssetBundlePath


    /// <summary>
    ///  Load File Manifest of Assetbundle
    /// </summary>
    /// <param name="callback">Callback.</param>
    public void LoadManifest (Action<bool> callback = null) {
        Debug.LogWarning("load manifest");
        if (string.IsNullOrEmpty(m_BaseDownloadingURL)) {
            setupAssetBundlePath();
        } // if

        LoadAssetBundleOperation(Utility.GetPlatformName(), (error) => {
            Debug.LogWarning("after load manifest : " + error);
            LoadedAssetBundle manifestBundle = null;
            if(m_LoadedAssetBundles.TryGetValue(Utility.GetPlatformName(), out manifestBundle)) {
                Debug.LogWarning(manifestBundle.m_AssetBundle.name + "===");
                m_assetBundleManifest = manifestBundle.m_AssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            } // if
            bool isSuccess = string.IsNullOrEmpty(error);
            callback(isSuccess);
        });
    } // LoadManifest ()


    public UnityWebRequestAsyncOperation LoadAssetBundleOperation(string assetBundleName, Action<string> callback = null) {
        string url =  m_BaseDownloadingURL + assetBundleName;
        Debug.Log("loader load url : " + url);
        UnityWebRequest download;

        // now alway using cache
        if(m_assetBundleManifest !=null) {
            download = UnityWebRequestAssetBundle.GetAssetBundle(url, m_assetBundleManifest.GetAssetBundleHash(assetBundleName),0);
        } else {
            download = UnityWebRequestAssetBundle.GetAssetBundle(url);
        }// else
        UnityWebRequestAsyncOperation downloadOperation = download.SendWebRequest();
        m_loadingBundleOperations.Add(assetBundleName, downloadOperation);
        downloadOperation.completed += (obj) => {
            Debug.Log("finish : " + downloadOperation.webRequest.url +"=="+ downloadOperation.webRequest.isNetworkError + "=="+ downloadOperation.webRequest.isHttpError);
            m_loadingBundleOperations.Remove(assetBundleName);
            if(!downloadOperation.webRequest.isNetworkError && !downloadOperation.webRequest.isHttpError) {
                if (!m_LoadedAssetBundles.ContainsKey(assetBundleName)) {
                    LoadedAssetBundle loadedAssetBundle = new LoadedAssetBundle(DownloadHandlerAssetBundle.GetContent(downloadOperation.webRequest));
                    m_LoadedAssetBundles.Add(assetBundleName, loadedAssetBundle);
                } // if
            } // if 
            if(callback !=null) {
                callback(downloadOperation.webRequest.error);
            } // if
        };
        return downloadOperation;
    } // UnityWebRequestAsyncOperation ()


    public T LoadAsset<T>(string assetName, string bundleName) where T: UnityEngine.Object {
        LoadedAssetBundle loadedAssetBundle = null;
        if (!m_LoadedAssetBundles.TryGetValue(bundleName, out loadedAssetBundle)) {
            Debug.LogError("bundle not loaded");
            return default(T);
        } else {
            if (!loadedAssetBundle.m_AssetBundle.Contains(assetName)) {
                Debug.LogError("there is no asset with name : " + assetName + "in bundle : " + bundleName);
                return default(T);
            }// if
            return loadedAssetBundle.m_AssetBundle.LoadAsset<T>(assetName);
        } // else

    } // 


    public async Task<T> LoadAssetAsync<T>(string assetName, string bundleName) where T : UnityEngine.Object{
        LoadedAssetBundle loadedAssetBundle = null;
//        return loadedAssetBundle.m_AssetBundle.LoadAssetAsync<T>(assetName);


        if (!m_LoadedAssetBundles.TryGetValue(bundleName, out loadedAssetBundle)) {
//            Debug.LogError("bundle not loaded");

            UnityWebRequestAsyncOperation loadBundleRequest = null;
            if(!m_loadingBundleOperations.TryGetValue(bundleName, out loadBundleRequest)) {
                loadBundleRequest = LoadAssetBundleOperation(bundleName);
            } // if
            while (!m_LoadedAssetBundles.TryGetValue(bundleName, out loadedAssetBundle)) {;
                await Task.Delay(TimeSpan.FromSeconds(0.1));
            } // while

        } // if
            
        //Debug.LogWarning("begin load from assetbundle : " + assetName + "====" + loadedAssetBundle.m_AssetBundle.name);
        if (!loadedAssetBundle.m_AssetBundle.Contains(assetName)) {
            Debug.LogError("there is no asset with name : " + assetName + "in bundle : " + bundleName);
            return default(T);
        } // if

        return loadedAssetBundle.m_AssetBundle.LoadAsset<T>(assetName);

    } // LoadAssetAsync ()

//    public LoadedAssetBundle GetAssetBundle(string name) {
//        if (m_assetBundleManifest == null) {
//            if (!m_DataLoaders.ContainsKey(name)) {
//                Debug.LogWarning("load manifest first");
//                LoadManifest();
//            } // if
//            return null;
//        } // if
//        LoadedAssetBundle bundle = null;
//        if (!m_LoadedAssetBundles.TryGetValue(name, out bundle)) {
//            LoadAssetBundleOperation(name);
//        } // if
//        return bundle;
//    } // GetAssetBundle ()


    #endregion

    /*
    public static LogMode logMode
    {
        get { return m_LogMode; }
        set { m_LogMode = value; }
    }
    // Variants which is used to define the active variants.
    public static string[] ActiveVariants
    {
        get { return m_ActiveVariants; }
        set { m_ActiveVariants = value; }
    }

    private static void Log(LogType logType, string text)
    {
        if (logType == LogType.Error)
            Debug.LogError("[AssetBundleManager] " + text);
        else if (m_LogMode == LogMode.All)
            Debug.Log("[AssetBundleManager] " + text);
    }

*/
    /*
    // The base downloading url which is used to generate the full downloading url with the assetBundle names.
    public static string BaseDownloadingURL
    {
        get { return m_BaseDownloadingURL; }
        set { m_BaseDownloadingURL = value; }
    }


    // AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
    public static AssetBundleManifest AssetBundleManifestObject
    {
        set {m_AssetBundleManifest = value; }
    }

    #if UNITY_EDITOR
    // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
    public static bool SimulateAssetBundleInEditor 
    {
        get
        {
            if (m_SimulateAssetBundleInEditor == -1)
                m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

            return m_SimulateAssetBundleInEditor != 0;
        }
        set
        {
            int newValue = value ? 1 : 0;
            if (newValue != m_SimulateAssetBundleInEditor)
            {
                m_SimulateAssetBundleInEditor = newValue;
                EditorPrefs.SetBool(kSimulateAssetBundles, value);
            }
        }
    }

    #endif

    private static string GetStreamingAssetsPath()
    {
        if (Application.isEditor)
            return "file://" +  System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return Application.streamingAssetsPath;
        else // For standalone player.
            return "file://" +  Application.streamingAssetsPath;
    }
    
    // Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
    public static LoadedAssetBundle GetLoadedAssetBundle (string assetBundleName, out string error) {
        if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
            return null;

        LoadedAssetBundle bundle = null;
        m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
        if (bundle == null)
            return null;

        // No dependencies are recorded, only the bundle itself is required.
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
            return bundle;

        // Make sure all dependencies are loaded
        foreach(var dependency in dependencies)
        {
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
                return bundle;

            // Wait all the dependent assetBundles being loaded.
            LoadedAssetBundle dependentBundle;
            m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            if (dependentBundle == null)
                return null;
        }

        return bundle;
    } // GetLoadedAssetBundle ()

    public static AssetBundleLoadManifestOperation Initialize ()
    {
        return Initialize(Utility.GetPlatformName());
    }

    // Load AssetBundleManifest.
    public static AssetBundleLoadManifestOperation Initialize (string manifestAssetBundleName)
    {
//        #if UNITY_EDITOR
//        Log (LogType.Info, "Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
//        #endif

        var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
        DontDestroyOnLoad(go);

        #if UNITY_EDITOR    
        // If we're in Editor simulation mode, we don't need the manifest assetBundle.
        if (SimulateAssetBundleInEditor)
            return null;
        #endif

        LoadAssetBundle(manifestAssetBundleName, true);
        var operation = new AssetBundleLoadManifestOperation (manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
        m_InProgressOperations.Add (operation);
        return operation;
    }

    // Load AssetBundle and its dependencies.
    static protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
    {
//        Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

        #if UNITY_EDITOR
        // If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
        if (SimulateAssetBundleInEditor)
            return;
        #endif

        if (!isLoadingAssetBundleManifest)
        {
            if (m_AssetBundleManifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
        }

        // Check if the assetBundle has already been processed.
        bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

        // Load dependencies.
        if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
            LoadDependencies(assetBundleName);
    }

    // Where we actuall call WWW to download the assetBundle.
    static protected bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest)
    {
        // Already loaded.
        LoadedAssetBundle bundle = null;
        m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
        if (bundle != null)
        {
            bundle.m_ReferencedCount++;
            return true;
        }

        // @TODO: Do we need to consider the referenced count of WWWs?
        // In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
        // But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
        if (m_DownloadingWWWs.ContainsKey(assetBundleName) )
            return true;

        WWW download = null;
        string url = m_BaseDownloadingURL + assetBundleName;

        // For manifest assetbundle, always download it as we don't have hash for it.
        if (isLoadingAssetBundleManifest)
            download = new WWW(url);
        else
            download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0); 

        m_DownloadingWWWs.Add(assetBundleName, download);

        return false;
    }


    // Where we get all the dependencies and load them all.
    static protected void LoadDependencies(string assetBundleName)
    {
        if (m_AssetBundleManifest == null)
        {
            Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            return;
        }

        // Get dependecies from the AssetBundleManifest object..
        string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
        if (dependencies.Length == 0)
            return;

//        for (int i=0;i<dependencies.Length;i++)
//            dependencies[i] = RemapVariantName (dependencies[i]);

        // Record and load all dependencies.
        m_Dependencies.Add(assetBundleName, dependencies);
        for (int i=0;i<dependencies.Length;i++)
            LoadAssetBundleInternal(dependencies[i], false);
    } // LoadDependencies ()


    // Unload assetbundle and its dependencies.
    static public void UnloadAssetBundle(string assetBundleName)
    {
        #if UNITY_EDITOR
        // If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
        if (SimulateAssetBundleInEditor)
            return;
        #endif

        //Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);

        UnloadAssetBundleInternal(assetBundleName);
        UnloadDependencies(assetBundleName);

        //Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
    }


    static protected void UnloadDependencies(string assetBundleName)
    {
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
            return;

        // Loop dependencies.
        foreach(var dependency in dependencies)
        {
            UnloadAssetBundleInternal(dependency);
        }

        m_Dependencies.Remove(assetBundleName);
    }

    static protected void UnloadAssetBundleInternal(string assetBundleName)
    {
        string error;
        LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
        if (bundle == null)
            return;

        if (--bundle.m_ReferencedCount == 0)
        {
            bundle.m_AssetBundle.Unload(false);
            m_LoadedAssetBundles.Remove(assetBundleName);

//            Log(LogType.Info, assetBundleName + " has been unloaded successfully");
        }
    }


    void Update() {
        // Collect all the finished WWWs.
        var keysToRemove = new List<string>();
        foreach (var keyValue in m_DownloadingWWWs)
        {
            WWW download = keyValue.Value;

            // If downloading fails.
            if (download.error != null)
            {
                m_DownloadingErrors.Add(keyValue.Key, string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error));
                keysToRemove.Add(keyValue.Key);
                continue;
            }


            // If downloading succeeds.
            if(download.isDone)
            {
                AssetBundle bundle = download.assetBundle;
                if (bundle == null)
                {
                    m_DownloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
                    keysToRemove.Add(keyValue.Key);
                    continue;
                }

                //Debug.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);
                m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle) );
                keysToRemove.Add(keyValue.Key);
            }
        }

        // Remove the finished WWWs.
        foreach( var key in keysToRemove)
        {
            WWW download = m_DownloadingWWWs[key];
            m_DownloadingWWWs.Remove(key);
            download.Dispose();
        }

        // Update all in progress operations
        for (int i=0;i<m_InProgressOperations.Count;)
        {
            if (!m_InProgressOperations[i].Update())
            {
                m_InProgressOperations.RemoveAt(i);
            }
            else
                i++;
        }
    } // Update ()


    // Load asset from the given assetBundle.
    public static AssetBundleLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
    {
        Debug.Log("Loading " + assetName + " from " + assetBundleName + " bundle");

        AssetBundleLoadAssetOperation operation = null;
        #if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
            operation = new AssetBundleLoadAssetOperationSimulation (target);
        }
        else
        #endif
        {
//            assetBundleName = RemapVariantName (assetBundleName);
//            LoadAssetBundle (assetBundleName);
            operation = new AssetBundleLoadAssetOperationFull (assetBundleName, assetName, type);

            m_InProgressOperations.Add (operation);
        }

        return operation;
    }


    //REMAP : using later
    // Remaps the asset bundle name to the best fitting asset bundle variant.
    /*static protected string RemapVariantName(string assetBundleName)
    {
        string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

        string[] split = assetBundleName.Split('.');

        int bestFit = int.MaxValue;
        int bestFitIndex = -1;
        // Loop all the assetBundles with variant to find the best fit variant assetBundle.
        for (int i = 0; i < bundlesWithVariant.Length; i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0])
                continue;

            int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);

            // If there is no active variant found. We still want to use the first 
            if (found == -1)
                found = int.MaxValue-1;

            if (found < bestFit)
            {
                bestFit = found;
                bestFitIndex = i;
            }
        }

        if (bestFit == int.MaxValue-1)
        {
            Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
        }

        if (bestFitIndex != -1)
        {
            return bundlesWithVariant[bestFitIndex];
        }
        else
        {
            return assetBundleName;
        }
    }*/




}
