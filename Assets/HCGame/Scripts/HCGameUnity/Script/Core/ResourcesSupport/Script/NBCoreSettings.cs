using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NBCore {

    public enum ResourceLoadType {
        AssetBundle,
        Resouces,
    } // ResourceLoadType


    public class NBCoreSettings : ScriptableObject {
        [SerializeField]
        private bool m_IsCore;

        [SerializeField]
        private string m_AssetBundleVersion;
        [SerializeField]
        private string m_AssetBundleURL;
        [SerializeField]
        private ResourceLoadType m_GameResourceLoadType;


        private static NBCoreSettings s_Instance;

        public static string GetPlatformName () {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
        }

        #if UNITY_EDITOR
        private static string GetPlatformForAssetBundles (BuildTarget target) {
            switch (target) {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.WebGL:
                return "WebGL";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                return null;
            }
        }
        #endif
        private static string GetPlatformForAssetBundles (RuntimePlatform platform) {
            switch (platform) {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                return null;
            }
        }

        public static NBCoreSettings instance {
            get {
                if (s_Instance == null) {
                    s_Instance = Resources.Load(typeof(NBCoreSettings).Name) as NBCoreSettings;
                    if (s_Instance == null) {
                        Debug.Log("Seems you didn't create NBCoreSettings. Please create it before use");
                        return null;
                    }
                }
                return s_Instance;
            }
        }


        public string AssetBundleVersion {
            get{
                return m_AssetBundleVersion;
            }
        } // AssetBundleVersion


        public bool isCore {
            get { return m_IsCore; }
        }


        public string assetBundleURL {
            get { return m_AssetBundleURL; }
        }

        public ResourceLoadType gameResourceLoadType
        {
            get { return m_GameResourceLoadType; }

        }
    }
}