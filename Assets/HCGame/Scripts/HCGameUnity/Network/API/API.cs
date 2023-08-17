using System;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class API
{
    private string token;
    private string url;

    private string getFullUrl(string path)
    {
        return url + path;
    }


    public void SetToken(string token)
    {
        Debug.Log("set token : " + token);
        this.token = string.IsNullOrEmpty(token) ? string.Empty : "Bearer " + token;
    }

    public void SetURL(string url)
    {
        this.url = url;
    }

    public async Task<string> PostAsync(string path, IDictionary<string, object> requestParams)
    {
        TaskCompletionSource<string> processTask = new TaskCompletionSource<string>();
        try
        {
            string fullURL = getFullUrl(path);
            Debug.Log($"Post Async Request : {fullURL} == {DateTime.Now.ToString()}");
            

            var request = new HTTPRequest(new Uri(fullURL), HTTPMethods.Post, (req, res) =>
            {
                Debug.Log($"Post Async Response : {res?.DataAsText} == {DateTime.Now.ToString()}");
                if (processTask?.Task.IsCompleted == false)
                {
                    processTask.SetResult(res?.DataAsText);
                    processTask = null;
                }
            });
            
            //TODO: add authen
            if (!string.IsNullOrEmpty(token))
            {
                Debug.Log("add header : " + token);
                request.AddHeader("Authorization", token);
            }

            // FormUrl encoded
            // foreach (var value in requestParams)
            // {
            //     Debug.Log("add params " + value.Key +"===" + value.Value);
            //     request.AddField(value.Key, value.Value.ToString());
            // }
            // request.FormUsage = BestHTTP.Forms.HTTPFormUsage.UrlEncoded;
            // request.Send();
            
            //=====raw json
            if (requestParams?.Count > 0)
            {
                var jsonData = JsonConvert.SerializeObject(requestParams, Formatting.None);
                Debug.Log("request with params: " + jsonData);
                request.RawData = Encoding.UTF8.GetBytes(jsonData);
            }
            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            // request.DisableCache = true;
            HTTPManager.KeepAliveDefaultValue = false;
            // HTTPManager.Logger.Level = Loglevels.All;
            request.Send();
            return await processTask.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"PostAsync Error: {ex.Message} === {ex.StackTrace}");
            return string.Empty;
        }
    }
    
    
    public async Task<string> PutAsync(string path, IDictionary<string, object> requestParams)
    {
        TaskCompletionSource<string> processTask = new TaskCompletionSource<string>();
        try
        {
            string fullURL = getFullUrl(path);
            Debug.Log($"Put Async Request : {fullURL} == {DateTime.Now.ToString()}");
            
            var request = new HTTPRequest(new Uri(fullURL), HTTPMethods.Put, (req, res) =>
            {
                Debug.Log($"Put Async Response : {res?.DataAsText} == {DateTime.Now.ToString()}");
                if (processTask?.Task.IsCompleted == false)
                {
                    processTask.SetResult(res?.DataAsText);
                    processTask = null;
                }
            });
            
            //TODO: add authen
            if (!string.IsNullOrEmpty(token))
            {
                Debug.Log("add header : " + token);
                request.AddHeader("Authorization", token);
            }
            //=====raw json
            if (requestParams?.Count > 0)
            {
                var jsonData = JsonConvert.SerializeObject(requestParams, Formatting.None);
                Debug.Log("request with params: " + jsonData);
                request.RawData = Encoding.UTF8.GetBytes(jsonData);
            }
            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            // request.DisableCache = true;
            HTTPManager.KeepAliveDefaultValue = false;
            // HTTPManager.Logger.Level = Loglevels.All;
            request.Send();
            return await processTask.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Put Async Error: {ex.Message} === {ex.StackTrace}");
            return string.Empty;
        }

    }

    public async Task<string> GetAsync(string path, IDictionary<string, object> requestParams)
    {
        TaskCompletionSource<string> processTask = new TaskCompletionSource<string>();
        try
        {
            string fullURL = getFullUrl(path);

            string paramPath = "";
            
            //request in path
            if (requestParams?.Count > 0)
            {
                paramPath += "?";
            
                foreach (var param in requestParams)
                {
                    paramPath += param.Key + "=" + param.Value + "&";
                    // paramPath += param.Key + "=" + JsonConvert.SerializeObject(param.Value) + "&";
                }
                //remove last &
                paramPath = paramPath.Remove(paramPath.Length - 1);
                fullURL += paramPath;
            }
            
            Debug.Log($"Get Async Request : {fullURL} == {DateTime.Now.ToString()}");
            var request = new HTTPRequest(new Uri(fullURL), HTTPMethods.Get, (req, res) =>
            {
                Debug.Log($"Get Async Response {fullURL} : {res?.DataAsText} == {DateTime.Now.ToString()}");
                if (processTask?.Task.IsCompleted == false)
                {
                    processTask.SetResult(res?.DataAsText);
                    processTask = null;
                }
            });

            // Note : Get only support raw data and params
            // if (requestParams?.Count > 0)
            // {
            //     var jsonData = JsonConvert.SerializeObject(requestParams, Formatting.None);
            //     Debug.Log("request with params: " + jsonData);
            //     request.RawData = Encoding.UTF8.GetBytes(jsonData);
            // }
            // request.AddHeader("Content-Type", "application/json; charset=utf-8");

            //TODO: add authen
            if (!string.IsNullOrEmpty(token))
            {
                Debug.Log("add header : " + token);
                request.AddHeader("Authorization", token);
            }
            // request.DisableCache = true;
            HTTPManager.KeepAliveDefaultValue = false;
            // HTTPManager.Logger.Level = Loglevels.All;
            request.Send();
            return await processTask.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"PostAsync Error: {ex.Message} === {ex.StackTrace}");
            return string.Empty;
        }
    }

    public void OnQuit()
    {
        BestHTTP.HTTPManager.OnQuit();
    }
}
