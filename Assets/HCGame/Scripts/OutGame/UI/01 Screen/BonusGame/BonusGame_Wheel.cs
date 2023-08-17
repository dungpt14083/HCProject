using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using BonusGame;
using System;
using BonusGamePlay;
using TMPro;

namespace BonusGame
{
    //pHẦN NÀY ĐÍNH CHÍNH LẠI VỚI SERVER VỀ TIỀN VỀ LÚC ĐẦU CÓ PHẢI LÀ GIÁ CỦA THĂNG VÉ XOAY HAY K
    public class BonusGame_Wheel : BonusGame_Screen
    {
        #region Sigleton

        public static BonusGame_Wheel ins;

        private void Awake()
        {
            if (ins != null)
            {
                Debug.LogError("Multi ins" + gameObject.name);
                Destroy(this);
            }

            ins = this;
        }

        #endregion

        [TitleGroup("___________  Config  __________")] [SerializeField] [TableList(ShowIndexLabels = true)]
        private List<WheelItemData> wheelList;

        [SerializeField] private List<WheelColor> wheelItemColorList;
        [SerializeField] public float wheelRotateTime;
        [SerializeField] public float offsetMoreTurnRotate;
        [SerializeField] private int indexWheelResult;

        [SerializeField] private Button pauseAutoSpin;
        [SerializeField] private GameObject bgWheel;

        //blinkImage
        [SerializeField] private Image blickImage;
        [SerializeField] private Color startColor = new Color(255, 255, 255);
        [SerializeField] private Color endColor = new Color(246, 217, 255);
        [SerializeField] private Color end2Color = new Color(246, 217, 255);
        [Range(0, 10)] [SerializeField] private float speed = 1;


        [TitleGroup("___________  State   __________")] [SerializeField]
        private wheelStateEnum gameState;

        [TitleGroup("___________ Spin   __________")] [SerializeField]
        private Toggle autoSpinToggle;

        [SerializeField] private TMP_Text txtUseTurn;
        [SerializeField] private GameObject timeNextFree;
        [SerializeField] private TMP_Text txtTimeNextFree;

        private Data data;
        private Queue<Data> _itemResponses = new Queue<Data>();

        #region STARTCLASSINSTANCE

        private void Start()
        {
            autoSpinToggle.onValueChanged.AddListener(ToggleValueChanged);
            pauseAutoSpin.onClick.AddListener(PauseAutoSpin);
        }


        private void imageBlink()
        {
            blickImage.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
            iconArrow.color = Color.Lerp(startColor, end2Color, Mathf.PingPong(Time.time * speed, 1));
        }


        private void OnEnable()
        {
            SetDefaultForWheel();
            SetState(wheelStateEnum.start);
            WheelSignals.InitWheel.AddListener(InitWheel);
            BonusGameConnection.Instance.OnRouletteResultReceive += OnResultReceive;
            BonusGameConnection.Instance.OnRouletteItemsReceive += OnItemsReceive;
        }

        private void OnDisable()
        {
            SetDefaultWheelParent();
            WheelSignals.InitWheel.RemoveListener(InitWheel);
            BonusGameConnection.Instance.OnRouletteResultReceive -= OnResultReceive;
            BonusGameConnection.Instance.OnRouletteItemsReceive -= OnItemsReceive;
        }

        private void SetDefaultForWheel()
        {
            SetDefaultWheelParent();
            _isPauseSpin = false;
            _freeUseTurn = 0;
            IsAutoSpin = false;
            IsFirstSpin = true;
            _isUseTicket = true;
        }

        #endregion


        #region INITWHEEL

        public override void ShowView()
        {
            base.ShowView();
            Executors.Instance.StartCoroutine(WaitToActive());
            InitWheel();
        }

        private IEnumerator WaitToActive()
        {
            yield return new WaitUntil(() => this.gameObject.activeSelf);
            InitWheel();
        }

