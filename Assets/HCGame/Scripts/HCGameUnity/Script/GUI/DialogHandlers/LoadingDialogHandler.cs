using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
using DG.Tweening;

public class LoadingDialogHandler : MonoBehaviour {

    [SerializeField]
    private Image loadingBar;

    [SerializeField] private TMP_Text downloadText;

    public Action<string> FinishedLoadingCallback;

    List<DataLoader> dataLoaders = new List<DataLoader>();
    List<DataLoader> removeList = new List<DataLoader>();
    int totalCount = 0;
    int m_extraLoadingCount = 0;
    int m_extraCompleteCount = 0;

    private bool isStop = false;


    public void OnBeginShow(List<DataLoader> dataLoaders, int extraLoadingCnt) 
    {
        loadingBar.fillAmount = 0.0f;

        this.dataLoaders = dataLoaders;
        totalCount = dataLoaders.Count;
        isStop = false;

        m_extraLoadingCount = extraLoadingCnt;
        m_extraCompleteCount = 0;

        gameObject.SetActive(true);
        
    }

    public void OnBeginHide()
    {
    }

    public void FinishedLoading()
    {
        gameObject.SetActive(false);
    }


    private void stopLoading(string error = "")
    {
        isStop = true;
        if (FinishedLoadingCallback != null)
        {
            FinishedLoadingCallback(error);
        } // if
    }


    void Update() {
        if (isStop) {
            return;
        } // if is hiding , don't need update
        //stopLoading();
        float progress = 0.0f;
        string text = "Completed ...";
        float extraTotalValue = 0f;
        float extraCompleteValue = 0f;
        if (0 < m_extraLoadingCount)
        {
            extraTotalValue = 1f;
            extraCompleteValue = (float)m_extraCompleteCount / (float)m_extraLoadingCount;
        }
        if (dataLoaders.Count > 0)
        {
            int count = 0;
            text = "Loading: " + dataLoaders[0].LoaderName;
            foreach (var dataLoader in dataLoaders)
            {
                progress += dataLoader.progress;
                count++;
                loadingBar.fillAmount = dataLoader.progress;
                if (!dataLoader.Update())
                {
                    if (!string.IsNullOrEmpty(dataLoader.Error))
                    {
                        Debug.LogError("loading error : " + dataLoader.LoaderName);
                        stopLoading(dataLoader.Error);
                        return;
                    } // if
                    removeList.Add(dataLoader);
                    dataLoader.Release();
                }// if
            } // foreach
            foreach (var loader in removeList)
            {
                if (dataLoaders.Contains(loader))
                {
                    dataLoaders.Remove(loader);
                } // if
            } // foreach
            removeList.Clear();
            // Debug.Log("before process : " + progress);
            //progress = (progress + (totalCount - count)) / totalCount;
            progress = (progress + (totalCount - count) + extraCompleteValue) / (totalCount + extraTotalValue);
            // Debug.Log("after process : " + progress);
        }
        else if (0 < (m_extraLoadingCount - m_extraCompleteCount))
        {
            text = "Loading: ...";
            progress = (totalCount + extraCompleteValue) / (totalCount + extraTotalValue);
        }
        else
        {
            progress = 1.0f;
        } // else
        //Debug.Log("===process == " + progress);
        loadingBar.fillAmount = progress;

        var percent = Mathf.RoundToInt(progress * 100);
        downloadText.text = text + " " + percent +"%";
        if (loadingBar.fillAmount >= 1.0f)
        {
            stopLoading();
        } // if
    } // Update ()

    #region Delegate

    public void OnLoadingExtraAdvance()
    {
        m_extraCompleteCount++;
    }

    #endregion Delegate
}
