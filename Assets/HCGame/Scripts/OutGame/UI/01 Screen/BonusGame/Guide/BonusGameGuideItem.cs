using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class BonusGameGuideItem : MonoBehaviour
{
    public GuideBonusGameType type;
    [SerializeField] private List<GameObject> listGuideInItem;
    [SerializeField] private List<Image> toggleImage;
    [SerializeField] private TMP_Text textBtnNext;
    [SerializeField] private GameObject panelTutorial;

    private int _index = 0;
    private int _countGuideInItem;
    private Action _closeOrDoneGuide;

    public void ShowView(Action callback, bool isShow)
    {
        this.gameObject.SetActive(isShow);
        if (isShow)
        {
            _closeOrDoneGuide = callback;
        }
    }

    private void OnEnable()
    {
        _countGuideInItem = listGuideInItem.Count;
        ShowFirst();
    }

    private void ShowFirst()
    {
        _index = 0;
        textBtnNext.text = "Next";
        ShowViewGuideInItem(_index);
    }

    public void Next()
    {
        _index++;
        if (_index >= _countGuideInItem)
        {
            _index = 0;
            textBtnNext.text = "Done";
            
            Close();
            return;
        }

        ShowViewGuideInItem(_index);
    }

    private void ShowViewGuideInItem(int index)
    {
        for (int i = 0; i < _countGuideInItem; i++)
        {
            if (index == i)
            {
                listGuideInItem[i].SetActive(true);
                toggleImage[i].gameObject.SetActive(true);
            }
            else
            {
                listGuideInItem[i].SetActive(false);
                toggleImage[i].gameObject.SetActive(false);
            }
        }
    }

    public void Close()
    {
        if (panelTutorial != null)
        {
            panelTutorial.SetActive(false);

        }
        _closeOrDoneGuide?.Invoke();
    }
}