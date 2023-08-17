using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScrollView : MonoBehaviour
{
    private Slider slider;
    private Vector2 visibleAreaSize;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform panelContent;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(Slider_OnValueChanged);

        RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        visibleAreaSize = new Vector2(scrollRectTransform.rect.width, scrollRectTransform.rect.height);
        scrollRect.onValueChanged.AddListener(ScrollView_OnValueChange);
    }

    private void Slider_OnValueChanged(float vol)
    {
        if(null == panelContent)
        {
            Debug.LogError("Slider!!! Null panel content!!!");
            return;
        }
        switch(slider.direction)
        {
            case Slider.Direction.LeftToRight:
            case Slider.Direction.RightToLeft:
                float scrollViewTotalWidth = panelContent.sizeDelta.x;
                float parentWidth = visibleAreaSize.x;
                if (panelContent.sizeDelta.x > parentWidth)
                {
                    scrollViewTotalWidth = panelContent.sizeDelta.x - parentWidth;
                    Debug.Log("Scroll view width: " + scrollViewTotalWidth);
                    panelContent.anchoredPosition = new Vector2(scrollViewTotalWidth * vol * -1.0f, panelContent.anchoredPosition.y);
                }
                break;
            case Slider.Direction.TopToBottom:
            case Slider.Direction.BottomToTop:
                float scrollViewTotalHeight = panelContent.sizeDelta.y;
                float parentHeight = visibleAreaSize.y;
                if (panelContent.sizeDelta.y > parentHeight)
                {
                    scrollViewTotalHeight = panelContent.sizeDelta.y - parentHeight;
                    Debug.Log("Scroll view height: " + scrollViewTotalHeight);
                    panelContent.anchoredPosition = new Vector2(panelContent.anchoredPosition.x, scrollViewTotalHeight * vol * 1.0f);
                }
                break;
        }
    }

    private void ScrollView_OnValueChange(Vector2 value)
    {
        switch (slider.direction)
        {
            case Slider.Direction.LeftToRight:
            case Slider.Direction.RightToLeft:
                float scrollViewTotalWidth = panelContent.sizeDelta.x;
                float parentWidth = visibleAreaSize.x;
                if (panelContent.sizeDelta.x > parentWidth)
                {
                    scrollViewTotalWidth = panelContent.sizeDelta.x - parentWidth;
                    slider.value = Mathf.Abs(Mathf.Floor(panelContent.anchoredPosition.x) / scrollViewTotalWidth);
                }
                break;
            case Slider.Direction.TopToBottom:
            case Slider.Direction.BottomToTop:
                float scrollViewTotalHeight = panelContent.sizeDelta.y;
                float parentHeight = visibleAreaSize.y;
                if (panelContent.sizeDelta.y > parentHeight)
                {
                    scrollViewTotalHeight = panelContent.sizeDelta.y - parentHeight;
                    Debug.Log("Scroll view height: " + scrollViewTotalHeight);
                    slider.value = Mathf.Abs(Mathf.Floor(panelContent.anchoredPosition.y) / scrollViewTotalHeight);
                }
                break;
        }
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(Slider_OnValueChanged);
        scrollRect.onValueChanged.RemoveListener(ScrollView_OnValueChange);
    }
}
