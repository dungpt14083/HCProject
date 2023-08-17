using RoyalMatch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
namespace MiniGame.MatchThree.Scripts.Network
{
    public class Match3_TopPanel : MonoBehaviour
    {
        public static Match3_TopPanel Instance;
        public TMP_Text userName1;
        public Image userAvatar1;
        public TMP_Text userName2;
        public Image userAvatar2;
        public GameObject objUser2;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogError("MULTI INSTANCE");
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            objUser2.SetActive(false);
            SetupSelf();
            if (HCAppController.Instance.findingRoomResponse != null)
            {
                if (HCAppController.Instance.findingRoomResponse.Mode == 2)
                {
                    objUser2.SetActive(true);
                }
            }
            HCAppController.Instance.LoadUserInfoInGame(HCAppController.Instance.findingRoomResponse, userName1, userAvatar1, userName2, userAvatar2);
        }



        public float _baseWidth, _percentWidth;
        public void SetupSelf()
        {
            _baseWidth = _frontTimeBar.rectTransform.sizeDelta.x;
        }

        public TextMeshProUGUI player2ScoreText, player2UserName;
        public void UpdateData(Response data)
        {
            Debug.LogWarning(data.DataGameOrtherPlayer.NumberSwap);
            Debug.LogWarning(data.DataGameOrtherPlayer.Points);
            Debug.LogWarning(data.DataGameOrtherPlayer.Status);
            Debug.LogWarning(data.DataGameOrtherPlayer.Nickname);

            player2ScoreText.text = "" + data.DataGameOrtherPlayer.Points;
            player2UserName.text = "" + data.DataGameOrtherPlayer.Nickname;
        }

        public TextMeshProUGUI user1ScoreText;
        public int _curTimeLeft;
        public TextMeshProUGUI _textTimeBar;
        public Image _frontTimeBar;
        public bool _isPauseGame;
        public bool isCounting = false;
        [Button]
        public void CountDownTime(PuzzleDataJson data)
        {
            if (isCounting) return;
            isCounting = true;


            var timePerRound = data.timePlay;
            _curTimeLeft = timePerRound;
            _curTimeLeft -= 4;// when received this value its already 2 second passed


            StartCoroutine(CountDownBar());
            IEnumerator CountDownBar()
            {
                while (_curTimeLeft > 0)
                {
                    _curTimeLeft--;
                    _percentWidth = (float)_curTimeLeft / (float)timePerRound;
                    _frontTimeBar.rectTransform.sizeDelta = new Vector2(_baseWidth * _percentWidth, _frontTimeBar.rectTransform.sizeDelta.y);

                    var minus = _curTimeLeft / 60;
                    var seconds = _curTimeLeft % 60;

                    _textTimeBar.text = "0" + minus + ":" + (seconds <= 9 ? "0" + seconds : seconds);
                    yield return new WaitForSeconds(1);
                }
                //function end ganme is called from server,


            }
        }


        [Button]
        public void SetPauseGame(bool value)
        {
            _isPauseGame = value;
        }

        public List<Sprite> targetSpiteList;
        public GameObject targetList;

        public PuzzleDataJson _dataTemp;
        [Button]
        public void SynchGameTarget(PuzzleDataJson data = null)
        {
            _dataTemp = data;
            if (data == null)
            {
                data = _dataTemp;
            }


            int idx = 0;
            data.target.ForEach(tar =>
            {
                targetList.transform.GetChild(idx).GetChild(0).GetComponent<Image>().sprite = targetSpiteList[tar.type];
                targetList.transform.GetChild(idx).GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + tar.amount;
                idx++;
            });



            foreach (Transform child in targetList.transform)
            {
                if (child.GetChild(0).GetComponent<Image>().sprite == null) child.gameObject.SetActive(false);
            }

        }

    }



}

