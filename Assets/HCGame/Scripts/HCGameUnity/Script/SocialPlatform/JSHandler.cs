using System;
using UnityEngine;
using SocialPlatform;

public class JSHandler : SingletonMono<JSHandler>
{
    public Action<string> googleLoginHandler;

    public void SendGoogleDataFromJS(string input)
    {
        Debug.Log("SendGoogleDataFromJS!!! " + input);
        googleLoginHandler?.Invoke(input);
    }
}