        public void InitWheel()
        {
            bgWheel.gameObject.SetActive(true);
            if (!this.gameObject.activeSelf) return;
            BonusgameConnectionManager.Instance.InitWheel();
        }

        private void Update()
        {
            if (gameState == wheelStateEnum.rotating)
            {
                imageBlink();
            }
        }

        private void OnItemsReceive(ItemResponse items)
        {
            SetDefaultWheelParent();
            ShowWheelNew(items);
        }


        #region SHOWTIMERAMAIN

        private Coroutine _autoTimeRemain;

        private void ShowTimeRemain(int timeLeft)
        {
            if (_autoTimeRemain != null) StopCoroutine(_autoTimeRemain);
            _autoTimeRemain = StartCoroutine(RunTimeRemain(timeLeft));
        }

        private IEnumerator RunTimeRemain(int timeLeft)
        {
            var tmp = timeLeft;
            while (true)
            {
                var timeOk = TimeSpan.FromSeconds(tmp);
                txtTimeNextFree.text = $"Next Free : {timeOk.Hours:00}h" + $"{timeOk.Minutes:00}m";
                if (tmp <= 0)
                {
                    InitWheel();
                    yield break;
                }

                yield return new WaitForSeconds(1.0f);
                tmp--;
            }
        }

        #endregion


        private void ShowWheelNew(ItemResponse items)
        {
            //yield return new WaitForEndOfFrame();
            bgWheel.gameObject.SetActive(false);
            FreeUseTurn = items.IsUsedFreeTurn;
            feeTicket = items.FeeTicketUsed;
            feeHcGem = items.FeeHcTokenUsed;

            timeNextFree.gameObject.SetActive(FreeUseTurn == 1);
            if (FreeUseTurn == 1)
            {
                ShowTimeRemain(items.TimeToNextFree);
            }

            TakeUserCurrencyAndCheck();

            wheelList.Clear();
            foreach (var item in items.Items)
            {
                var data = new WheelItemData()
                {
                    rewardType = (wheelRewardType)item.Type,
                    value = item.Quantity,
                };
                wheelList.Add(data);
            }

            if (wheelList.Count == 0) return;
            var degreePerWheelItem = 360 / wheelList.Count;
            float wheelItemFillAmount = 1f / wheelList.Count;
            var wheelIndex = 0;
            wheelList.ForEach(wheel =>
            {
                var newWheelItem = BonusPool.Spawn(wheelItemBGPref, wheelParent.transform);
                newWheelItem.name += "__" + wheelIndex;

                //Từ đây lấy thằng vị trí mới của background
                var tempRotation = newWheelItem.GetComponent<RectTransform>().eulerAngles;
                tempRotation.z = degreePerWheelItem * wheelIndex;
                newWheelItem.GetComponent<RectTransform>().eulerAngles = tempRotation;

                //Đây ộng thêm từ thằng background cộng thêm độ thêm
                var wheelItemScript = newWheelItem.GetComponent<WheelItem>();

                wheelItemScript.myItemPref.transform.localEulerAngles = new Vector3(
                    wheelItemScript.myItemPref.transform.localEulerAngles.x,
                    wheelItemScript.myItemPref.transform.localEulerAngles.y,
                    /*wheelItemScript.myItemPref.transform..z + */(degreePerWheelItem * 0.5f));


                wheelItemScript.myWheelItemBG.color = wheelItemColorList[wheelIndex].originalColor;
                wheelItemScript.myOriginalColor = wheelItemColorList[wheelIndex].originalColor;
                wheelItemScript.myBlinkColor = wheelItemColorList[wheelIndex].blinkColor;

                // setup number value for item
                wheelItemScript.myName.text = "" + wheelList[wheelIndex].value;

                if (wheelList[wheelIndex].rewardType == wheelRewardType.noReward) //no reward chuc ban may  man lan sau
                {
                    wheelItemScript.ShowItem(false, wheelList[wheelIndex].rewardType, "good luck next time!");
                }
                else if (wheelList[wheelIndex].rewardType == wheelRewardType.add_turn)
                {
                    wheelItemScript.ShowItem(false, wheelList[wheelIndex].rewardType, "" + "you again " +
                        wheelList[wheelIndex].value + " turn" +
                        (wheelList[wheelIndex].value == 1 ? "" : "s"));
                }
                else if (wheelList[wheelIndex].rewardType == wheelRewardType.jackpot)
                {
                    wheelItemScript.ShowItem(false, wheelList[wheelIndex].rewardType,
                        $"{wheelList[wheelIndex].value:00}% jackpot");
                }
                else
                {
                    wheelItemScript.ShowItem(true, wheelList[wheelIndex].rewardType,
                        wheelList[wheelIndex].value.ToString(), wheelList[wheelIndex].value);
                }

                newWheelItem.transform.eulerAngles = new Vector3(
                    newWheelItem.transform.eulerAngles.x,
                    newWheelItem.transform.eulerAngles.y,
                    /*newWheelItem.transform.eulerAngles.z +*/ 105.0f + degreePerWheelItem * wheelIndex);

                wheelItemScript.myWheelItemBG.fillAmount = wheelItemFillAmount + .001f;
                wheelIndex++;
            });
        }

