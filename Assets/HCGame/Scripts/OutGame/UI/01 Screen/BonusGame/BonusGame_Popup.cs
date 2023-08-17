using System;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
#if UNITY_WEBGL == false
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.IO.Pem;
#endif
using UnityEngine.TerrainUtils;

namespace BonusGame
{
    public class BonusGame_Popup : UIPopupView<BonusGame_Popup>
    {
        public GameObject startPos, stayPos, endPos;
        [SerializeField] private MsgItem[] msgItems;
        [SerializeField] private GameObject msgNotice;
        [SerializeField] private GameObject msgNoticeBigJackPot;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button buttonClaim;
        [SerializeField] private Button buttonClaimBigJackPot;
        [SerializeField] private Button closeBigJackPot;
        [SerializeField] private TMP_Text valueBigJackPot;
        [SerializeField] private float coolDownTimer = 1.0f;

        [SerializeField] private Button buttonClose;

        private void OnEnable()
        {
            BonusGameSignals.ClosePopupReward.AddListener(CloseInvoke);
        }

        private void OnDisable()
        {
            BonusGameSignals.ClosePopupReward.RemoveListener(CloseInvoke);
        }

        private void CloseInvoke()
        {
            buttonClose.onClick.Invoke();
        }

        private Coroutine _coolDownCoroutine;

        public void ShowRewardWheelAndScratch(wheelRewardType rewardType, int value, Action firstCallback,
            Action callback, bool isAutoSpin)
        {
            msgNoticeBigJackPot.SetActive(false);
            GameSignals.ClaimRewardDone.Dispatch(false);
            ScratchSignals.ShowHideScratch.Dispatch(false);
            SetDefaultShowItems();
            msgItems[0].gameObject.SetActive(true);
            msgItems[0].SetValueWheelAndScratch(rewardType, value);

            var size = 540;
            var tmp = content.sizeDelta;
            tmp.x = size;
            content.sizeDelta = tmp;

            msgNotice.SetActive(true);
            msgNotice.transform.position = startPos.transform.position;
            msgNotice.transform.DOMoveX(stayPos.transform.position.x, 1);
            //.OnComplete((() => Debug.LogError(msgNotice.transform.ToString())));
            BonusGame_Manager.Instance.ShowOpacity();

            if (isAutoSpin)
            {
                buttonClaim.gameObject.SetActive(false);
                _coolDownCoroutine = StartCoroutine(CoolDownCloseWheel(firstCallback, callback));
            }
            else
            {
                buttonClaim.gameObject.SetActive(true);
                buttonClaim.onClick.RemoveAllListeners();
                buttonClaim.onClick.AddListener(() => { CloseRewardWheelAndScratchFunc(firstCallback, callback); });
            }

            buttonClose.onClick.RemoveAllListeners();
            buttonClose.onClick.AddListener(() => { CloseRewardWheelAndScratchFunc(firstCallback, callback); });
        }

        private IEnumerator CoolDownCloseWheel(Action firstCallback, Action callback)
        {
            yield return new WaitForSeconds(coolDownTimer);
            CloseRewardWheelAndScratchFunc(firstCallback, callback);
        }

        private void CloseRewardWheelAndScratchFunc(Action firstCallback, Action callback)
        {
            buttonClose.onClick.RemoveAllListeners();
            buttonClaim.onClick.RemoveAllListeners();
            UpdateMoney();

            ShowAnimation();
            msgNotice.transform.DOMoveX(endPos.transform.position.x, 1).OnComplete(() =>
            {
                ScratchSignals.InitScratch.Dispatch();
                WheelSignals.InitWheel.Dispatch();
                GameSignals.ClaimRewardDone.Dispatch(true);
                msgNotice.SetActive(false);
            });
            firstCallback?.Invoke();
            BonusGame_Manager.Instance.HideOpacity(callback);
        }


        public void ShowMsgRandomBox(WheelItemData[] data, Action callback)
        {
            msgNoticeBigJackPot.SetActive(false);
            SetDefaultShowItems();
            for (var i = 0; i < data.Length; i++)
            {
                msgItems[i].gameObject.SetActive(true);
                msgItems[i].SetValue(data[i]);
            }

            var size = data.Length >= 3 ? 840 : 540;
            var tmp = content.sizeDelta;
            tmp.x = size;
            content.sizeDelta = tmp;

            msgNotice.SetActive(true);
            msgNotice.transform.position = startPos.transform.position;
            msgNotice.transform.DOMoveX(stayPos.transform.position.x, 1);
            BonusGame_Manager.Instance.ShowOpacity();

            buttonClaim.onClick.RemoveAllListeners();
            buttonClaim.onClick.AddListener(() => { CloseRewardRandomBoxFunc(callback); });
            buttonClose.onClick.RemoveAllListeners();
            buttonClose.onClick.AddListener(() => { CloseRewardRandomBoxFunc(callback); });
        }

