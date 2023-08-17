
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Linq;
using SuperScrollView;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NBCore;

public enum DeviceScreenState {
    Portrait,
    LandScape
}

public static class Utility {

    
    public static string RemoveSuffix(this string path)
    {
        if (path.Contains("."))
        {
            var lastDotIndex = path.LastIndexOf('.');
            var lastSlashIndex = path.LastIndexOf('/');
            if (lastDotIndex > lastSlashIndex)
            {
                path = path.Remove(lastDotIndex);
            }
        }
        return path;
    }
    
    public static string GetPlatformName()
    {
        #if UNITY_EDITOR
        return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        #else
        return GetPlatformForAssetBundles(Application.platform);
        #endif
    }
        
    #if UNITY_EDITOR
    private static string GetPlatformForAssetBundles(BuildTarget target)
    {
        switch(target)
        {
        case BuildTarget.Android:
            return "Android";
        case BuildTarget.iOS:
            return "iOS";
        case BuildTarget.WebGL:
            return "WebGL";
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
            return "Windows";
        case BuildTarget.StandaloneOSXIntel:
        case BuildTarget.StandaloneOSXIntel64:
//        case BuildTarget.StandaloneOSX:
            return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
        default:
            return null;
        }
    }
    #endif

    private static string GetPlatformForAssetBundles(RuntimePlatform platform)
    {
        switch(platform)
        {
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

    public static float GetScaledWith(float width) {
        float guiWidth = HCGUIManager.Instance.GetComponent<RectTransform>().rect.width;
//        if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
//            guiWidth = GUIManager.Instance.DefaultReferenceSolution.x;
//        } else {
//            guiWidth = GUIManager.Instance.DefaultReferenceSolution.y;
//        } // else
        return (width * (float)(guiWidth/Screen.width));

    }// GetScaledWith ()

    public static float GetScaledHeight(float height) {
        //if landscape => height will be x value
        float guiHeight = HCGUIManager.Instance.GetComponent<RectTransform>().rect.height;
//        if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
//            guiHeight = GUIManager.Instance.DefaultReferenceSolution.y;
//        } else {
//            guiHeight = GUIManager.Instance.DefaultReferenceSolution.x;
//        } // else
        Debug.LogWarning("scaled screen size : " + guiHeight);
        return height * (float)(guiHeight/Screen.height) ;
    }// GetScaledHeight ()


    public static DeviceScreenState GetDeviceScreenState() {
        if (Screen.height > Screen.width) {
            return DeviceScreenState.Portrait;
        } // if
        return DeviceScreenState.LandScape;
    } // 


    #region Serialize byte data
    public static byte[] Serialize<T> (T data) {
        byte[] bytes = null;
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream()) {
            formatter.Serialize(stream, data);
            bytes = stream.ToArray();
        } // using
        return bytes;
    }


    public static T Deserialize<T> (byte[] bytes) {
        var formatter = new BinaryFormatter();
        var data = default(T);
        using (var stream = new MemoryStream(bytes)) {
            data = (T)formatter.Deserialize(stream);
        }
        return data;
    }

    #endregion

    //cmdev
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(position);
    }
    //cmdev

    public static T ParseStringToEnum<T>(string sEnum)
    {
        return (T)System.Enum.Parse(typeof(T), sEnum, true);
    }

    #region VIBRATE

#if UNITY_ANDROID
    static AndroidJavaClass unity;
    static AndroidJavaObject ca;
    static AndroidJavaObject vibratorService;
#endif
    public static void Vibrate(long miliSec)
    {
        Debug.Log("device vibrate");
#if UNITY_IPHONE
        Handheld.Vibrate();
        return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        try {
            if (unity == null)
            {
                unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                ca = unity.GetStatic<AndroidJavaObject>("currentActivity");
                vibratorService = ca.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }

            if (vibratorService != null) vibratorService.Call("vibrate", miliSec);
        } catch(Exception ex)
        {
            Debug.LogError(" Vibrate Exception : " + ex.ToString() +"==" + ex.StackTrace);
        }
#endif
    }

    #endregion

    public static bool OnMultiplyHorizontal(LoopListView2 _loopListView, GameObject objMultiply, int countData, float mItemWidthOrHeight, float target)
    {
        int mRowCount = (int)Math.Round(Math.Abs(_loopListView.GetComponent<RectTransform>().rect.width / mItemWidthOrHeight));
        if (countData > 0)
        {
            int mColumnCount = (int)Math.Round((float)Math.Abs(countData / mRowCount));
            if (countData - (mColumnCount * mRowCount) >= 1)
                mColumnCount += 1;

            float lenghtItem = mColumnCount * mItemWidthOrHeight;

            if (lenghtItem > target)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    
    
    public static byte[] HexStringToByteArray( this string hex) {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }

}
