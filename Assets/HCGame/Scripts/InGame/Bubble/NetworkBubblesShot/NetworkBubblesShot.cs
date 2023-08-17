using BestHTTP;
using BestHTTP.WebSocket;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HcGames;
using System;
using UnityEngine;

public class NetworkBubblesShot
{
    WebSocket _webSocket = null;

    public Action<string> OnReceivedMessage = null;
    public Action<byte[]> OnReceivedBinary = null;
    public Action<ushort, string> OnClosedCallback = null;
    public Action<string> OnErrorCallback = null;
    public Action OnConnectedCallback = null;
    public void SendMessage(string message)
    {
        Debug.Log("Send message!!! " + message);
        if (null != _webSocket)
        {
            _webSocket.Send(message);
        }
    }

    public void SendBinaryData(byte[] data)
    {
        if (null != data && null != _webSocket)
        {
            _webSocket.Send(data);
        }
    }
    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        OnReceivedMessage?.Invoke(message);
    }
    public void CloseNetwork()
    {
        _webSocket?.Close();
        _webSocket = null;
        Debug.Log("Websocket!!! Game BubblesShot close!!!");
    }
    public void SendPing()
    {
        _webSocket.Send("ping");
    }
    public void InitNetwork(string url)
    {
        Debug.Log("Bubble shot network connect to socket");
        if (null != _webSocket)
        {
            _webSocket.Close();
            _webSocket = null;
        }

        //_webSocket = new WebSocket(new Uri(url), protocol: "echo-protocol", origin: "");
        _webSocket = new WebSocket(new Uri(url));

#if !UNITY_WEBGL || UNITY_EDITOR
        this._webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
        if (HTTPManager.Proxy != null)
            this._webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                internalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif

        _webSocket.OnOpen += OnWebSocketOpen;
        _webSocket.OnMessage += OnMessageReceived;
        _webSocket.OnBinary += OnBinaryReceived;
        _webSocket.OnClosed += OnClosed;
        _webSocket.OnError += OnError;
        _webSocket.Open();
    }

    private void OnError(WebSocket webSocket, string reason)
    {
        OnErrorCallback?.Invoke(reason);
    }

    private void OnClosed(WebSocket webSocket, ushort code, string message)
    {
        OnClosedCallback?.Invoke(code, message);
    }

    private void OnBinaryReceived(WebSocket webSocket, byte[] data)
    {
        OnReceivedBinary?.Invoke(data);
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        OnConnectedCallback?.Invoke();
    }

    private async UniTask<ByteString> SendRequestData(int packageHeader, ByteString data)
    {
        UniTaskCompletionSource<ByteString> responseWaiting = new UniTaskCompletionSource<ByteString>();
        SendByteString((uint)packageHeader, data);
        return await responseWaiting.Task;
    }

    private void SendByteString(uint actionID, ByteString data)
    {
        HcGames.PackageData packageData = new HcGames.PackageData();
        packageData.Header = (uint)actionID;
        packageData.Data = data;
        SendBinaryData(packageData.ToByteArray());
    }
}
