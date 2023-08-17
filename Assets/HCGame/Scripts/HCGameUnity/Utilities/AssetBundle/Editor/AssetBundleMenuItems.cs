using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleMenuItems {

    const string kSimulationMode = "Assets/AssetBundles/Simulation Mode";

//    [MenuItem(kSimulationMode)]
//    public static void ToggleSimulationMode ()
//    {
//        AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
//    }
//
//    [MenuItem(kSimulationMode, true)]
//    public static bool ToggleSimulationModeValidate ()
//    {
//
//        Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
//        return true;
//    }

    [MenuItem ("Assets/AssetBundles/Build AssetBundles")]
    static public void BuildAssetBundles ()
    {
//        BuildScript.FindAllAssetsInBundle("model");
        BuildScript.BuildAssetBundles();
    }
}
