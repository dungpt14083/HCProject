using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingTextvalue : MonoBehaviour
{
    public static LoadingTextvalue INS;
    [SerializeField] TMP_Text _loadingText;
    [SerializeField] float _timeLoading;
    [SerializeField] float _loadingProgress;
    // Start is called before the first frame update
    void Start()
    {
        INS = this;
      //  StartCoroutine(LoadTextValue());
    }

    // Update is called once per frame
   public void LoadTextValue()
    {
        _loadingProgress = 0f;
       
        DOTween.To(() => _loadingProgress, x => _loadingProgress = x, 1f, _timeLoading)
            .OnUpdate(() =>
            {
                _loadingText.SetText("{0}%", Mathf.RoundToInt(_loadingProgress * 100));
               
            }).OnComplete(() =>
            {
                Debug.Log("endTextLoading");
            });
    }
}