        private void CloseRewardRandomBoxFunc(Action callback)
        {
            buttonClose.onClick.RemoveAllListeners();
            buttonClaim.onClick.RemoveAllListeners();
            ShowAnimation();
            UpdateMoney();

            msgNotice.transform.DOMoveX(endPos.transform.position.x, 1).OnComplete(() =>
            {
                callback?.Invoke();
                RandomBoxSignals.InitRandomBox.Dispatch();
                RandomBoxSignals.ResetAnimationRandomBoxToIdle.Dispatch();
                msgNotice.SetActive(false);
            });
            BonusGame_Manager.Instance.HideOpacity(null);
        }

        private void ErrorAddmoney(string obj)
        {
            //throw new NotImplementedException();
        }

        private void SuccessRewardAddMoney(JObject obj)
        {
            //throw new NotImplementedException();
        }


        private void ShowAnimation()
        {
            List<wheelRewardType> type = new List<wheelRewardType>();
            for (int i = 0; i < msgItems.Length; i++)
            {
                if (msgItems[i].gameObject.activeSelf)
                {
                    if (!type.Contains(msgItems[i].Type))
                    {
                        type.Add(msgItems[i].Type);
                    }
                }
            }

            CoinFlyAnimation.Instance.SpawnListBonusGameRewardClaim(type, new Vector2(0, 0), null);
        }

        public void ShowBigJackPot(int value, Action firstCallback, Action callback, bool isAutoSpin)
        {
            ScratchSignals.ShowHideScratch.Dispatch(false);
            msgNoticeBigJackPot.SetActive(true);
            msgNotice.SetActive(false);
            valueBigJackPot.text = value.ToString();

            msgNoticeBigJackPot.transform.position = startPos.transform.position;
            msgNoticeBigJackPot.transform.DOMoveX(stayPos.transform.position.x, 1);
            BonusGame_Manager.Instance.ShowOpacity();


            if (_coolDownCoroutine != null)
            {
                StopCoroutine(_coolDownCoroutine);
            }

            if (isAutoSpin)
            {
                buttonClaimBigJackPot.gameObject.SetActive(false);
                closeBigJackPot.gameObject.SetActive(false);
                _coolDownCoroutine = StartCoroutine(CoolDownCloseBigJackPot(callback));
            }
            else
            {
                buttonClaimBigJackPot.gameObject.SetActive(true);
                closeBigJackPot.gameObject.SetActive(true);
                buttonClaimBigJackPot.onClick.RemoveAllListeners();
                buttonClaimBigJackPot.onClick.AddListener(() => { CloseBigJackPotFunc(callback); });
                closeBigJackPot.onClick.RemoveAllListeners();
                closeBigJackPot.onClick.AddListener(() => { CloseBigJackPotFunc(callback); });
            }
        }

        private IEnumerator CoolDownCloseBigJackPot(Action callback)
        {
            yield return new WaitForSeconds(coolDownTimer);
            CloseBigJackPotFunc(callback);
        }

        private void CloseBigJackPotFunc(Action callback)
        {
            msgNoticeBigJackPot.transform.DOMoveX(endPos.transform.position.x, 1).OnComplete(() =>
            {
                callback?.Invoke();
                ScratchSignals.InitScratch.Dispatch();
                GameSignals.ClaimRewardDone.Dispatch(true);
                msgNoticeBigJackPot.SetActive(false);
            });
            BonusGame_Manager.Instance.HideOpacity(null);
            UpdateMoney();
        }

        private void UpdateMoney()
        {
            var url = GameConfig.API_URL + GameConfig.API_TAIL_CLAIMBONUSGAME +
                      $"{HCAppController.Instance.userInfo.UserCodeId}?" +
                      $"access_token={HCAppController.Instance.GetTokenLogin()}" +
                      $"&deviceId={HCAppController.Instance.currentDeviceId}";
            StartCoroutine(APIUtils.RequestWebApiGetJsonBonusReward(url, SuccessRewardAddMoney, ErrorAddmoney));
        }


        private void SetDefaultShowItems()
        {
            for (int i = 0; i < msgItems.Length; i++)
            {
                msgItems[i].gameObject.SetActive(false);
            }
        }
    }
}