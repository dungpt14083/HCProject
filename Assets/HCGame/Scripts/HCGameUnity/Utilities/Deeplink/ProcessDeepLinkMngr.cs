using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessDeepLinkMngr : MonoBehaviour
{
    public static ProcessDeepLinkMngr Instance { get; private set; }

    public Action<string> OnDeepLinkActivatedAction;
    public string deeplinkURL;

    public string deeplinkJWT;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                
            Application.deepLinkActivated += onDeepLinkActivated;
            // if (!string.IsNullOrEmpty(Application.absoluteURL))
            // {
            //     // Cold start and Application.absoluteURL not null so process Deep Link.
            //     onDeepLinkActivated(Application.absoluteURL);
            // }
            // // Initialize DeepLink Manager global variable.
            // else deeplinkURL = "[none]";
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
    private void onDeepLinkActivated(string url)
    {
        // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
        // deeplinkURL = url;
        //
        // // Decode the URL to determine action. 
        // // In this example, the app expects a link formatted like this:
        // // unitydl://mylink?scene1
        // string paramValue = url.Split("?"[0])[1];
        // Debug.LogWarning("param value: " + paramValue);
        // deeplinkJWT = paramValue;

        Debug.LogWarning("Ondeeplink activated : " + url);
        var jwt = GetJWTFromDeepLink(url);
        OnDeepLinkActivatedAction?.Invoke(jwt);
    }


    public static string GetJWTFromDeepLink(string url)
    {
        
        Debug.LogWarning("Get JWT From deeplink : " + url);
        try
        {
            if (!string.IsNullOrEmpty(url))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                string paramValue = url.Split("?"[0])[1];
                Debug.LogWarning("param value: " + paramValue);
                return paramValue;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
        return string.Empty;
    }
    
    
}