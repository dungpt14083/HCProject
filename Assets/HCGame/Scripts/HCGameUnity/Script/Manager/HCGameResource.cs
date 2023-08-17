using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBCore;
using System.IO;
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class HCGameResource
{

    private const string ADDRESSABLE_BASE_PATH = "Assets/HCGameUnity/GameResources/";
    
    /// <summary>
    /// Wrapper for Resources loading
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async UniTask<T> LoadAssetFromResources<T>(string path) where T : UnityEngine.Object
    {
        // Debug.Log($"Load Asset from resource {path}");
        try
        {
            // check if have '.' index after last '/' => remove suffix
            path = path.RemoveSuffix();
            Debug.Log($"Load Asset from resource {path}");
            ResourceRequest request = Resources.LoadAsync<T>(path);
            while (!request.isDone)
            {
                await UniTask.Delay(100);
            }
            // Debug.Log("load request done : " + path);
            return (T)request.asset;
        }
        catch (Exception ex)
        {
            Debug.LogError("Can not find asset : " + path + ex.ToString());
            return default(T);
        } // catch
    }

    public static T LoadAsseFromtResourcesNonAsync<T>(string path) where T : UnityEngine.Object
    {
        try
        {
            return Resources.Load<T>(path);
        }
        catch (Exception e)
        {
            Debug.Log("Cannot load asset from resource " + path + "with error " + e.Message);
        }
        return null;
    }

    /// <summary>
    /// Load Asset by Addressable 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object
    {
        Debug.Log($"Load Asset {path}");
        try
        {
            AsyncOperationHandle<T> download = Addressables.LoadAssetAsync<T>(path);
            T result = await download.Task;
            return result;
            //return await Resources.LoadAsync<T>(path) as T;
        }
        catch (Exception ex)
        {
            Debug.LogError("Can not find asset : " + path + ex.ToString());
            return await HCGameResource.LoadAssetAsync<T>(path) as T;
        } // catch
    } // LoadAssetAsync ()

}
