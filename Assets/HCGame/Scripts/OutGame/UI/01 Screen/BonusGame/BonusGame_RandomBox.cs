using System;
using System.Collections;
using System.Collections.Generic;
using BonusGamePlay;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;

namespace BonusGame
{
    public class BonusGame_RandomBox : BonusGame_Screen
    {
        #region ThangRefactor

        [SerializeField] private string idleState;
        [SerializeField] private string openState;
        [SerializeField] private SkeletonGraphic graphic;

        public override void ShowView()
        {
            base.ShowView();
            ShowIdle();
            Executors.Instance.StartCoroutine(WaitToActive());
        }

        private IEnumerator WaitToActive()
        {
            yield return new WaitUntil(() => this.gameObject.activeSelf);
            SendInitRandomBox();
        }

        private void SendInitRandomBox()
        {
            if (!this.gameObject.activeSelf) return;
            BonusgameConnectionManager.Instance.InitRandomBox();
        }

        private void OnEnable()
        {
            _isUseTicket = true;
            TakeUserCurrencyAndCheck();
            RandomBoxSignals.ResetAnimationRandomBoxToIdle.AddListener(ShowIdle);
            RandomBoxSignals.InitRandomBox.AddListener(SendInitRandomBox);
            BonusGameConnection.Instance.OnReceiveInitRandomBox += OnDataReceiveAndInitRandomBox;
            BonusGameConnection.Instance.OnListRewardRandomBox += OnListRewardRandomBox;
            openButton.onClick.RemoveAllListeners();
            openButton.interactable = true;
            openButton.onClick.AddListener(StartOpenBox);
        }

        private void OnDisable()
        {
            BonusGameConnection.Instance.OnReceiveInitRandomBox -= OnDataReceiveAndInitRandomBox;
            BonusGameConnection.Instance.OnListRewardRandomBox -= OnListRewardRandomBox;
            RandomBoxSignals.ResetAnimationRandomBoxToIdle.RemoveListener(ShowIdle);
            RandomBoxSignals.InitRandomBox.RemoveListener(SendInitRandomBox);
        }

        private void StartOpenBox()
        {
            OpenBoxByAll(_isUseTicket);
        }

        private void OpenBoxByAll(bool isUseTicket)
        {
            openButton.interactable = false;
            RunAnimationBox(isUseTicket);
        }


        #region ANIMATION BOX

        private void ShowIdle()
        {
            graphic.Skeleton.SetSkin("default");
            graphic.AnimationState.ClearTracks();
            var track = graphic.AnimationState.SetAnimation(0, idleState, true);
            graphic.Skeleton.SetToSetupPose();
        }


        private void RunAnimationBox(bool isUseTicket)
        {
            BonusGame_Manager.Instance.IsRunning = true;
            graphic.Skeleton.SetSkin("default");
            graphic.AnimationState.ClearTracks();
            var track = graphic.AnimationState.SetAnimation(0, openState, false);
            track.Complete += entry =>
            {
                BonusgameConnectionManager.Instance.OpenRandomBox(isUseTicket);
                BonusGame_Manager.Instance.IsRunning = false;
            };
            graphic.Skeleton.SetToSetupPose();
        }

        #endregion


        #region SHOWREWARDPOPUP

        private void OnListRewardRandomBox(ListRewardRandomBox list)
        {
            Debug.LogWarning("OnListRewardRandomBox");
            if (list.Status == 400)
            {
                ShowError(list.MessageType, BonusGameType.Box);
            }
            else
            {
                var datas = new WheelItemData[list.Items.Count];
                for (var index = 0; index < list.Items.Count; index++)
                {
                    var item = list.Items[index];
                    var data = new WheelItemData()
                    {
                        rewardType = (wheelRewardType)item.Type,
                        value = item.Quantity,
                    };
                    datas[index] = data;
                }

                HcPopupManager.Instance.ShowMsgRandomBox(datas, OnRewardAnimationFinished);
            }
        }

        private void OnRewardAnimationFinished()
        {
            openButton.interactable = true;
            ShowIdle();
        }

        #endregion

        #endregion


        #region INITFREERANDOMBOX

        private void OnDataReceiveAndInitRandomBox(ListRewardRandomBox ramdomBox)
        {
            feeTicket = ramdomBox.FeeTicketUsed;
            feeHcGem = ramdomBox.FeeHcTokenUsed;
            TakeUserCurrencyAndCheck();
            ShowIdle();
        }

        protected override void TakeUserCurrencyAndCheck()
        {
            base.TakeUserCurrencyAndCheck();
            openButton.onClick.RemoveAllListeners();
            openButton.interactable = true;
            openButton.onClick.AddListener(StartOpenBox);
        }

        #endregion
    }
}