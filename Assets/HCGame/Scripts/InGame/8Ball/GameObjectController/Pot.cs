using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Pot : MonoBehaviour
{
    [SerializeField]private TMP_Text text;
    private int point;
    [HideInInspector]
    public int Point => point;

    public void SetPoint(int point)
    {
        this.point = point;
        if (text != null)
        {
            text.gameObject.SetActive(true);
            text.text = point.ToString();
        }
    }
    public void HidePoint()
    {
        if (text != null) text.gameObject.SetActive(false);
    }
}
