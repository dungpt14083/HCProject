using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchImage : MonoBehaviour
{
    [SerializeField] private Image imgTarget;
    [SerializeField] private Sprite sprite_A;
    [SerializeField] private Sprite sprite_B;
    [SerializeField] private bool preserveAspect;
    [SerializeField] private bool canOnlySwitchToB = false;

    public void Awake()
    {
        if(null == imgTarget)
        {
            imgTarget = GetComponent<Image>();
        }
    }

    public void SwitchSprite()
    {
        if(imgTarget.sprite == sprite_A)
        {
            imgTarget.sprite = sprite_B;
        }
        else if(imgTarget.sprite == sprite_B && false == canOnlySwitchToB)
        {
            imgTarget.sprite = sprite_A;
        }
        imgTarget.preserveAspect = preserveAspect;
    }

    public void SwitchToSrpiteA()
    {
        imgTarget.sprite = sprite_A;
        imgTarget.preserveAspect = preserveAspect;
    }
    public void SwitchToSrpiteB()
    {
        imgTarget.sprite = sprite_B;
        imgTarget.preserveAspect = preserveAspect;
    }

    public bool IsShowingSpriteA()
    {
        return imgTarget.sprite == sprite_A;
    }
}
