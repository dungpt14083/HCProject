using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonTab : UIButtonBase<TypeButtonTab>
{
    public override void OnSelectFilter(TypeButtonTab info)
    {
        if (this._info == info)
        {
            selectGo.gameObject.SetActive(true);
            disableGo.gameObject.SetActive(false);
        }
        else
        {
            disableGo.gameObject.SetActive(true);
            selectGo.gameObject.SetActive(false);
        }
    }
}

public enum TypeButtonTab
{
    Tab1,
    Tab2,
    None
}