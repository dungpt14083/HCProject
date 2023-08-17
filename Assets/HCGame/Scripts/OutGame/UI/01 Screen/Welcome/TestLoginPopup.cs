using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestLoginPopup : UIPopupView<TestLoginPopup>
{
    public Button btClose;
    public Button btSubmit;
    public TMP_InputField inputUrlLoginApi;
    public TMP_InputField inputUrlOutGameSocket;
    public TMP_InputField inputUrl8Ball;
    public TMP_InputField inputUserName;

    protected override void Awake()
    {
        base.Awake();
        btClose.onClick.AddListener(CloseView);
        btSubmit.onClick.AddListener(Submit);
    }

    public void Show()
    {
        inputUrlLoginApi.text = HCAppController.Instance.GetUrlHCAppWebApi();
        inputUrlOutGameSocket.text = HCAppController.Instance.GetUrlHCAppWebSocket();
        inputUrl8Ball.text = HCAppController.Instance.GetUrlEightBall();
    }


    public void Submit()
    {
        if (!inputUrlLoginApi.text.Equals(string.Empty))
        {
            HCAppController.Instance.SetUrlHCAppWebApi(inputUrlLoginApi.text);
        }

        if (!inputUrlOutGameSocket.text.Equals(string.Empty))
        {
            HCAppController.Instance.SetUrlHCAppWebSocket(inputUrlOutGameSocket.text);
        }

        if (!inputUrl8Ball.text.Equals(string.Empty))
        {
            HCAppController.Instance.SetUrlEightBall(inputUrl8Ball.text);
        }

        if (inputUserName.text.Equals(string.Empty))
        {
            return;
        }

        Executors.Instance.StartCoroutine(LoginGuestRequest());
        CloseView();
    }

    public IEnumerator LoginGuestRequest()
    {
        var data1 = new TestData();
        data1.deviceId = $"{SystemInfo.deviceUniqueIdentifier}{inputUserName.text}";
        string url = HCAppController.Instance.GetUrlHCAppWebApi();
        var request = new UnityWebRequest(url, "POST");
        var fds = JsonUtility.ToJson(data1);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(fds);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("Erro: " + request.error);
        }
        else
        {
            string data = request.downloadHandler.text;
            ResponLogin respon = JsonUtility.FromJson<ResponLogin>(data);
            HCAppController.Instance.SetTokenLogin(respon.id_token);
            ConnectToHCServer(HCAppController.Instance.GetUrlHCAppWebSocket(), respon.id_token, data1.deviceId);
            UISignals.LoginSuccess.Dispatch();
        }
    }

    public void ConnectToHCServer(string url = "", string access_token = "", string deviceId = "")
    {
        url = $"{url}?access_token={access_token}&deviceId={deviceId}";
        HCAppController.Instance.currentDeviceId = deviceId;
        HCAppController.Instance.ConnectToServer(url);
    }
}