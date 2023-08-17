using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoutPopup : UIPopupView<LogoutPopup>
{
    public Button btClose;
    public Button btCloseBg;
    public Button btComfirm;
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    private string API_URL = "http://192.168.2.68:8022";
    private string RouletteURL = "";
    private Coroutine _sendToServerToLogout;

    public void Show(string _content, string _title = "LOG OUT")
    {
        btClose.onClick.RemoveAllListeners();
        btComfirm.onClick.RemoveAllListeners();
        btCloseBg.onClick.RemoveAllListeners();
        btClose.onClick.AddListener(CloseView);
        btComfirm.onClick.AddListener(Comfirm);
        btCloseBg.onClick.AddListener(CloseView);
        title.text = _title;
        content.text = _content;
    }

    public void Comfirm()
    {
        Debug.Log("Quit App: " + HCAppController.Instance.userInfo.UserCodeId);

        Debug.Log("Quit App: " + HCAppController.Instance.GetTokenLogin());
        StartCoroutine(getLogOut());
    }

    private IEnumerator getLogOut()
    {
        RouletteURL = API_URL + $"/api/log-out?" +
                      $"userCodeId={HCAppController.Instance.userInfo.UserCodeId}";
        var request = new UnityWebRequest(RouletteURL, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string token = HCAppController.Instance.GetTokenLogin();
        if (token != string.Empty) request.SetRequestHeader("Authorization", token);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            var data = request.downloadHandler.text;
            var response = JObject.Parse(data);
            var isLogOut = (bool)response["isSuccess"];
            if (isLogOut)
            {
                HCAppController.Instance.networkControlerHCApp.CloseNetwork();
                ScreenManagerHC.Instance.ShowLoginScreen();
                CloseView();
            }
            else
            {
                // showError();
            }
        }
    }
    private void showError()
    {
        HcPopupManager.Instance.ShowNotifyPopup("Error LogOut", "Notify");
    }
}