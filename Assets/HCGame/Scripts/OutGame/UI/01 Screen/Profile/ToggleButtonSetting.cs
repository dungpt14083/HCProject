using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonSetting : MonoBehaviour
{
    [SerializeField] RectTransform uiHandleRectTransform ;
    [SerializeField] Color backgroundActiveColor ;
    [SerializeField] Color handleActiveColor ;

    Image  handleImage ;
    public Sprite HandleSpriteOn,HandleSpriteOff;
     Image backgroundImage;
    [SerializeField] private Sprite BackgroundDefult, BackgroundActive;

    Color backgroundDefaultColor, handleDefaultColor ;

    Toggle toggle ;

    Vector2 handlePosition ;

    void Awake ( ) {
        toggle = GetComponent <Toggle> ( ) ;

        handlePosition = uiHandleRectTransform.anchoredPosition ;

        backgroundImage = uiHandleRectTransform.parent.GetComponent <Image> ( ) ;
        handleImage = uiHandleRectTransform.GetComponent <Image> ( ) ;

        backgroundDefaultColor = backgroundImage.color ;
        handleDefaultColor = handleImage.color ;

        toggle.onValueChanged.AddListener (OnSwitch) ;

        if (toggle.isOn)
            OnSwitch (true) ;
    }

    void OnSwitch (bool on) {
        //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
        uiHandleRectTransform.DOAnchorPos (on ? handlePosition * -1 : handlePosition, .4f).SetEase (Ease.Linear) ;

       backgroundImage.sprite = on ? BackgroundDefult : BackgroundActive ; // no anim
      //  backgroundImage.sprite (on ? BackgroundDefult : BackgroundActive, .6f) ;
      handleImage.sprite = on ? HandleSpriteOn : HandleSpriteOff;
      // handleImage.sprite = on ? BackgroundActive : BackgroundDefult ; // no anim
      // handleImage.DOColor (on ? handleActiveColor : handleDefaultColor, .4f) ;
    }

    void OnDestroy ( ) {
        toggle.onValueChanged.RemoveListener (OnSwitch) ;
    }
}
