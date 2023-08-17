using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using NBCore;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;
public class HCGUIManager : SingletonMono<HCGUIManager>
{
    private RectTransform mRect;

    private GUIManager guiManager;

    [Header("Debug")]
    [SerializeField] private bool isDebug = false;
    public bool IsDebug
    {
        get
        {
            return isDebug;
        }
    }
    [SerializeField] private TMP_Text txtFpsCounter;

    [SerializeField] private float fakeLoadingTime = 2.0f;
    public float FakeLoadingTime
    {
        get
        {
            return fakeLoadingTime;
        }
    }

    //dialog base
    [Header("DialogHandler")]

    public GUIDialogBase mainDialog;
    public GUIDialogBase homeDialog;
    public GUIDialogBase userInfoDialog;
    public GUIDialogBase loginDialog;
    public GUIDialogBase walletQRDialog;
    public GUIDialogBase walletconnectInfo;
    public GUIDialogBase checkInternetDialog;


    [Header("UI object")]
    [SerializeField] private LoadingDialogHandler loadingDialogHandler;
    [SerializeField] private GameObject splashScreen;

    [Header("Anchor")]
    public Transform BegingDraggedAnchor;


    [Header("GuiObject")]
    public GameObject BlackBackground;
    public GameObject OverlayBackground;
    [SerializeField]
    private GameObject waitingDialog;
    [SerializeField]
    private WifiAlertDialogHandler wifiAlertDialog;
    [SerializeField]
    public GameObject objTweenMoveCamera;

    [SerializeField]
    private ConformDialogHandler conformDialogHandler;

    [SerializeField]
    private MessageDialogHandler messageDialogHandler;

    [SerializeField] private MessageDialogHandler infoDialogHandler;

    // [SerializeField] private PopupDialogHandler popupDialogHandler;

    private List<GUIDialogBase> showingGUIs = new List<GUIDialogBase>();
    
    private void Awake()
    {
        guiManager = GetComponent<GUIManager>();
        //Debug.LogWarning(guiManager == null);
        mRect = GetComponent<RectTransform>();
        DontDestroyOnLoad(this);

        SetUpDebug();
    }

    public float GetCanvasScale
    {
        get
        {
            if (null == mRect)
            {
                return 1;
            }
            return mRect.transform.localScale.x;
        }

    }

    public Rect GetCanvasRect
    {
        get { return mRect.rect; }

    }

  


    #region show hide for each of gui

    public static void HideAllGUI()
    {

        while (Instance.showingGUIs.Count > 0)
        {
            Instance.HideGUI(Instance.showingGUIs[0]);
        }
        Instance.splashScreen.SetActive(false);
    } // HideAllGui ()

    public void ShowGUIFader(GUIDialogBase dialog, object param = null)
    {
        ScreenFader.Instance.FadeIn(0.5f, () =>
        {
            guiManager.ShowPanel(dialog, param);
            showingGUIs.Add(dialog);
            ScreenFader.Instance.FadeOut(1f);
        });

    }

    public void ShowGUI(GUIDialogBase dialog, params object[] param)
    {
        guiManager.ShowPanel(dialog, param);
        Debug.Log("Show GUI : " + dialog.name);

        if (false == showingGUIs.Contains(dialog))
        {
            showingGUIs.Add(dialog);
        }
    }
    // ShowGUI ()

    public void HideGUI(GUIDialogBase dialog)
    {
        // Debug.Log("Hide GUI : " + dialog.name);
        showingGUIs.Remove(dialog);
        guiManager.HidePanel(dialog);
    }
    // HideGUI ()

    public bool IsActiveDialog(GUIDialogBase dialog)
    {
        return showingGUIs.Exists(d => d == dialog);
    }
    // IsActiveDialog (GUIDialogBase)

    public int ActiveDialogCount()
    {
        return showingGUIs.Count;
    }

   
    #endregion


    #region show Resouces UI

    public async UniTask ShowWaiting(bool isWaiting, bool isOverlay = false)
    {
        Debug.LogWarning("Show Hide Waiting : " + isWaiting);
        if (!isWaiting)
        {
            await UniTask.Delay(200);
        }
        BlackBackground.SetActive(isWaiting);
        OverlayBackground.SetActive(isOverlay);
        waitingDialog.SetActive(isWaiting);
        var textContent = waitingDialog.GetComponentInChildren<TMPro.TMP_Text>();
        if (textContent != null)
        {
            if (!isWaiting)
                textContent.text = "Please waiting...";

        }
    }

    public void ShowWifiAlert(bool isShow, Action callback = null)
    {
        BlackBackground.SetActive(isShow);
        wifiAlertDialog.gameObject.SetActive(isShow);
        wifiAlertDialog.ShowWifiAlert(isShow, callback);
    } // showWifiAlert()

