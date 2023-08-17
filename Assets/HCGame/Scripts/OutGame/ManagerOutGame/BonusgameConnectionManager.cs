using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.WebSocket;
using BonusGame;
using BonusGamePlay;
using UnityEngine;
using Google.Protobuf;

public class BonusgameConnectionManager : SingletonMonoAwake<BonusgameConnectionManager>
{
    public bool Connected => _connected;

    private WebSocket _webSocket;
    private bool _connected = false;
    private Action _onConnectedSocket;
    private Action<string> _onConnectedErrorSocket;
    private Action<string> _onDisconnectedSocket;

    public void TryConnect(Action callbackSuccess, Action<string> callbackError)
    {
        _onConnectedSocket = callbackSuccess;
        _onConnectedErrorSocket = callbackError;

        Disconnect();

        var address = HCAppController.Instance.GetUrlBonusGame();
        _webSocket = new WebSocket(new Uri(address));
        _webSocket.StartPingThread = true;
#if !UNITY_WEBGL || UNITY_EDITOR
        this._webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
        if (HTTPManager.Proxy != null)
            this._webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                internalRequest.Proxy =
                    new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif
        this._webSocket.OnOpen += OnOpen;
        this._webSocket.OnMessage += OnMessageReceived;
        this._webSocket.OnBinary += OnByteDataReceived;
        this._webSocket.OnClosed += OnClosed;
        this._webSocket.OnError += OnError;

        _webSocket.Open();
    }

    void OnOpen(WebSocket ws)
    {
        _connected = true;
        Debug.Log("WebSocket BonusGame Open!");
        _onConnectedSocket?.Invoke();
    }

    void OnMessageReceived(WebSocket ws, string message)
    {
        Debug.Log(string.Format("Message received string: <color=yellow>{0}</color>", message));
    }

    void OnByteDataReceived(WebSocket ws, byte[] data)
    {
        Debug.Log("WebSocket OnByteDataReceived!");
        var packageData = PackageData.Parser.ParseFrom(data);
        if (BonusGameConnection.Instance != null && BonusGameConnection.Instance.gameObject.activeSelf)
        {
            BonusGameConnection.Instance.OnByteDataReceived(data);
        }
    }

    //CÓ THỂ XÉT VIỆC ĐANG TRONG GAME MÀ MẤT KẾT NỐI THÌ SẼ LÀM CÁI GÌ Ở ĐÂY HIỆN LEEN POPUP MẤT KẾT NỐI
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        var fullMsg = string.Format("WebSocket closed! Code: {0} Message: {1}", code, message);
        Debug.LogError(fullMsg);
        _webSocket = null;
        _onDisconnectedSocket?.Invoke(fullMsg);
        if (BonusGame_Manager.Instance != null && BonusGame_Manager.Instance.gameObject.activeSelf)
        {
            TryConnect(null, null);
            BonusGame_Manager.Instance.ShowCurrentBonusGame();
        }
    }


    void OnError(WebSocket ws, string error)
    {
        var tmpError = ("ConnectErrorBonus" + error + ws);
        _webSocket = null;
        _onConnectedErrorSocket?.Invoke(tmpError);
    }

    public void Disconnect()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
            _webSocket = null;
        }
    }

    #region HEALTHCHECK

    private float _pingInterval = 5.0f;
    private float _timeSinceLastPing = 0.0f;

    private void Update()
    {
        _timeSinceLastPing += Time.deltaTime;
        if (_timeSinceLastPing >= _pingInterval)
        {
            SendPing();
            _timeSinceLastPing = 0.0f;
        }
    }

    private void SendPing()
    {
        if (_webSocket is { IsOpen: true })
        {
            _webSocket.Send("ping");
        }
    }

    #endregion

    #region SENDTOSERVER

    private readonly PackageData _packageData = new PackageData();

    private void SendBinaryData(byte[] data)
    {
        if (data != null && _webSocket != null)
        {
            _webSocket.Send(data);
        }
    }

    private void SendWithPayType(bool isUseTicket)
    {
        BonusGamePlay.UserInfo userInfo = new BonusGamePlay.UserInfo()
        {
            Token = HCAppController.Instance.GetTokenLogin(),
            PayType = isUseTicket ? 1 : 2
        };
        _packageData.Data = userInfo.ToByteString();
        SendBinaryData(_packageData.ToByteArray());
    }

    private void SendWithOnlyToken()
    {
        BonusGamePlay.UserInfo userInfo = new BonusGamePlay.UserInfo()
        {
            Token = HCAppController.Instance.GetTokenLogin(),
        };
        _packageData.Data = userInfo.ToByteString();
        SendBinaryData(_packageData.ToByteArray());
    }

    public void InitRandomBox()
    {
        _packageData.Header = 5005;
        SendWithOnlyToken();
    }

    public void OpenRandomBox(bool isUseTicket)
    {
        _packageData.Header = 5002;
        SendWithPayType(isUseTicket);
    }

    public void InitScratch()
    {
        _packageData.Header = 5004;
        SendWithOnlyToken();
    }

    public void StartScratch(bool isUseTicket)
    {
        _packageData.Header = 5000;
        SendWithPayType(isUseTicket);
    }

    public void InitWheel()
    {
        _packageData.Header = 5003;
        SendWithOnlyToken();
    }

    public void StartRotateWheel(bool isUseTicket)
    {
        _packageData.Header = 5001;
        SendWithPayType(isUseTicket);
    }

    #endregion
}