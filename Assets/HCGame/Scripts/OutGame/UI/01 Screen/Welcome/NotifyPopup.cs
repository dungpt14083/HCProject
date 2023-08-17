using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotifyPopup : UIPopupView<NotifyPopup>
{
    public Button btClose;
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public void Show(string _content, string _title = "NOTIFY")
    {
        btClose.onClick.RemoveAllListeners();
        btClose.onClick.AddListener(CloseView);
        title.text = _title;
        content.text = _content;
    }
}