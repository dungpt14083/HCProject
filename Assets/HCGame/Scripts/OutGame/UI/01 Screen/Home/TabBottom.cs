using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;

public class TabBottom : MonoBehaviour
{
    public TabBottomType tabBottomType;
    public Button btTab;
    public Image icon;
    public Image bg;
    public TMP_Text title;
    public Action callbackShowPage;
    public Action callbackShowPageFirst;

    private Vector2 sizeNormal = new Vector2(120, 110);
    private Vector2 sizeScale = new Vector2(150, 140);
    private Color32 colorHideImg = new Color32(255, 255, 255, 1);
    private Color32 colorActiveImg = new Color32(255, 255, 255, 255);

    private bool isSelected = false;
    public Action hideAllButton;

    private void Awake()
    {
        btTab.onClick.AddListener(TabClick);
    }

    public void TabClick()
    {
        hideAllButton?.Invoke();
        btTab.image.color = colorHideImg;
        bg.enabled = true;
        isSelected = true;
        if (tabBottomType == TabBottomType.Home)
        {
            ChangeIcon(ResourceManager.Instance?.sprHomeIcon);
        }
        else if (tabBottomType == TabBottomType.Event)
        {
            ChangeIcon(ResourceManager.Instance?.sprEventIcon);
        }

        Debug.Log($"tabBottomType {tabBottomType} isSelected {isSelected}");
        callbackShowPage?.Invoke();
        if (!isSelected)
        {
            EffectSelect();
        }
    }

    public void TabClickFirst()
    {
        hideAllButton?.Invoke();
        btTab.image.color = colorHideImg;
        bg.enabled = true;
        isSelected = true;
        if (tabBottomType == TabBottomType.Home)
        {
            ChangeIcon(ResourceManager.Instance?.sprHomeIcon);
        }
        else if (tabBottomType == TabBottomType.Event)
        {
            ChangeIcon(ResourceManager.Instance?.sprEventIcon);
        }

        Debug.Log($"tabBottomType {tabBottomType} isSelected {isSelected}");
        callbackShowPageFirst?.Invoke();
        if (!isSelected)
        {
            EffectSelect();
        }
    }

    public void ChangeIcon(Sprite iconNew)
    {
        icon.sprite = iconNew;
    }

    public void EffectSelect()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(icon.rectTransform.DOSizeDelta(sizeScale, 0.5f));
    }

    public void UnTabClick()
    {
        Debug.Log("fsfsssssssssssssssssssss");
        btTab.image.color = colorActiveImg;
        bg.enabled = false;
        EffectUnSelect();
    }

    public void EffectUnSelect()
    {
        if (isSelected)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(icon.rectTransform.DOSizeDelta(sizeNormal, 0.5f));
        }
        else
        {
            icon.rectTransform.sizeDelta = sizeNormal;
        }

        isSelected = false;
    }
}