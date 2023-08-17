using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.U2D;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

namespace BonusGame
{
    public class WheelItem : MonoBehaviour
    {
        public TextMeshProUGUI myName;
        public Image myWheelItemBG;
        public GameObject myItemPref;
        public Color myBlinkColor, myOriginalColor;
        public float blinkTime, backTime;
        public int repeatBlinkTime;


        public GameObject showHaveIcon;
        public GameObject showOnlyText;

        public Image iconShowHaveIcon;
        public TMP_Text txtShowHaveIcon;
        public TMP_Text txtShowOnlyText;


        public void ShowItem(bool isShowHaveIcon, wheelRewardType typeReward, string content, int value = 0)
        {
            showHaveIcon.SetActive(isShowHaveIcon);
            showOnlyText.SetActive(!isShowHaveIcon);

            if (isShowHaveIcon)
            {
                BonusGame_Manager.Instance.SetImageReward(iconShowHaveIcon, typeReward);
                txtShowHaveIcon.text = StringUtils.FormatMoneyK(value);
            }
            else
            {
                txtShowOnlyText.text = content;
            }
        }


        [Button]
        public void LightUp()
        {
            /*
            myWheelItemBG.DOColor(myBlinkColor, blinkTime);
            if (BonusGame_Wheel.ins.currentWheelLightUp != null)
                BonusGame_Wheel.ins.currentWheelLightUp
                    .LightDown(); // when new wheelItem light up, the old wheel light down.
                    */
            BonusGame_Wheel.ins.currentWheelLightUp = this;
        }


        public void LightDown()
        {
            myWheelItemBG.DOColor(myOriginalColor, backTime);
        }

        [Button]
        public void RepeatBlink(Action onComplete)
        {
            float finishTime = 0;
            for (int i = 0; i < repeatBlinkTime; i++)
            {
                BonusGame_Manager.Instance.DelayedCall(i * .4f,
                    () =>
                    {
                        myWheelItemBG.DOColor(myOriginalColor, .2f).OnComplete(() =>
                        {
                            myWheelItemBG.DOColor(myBlinkColor, .2f);
                        });
                    });
                finishTime += .4f;
            }

            if (onComplete != null)
                BonusGame_Manager.Instance.DelayedCall(finishTime + 1f, () => { onComplete.Invoke(); });
        }
    }
}