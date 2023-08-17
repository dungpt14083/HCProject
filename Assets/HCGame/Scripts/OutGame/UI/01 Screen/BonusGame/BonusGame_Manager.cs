using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cache = Unity.VisualScripting.Cache;

namespace BonusGame
{
    public class BonusGame_Manager : UIView<BonusGame_Manager>
    {
        public Image opacityBG;
        public Sprite hcToken, gold, ticket, x1Turn;

        [SerializeField] private Button game1Btn, game2Btn, game3Btn;
        [SerializeField] private BonusGame_Wheel bonusGameWheel;
        [SerializeField] private BonusGame_Scratch bonusGameScratch;
        [SerializeField] private BonusGame_RandomBox bonusGameRandomBox;
        [SerializeField] private GameObject frame;
        [SerializeField] private TMPro.TextMeshProUGUI gameName;
        [SerializeField] private Image iconGame;
        [SerializeField] private List<Sprite> listSpriteIcon;

        private string Roulette = "Roulette";
        private string Scratch = "Scratch";
        private string RandomBox = "Random box";

        private string RouletteURL = "";
        private string ScratchURL = "";
        private string RandomBoxURL = "";

        private BonusGameType _currentType = BonusGameType.None;

        public bool IsRunning { get; set; }

        private void Start()
        {
            Application.targetFrameRate = 60;
            game1Btn.onClick.AddListener(() => ShowViewBonusGame(BonusGameType.Wheel));
            game2Btn.onClick.AddListener(() => ShowViewBonusGame(BonusGameType.Scratch));
            game3Btn.onClick.AddListener(() => ShowViewBonusGame(BonusGameType.Box));
        }

        private void OnEnable()
        {
            IsRunning = false;
            GameSignals.CloseAllPopup.AddListener(CloseView);
        }


        private void OnDisable()
        {
            GameSignals.CloseAllPopup.RemoveListener(CloseView);
            BonusgameConnectionManager.Instance.Disconnect();
        }

        public void ShowGuides()
        {
            if (IsRunning) return;
            BonusGameGuide.OpenPopup(guide =>
            {
                switch (_currentType)
                {
                    case BonusGameType.Wheel:
                        guide.ShowView(GuideBonusGameType.Roulette);
                        break;
                    case BonusGameType.Scratch:
                        guide.ShowView(GuideBonusGameType.Scratch);
                        break;
                    case BonusGameType.Box:
                        guide.ShowView(GuideBonusGameType.RandomBox);
                        break;
                    default:
                        break;
                }
            });
        }

        #region FadeInFadeOutHaveRay

        public void ShowOpacity()
        {
            opacityBG.color = new Color(0, 0, 0, 0);
            opacityBG.raycastTarget = true;
            opacityBG.DOFade(0.7f, 1);
        }

        public void HideOpacity(Action cb)
        {
            opacityBG.DOFade(0, 1).OnComplete(() =>
            {
                cb?.Invoke();
                opacityBG.raycastTarget = false;
            });
        }

        public void ClosePopupReward()
        {
            BonusGameSignals.ClosePopupReward.Dispatch();
        }

        #endregion

        public void DelayedCall(float time, Action cb)
        {
            StartCoroutine(DelayedCalled());

            IEnumerator DelayedCalled()
            {
                yield return new WaitForSeconds(time);
                cb.Invoke();
            }
        }

        public void SetImageReward(Image image, wheelRewardType rewardType)
        {
            image.color = Color.white;
            switch (rewardType)
            {
                case wheelRewardType.hc_token:
                    image.sprite = ResourceManager.Instance.GetIconReward(RewardType.Token);
                    break;
                case wheelRewardType.gold:
                    image.sprite = ResourceManager.Instance.GetIconReward(RewardType.Gold);
                    ;
                    break;
                case wheelRewardType.noReward:
                    image.color = new Color(0, 0, 0, 0);
                    break;
                case wheelRewardType.add_turn:
                    image.sprite = x1Turn;
                    break;
                case wheelRewardType.ticket:
                    image.sprite = ResourceManager.Instance.GetIconReward(RewardType.Ticket);
                    break;
            }
        }


        /*
        #region TODOGETTOPUSSER

        private Coroutine _coroutineGetTopUser;

        public void ShowTopUser()
        {
            if (IsRunning)
            {
                return;
            }

            if (_coroutineGetTopUser != null)
            {
                StopCoroutine(_coroutineGetTopUser);
            }

            var url = GameConfig.API_URL + GameConfig.API_TAIL_GETTOPUSERBONUSGAME + $"{activeGame}" +
                      "?access_token=" +
                      $"{HCAppController.Instance.GetTokenLogin()}" + "&deviceId=" +
                      $"{HCAppController.Instance.currentDeviceId}";
            _coroutineGetTopUser =
                StartCoroutine(APIUtils.RequestWebApiGetJson(url, SuccesShowTopBonusGame, ErrorShowTopBonusGame));
        }

        private void SuccesShowTopBonusGame(JObject obj)
        {
            bonusGameRankPopup.ShowView(GuideBonusGameType.Roulette, new ListResponseRank(obj));
        }

        private void ErrorShowTopBonusGame(string error)
        {
            //
        }

        #endregion
        */

        public void ShowCurrentBonusGame()
        {
            ShowViewBonusGame(_currentType);
        }


        [Button]
        public void ShowViewBonusGame(BonusGameType bonusGameType)
        {
            if (IsRunning) return;
            SetDefault();
            _currentType = bonusGameType;
            switch (bonusGameType)
            {
                case BonusGameType.Wheel:
                    gameName.text = Roulette;
                    iconGame.sprite = listSpriteIcon[0];
                    iconGame.SetNativeSize();
                    bonusGameWheel.ShowView();
                    break;
                case BonusGameType.Scratch:
                    gameName.text = Scratch;
                    iconGame.sprite = listSpriteIcon[1];
                    iconGame.SetNativeSize();
                    bonusGameScratch.ShowView();
                    break;
                case BonusGameType.Box:
                    gameName.text = RandomBox;
                    iconGame.sprite = listSpriteIcon[2];
                    iconGame.SetNativeSize();
                    bonusGameRandomBox.ShowView();
                    break;
                default:
                    break;
            }
        }


        private void SetDefault()
        {
            _currentType = BonusGameType.None;
            bonusGameWheel.CloseView();
            bonusGameScratch.CloseView();
            bonusGameRandomBox.CloseView();
        }
    }


    public class ListResponseRank
    {
        public List<ResBonusGameRankDTO> list = new List<ResBonusGameRankDTO>();
        public long countTime = 0;

        public ListResponseRank(JObject data)
        {
            countTime = (long)data["countTime"];
            list = CalListData((JArray)data["bonusGameTopRankDTOS"]);
        }

        private List<ResBonusGameRankDTO> CalListData(JArray arrayData)
        {
            var tmp = new List<ResBonusGameRankDTO>();

            foreach (var jDataJObject in arrayData)
            {
                tmp.Add(new ResBonusGameRankDTO(jDataJObject));
            }

            tmp.OrderBy(s => s.totalReward).ToList();
            return tmp;
        }
    }

    public class ResBonusGameRankDTO
    {
        public long hcUserId;
        public string avatar;
        public string username;
        public long totalReward;

        public ResBonusGameRankDTO(JToken data)
        {
            hcUserId = (long)data["hcUserId"];
            avatar = (string)data["avatar"];
            username = (string)data["username"];
            totalReward = (long)data["totalReward"];
        }
    }
}