using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpdateSizeBaseOnResolution : MonoBehaviour
{
    [Serializable]
    public class SizeOnResolution
    {
        public Vector2 size;
        public float resolutionAspect;
    }

    //[SerializeField] private List<SizeOnResolution> sizeOnResolutionList = new List<SizeOnResolution>() 
    //{
    //    new SizeOnResolution(){size = new Vector2(1920, 1080), resolutionAspect = 1.78f},
    //    new SizeOnResolution(){size = new Vector2(1920, 1200), resolutionAspect = 1.6f },
    //};

    private SizeOnResolution originalSize = new SizeOnResolution();

    private RectTransform rectTransform;
    private Vector2 currentResolution = new Vector2();
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        originalSize.size = rectTransform.sizeDelta;
        originalSize.resolutionAspect = (originalSize.size.x) / (originalSize.size.y);
        originalSize.resolutionAspect = Mathf.Round(originalSize.resolutionAspect * 100) / 100;

        currentResolution.x = Screen.width;
        currentResolution.y = Screen.height;
        UpdateSize();
    }

    private void FixedUpdate()
    {
        if(currentResolution.x != Screen.width || currentResolution.y != Screen.height)
        {
            UpdateSize();

            currentResolution.x = Screen.width;
            currentResolution.y = Screen.height;
        }
    }

    public void UpdateSize()
    {
        if(null == rectTransform)
        {
            Debug.LogError("UpdateSizeBaseOnResolution!!! Null rect transform!!!");
            return;
        }
        float currentAspect = ((float)Screen.width) / ((float)Screen.height);
        //Round currentAspect to 2 decimal place
        currentAspect = Mathf.Round(currentAspect * 100) / 100;

        //foreach (SizeOnResolution sizeOnResolution in sizeOnResolutionList)
        //{
        //    if (currentAspect == sizeOnResolution.resolutionAspect)
        //    {
        //        rectTransform.sizeDelta = sizeOnResolution.size;
        //        return;
        //    }
        //}

        //No fixed size exist, so switch to auto sizing
        float sizingByWidth_Height = Screen.width * originalSize.size.y / originalSize.size.x;
        if(sizingByWidth_Height >= Screen.height)
        {
            rectTransform.sizeDelta = new Vector2(Screen.width / HCGUIManager.Instance.GetCanvasScale, sizingByWidth_Height / HCGUIManager.Instance.GetCanvasScale);
            return;
        }

        float sizingByHeight_Width = Screen.height * originalSize.size.x / originalSize.size.y;
        if (sizingByHeight_Width >= Screen.width)
        {
            rectTransform.sizeDelta = new Vector2(sizingByHeight_Width / HCGUIManager.Instance.GetCanvasScale, Screen.height / HCGUIManager.Instance.GetCanvasScale);
            return;
        }

        Debug.LogError("UpdateSizeBaseOnResolution!!!" + rectTransform.gameObject.name + " No available aspect found for current solution!!! Current aspect: " + currentAspect + "! Current resolution " + Screen.width + "=" + Screen.height);
    }
}
