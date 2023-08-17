using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace BubblesShot
{
    public class FinishGame : UIScreen
    {
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _clearBoard;
        [SerializeField] private TextMeshProUGUI _bonusTime;
        [SerializeField] private TextMeshProUGUI _totalScore;
        [SerializeField] private Image _titleImg;
        [SerializeField] Sprite[] _titleSprites;

        DataGamePlay _dataGamePlay;
        // Start is called before the first frame update
        void Start()
        {
            //WebSocket - Send finish game

        }

        public void OnClickExit()
        {
            UIManager.Instance.HidePopup();
            SessionPref.ResetSessionPref();
            Application.Quit();
        }
        public override IEnumerator PushRoutine(ScreenInfo info)
        {
            if (info != null)
            {
                int score = info.TryGet<int>("score");
                bool isClearBoard = info.TryGet<bool>("isClearBoard");
                int timeRemain = info.TryGet<int>("timeRemain");
                int type = info.TryGet<int>("type");

                _score.text = score.ToString();
                if (isClearBoard)
                    _clearBoard.text = "1000";
                else
                    _clearBoard.text = "0";
                _bonusTime.text = (timeRemain * 10).ToString();

                int totalScore = (timeRemain * 10) + score;
                totalScore += isClearBoard ? 1000 : 0;
                _totalScore.text = totalScore.ToString();

                //
                _titleImg.sprite = _titleSprites[type];
            }

            yield return null;
        }
    }

}