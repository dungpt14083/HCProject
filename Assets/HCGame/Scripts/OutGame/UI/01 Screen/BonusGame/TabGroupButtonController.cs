using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabGroupButtonController : MonoBehaviour
{
    public List<GameObject> objectListGroupButton;
    private int _currentIndex = 0;
    public Button btnNext;
    public TMP_Text textIndex;

    private void Awake()
    {
        btnNext.onClick.AddListener(ShowNextObject);
    }

    private void Start()
    {
        for (int i = 1; i < objectListGroupButton.Count; i++)
        {
            objectListGroupButton[i].gameObject.SetActive(false);
        }
        UpdateIndexText();
    }

    private void ShowNextObject()
    {
        objectListGroupButton[_currentIndex].gameObject.SetActive(false);
        _currentIndex++;
        if (_currentIndex >= objectListGroupButton.Count)
        {
            _currentIndex = 0;
        }

        objectListGroupButton[_currentIndex].gameObject.SetActive(true);
        UpdateIndexText();
    }
    private void UpdateIndexText()
    {
        textIndex.text = (_currentIndex + 1).ToString() + " / " + objectListGroupButton.Count.ToString();
    }
}