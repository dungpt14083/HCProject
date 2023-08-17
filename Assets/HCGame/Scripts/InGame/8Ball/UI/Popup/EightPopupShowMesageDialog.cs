using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EightPopupShowMesageDialog : MonoBehaviour
{
    [SerializeField] AutoHidePopupUI _autoHidePopupUI;
    [SerializeField] TextMeshProUGUI _txtMesage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowMesagePopup(string txtMesage, float delay = 0, Action hideSuccessed = null)
    {       
        _txtMesage.SetText(txtMesage);
        _autoHidePopupUI.ShowPopup(delay, hideSuccessed);
    }
}
