using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace BubblesShot
{
    public class Tutorials : UIScreen
    {

        [SerializeField] private GameObject _step1;
        [SerializeField] private GameObject _step2;
        [SerializeField] private GameObject _step3;
        [SerializeField] private GameObject _step4;
        [SerializeField] private GameObject _step5;

        [SerializeField] private Image _status1;
        [SerializeField] private Image _status2;
        [SerializeField] private Image _status3;
        [SerializeField] private Image _status4;
        [SerializeField] private Image _status5;

        [SerializeField] private Sprite _statusShow;
        [SerializeField] private Sprite _statusHide;

        [SerializeField] private TextMeshProUGUI _btnText;

        private int _countOfStep;

        public static Tutorials Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        // Use this for initialization
        void Start()
        {
            _countOfStep = 1;
            UpdateUI();
        }

        public void OnClickNextButton()
        {
            _countOfStep++;
            if (_countOfStep <= 5)
                UpdateUI();
            else
            {
                OnClose();
            }
        }
        public void UpdateUI()
        {
            switch (_countOfStep)
            {
                case 1:
                    _step1.SetActive(true);
                    _step2.SetActive(false);
                    _step3.SetActive(false);
                    _step4.SetActive(false);
                    _step5.SetActive(false);
                    _status1.sprite = _statusShow;
                    _status2.sprite = _statusHide;
                    _status3.sprite = _statusHide;
                    _status4.sprite = _statusHide;
                    _status5.sprite = _statusHide;
                    _btnText.text = "Next";
                    break;
                case 2:
                    _step1.SetActive(false);
                    _step2.SetActive(true);
                    _step3.SetActive(false);
                    _step4.SetActive(false);
                    _step5.SetActive(false);
                    _status1.sprite = _statusHide;
                    _status2.sprite = _statusShow;
                    _status3.sprite = _statusHide;
                    _status4.sprite = _statusHide;
                    _status5.sprite = _statusHide;
                    _btnText.text = "Next";
                    break;
                case 3:
                    _step1.SetActive(false);
                    _step2.SetActive(false);
                    _step3.SetActive(true);
                    _step4.SetActive(false);
                    _step5.SetActive(false);
                    _status1.sprite = _statusHide;
                    _status2.sprite = _statusHide;
                    _status3.sprite = _statusShow;
                    _status4.sprite = _statusHide;
                    _status5.sprite = _statusHide;
                    _btnText.text = "Next";
                    break;
                case 4:
                    _step1.SetActive(false);
                    _step2.SetActive(false);
                    _step3.SetActive(false);
                    _step4.SetActive(true);
                    _step5.SetActive(false);
                    _status1.sprite = _statusHide;
                    _status2.sprite = _statusHide;
                    _status3.sprite = _statusHide;
                    _status4.sprite = _statusShow;
                    _status5.sprite = _statusHide;
                    _btnText.text = "Next";
                    break;
                case 5:
                    _step1.SetActive(false);
                    _step2.SetActive(false);
                    _step3.SetActive(false);
                    _step4.SetActive(false);
                    _step5.SetActive(true);
                    _status1.sprite = _statusHide;
                    _status2.sprite = _statusHide;
                    _status3.sprite = _statusHide;
                    _status4.sprite = _statusHide;
                    _status5.sprite = _statusShow;
                    _btnText.text = "Continue";
                    break;
            }
        }
        public void OnClose()
        {
            GamePlay.Instance.GameStatus = GameState.Playing;
            UIManager.Instance.HidePopup();
        }

        //For tutorial
        public void OnClickTutorialInHomeMenu()
        {
            SessionPref.SetTutorial(true);
            SceneManager.LoadScene("BubbleLoading");
        }
    }
}
