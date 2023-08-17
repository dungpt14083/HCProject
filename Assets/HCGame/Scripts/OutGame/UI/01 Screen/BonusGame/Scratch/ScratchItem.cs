using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchItem : MonoBehaviour
{
    [SerializeField] private GameObject surface;
    [SerializeField] private GameObject hideResult;

    private void OnEnable()
    {
        ShowScratch(false);
        ShowHideResult(true);
        ScratchSignals.ShowHideResult.AddListener(ShowHideResult);
        ScratchSignals.ShowHideScratch.AddListener(ShowScratch);
    }

    private void OnDisable()
    {
        ScratchSignals.ShowHideResult.RemoveListener(ShowHideResult);
        ScratchSignals.ShowHideScratch.RemoveListener(ShowScratch);
    }

    private void ShowScratch(bool isShow)
    {
        surface.SetActive(isShow);
    }

    private void ShowHideResult(bool flag)
    {
        hideResult.SetActive(flag);
    }
}