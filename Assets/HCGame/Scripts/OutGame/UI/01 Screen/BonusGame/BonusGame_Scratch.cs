using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO3;
using BonusGamePlay;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

namespace BonusGame
{
    public class BonusGame_Scratch : BonusGame_Screen
    {
        [SerializeField] private TMP_Text timeText;

        [SerializeField] private ScratchCard[] cards;

        //blinkImage
        [SerializeField] private Image blickImage;
        [SerializeField] private Color startColor = new Color(255, 255, 255);
        [SerializeField] private Color endColor = new Color(246, 217, 255);
        [Range(0, 10)] [SerializeField] private float speed = 1;

        [SerializeField] private Button btnScratchAll;

        private DataScratch _dataScratch;

        private bool isScratching;
        private float timing;

        private bool _isHaveError = false;
        private int _typeError = 1;

        private void OnEnable()
        {
            SetDefaultInitFirst();
            ScratchSignals.InitScratch.AddListener(SendToInitScratch);
            BonusGameConnection.Instance.OnListResponseScratch += OnDataReceiveAndInitScratch;
            BonusGameConnection.Instance.OnItemResponseScratch += OnDataResponseScratch;
        }

        private void OnDisable()
        {
            ScratchSignals.InitScratch.RemoveListener(SendToInitScratch);
            BonusGameConnection.Instance.OnListResponseScratch -= OnDataReceiveAndInitScratch;
            BonusGameConnection.Instance.OnItemResponseScratch -= OnDataResponseScratch;
        }

        private void SetDefaultInitFirst()
        {
            _isHaveError = false;
            _typeError = 1;
            _isUseTicket = true;
            TakeUserCurrencyAndCheck();
            AllowScratch(false);
        }

        private void Start()
        {
            openButton.onClick.AddListener(Scratch);
            AllowScratch(false);
        }

        public override void ShowView()
        {
            base.ShowView();
            Executors.Instance.StartCoroutine(WaitToActive());
            SendToInitScratch();
        }

        private IEnumerator WaitToActive()
        {
            yield return new WaitUntil(() => this.gameObject.activeSelf);
            SendToInitScratch();
        }

        #region INITSCRATCH

        private void SendToInitScratch()
        {
            if (!this.gameObject.activeSelf) return;
            BonusgameConnectionManager.Instance.InitScratch();
        }

        //Đây là nơi data nó sẽ để hiển thị ban đầu khi init
        private void OnDataReceiveAndInitScratch(ListResponseScratch listResponseScratch)
        {
            //Mình ban đầu inti chưa xd là quay đưc hay không nên mình sẽ ẩn thằng ScratchMesh và bật thằng hideresult lên
            ScratchSignals.ShowHideResult.Dispatch(true);
            ScratchSignals.ShowHideScratch.Dispatch(false);

            //InitButton một lần khi nhận data Init==>Sau này thao tác các cái button nhận thưởng lại bonuspopup lại cập nhật tiếp
            feeTicket = listResponseScratch.FeeTicketUsed;
            feeHcGem = listResponseScratch.FeeTicketUsed;
            TakeUserCurrencyAndCheck();

            for (var index = 0; index < listResponseScratch.Data.Count; index++)
            {
                var item = listResponseScratch.Data[index];
                cards[index].ShowView(item.Quantity, item.Type, item.BonusGameRewardName);
            }
        }

        #endregion

        #region SCRATCHING

        private void Update()
        {
            if (isScratching)
            {
                imageBlink();
                timing -= Time.deltaTime;
                var time = (int)timing;
                timeText.text = "00:0" + time;
                if (timing <= 0)
                {
                    EndScratch();
                }
            }
        }

        private void imageBlink()
        {
            blickImage.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
        }

        private void EndScratch()
        {
            isScratching = false;
            BonusGame_Manager.Instance.IsRunning = false;
            AllowScratch(false);
            blickImage.color = startColor;

            if (_dataScratch != null)
            {
                if ((wheelRewardType)_dataScratch.Type == wheelRewardType.jackpot)
                {
                    HcPopupManager.Instance.ShowBigJackPot(_dataScratch.Quantity, FirstCallback,
                        SecondCallback, false);
                }
                else
                {
                    HcPopupManager.Instance.ShowRewardWheelAndScratch((wheelRewardType)_dataScratch.Type,
                        _dataScratch.Quantity,
                        FirstCallback, SecondCallback, false);
                }
            }
        }

        private void FirstCallback()
        {
            timeText.gameObject.SetActive(false);
            foreach (var card in cards)
            {
                card.ResetRenderTexture();
            }
        }

        private void ShowScratchAll()
        {
        }


        private void ScratchAll()
        {
            timeText.gameObject.SetActive(false);
            btnScratchAll.interactable = false;
            ScratchSignals.ShowHideScratch.Dispatch(false);
            EndScratch();
        }

        private void SecondCallback()
        {
            openButton.interactable = true;
        }

        #endregion

        private void OnDataResponseScratch(ItemResponseScratch itemResponseScratch)
        {
            if (itemResponseScratch.Status == 400)
            {
                _isHaveError = true;
                _typeError = itemResponseScratch.MessageType;
                if (_isHaveError)
                {
                    ShowError(_typeError, BonusGameType.Scratch);
                }
            }
            else
            {
                ScratchSignals.ShowHideScratch.Dispatch(true);
                ScratchSignals.ShowHideResult.Dispatch(false);
                _isHaveError = false;
                var item = itemResponseScratch.Data;
                _dataScratch = item;
                timeText.gameObject.SetActive(true);
                openButton.interactable = false;
                isScratching = true;
                BonusGame_Manager.Instance.IsRunning = true;
                timing = 5.0f;
                AllowScratch(true);
                openButton.gameObject.SetActive(false);
                btnScratchAll.gameObject.SetActive(true);
                btnScratchAll.onClick.RemoveAllListeners();
                btnScratchAll.interactable = true;
                btnScratchAll.onClick.AddListener(ScratchAll);
            }
        }

        private void AllowScratch(bool isAllow)
        {
            foreach (var card in cards)
            {
                card.InputEnabled = isAllow;
            }
        }

        protected override void TakeUserCurrencyAndCheck()
        {
            base.TakeUserCurrencyAndCheck();
            btnScratchAll.gameObject.SetActive(false);
        }

        #region STARTSCRATCH

        private void Scratch()
        {
            BonusgameConnectionManager.Instance.StartScratch(_isUseTicket);
        }

        #endregion
    }
}