using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MoreMountains.Feedbacks;
using Zenject;
using Salday.EventBus;
using System.Collections;
using UnityEngine.Networking;
using System.Net;
using System.Collections.Specialized;

public class TestData
{
    public string deviceId;
}

public class ResponLogin
{
    public string id_token;
}

public class SignInPopup : MonoBehaviour
{
    [SerializeField] Button buttonClose;
    [SerializeField] Button buttonGoogle;
    [SerializeField] Button buttonApple;
    [SerializeField] Button buttonFNCY;
    [SerializeField] Button buttonGuets;

    private void Awake()
    {
        buttonGuets.onClick.AddListener(LoginAsGuest);
    }

    private void OnEnable()
    {
        //UISignals.LoginSuccess.AddListener(Hide);
    }

    private void OnDisable()
    {
        //UISignals.LoginSuccess.RemoveListener(Hide);
    }

    public void LoginAsGuest()
    {
#if Test
        HcPopupManager.Instance.ShowTestLoginPopup();
#else
		StartCoroutine(LoginGuestRequest());
#endif
        //LoginGuestRequest();
    }

    public IEnumerator LoginGuestRequest()
    {
        var data1 = new TestData();
        data1.deviceId = HCHelper.GetDeviceId();
        string url = HCAppController.Instance.GetUrlHCAppWebApi();
        Debug.Log("LoginGuestRequest url =" + url);
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
        }
    }

    public void ConnectToHCServer(string url = "", string access_token = "", string deviceId = "")
    {
        url = $"{url}?access_token={access_token}&deviceId={deviceId}";
        Debug.Log("ConnectToHCServer url = " + url);
        HCAppController.Instance.currentDeviceId = deviceId;
        HCAppController.Instance.ConnectToServer(url);
    }
}