        private void SetDefaultWheelParent()
        {
            for (int i = wheelParent.childCount - 1; i >= 0; i--)
            {
                BonusPool.DeSpawn(wheelParent.GetChild(i));
            }
        }

        #endregion


        #region SETTINGSHOWBUTTONSPIN

        [SerializeField] private Image iconSpin;
        [SerializeField] private List<Sprite> spriteIcon;
        [SerializeField] private Image iconSpinBg;
        [SerializeField] private Image iconArrow;


        //HIỂN THỊ DỰA VÀO THẰNG LƯỢT FREETURN HOẶC LÀ SỬ DỤNG TICKET HOẶC LÀ SỬ DỤNG HCTOKEN
        protected override void TakeUserCurrencyAndCheck()
        {
            var userTicket = HCAppController.Instance.userInfo.UserTicket;
            var userToken = HCAppController.Instance.userInfo.UserToken;

            if (FreeUseTurn == 0)
            {
                SettingButtonFreeTurn();
                iconSpin.sprite = spriteIcon[0];
                iconSpin.SetNativeSize();
                iconSpinBg.sprite = spriteIcon[0];
                iconSpinBg.SetNativeSize();
            }
            else if (_isUseTicket)
            {
                SettingButtonPlay(true, userTicket >= feeTicket ? true : false);
                iconSpin.sprite = spriteIcon[1];
                iconSpin.SetNativeSize();
                iconSpinBg.sprite = spriteIcon[1];
                iconSpinBg.SetNativeSize();
            }
            else
            {
                SettingButtonPlay(false, userToken >= feeHcGem ? true : false);
                iconSpin.sprite = spriteIcon[1];
                iconSpin.SetNativeSize();
                iconSpinBg.sprite = spriteIcon[1];
                iconSpinBg.SetNativeSize();
            }
        }

        protected void SettingButtonFreeTurn()
        {
            openButton.onClick.RemoveAllListeners();
            txtPrice.gameObject.SetActive(false);
            txtUseTurn.gameObject.SetActive(true);
            txtUseTurn.text = $"Free";
            BonusGame_Manager.Instance.SetImageReward(imgPrice, wheelRewardType.ticket);
            openButton.onClick.AddListener(RotateWheel);
        }

        #endregion

        #region SENDSTARTWWHEELAFTERINIT

        public void RotateWheel()
        {
            openButton.interactable = false;
            BonusgameConnectionManager.Instance.StartRotateWheel(_isUseTicket);
        }

        protected override void SettingButtonPlay(bool isUseTicket, bool isEnoughCurrency)
        {
            base.SettingButtonPlay(isUseTicket, isEnoughCurrency);
            txtUseTurn.gameObject.SetActive(false);
            openButton.onClick.RemoveAllListeners();
            openButton.onClick.AddListener(RotateWheel);
        }

