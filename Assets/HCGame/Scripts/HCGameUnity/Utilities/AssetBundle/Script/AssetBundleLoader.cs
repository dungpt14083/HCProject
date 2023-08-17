using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public abstract class DataLoader : IEnumerator {
    public float progress { get; set; } //progress of this dataloader
    public string Error{ get; set; }
    public string LoaderName { get; set; }


    #region IEnumerator implementation

    public bool MoveNext () {
        return !IsDone();
    }

    public void Reset () {

    }

    public object Current {
        get {
            return null;
        }
    }

    public abstract bool IsDone();
    public abstract bool Update();

    public virtual void Release()
    {

    }

    #endregion



} // DataLoader


public class AddressAssetLoader : DataLoader
{
    AsyncOperationHandle request;

    public AddressAssetLoader(AsyncOperationHandle request, string addressBundleName = "")
    {
        this.request = request;
        LoaderName = addressBundleName;
    }

    public override bool IsDone()
    {
        if (request.IsValid() && request.IsDone)
        {
            return true;
        }// if
        return false;
    }

    public override bool Update()
    {
        progress = request.GetDownloadStatus().Percent;
        // Debug.LogWarning("addressbundle process  : " + LoaderName +" === " + progress +"===" + request.IsDone +" ===" + request.GetDownloadStatus().DownloadedBytes +"/" +request.GetDownloadStatus().TotalBytes);
        if (request.IsValid() && request.IsDone)
        {
            Debug.LogWarning("addressbundle load finished   : " + LoaderName  + "===" + request.Status);
            if(request.Status == AsyncOperationStatus.Failed)
            {
                Error = request.OperationException.Message;
            } else
            {
                Error = string.Empty;
            }
            return false;
        } // if

        return true;
    }

    public override void Release()
    {
        Addressables.Release(request);
    }
}

/// <summary>
/// AssetBundle Loader using to show loading load Assetbundle
/// </summary>
public class AssetBundleLoader : DataLoader {

    //UnityWebRequestAsyncOperation requestOperation = null;

    AsyncOperation baseRequest = null;
    string bundleName;


    public AssetBundleLoader(string bundleName) {
        LoaderName = bundleName;
        this.bundleName = bundleName;
        Error = string.Empty;
    } // AssetBundleLoader

    public override bool IsDone () {

        if (baseRequest != null && baseRequest.isDone)
        {
            return true;
        }// if
        return false;
    }


    // Return true if need continue update
    public override bool Update () {

        if (baseRequest == null)
        {
            baseRequest = AssetBundleManager.Instance.LoadAssetBundleOperation(bundleName);
            return true;
        } // if
        progress = baseRequest.progress;
        
        if (baseRequest != null && baseRequest.isDone)
        {
            Error = GetError();
            return false;
        } //if
        return true;
    } // Update ()

    private string GetError ()
    {
        string result = string.Empty;
        UnityWebRequestAsyncOperation webRequest = (UnityWebRequestAsyncOperation)baseRequest;
        result = webRequest.webRequest.isNetworkError || webRequest.webRequest.isHttpError ? webRequest.webRequest.error : string.Empty;
        return result;
    }
}


public class SceneLoader : DataLoader
{
    AsyncOperation requestOperation;

    string sceneName;

    public SceneLoader(string sceneName)
    {
        this.sceneName = sceneName;

    }


    public override bool IsDone()
    {
        if (requestOperation != null && requestOperation.isDone)
        {
            return true;
        }
        return false;
    }

    public override bool Update ()
    {
        if(requestOperation == null)
        {
            var test = SceneManager.LoadSceneAsync(sceneName);
            
            requestOperation = SceneManager.LoadSceneAsync(sceneName);
            return true;
        } // if
        progress = requestOperation.progress;
        if(requestOperation != null && requestOperation.isDone)
        {
            return false;
            //if (requestOperation)
            //{
            //    Error = requestOperation.webRequest.error;
            //} // if
        }
        return true;
    }

    private void getError ()
    {
        //(Scene)
    }
}

public class FakeDataLoader : DataLoader
{
    private float timeAlpha = 0;
    private float timeFakeLoading = 2.0f;

    public FakeDataLoader(float inputFakeTimeLoading = 2.0f)
    {
        timeAlpha = 0;
        progress = 0;
        timeFakeLoading = inputFakeTimeLoading;
    }

    public override bool IsDone()
    {
        if (progress >= 1)
        {
            return true;
        }
        return false;
    }

    public override bool Update()
    {
        progress = timeAlpha / timeFakeLoading;
        timeAlpha += Time.deltaTime;
        if(progress >= 1)
        {
            return false;
        }
        return true;
    }
}

public class LocalConfigLoader : DataLoader
{
    public LocalConfigLoader()
    {
        LConfigManager.Instance.Init();
    }

    public override bool IsDone()
    {
        return LConfigManager.Instance.HasLoadedLocalConfig;
    }

    public override bool Update()
    {
        if(null == LConfigManager.Instance || false == IsDone())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
