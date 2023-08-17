using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NBCore {
    
    public static class GameResources {
        public enum LoadType {
            AssetBundle,
            Resources,
            AssetBundleToResources,
            ResourcesToAssetBundle
        }

        public const string AssetBundleName = "gameresources";
        public static LoadType loadType;

        private static AssetBundle s_AssetBundle;
        private static string[] s_AssetPaths;
        private static List<string> s_RootPaths;

        static GameResources () {
//            loadType = NBCoreSettings.instance.gameResourceLoadType;
        }

        public static AssetBundle assetBundle {
            get { return s_AssetBundle; }
            set {
                s_AssetBundle = value;
                Init();
            }
        }

        private static void Init () {
            Debug.LogWarning("load type : " + loadType);
            if (loadType == LoadType.Resources)
                return;

#if UNITY_EDITOR
            if (AssetDatabase.GetAssetPathsFromAssetBundle(GameResources.AssetBundleName).Length == 0)
                throw new NullReferenceException("GameResources is not assigned");
#else
        if (s_AssetBundle == null)
            throw new NullReferenceException("AssetBundle is null");
#endif

            // TODO: review later

//        if (s_AssetBundle == null) {
//            Debug.LogWarning("s_AssetBundle = null");
//            if (!FHSceneManager.instance.GetCurrentSceneName().Equals(FHScenes.Intro)) {
//                GuiManager.instance.ShowError(FHResultCode.HTTP_ERROR, (r) => {
//                    FHSceneManager.instance.LoadScene(FHScenes.Intro);
//                    return true;
//                });
//            } // if
//        } // if

            InitRootPaths();
        }

        private static void InitRootPaths () {
            s_RootPaths = new List<string>();
#if UNITY_EDITOR
            s_AssetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(GameResources.AssetBundleName);
#else
        s_AssetPaths = assetBundle.GetAllAssetNames();
#endif
            for (int i = 0; i < s_AssetPaths.Length; i++) {
                var name = s_AssetPaths[i];
                var lowerName = s_AssetPaths[i].ToLower();
                var rootPath = name.Substring(0, lowerName.IndexOf(GameResources.AssetBundleName) + GameResources.AssetBundleName.Length);
                if (s_RootPaths.Contains(rootPath))
                    break;
                s_RootPaths.Add(rootPath);
            }
        }

        public static T Load<T> (string assetName) where T : UnityEngine.Object {
//        Debug.LogWarning("load : " + assetName + "load type : " + loadType);
            if (loadType != LoadType.Resources && s_AssetPaths == null) {
                Init();
            }

            return default(T);
//            switch (loadType) {
//            case LoadType.Resources:
//                return Resources.Load<T>(assetName);
//            case LoadType.AssetBundleToResources:
//                return LoadAssetFromAssetBundle<T>(assetName) ?? Resources.Load<T>(assetName);
//            case LoadType.ResourcesToAssetBundle:
//                return Resources.Load<T>(assetName) ?? LoadAssetFromAssetBundle<T>(assetName);
//            default:
//                return LoadAssetFromAssetBundle<T>(assetName);
//            }
        }

        private static T LoadAssetFromAssetBundle<T> (string assetName) where T : UnityEngine.Object {
            if (s_AssetPaths == null)
                Init();
            var validAssetName = GetValidAssetName(assetName);
            if (string.IsNullOrEmpty(validAssetName)) {
                Debug.LogError("There is no asset with name \"" + assetName + "\" in " + AssetBundleName);
                return null;
            }

#if UNITY_EDITOR
            return AssetDatabase.LoadMainAssetAtPath(validAssetName) as T;
#else
        return assetBundle.LoadAsset<T>(validAssetName);
#endif
        }

        private static string GetValidAssetName (string assetName) {
            assetName = assetName.Replace("\\", "/");

            List<string> checkAssetNames = new List<string>();
            for (int i = 0; i < s_RootPaths.Count; i++) {
                var checkAssetName = string.Format("{0}/{1}", s_RootPaths[i].ToLower(), assetName.ToLower());
                checkAssetNames.Add(checkAssetName);
            }

            for (int i = 0; i < s_AssetPaths.Length; i++) {
                var lowerAssetPath = s_AssetPaths[i].ToLower();
                for (int x = 0; x < checkAssetNames.Count; x++) {
                    if (lowerAssetPath.Contains(checkAssetNames[x])) {
                        return s_AssetPaths[i];
                    }
                }
            }
            return string.Empty;
        }


        public static void Clear () {
//        if (s_AssetBundle != null) {
//            s_AssetBundle.Unload(true);
//        } // if
//        s_AssetBundle = null;
//        s_AssetPaths = null;
        }
        // clear ()
    }
}