        #endregion


        #region RECEIVEDRESPONSPIN

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnResultReceive(Response response)
        {
            data = response.Data;
            indexWheelResult = response.Index - 1;
            if (response.Status == 400)
            {
                //Tùy lỗi k đủ tiền hoặc là không đủ 1 2 để client sủa đã.
                ShowError(response.MessageType, BonusGameType.Wheel);
            }
            else
            {
                OnResponse();
            }
        }

        private void OnResponse()
        {
            indexWheelResult++;
            StartRotate();
        }

        private void StartRotate()
        {
            SetState(wheelStateEnum.rotating);
            var wheelIndexToRotate = indexWheelResult;

            var posToRotate = new Vector3(0, 0, 0);
            if (wheelIndexToRotate < wheelParent.transform.childCount)
            {
                posToRotate = wheelParent.transform.GetChild(wheelIndexToRotate).GetComponent<WheelItem>()
                    .iconShowHaveIcon
                    .transform.position;
            }
            else if (wheelParent.transform.childCount > 0)
            {
                posToRotate = wheelParent.transform.GetChild(0).GetComponent<WheelItem>()
                    .iconShowHaveIcon
                    .transform.position;
            }

            var dirToRotate = posToRotate - wheelParent.transform.position;

            var angleToRotate = Mathf.Atan2(dirToRotate.y, dirToRotate.x) * Mathf.Rad2Deg;

            angleToRotate += 120.0f;
            if (angleToRotate < 0)
            {
                angleToRotate = 360 - angleToRotate * -1;
            }

            angleToRotate = -angleToRotate - 120;

            if (angleToRotate >= 0)
            {
                angleToRotate = -270 + ( /*-(30 - */angleToRotate);
            }

            angleToRotate += -360 * offsetMoreTurnRotate;

            wheelParent.transform
                .DOLocalRotate(new Vector3(0, 0, angleToRotate), wheelRotateTime, RotateMode.LocalAxisAdd)
                .OnComplete(() => { OnFinishWheel(); });
            wheelBorder.transform.DOLocalRotate(new Vector3(0, 0, angleToRotate), wheelRotateTime,
                RotateMode.LocalAxisAdd);
        }

        //KHI KẾT THÚC XOAY THÌ CHECK AUTOSPIN HAY KHNG NÈ VÀ TÌM CÁCH HIỂN THỊ THĂNG QUÀ LÀ THẰNG NÀO BIGJACPOT HAY THƯỜNG
        public void OnFinishWheel()
        {
            blickImage.color = startColor;
            //Xoay xong thì hiện nút có thể auto spin::
            if (IsFirstSpin == true)
            {
                IsFirstSpin = false;
            }

            currentWheelLightUp.RepeatBlink(() =>
            {
                if ((wheelRewardType)data.Type == wheelRewardType.jackpot)
                {
                    HcPopupManager.Instance.ShowBigJackPot(data.Quantity, () =>
                    {
                        //Animation tiền bay lên::
                    }, CheckAutoSpin, IsAutoSpin);
                }
                else
                {
                    SetState(wheelStateEnum.finish);

                    HcPopupManager.Instance.ShowRewardWheelAndScratch((wheelRewardType)data.Type, data.Quantity,
                        () => { }, CheckAutoSpin, IsAutoSpin);
                }
            });
        }


        private void CheckAutoSpin()
        {
            SetState(wheelStateEnum.finish);
            openButton.interactable = true;
            if (IsAutoSpin)
            {
                RotateWheel();
            }
        }

        private void AutoSpinFirst()
        {
            if (gameState == wheelStateEnum.rotating) return;
            if (IsAutoSpin)
            {
                RotateWheel();
            }
        }


