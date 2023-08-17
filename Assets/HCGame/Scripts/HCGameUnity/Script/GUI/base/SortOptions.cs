using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SortOptions : MonoBehaviour
{
    [SerializeField] private Sprite sprite_Sort_Up;
    [SerializeField] private Sprite sprite_Sort_Down;
    [SerializeField] private TMP_Text txtSort;
    [SerializeField] private Image imgSortDirection;
    [SerializeField] private TweenRectTransformLocalScale panelSortOptions;
    // Start is called before the first frame update


    private ESortType _currentSort;
    public ESortType SortType
    {
        get { return _currentSort; }
    }

    private ESortType _defaultSortType;

    public Action<ESortType> OnUpdateSortOption_Callback;

    private void UpdateSortSelectedUI(ESortType sortType)
    {
        switch (sortType)
        {
            case ESortType.Recent_Up:
                txtSort.text = "Most Recent";
                imgSortDirection.sprite = sprite_Sort_Up;
                break;
            case ESortType.Recent_Down:
                txtSort.text = "Most Recent";
                imgSortDirection.sprite = sprite_Sort_Down;
                break;
            case ESortType.Rarity_Up:
                txtSort.text = "Rarity";
                imgSortDirection.sprite = sprite_Sort_Up;
                break;
            case ESortType.Rarity_Down:
                txtSort.text = "Rarity";
                imgSortDirection.sprite = sprite_Sort_Down;
                break;
        }
    }

    public void Init(ESortType sortType)
    {
        _defaultSortType = sortType;
        UpdateSortUI((int)sortType);
        panelSortOptions.Reset();
    }

    public void btnOpenSortOptions_onClicked()
    {
        panelSortOptions.OnBeginShow();
    }

    public void btnCloseSortOptions_onClicked()
    {
        panelSortOptions.OnBeginHide();
    }

    public void UpdateSortUI(int inputSort)
    {
        _currentSort = (ESortType)inputSort;
        UpdateSortSelectedUI(_currentSort);
        btnCloseSortOptions_onClicked();
        OnUpdateSortOption_Callback?.Invoke(_currentSort);
    }

    public void ResetSortUI()
    {
        _currentSort = _defaultSortType;
        UpdateSortSelectedUI(_currentSort);
        btnCloseSortOptions_onClicked();
        panelSortOptions.Reset();
    }
}