    public bool IsShowDialog(GUIDialogBase panelController)
    {
        return guiManager.IsShowDialog(panelController);
    }

    public bool IsOnlyShowingMainDialog()
    {
        if (2 == ActiveDialogCount() && mainDialog == IsShowDialog(mainDialog) && userInfoDialog == IsShowDialog(userInfoDialog))
        {
            return true;
        }
        return false;
    }

    public void ShowLoading(List<DataLoader> dataloaders, int extraLoaderCnt, Action<string> callback = null)
    {

        //if(!loadingDialogHandler.gameObject.activeSelf)
        {
            loadingDialogHandler.gameObject.SetActive(true);
            loadingDialogHandler.FinishedLoadingCallback = callback;
            loadingDialogHandler.OnBeginShow(dataloaders, extraLoaderCnt);
        }
    } // ShowLoading

    public void HideLoading(float delayTime = 0.2f)
    {

        if (loadingDialogHandler.gameObject.activeSelf)
        {
            DOVirtual.DelayedCall(delayTime, () =>
            {
                loadingDialogHandler.OnBeginHide();
                loadingDialogHandler.gameObject.SetActive(false);
            }, false);
        }
    }
    
    
    // public T ShowHubInGame<T>(HUB_POOLER hubType) where T : LItemInPooler
    // {
    //     GameObject element = hubPooler.SpawnAndAddIntoCache(hubType.ToString(), Vector3.zero);
    //     T uiElement = element.GetComponent<T>();
    //     uiElement.OnShow();
    //     element.transform.SetParent(hubCanvas.OtherAnchor);
    //     element.transform.localScale = Vector3.one;
    //     return uiElement;
    // }

    // public void HideHubInGame<T>(T item) where T : LItemInPooler
    // {
    //     // Debug.Log("HidHubPlayer inGame :" + item.name);
    //     hubPooler?.ReturnToPoolAndRemoveFromCache(item, true);
    // }
    
    #endregion


    #region Static Call

    public static void ShowMessageDialog(MessageBoxData messageBoxData)
    {
        Instance.messageDialogHandler.OnShow(messageBoxData);
    }

    public static void HideMessageDialog()
    {
        Instance.messageDialogHandler.OnHide();
    }

    public static async UniTask<ConformDialogHandler.EConfirmDialogResult> ShowConfirmDialog(string title, string content, string okText = "Okay", string cancelText = "Cancel")
    {
        try
        {
            Instance.BlackBackground.SetActive(true);
            return await Instance.conformDialogHandler.Show(title, content, okText, cancelText);
        }
        finally
        {
            Instance.BlackBackground.SetActive(false);

        }
    }

    public static bool IsPointerOverDialog(GUIDialogBase inputDialog, Vector2 inputPosition)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = inputPosition;
        // inputActions.PlayerInput.DragPosition.
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (RaycastResult result in results)
        {
            if (true == BUtil.IsChildrenOfObject(result.gameObject.transform, inputDialog.transform))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsPointerOverGameObject(GameObject inputObject, Vector2 inputPosition)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = inputPosition;
        // inputActions.PlayerInput.DragPosition.
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (RaycastResult result in results)
        {
            if (true == BUtil.IsChildrenOfObject(result.gameObject.transform, inputObject.transform))
            {
                return true;
            }
        }

        return false;
    }

    public static void ShowWalletQRCode()
    {
        Instance.ShowGUI(Instance.walletQRDialog);
    }

    public static void HideWalletQRCode()
    {
        Instance.HideGUI(Instance.walletQRDialog);
    }

    public void ShowWCInfo()
    {
        ShowGUI(walletconnectInfo);
    }

    public void HideWCInfo()
    {
        HideGUI(walletconnectInfo);
    }
    #endregion

    #region Debug

    private void SetUpDebug()
    {
        if (true == isDebug)
        {
            txtFpsCounter.gameObject.SetActive(true);

            SetupFPSCounter();
        }
        else
        {
            txtFpsCounter.gameObject.SetActive(false);
        }

    }
    private void SetupFPSCounter()
    {
        StartCoroutine(UpdateFPSCounter());
        IEnumerator UpdateFPSCounter()
        {
            int lastFrameIndex = 0;
            float[] frameDeltaTimeArray = new float[50];
            float total = 0;
            while (true)
            {
                frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
                lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

                total = 0;
                foreach (float deltaTime in frameDeltaTimeArray)
                {
                    total += deltaTime;
                }

                txtFpsCounter.text = Mathf.RoundToInt(frameDeltaTimeArray.Length / total).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
    }
    #endregion


}