        public void SetState(wheelStateEnum state)
        {
            gameState = state;
            switch (gameState)
            {
                case wheelStateEnum.none:
                    break;
                case wheelStateEnum.start:
                    break;
                case wheelStateEnum.rotating:
                    BonusGame_Manager.Instance.IsRunning = true;
                    break;
                case wheelStateEnum.finish:
                    BonusGame_Manager.Instance.IsRunning = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region FUNCTIONFORPAUSEANDAUTOSPINBUTTON

        //lẦN ĐẦU KHÔNG HIỂN THỊ AUTOSPIN KHI ONENABLE
        //tHÌ SẼ ẨN CÁI THẰNG AUTOSPIN ĐI NHÉ...
        private bool _isFirstSpin = true;

        private bool IsFirstSpin
        {
            get { return _isFirstSpin; }
            set
            {
                _isFirstSpin = value;
                if (!value)
                {
                    autoSpinToggle.gameObject.SetActive(true);
                    autoSpinToggle.isOn = false;
                }
                else
                {
                    autoSpinToggle.gameObject.SetActive(false);
                    openButton.gameObject.SetActive(true);
                }
            }
        }


        //NẤU MÀ ẤN VÀO THẰNG AUTOSPIN CHECK THÌ BẮT ĐÂU ĐẨY FLAG VÀO ĐỂ CHẠY SETACTIVE NÚT PAU TRONG ĐÓ HAY K
        private bool _IsAutoSpin = false;

        private void ToggleValueChanged(bool value)
        {
            if (value != IsAutoSpin)
            {
                IsAutoSpin = value;
            }
        }

        private bool IsAutoSpin
        {
            get { return _IsAutoSpin; }
            set
            {
                _IsAutoSpin = value;
                IsPauseSpin = value;
                if (value)
                {
                    AutoSpinFirst();
                }
            }
        }


        [Button]
        //KHI NÚT PAUSE ĐƯỢC ẤN THÌ HẠY VÀO PAUSE VÀ SET 
        private void PauseAutoSpin()
        {
            autoSpinToggle.isOn = false;
            //pauseAutoSpin.GetComponent<Image>().color = !_isPauseSpin ? Color.white : Color.gray;
        }

        private bool _isPauseSpin = false;

        private bool IsPauseSpin
        {
            get { return _isPauseSpin; }
            set
            {
                _isPauseSpin = value;
                pauseAutoSpin.gameObject.SetActive(value);
                openButton.gameObject.SetActive(!value);
            }
        }

        //CÁI FREEUSETURN ĐỂ CHẠY
        private int _freeUseTurn = 0;

        private int FreeUseTurn
        {
            get { return _freeUseTurn; }
            set
            {
                _freeUseTurn = value;
                TakeUserCurrencyAndCheck();
            }
        }

        protected override bool IsUseTicket
        {
            get { return _isUseTicket; }
            set
            {
                _isUseTicket = value;
                PauseAutoSpin();
            }
        }

        #endregion


        [TitleGroup("___________  Reference  __________")] [SerializeField]
        private WheelItem wheelItemBGPref;

        [SerializeField] private RectTransform wheelParent;
        [SerializeField] private RectTransform wheelBorder;


        public WheelItem currentWheelLightUp;

        void OnGUI()
        {
            GUI.color = Color.red;
            GUIStyle guiStyle = new GUIStyle("button");
            guiStyle.fontSize = 50;
        }
    }


    #region SubClassForWheel

    [System.Serializable]
    [SerializeField]
    public class WheelItemData
    {
        public wheelRewardType rewardType;
        public int value;
    }

    [System.Serializable]
    public class WheelColor
    {
        public Color originalColor;
        public Color blinkColor;
    }

    public enum wheelRewardType
    {
        hc_token = 2,
        gold = 1,
        noReward = 6,
        add_turn = 5,
        ticket = 3,
        jackpot = 4
    }

    public enum wheelStateEnum
    {
        none,
        start,
        rotating,
        finish,
    }

    #endregion
}