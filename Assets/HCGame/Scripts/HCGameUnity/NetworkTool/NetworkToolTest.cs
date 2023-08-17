using System;
using AssemblyCSharp.GameNetwork;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HcGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum NetworkState
{
    Disconnected,
    Connecting,
    Connected
}
public class NetworkToolTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField urlInput;
    [SerializeField] private Button connectBtn;
    
    [SerializeField] private GameObject connectedRegion;
    [SerializeField] private TMP_InputField actionInput;
    [SerializeField] private Button sendBtn;
    [SerializeField] private TMP_InputField dataInput;
    [SerializeField] private TMP_InputField revInput;


    private NetworkState connectState;
    
    // Start is called before the first frame update
    void Start()
    {
        EightBallNetworkManager.Instance.OnConnectedSocketCallback += OnConnectedToServer;
        EightBallNetworkManager.Instance.OnDisconnectedSocketCallback += OnDisconnected;
        EightBallNetworkManager.Instance.OnResponseTextDataCallback += OnTextDataResponse;
        EightBallNetworkManager.Instance.Init();

        connectState = NetworkState.Disconnected;
        refreshState();
        //urlInput.text = "ws://192.168.0.62:8080";
        urlInput.text = "ws://18.141.169.208:8080";
        var test = new CreateRoom();
        test.UserCodeId = "1";
        test.GameType = 1;
        test.Level = 1;
        Debug.Log("test data: " + test.ToString());
        // { "userId": "1", "level": 1, "gameType": 1 }
    }
    
    private void refreshState()
    {
        connectedRegion.SetActive(connectState == NetworkState.Connected);
        connectBtn.GetComponentInChildren<TMP_Text>().text = connectState == NetworkState.Connecting ? "Connecting" : connectState == NetworkState.Connected ? "Disconnect" : "Connect";
        connectBtn.onClick.RemoveAllListeners();
        connectBtn.onClick.AddListener(connectState == NetworkState.Connected? OnDisconnectClicked : OnConnectClicked);
        connectBtn.interactable = (connectState != NetworkState.Connecting);
        
        sendBtn.onClick.RemoveAllListeners();
        sendBtn.onClick.AddListener(OnSendClicked);
    } 

    private void OnConnectClicked()
    {
        try
        {
            if (string.IsNullOrEmpty(urlInput.text))
            {
                Debug.LogError("url is null");
                return;
            }
            connectState = NetworkState.Connecting;
            EightBallNetworkManager.Instance.Connect(urlInput.text);
            refreshState();
        }
        catch (Exception e)
        {
            Debug.LogError("connect clicked exception: " + e);
            revInput.text = e.Message;
        }
    }

    private async void OnDisconnectClicked()
    {
        connectBtn.interactable = false;
        EightBallNetworkManager.Instance.SendByteString(7, null);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        EightBallNetworkManager.Instance.Disconnect();
    }

    private void OnTextDataResponse(string responseText)
    {
        revInput.text += $"\n{responseText}";
    }

    private async void OnSendClicked()
    {
        var actionString = actionInput.text;
        var dataString = dataInput.text;
        if (string.IsNullOrEmpty(actionString))
        {
            Debug.LogError("action empty");
            return;
        }
        var action = uint.Parse(actionString);
        sendBtn.interactable = false;
        revInput.text = string.Empty;
        EightBallNetworkManager.Instance.SendByteString(action, getByteStringData(action, dataString));
        sendBtn.interactable = true;
    }
    private ByteString getByteStringData(uint actionID, string jsonData)
    {
        switch ((PackageHeader)actionID)
        {
            case PackageHeader.CREATE_ROOM : // create room
                var createData = FindingRoom.Parser.ParseJson(jsonData);
                return createData.ToByteString();
            case PackageHeader.FINDING_ROOM: // find room
                var findData = CreateRoom.Parser.ParseJson(jsonData);
                return findData.ToByteString();
            case PackageHeader.BILLIARDS_SHOOT:// billiard shoot
                var shootData = Shoot.Parser.ParseJson(jsonData);
                return shootData.ToByteString();
            case PackageHeader.BILLIARDS_UPDATE_BALL:// ball data 
                var ballData = Shoot.Parser.ParseJson(jsonData);
                return ballData.ToByteString();
            case PackageHeader.BILLIARDS_UPDATE_CUE_POS:// cue pos 
                var cuePosData = Shoot.Parser.ParseJson(jsonData);
                return cuePosData.ToByteString();
            default:
                break;
        }

        switch(actionID)
        {
            case Solitaire.NetworkController.NetworkHeader.UPDATE_POINT:
                var updatePointData = UpdatePoint_Soli.Parser.ParseJson(jsonData);
                return updatePointData.ToByteString();
        }

        return ByteString.Empty;
    }

    #region eventHandler

    private void OnConnectedToServer()
    {
        connectState = NetworkState.Connected;
        refreshState();
        // handleResponseData(Convert.FromBase64String("CAESBggCEAMYCg=="));
    }

    private void OnDisconnected(string msg)
    {
        connectState = NetworkState.Disconnected;
        refreshState();
        revInput.text = msg;
    }
    

    #endregion
        
    
}
