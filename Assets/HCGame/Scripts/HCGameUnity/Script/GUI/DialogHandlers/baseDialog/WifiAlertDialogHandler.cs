using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WifiAlertDialogHandler : MonoBehaviour {

    private Action reloadCallback;
    private Action quitCallback;

    bool isWifiAlert;
    bool isError;

    //==== wifi
    const string WIFI_TEXTURE_PATH = "IncludeData/Texture/WifiError/FB_ui_wifi_0";

    [SerializeField]
    private Image wifiIconImage;


    // ===== error




    #region WifiAlert

    public void ShowWifiAlert(bool isShow, Action reloadAction) {
        isWifiAlert = isShow;
        reloadCallback = reloadAction;
    } // ShowWifiAlert ()

    int imageCount = 3;
    float timechangeCount = 0.0f;
    private void wifiAlertUpdate () {
        timechangeCount += Time.deltaTime;
        if(timechangeCount >= 0.5) {
            
            timechangeCount = 0.0f;
            string imagePath = WIFI_TEXTURE_PATH + imageCount;

            wifiIconImage.sprite = Resources.Load<Sprite>(imagePath);
            if(imageCount >= 7) {
                imageCount = 3;
            }else {
                imageCount++;
            } // else
            //Debug.LogWarning("update wifi image: " + wifiIconImage.sprite.name);
        } // if
    } // wifiAlertUpdate ()
    #endregion


    void Update () {
        // wifi check
        if (isWifiAlert)
        {
            wifiAlertUpdate();
        }
        
    }

}
