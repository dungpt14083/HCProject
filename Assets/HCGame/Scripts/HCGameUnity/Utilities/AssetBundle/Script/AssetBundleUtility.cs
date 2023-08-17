using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBCore;


public class AssetBundleUtility {

    //const
    public const string BASE_PATH_RESOUCES = "Assets/GameData/";

    // BundleName
    public const string UI_BUNDLE = "ui";
    public const string MAIN_BUNDLE = "main";
    public const string SCENEMAP_BUNDLE ="scenemap";
    public const string DATA1 = "data1";
    public const string CONFIG = "config";



//    public static List<string> GetBeginBundles() {
//        return new List<string>{ UI_BUNDLE, MAIN_BUNDLE, SCENEMAP_BUNDLE, DATA1 };
//    } // getBeginBundles


    public static string GetAssetBundlePath () {
        return "AssetBundles/" + NBCoreSettings.instance.AssetBundleVersion + "/" + Utility.GetPlatformName();

    } // GetAssetBundlePath ()


    public static string GetAssetBundleURL () {
        return NBCoreSettings.instance.assetBundleURL +"/"+NBCoreSettings.instance.AssetBundleVersion + "/" + Utility.GetPlatformName()+"/";
    } // 


}
