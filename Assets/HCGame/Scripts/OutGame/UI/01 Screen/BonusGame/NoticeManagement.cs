using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

namespace BonusGame
{
    public class NoticeManagement : MonoBehaviour
    {
        [SerializeField] private GameObject noticeNotEnoughGems;
        [SerializeField] private GameObject noticeGuideUseGem;

        [SerializeField] private Button btnNoticeGuideUseGem;
        [SerializeField] private TMP_Text feeGuide;

        [SerializeField] private Button btnNoticeNotEnoughGems;
        
        [SerializeField] private TMP_Text tmpAction;
        [SerializeField] private TMP_Text tmpTitleFail;



        private Action _callBack;

        private void OnEnable()
        {
            SetDefault();
        }

        public void ShowNoticeGuideUseGem(int fee, Action callback,BonusGameType type)
        {
            noticeGuideUseGem.GameObject().SetActive(true);
            noticeNotEnoughGems.gameObject.SetActive(false);

            feeGuide.text = "Use " + fee.ToString();
            switch (type)
            {
                case BonusGameType.Box:
                    tmpAction.text = "Open";
                    break;
                case BonusGameType.Scratch:
                    tmpAction.text = "Scratch";
                    break;
                case BonusGameType.Wheel:
                    tmpAction.text = "Spin";
                    break;
                default:
                    break;
            }

            _callBack = callback;
            btnNoticeGuideUseGem.onClick.RemoveAllListeners();
            btnNoticeGuideUseGem.onClick.AddListener(InvokeCallBack);
        }

        public void ShowNoticeNotEnoughGems(Action callback,BonusGameType type)
        {
            noticeGuideUseGem.GameObject().SetActive(false);
            noticeNotEnoughGems.gameObject.SetActive(true);

            switch (type)
            {
                case BonusGameType.Box:
                    tmpTitleFail.text = "OPEN FAIL";
                    break;
                case BonusGameType.Scratch:
                    tmpTitleFail.text = "SCRATCH FAIL";
                    break;
                case BonusGameType.Wheel:
                    tmpTitleFail.text = "SPIN FAIL";
                    break;
                default:
                    break;
            }
            
            _callBack = callback;
            btnNoticeNotEnoughGems.onClick.RemoveAllListeners();
            btnNoticeNotEnoughGems.onClick.AddListener(InvokeCallBack);
        }

        private void InvokeCallBack()
        {
            SetDefault();
            _callBack?.Invoke();
        }

        private void SetDefault()
        {
            noticeNotEnoughGems.SetActive(false);
            noticeGuideUseGem.SetActive(false);
        }
    }
}