using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Invite : UIPopupView<Invite>
{
    public TMP_Text txtReferra;
    public TMP_Text txtInviteStatus;
    public Button btCopy;
    public Slider progressbar;
    public ItemRewardInvite itemRewardDefault;
    public Button btnClose, BtnBgClose;

    protected override void Awake()
    {
        base.Awake();
        btCopy.onClick.AddListener(CopyReferra);
        btnClose.onClick.AddListener(CloseView);
        BtnBgClose.onClick.AddListener(CloseView);
    }

    
    public void Show()
    {
        var numberInvited = HCAppController.Instance.userInfo.NumberInvited;
        var maxNumberInvited = HCAppController.Instance.userInfo.MaxNumberInvited;
        //Data Test
        List<Reward> rewards = new List<Reward>
        {
            new Reward { Reward_ = 10, RewardType = (int)RewardType.Gold },
            new Reward { Reward_ = 10, RewardType = (int)RewardType.Gold },
            new Reward { Reward_ = 10, RewardType = (int)RewardType.Gold },
            new Reward { Reward_ = 10, RewardType = (int)RewardType.Gold },
            new Reward { Reward_ = 10, RewardType = (int)RewardType.Gold },
        };
        // HCAppController.Instance.userInfo.ReferralCode = "abc";
        /////////

        txtReferra.text = HCAppController.Instance.userInfo.ReferralCode;
        txtInviteStatus.text = $"(You invited {numberInvited}/{maxNumberInvited} friends)";
        Debug.Log("loginvint" + txtInviteStatus.text.ToString());
        Debug.Log("log invint" + maxNumberInvited);

        progressbar.value = maxNumberInvited > 0 ? numberInvited / (float)maxNumberInvited : 1;

        HCHelper.DestroyChil(itemRewardDefault.transform.parent, false);
        for (int i = 0; i < rewards.Count; i++)
        {
            var itemReward = Instantiate(itemRewardDefault, itemRewardDefault.transform.parent);
            itemReward.Show(i+1, rewards[i].Reward_, (RewardType)rewards[i].RewardType);
        }

      
    }

    public void Hide()
    {
      CloseView();
    }

    public string GetTitle()
    {
        return "Invite";
    }

    public void CopyReferra()
    {
        UniClipboard.SetText(HCAppController.Instance.userInfo.ReferralCode);
        Debug.Log("UniClipboard CopyRefernal " + UniClipboard.GetText());
    }
}