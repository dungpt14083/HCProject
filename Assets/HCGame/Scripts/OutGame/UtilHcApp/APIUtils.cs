using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public static class APIUtils
{
    //GỬI REQUEST LÊN SERVER LẤY THẰNG DATA BYE

    #region BYTEEEEEEEEEEEE

    public static IEnumerator RequestWebApiGetByte(string url, Action<byte[]> actionPostCallBack = null,
        Action<string> actionError = null)
    {
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string token = HCAppController.Instance.GetTokenLogin();
        if (token != string.Empty) request.SetRequestHeader("Authorization", token);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError("Error: " + request.error);
            actionError.Invoke(request.error);
        }
        else
        {
            var data = request.downloadHandler.data;
            actionPostCallBack.Invoke(data);
        }
    }

    #endregion


    //GỬI DATA LÊN LẤY JSON VỀ

    #region JSON

    public static IEnumerator RequestWebApiPost(string url, string dataSend, string tokenAuth = "",
        Action<string> actionPostCallBack = null, Action<string> actionError = null)
    {
        var request = new UnityWebRequest(url, "POST");
        if (tokenAuth != string.Empty) request.SetRequestHeader("Authorization", tokenAuth);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(dataSend);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("RequestWebApiPost url " + url);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("Error: " + request.error);
            actionError?.Invoke(request.error);
        }
        else
        {
            string data = request.downloadHandler.text;
            actionPostCallBack?.Invoke(data);
        }
    }

    public static IEnumerator RequestWebApiGetJson(string url, Action<JObject> actionGetCallBack = null,
        Action<string> actionError = null)
    {
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string token = HCAppController.Instance.GetTokenLogin();
        if (token != string.Empty) request.SetRequestHeader("Authorization", token);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError("Error: " + request.error);
            actionError.Invoke(request.error);
        }
        else
        {
            var dataDown = request.downloadHandler.text;
            var response = JObject.Parse(dataDown);
            actionGetCallBack.Invoke(response);
        }
    }

    public static IEnumerator RequestWebApiGetJsonBonusReward(string url, Action<JObject> actionGetCallBack = null,
        Action<string> actionError = null)
    {
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string token = HCAppController.Instance.GetTokenLogin();
        if (token != string.Empty) request.SetRequestHeader("Authorization", token);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError("Error: " + request.error);
            actionError.Invoke(request.error);
        }
        else
        {
            //
        }
    }

    #endregion
}