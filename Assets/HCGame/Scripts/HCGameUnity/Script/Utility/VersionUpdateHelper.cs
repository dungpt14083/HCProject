using Cysharp.Threading.Tasks;
using UnityEngine;

public class VersionUpdateHelper
{
    public static async UniTask<bool> GetAndCheckVersion()
    {
        
        bool isNeedUpdated = true;

        #if UNITY_WEBGL
            isNeedUpdated = false;
        #endif

        // var versionResp = await LNetworkManager.Instance.GetAppVersion();
        // if (!string.IsNullOrEmpty(versionResp.appVersion))
        // {
        //     var serverVersion = new FVersion(versionResp.appVersion);
        //     var currentVersion = new FVersion(Application.version);
        //     if (currentVersion.CompareVersion(serverVersion))
        //     {
        //         isNeedUpdated = false;
        //     }
        // }
        
        return isNeedUpdated;
    }

    class FVersion
    {
        public int major;
        public int minor;
        public int patch;

        public FVersion(string versionString)
        {
            var check = versionString.Split('.');
            major = int.Parse(check[0]);
            minor = int.Parse(check[1]);
            patch = int.Parse(check[2]);
        }
        public string GetVersionString()
        {
            return $"{major}.{minor}.{patch}";
        }

        public bool CompareVersion(FVersion otherVersion)
        {
            Debug.LogWarning("compare : " + GetVersionString() +"===" + otherVersion.GetVersionString());
            if (major == otherVersion.major)
            {
                //continue compare
                if (minor == otherVersion.minor)
                {
                    //continue
                    return patch >= otherVersion.patch;
                }
                else
                {
                    return minor > otherVersion.minor;
                }
            }
            else
            {
                return major > otherVersion.major;
            }
        }
    }
}