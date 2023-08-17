using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace BonusGame
{
    public class RewardAnimation : MonoBehaviour
    {
        [SerializeField] private Image[] imagess;
        private Sprite _sprite;
        [SerializeField] private Transform tokenPos, goldPos, ticketPos;
        [SerializeField] private Transform[] positions;
        private wheelRewardType _rewardType;

        private Vector3 _tokenPosV3, _goldPosV3, _ticketPosV3;

        private void Awake()
        {
            var tokenGameObjectPos = GameObject.FindWithTag("Token");
            _tokenPosV3 = tokenGameObjectPos ? tokenGameObjectPos.transform.position : tokenPos.position;
            var goldGameObjectPos = GameObject.FindWithTag("Gold");
            _goldPosV3 = goldGameObjectPos ? goldGameObjectPos.transform.position : goldPos.position;
            var ticketGameObjectPos = GameObject.FindWithTag("Ticket");
            _ticketPosV3 = ticketGameObjectPos ? ticketGameObjectPos.transform.position : ticketPos.position;
        }

        public void StartAnimation(wheelRewardType rewardType)
        {
            _rewardType = rewardType;
            switch (_rewardType)
            {
                case wheelRewardType.hc_token:
                    _sprite = ResourceManager.Instance.GetIconReward(RewardType.Token);
                    break;
                case wheelRewardType.gold:
                    _sprite = ResourceManager.Instance.GetIconReward(RewardType.Gold);
                    break;
                case wheelRewardType.noReward:
                    break;
                case wheelRewardType.add_turn:
                    _sprite = BonusGame_Manager.Instance.x1Turn;
                    break;
                case wheelRewardType.ticket:
                    _sprite = ResourceManager.Instance.GetIconReward(RewardType.Ticket);
                    break;
                case wheelRewardType.jackpot:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_rewardType), _rewardType, null);
            }

            Animate();
        }

        private void Animate()
        {
            for (var index = 0; index < imagess.Length; index++)
            {
                var item = imagess[index];
                item.gameObject.SetActive(true);
                item.sprite = _sprite;
                item.transform.DOMove(positions[index].position, 0.2f).SetDelay(index * 0.05f);
            }

            StartCoroutine(EndAnimation());
        }

        public IEnumerator EndAnimation()
        {
            yield return new WaitForSeconds(1.0f);
            var pos = Vector3.zero;
            switch (_rewardType)
            {
                case wheelRewardType.hc_token:
                    pos = _tokenPosV3;
                    break;
                case wheelRewardType.gold:
                    pos = _goldPosV3;
                    break;
                case wheelRewardType.noReward:
                    break;
                case wheelRewardType.add_turn:
                    break;
                case wheelRewardType.ticket:
                    pos = _ticketPosV3;
                    break;
                case wheelRewardType.jackpot:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_rewardType), _rewardType, null);
            }

            for (var index = 0; index < imagess.Length; index++)
            {
                var item = imagess[index];
                item.gameObject.SetActive(true);
                item.sprite = _sprite;
                item.transform.DOMove(pos, 0.5f).SetDelay(index * 0.05f).OnComplete(() =>
                {
                    item.gameObject.SetActive(false);
                    item.transform.DOLocalMove(Vector3.zero, 0f);
                    this.gameObject.SetActive(false);
                });
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        #region SHOWTMP

        private RewardType _rewardTypeTmp;

        public void StartAnimationTmp(RewardType rewardType)
        {
            _rewardTypeTmp = rewardType;
            switch (_rewardTypeTmp)
            {
                case RewardType.Token:
                    _sprite = BonusGame_Manager.Instance.hcToken;
                    break;
                case RewardType.Gold:
                    _sprite = BonusGame_Manager.Instance.gold;
                    break;
                case RewardType.Ticket:
                    _sprite = BonusGame_Manager.Instance.ticket;
                    break;
                default:
                    break;
            }
            AnimateTmp();
        }

        
        
        private void AnimateTmp()
        {
            for (var index = 0; index < imagess.Length; index++)
            {
                var item = imagess[index];
                item.gameObject.SetActive(true);
                item.sprite = _sprite;
                item.SetNativeSize();
                item.transform.DOMove(positions[index].position, 0.2f).SetDelay(index * 0.05f);
            }

            StartCoroutine(EndAnimationTmp());
        }

        public IEnumerator EndAnimationTmp()
        {
            yield return new WaitForSeconds(1.0f);
            var pos = Vector3.zero;
            switch (_rewardTypeTmp)
            {
                case RewardType.Token:
                    pos = _tokenPosV3;
                    break;
                case RewardType.Gold:
                    pos = _goldPosV3;
                    break;
                case RewardType.Ticket:
                    pos = _ticketPosV3;
                    break;
                default:
                    //
                break;
            }

            for (var index = 0; index < imagess.Length; index++)
            {
                var item = imagess[index];
                item.gameObject.SetActive(true);
                item.sprite = _sprite;
                item.transform.DOMove(pos, 0.5f).SetDelay(index * 0.05f).OnComplete(() =>
                {
                    item.gameObject.SetActive(false);
                    item.transform.DOLocalMove(Vector3.zero, 0f);
                    this.gameObject.SetActive(false);
                });
            }
        }
        
        
        
        
        
        
        #endregion
        
        
        
        
        
    }
}