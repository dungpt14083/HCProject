using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class DemoScript : MonoBehaviour {

    void Start() {

    } // Start ()



    public void OnLoadAssetBundleClick () {
        AssetBundleManager.Instance.LoadManifest((success) => {
            if(!success) {
                Debug.LogError(" load manifest error");
            } else {
                // continue loadBasic Bundle
                StartCoroutine(loadBeginBundle());
            } // else
        });
    } // OnLoadAssetBundleClick ()


    IEnumerator loadBeginBundle () {
        // continue loadBasic Bundle
//        List<string> basicBundles = new List<string>{AssetBundleUtility.UI_BUNDLE, AssetBundleUtility.MAIN_BUNDLE, AssetBundleUtility.SCENEMAP_BUNDLE};
//        foreach(string bundleName in basicBundles) {
//            yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetBundle(bundleName));
//        } // foreach
        yield return null;
        Debug.LogWarning("load all assetBundle");
    } // loadBeginBundle


    public void OnClearCacheClick () {
        Caching.ClearCache();
    } // OnClearCacheClick ()
}
