using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private void Start()
    {
        SetSizeScreen();
        this.transform.SetAsLastSibling();
    }
    private void SetSizeScreen()
    {
        var screenScaler = this.gameObject.GetComponent<CanvasScaler>();
        screenScaler.matchWidthOrHeight = Screen.height / (float)Screen.width >= 2
            ? 0f : Screen.height / (float)Screen.width >= 1.7 
                ? 0.5f : 1f;
    }
}

