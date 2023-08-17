using System;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.WebSocket;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HcGames;
using Newtonsoft.Json;
using UnityEngine;

public class HCSocketNetworkManager : Singleton<HCSocketNetworkManager>
{
    public Action OnConnectedSocket;
    public Action<string> OnDisconnectedSocket;
    public Action<string> OnMessageReceive;
    public Action<byte[]> OnByteDataReceive;
    
    
    WebSocket webSocket;
    private bool connected = false;


    public void Connect(string address, Action connectedCallBack)
    {
        Debug.Log("8 Ball network connect to socket");
        Disconnect();
        //connect new socket
        webSocket = new WebSocket(new Uri(address));
        
        webSocket.StartPingThread = true;

#if !UNITY_WEBGL || UNITY_EDITOR
        this.webSocket.StartPingThread = true;
        
    #if !BESTHTTP_DISABLE_PROXY
            if (HTTPManager.Proxy != null)
                this.webSocket.OnInternalRequestCreated = (ws, internalRequest) => internalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif
        this.webSocket.PingFrequency = 5000;
        this.OnConnectedSocket = connectedCallBack;
        this.webSocket.OnOpen += OnOpen;
        this.webSocket.OnMessage += OnMessageReceived;
        this.webSocket.OnBinary += OnByteDataReceived;
        this.webSocket.OnClosed += OnClosed;
        this.webSocket.OnError += OnError;
        webSocket.Open();
        Debug.Log("HCSocketNetworkManager Connecting url: " + address);

    }


    public void Disconnect()
    {
        if (webSocket != null)
        {
            Debug.Log("close socket ");
            webSocket.Close();
            webSocket = null;
        }
    }
    public void SendPing()
    {
        webSocket.Send("ping");
    }
    public void SendBinaryData(byte[] data)
    {
        if (data != null && webSocket != null)
        {
            //Debug.Log("send binary Data : " + Convert.ToBase64String(data));
            webSocket.Send(data);
        }
    }
    
    #region WebSocket Event Handlers

    /// <summary>
    /// Called when the web socket is open, and we are ready to send and receive data
    /// </summary>
    void OnOpen(WebSocket ws)
    {
        connected = true;
        OnConnectedSocket?.Invoke();
    }

    /// <summary>
    /// Called when we received a text message from the server
    /// </summary>
    void OnMessageReceived(WebSocket ws, string message)
    {
        Debug.Log(string.Format("Message received: <color=yellow>{0}</color>", message));
        OnMessageReceive?.Invoke(message);
    }

    /// <summary>
    /// Called when we received a byte data message from the server
    /// </summary>
    void OnByteDataReceived(WebSocket ws, byte[] data)
    {
        // Debug.Log(string.Format("Message received: <color=yellow>{0}</color>", data));
        OnByteDataReceive?.Invoke(data);
    }

    /// <summary>
    /// Called when the web socket closed
    /// </summary>
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        var fullMsg = string.Format("WebSocket closed! Code: {0} Message: {1}", code, message);
        Debug.LogError(fullMsg);
        webSocket = null;
        OnDisconnectedSocket?.Invoke(fullMsg);
    }

    /// <summary>
    /// Called when an error occured on client side
    /// </summary>
    void OnError(WebSocket ws, string error)
    {
        var fullMsg = string.Format("An error occured: <color=red>{0}</color>", error);
        Debug.LogError(fullMsg);
        webSocket = null;
        OnDisconnectedSocket?.Invoke(fullMsg);

    }

    #endregion
    
}