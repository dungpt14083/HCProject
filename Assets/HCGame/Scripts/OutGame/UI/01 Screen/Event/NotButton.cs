using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotButton : MonoBehaviour
{
    [SerializeField] private GameObject toggle;
    public int index;
    public void ShowToggle(int indexIn)
    {
        if (indexIn != index)
        {
            toggle.gameObject.SetActive(false);
        }
        else
        {
            toggle.gameObject.SetActive(true);
        }
    }
}