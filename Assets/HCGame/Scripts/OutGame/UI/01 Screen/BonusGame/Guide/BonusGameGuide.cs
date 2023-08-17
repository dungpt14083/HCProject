using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGameGuide : UIPopupView<BonusGameGuide>
{
    [SerializeField] private List<BonusGameGuideItem> itemGuide;

    public void ShowView(GuideBonusGameType type)
    {
        GameSignals.ClaimRewardDone.Dispatch(false);
        this.gameObject.SetActive(true);
        this.transform.SetAsLastSibling();
        for (int i = 0; i < itemGuide.Count; i++)
        {
            if (itemGuide[i].type == type)
            {
                itemGuide[i].ShowView(Close, true);
            }
            else
            {
                itemGuide[i].ShowView(Close, false);
            }
        }
    }

    public void Close()
    {
        GameSignals.ClaimRewardDone.Dispatch(true);
        this.gameObject.SetActive(false);
    }
}

public enum GuideBonusGameType
{
    Roulette,
    Scratch,
    RandomBox
}