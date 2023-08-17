using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAccountPopup : UIPopupView<DeleteAccountPopup>
{
    public Button btClose;
    public Button btClsoeBg;
    public Button btComfirm;
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    protected  override  void Awake()
    {
        base.Awake();
        btClose.onClick.AddListener(CloseView);
        btComfirm.onClick.AddListener(Comfirm);
        btClsoeBg.onClick.AddListener(CloseView);
    }

    public void Show(string _content, string _title = "DELETE ACCOUNT")
    {
        title.text = _title;
        content.text = _content;
    }
    public void Comfirm()
    {
        DeleteAccountRequest request = new DeleteAccountRequest
        {
            userCodeId = HCAppController.Instance.userInfo.UserCodeId,
            accessToken = HCAppController.Instance.GetTokenLogin()
        };
        string token = HCAppController.Instance.GetTokenLogin();
        StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrl_DeleteAccount(), JsonUtility.ToJson(request), token,
            (data) => {
                responeDataDelete response = JsonUtility.FromJson<responeDataDelete>(data);
                if (response.isSuccess)
                {
                    HcPopupManager.Instance.ShowNotifyPopup("Delete account success ","Delete account");
                    ScreenManagerHC.Instance.ShowLoginScreen();
                }
            }));
    }
}

public class responeDataDelete
{
    public string message;
    public string totalDayClaim;
    public bool isSuccess;
}

// RequestWebApiPost DeleteAccountRequest data {
//     "message" : "Delete account success",
//     "totalDayClaim" : null,
//     "isSuccess" : true
// }