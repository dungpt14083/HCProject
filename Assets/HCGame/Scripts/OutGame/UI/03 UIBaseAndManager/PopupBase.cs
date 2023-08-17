using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class PopupBase : MonoBehaviour
{




    // Start is called before the first frame update
    [TitleGroup("___________  Reference  __________")]
    [SerializeField]
    private CanvasGroup myCanvasGroup;


    [SerializeField]
    public GameObject frame;



    [TitleGroup("___________  Config  __________")]
    [SerializeField]
    private float fadeTime = .3f;



    public void OnClickCloseBtn()
    {
        DOTween.To(() => myCanvasGroup.alpha, v => myCanvasGroup.alpha = v, 0, fadeTime).Play().OnComplete(() =>
        {
            DisableDisplay();
        });
    }

    [Button]
    public void DisplaySelf()
    {
        myCanvasGroup.blocksRaycasts = true;
        myCanvasGroup.interactable = true;
        frame.gameObject.SetActive(true);
        myCanvasGroup.alpha = 0;
        DOTween.To(() => myCanvasGroup.alpha, v => myCanvasGroup.alpha = v, 1, fadeTime).Play().OnComplete(() =>
        {
        });
    }

    private void Start()
    {
        DisableDisplay();
    }
    public void DisableDisplay()
    {
        frame.gameObject.SetActive(false);
        myCanvasGroup.alpha = 0;
        myCanvasGroup.interactable = false;
        myCanvasGroup.blocksRaycasts = false;
    }
}
