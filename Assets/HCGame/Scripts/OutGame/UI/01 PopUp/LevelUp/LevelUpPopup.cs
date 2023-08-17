using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LevelUpPopup : UIPopupView<LevelUpPopup>
{
    [SerializeField] private TMP_Text txtLevel;
    [SerializeField] private GameObject txtReward;
    [SerializeField] private ItemRewardLevelUp itemRewardDefault;
    [SerializeField] private Button btContinue;
    [SerializeField] private Transform holder;

    private void Awake()
    {
        btContinue.onClick.AddListener(Continue);
    }

    public void Show()
    {
        SetDefault();
        if (HCAppController.Instance.listLevelUp == null || HCAppController.Instance.listLevelUp.Count <= 0) return;
        var nextLevelUp = HCAppController.Instance.listLevelUp[HCAppController.Instance.listLevelUp.Count - 1];
        LoadData(nextLevelUp);
        HCAppController.Instance.listLevelUp.RemoveAt(HCAppController.Instance.listLevelUp.Count - 1);
    }

    public void LoadData(LevelUp levelUp)
    {
        SetDefault();
        txtLevel.text = levelUp.Level.ToString();
        if (levelUp.Reward == null || levelUp.Reward.Count <= 0) return;
        txtReward.SetActive(true);
        foreach (var reward in levelUp.Reward)
        {
            var itemReward = Instantiate(itemRewardDefault, holder);
            itemReward.Show(reward);
        }
    }

    public void Continue()
    {
        if (HCAppController.Instance.listLevelUp.Count <= 0)
        {
            ClosePopup();
            return;
        }

        var nextLevelUp = HCAppController.Instance.listLevelUp[HCAppController.Instance.listLevelUp.Count - 1];
        LoadData(nextLevelUp);
        HCAppController.Instance.listLevelUp.RemoveAt(HCAppController.Instance.listLevelUp.Count - 1);
    }

    public void SetDefault()
    {
        txtLevel.text = "";
        txtReward.SetActive(false);
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }
}