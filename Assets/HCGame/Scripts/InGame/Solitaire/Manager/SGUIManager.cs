using System;
using ModestTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Sequence = Unity.VisualScripting.Sequence;

namespace Solitaire
{
    public class SGUIManager : SingletonMono<SGUIManager>
    {
        private Card _draggingCard = null;
        public GameObject cardPrefab;

        [SerializeField] private Stock stock;
        [SerializeField] private Transform wasteTransform;

        [SerializeField] private List<CardSlot> cardSlots;

        [SerializeField] private Foundation _foundation;

        [Header("UI")]
        [SerializeField] private TMP_Text txtTimer;

        [SerializeField] private TMP_Text txtScore;
        [SerializeField] private TMP_Text txtCompetitionScore;

        [SerializeField] private TMP_Text txtUsername;
        [SerializeField] private TMP_Text txtCompetitionName;

        [SerializeField] private Image imgUserAvatar;
        [SerializeField] private Image imgCompetitionAvatar;

        [SerializeField] private GameObject btnUndo;

        [SerializeField] public GameObject layerChatGreat;
        [SerializeField] public int oldScore=0;
        [SerializeField] public int newScoreadd=0;
        
        //control menu
        [SerializeField] private Button btnShowFinishedGame;
        [SerializeField] private Button btnShowMenu;
        [SerializeField] private GameObject DetailMenu;
        public GameObject panelCompetionInfo;
        public GameObject panelCompetionInfo2;
        private bool isShowMenu=true;

        [SerializeField] private GameObject panelWaitForResult;

        [SerializeField] private GameObject panelResult;
        [SerializeField] private TMP_Text txtResult;

        [SerializeField] private GameObject btnAuto;

        [Header("Submit score")]
        [SerializeField] private GameObject panelConfirmSubmit;
        //[SerializeField] private GameObject panelSubmitScore;
        //[SerializeField] private GameObject panelSubmitScoreTimeUp;
        //[SerializeField] private TMP_Text txtSubmitTitle;
        //[SerializeField] private TMP_Text txtSubmitScore;
        //[SerializeField] private TMP_Text txtSubmitTimeBonus;
        //[SerializeField] private TMP_Text txtSubmitFinalScore;

        public TMP_Text userName1;
        public Image userAvatar1;
        public TMP_Text userName2;
        public Image userAvatar2;

        [Header("Waitting rom")]
        [SerializeField] private GameObject panelConnectToServer;
        [SerializeField] private GameObject panelLogin;
        [SerializeField] private GameObject panelRoom;
        [SerializeField] private TMP_InputField inputUrl;
        [SerializeField] private Image imgLoading;
        [SerializeField] private TMP_Text txtMessage;

        [Header("Notification panel")]
        [SerializeField] private GameObject panelNotification;
        [SerializeField] private TMP_Text txtNotification;
        
        [Header("effect text score")]
        [SerializeField] private TMP_Text textComponent;

        public Transform startposTextscore;
        public Transform endposTextscore;
        
        TimeSpan   tmpTimeSpan;
        [Header("Bool status button")] 
        public bool isCanUndo;

        [SerializeField] private Sprite CanUndo, UncanUndo;

        public Vector2 CanvasScale
        {
            get
            {
                Vector2 canvasScale = new Vector2(transform.localScale.x, transform.localScale.y);
                return canvasScale;
            }
        }
        
        public Vector2 GetCanvasScale()
        {
            Canvas canvas = GetComponent<Canvas>();
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            Vector2 referenceResolution = canvasScaler.referenceResolution;
            Vector2 currentResolution = new Vector2(Screen.width, Screen.height);
            Vector2 scale = new Vector2(currentResolution.x / referenceResolution.x, currentResolution.y / referenceResolution.y);
            return scale;
        }

        private void Awake()
        {
            
            SGameManager.Instance.OnFinishedInit += Init;
            SGameManager.Instance.OnUpdateWasteCards_Callback += OnUpdateWasteCards;
            SGameManager.Instance.OnResetStockCards += OnResetStockCard;
            SGameManager.Instance.OnUpdateTimer_Callback += OnUpdateTimer;
            SGameManager.Instance.OnUpdateScore_Callback += OnUpdatePlayerScore;
            SGameManager.Instance.OnUpdateCompetitionScore_Callback += OnUpdateCompetitionScore;
            SGameManager.Instance.OnTimeOut_Callback += OnTimeOut_Handler;
            SGameManager.Instance.OnPrepareToSubmitPoint_Callback += OnPrepareToSubmitPoint_Handler;
            SGameManager.Instance.OnCommitPoint_Callback += OnCommitPoint_Handler;
            SGameManager.Instance.OnEndGameResult_Callback += OnEndGameResult_Handler;
            SGameManager.Instance.OnStartGameHC_Callback += OnStartGameHC_Handler;
            
            btnShowMenu.onClick.AddListener(ShowMenuDetail);
            btnShowFinishedGame.onClick.AddListener(showFinishGame);
			
			SGameManager.Instance.OnReceiveError_Callback += OnError_Handler;
        }
        private void Start()
        {
            isCanUndo = false;
            disableButtonUndo();
			Utils.CardSlots = cardSlots;
#if HCAPP_FOLLOW
            HcPopupManager.Instance.ShowEightGameLoading(false);
            panelConnectToServer.SetActive(false);
            SolitarieEffect321.Ins.PlayEffect321(() =>
            {
                SGameManager.Instance.Ready();
                showModeGame();
                StartCoroutine(SGameManager.Instance.FixedUpdateSec());
            });
            

#endif
            HCAppController.Instance.LoadUserInfoInGame(HCAppController.Instance.findingRoomResponse, userName1, userAvatar1, userName2, userAvatar2);
        }

        

        private void showModeGame()
        {
            switch (SGameManager.Instance.modeGame)
            {
                case 1:
                    SGUIManager.Instance.panelCompetionInfo2.SetActive(true);
                    break;
                case 2:
                    SGUIManager.Instance.panelCompetionInfo.SetActive(true);
                    break;
                        
            }
        }

        private void OnDestroy()
        {
            SGameManager.Instance.OnFinishedInit -= Init;

            SGameManager.Instance.OnUpdateWasteCards_Callback -= OnUpdateWasteCards;
            SGameManager.Instance.OnResetStockCards -= OnResetStockCard;

            SGameManager.Instance.OnUpdateTimer_Callback -= OnUpdateTimer;
            SGameManager.Instance.OnUpdateScore_Callback -= OnUpdatePlayerScore;
            SGameManager.Instance.OnUpdateCompetitionScore_Callback -= OnUpdateCompetitionScore;
            SGameManager.Instance.OnTimeOut_Callback -= OnTimeOut_Handler;
            SGameManager.Instance.OnPrepareToSubmitPoint_Callback -= OnPrepareToSubmitPoint_Handler;
            SGameManager.Instance.OnCommitPoint_Callback -= OnCommitPoint_Handler;
            SGameManager.Instance.OnEndGameResult_Callback -= OnEndGameResult_Handler;

            SGameManager.Instance.OnStartGameHC_Callback -= OnStartGameHC_Handler;

            SGameManager.Instance.OnReceiveError_Callback -= OnError_Handler;
        }

        private void LateUpdate()
        {
            if (null != _draggingCard)
            {
                _draggingCard.SetAnchorPosition(SGameManager.Instance.InputSystem.ScreenPosition);
            }
        }

        private Card RaycastForCard(Vector2 mousePos, List<Card> cardsToIgnore = null)
        {
            PointerEventData currentPointerPosition = new PointerEventData(EventSystem.current);
            currentPointerPosition.position = mousePos;
            List<RaycastResult> raycastResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(currentPointerPosition, raycastResult);
            foreach (RaycastResult result in raycastResult)
            {
                Card card = result.gameObject.GetComponent<Card>();
                if (null != card && true == card.IsFaceUp)
                {
                    if(null == cardsToIgnore)
                    {
                        return card;
                    }
                    if(null != cardsToIgnore && false == cardsToIgnore.Contains(card))
                    {
                        return card;
                    }
                }
            }
            return null;
        }

        private T RaycastObject<T>(Vector2 mousePos, List<T> objectsToIgnore = null)
        {
            PointerEventData currentPointerPosition = new PointerEventData(EventSystem.current);
            currentPointerPosition.position = mousePos;
            List<RaycastResult> raycastResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(currentPointerPosition, raycastResult);
            foreach (RaycastResult result in raycastResult)
            {
                T raycastForObj = result.gameObject.GetComponent<T>();
                if (null != raycastForObj)
                {
                    if (null == objectsToIgnore)
                    {
                        return raycastForObj;
                    }
                    if (null != objectsToIgnore && false == objectsToIgnore.Contains(raycastForObj))
                    {
                        return raycastForObj;
                    }
                }
            }
            return default(T);
        }

        private void Init()
        {
            txtUsername.text = HCAppController.Instance.userInfo.UserName;
            imgUserAvatar.sprite = HCAppController.Instance.myAvatar;

            int currentCardIndex = -1;
            panelConnectToServer.SetActive(false);
            Card[] cardList = SGameManager.Instance.CurrentCards.Values.ToArray();
            for (int i = 0; i < cardSlots.Count; i++)
            {
                Card parentCard = null;
                for (int cardCount = 0; cardCount < i + 1; cardCount++)
                {
                    currentCardIndex++;

                    Card currentCard = cardList[currentCardIndex];
                    currentCard.transform.position = stock.transform.position;
                    if(null == parentCard)
                    {
                        currentCard.SetParent(cardSlots[i].transform);
                    }
                    else
                    {
                        currentCard.SetParent(parentCard.transform);
                    }
                    parentCard = currentCard;

                    currentCard.MoveToPosition(Vector2.zero, () => 
                    {
                        if(true == currentCard.IsLastCard())
                        {
                            currentCard.FaceUp();
                        }
                    });
                }
            }

            Card[] stockCards = SGameManager.Instance.StockCards.Values.ToArray();
            foreach (Card card in stockCards)
            {
                card.SetParent(stock.transform);
                card.transform.position = stock.transform.position;
                card.SetAnchorPosition(new Vector2(0, card.rectTransform.anchoredPosition.y));
            }
        }

        private void CheckForAllCardFaceUp()
        {
            foreach (CardSlot cardSlot in cardSlots)
            {
                if (cardSlot.transform.childCount > 0)
                {
                    foreach (Card card in cardSlot.GetCards())
                    {
                        if(false == card.IsFaceUp)
                        {
                            btnAuto.gameObject.SetActive(false);
                            return;
                        }
                    }
                }
            }

            //btnAuto.gameObject.SetActive(true);
            btnAuto_onClicked();
        }

        private Card UpdateCardSlots()
        {
            foreach (CardSlot cardSlot in cardSlots)
            {
                if(cardSlot.transform.childCount > 0)
                {
                    Card lastCard = cardSlot.GetLastCard();
                    if(null != lastCard && false == lastCard.IsFaceUp)
                    {
                        lastCard.FaceUp();
                        CheckForAllCardFaceUp();
                        return lastCard;
                    }
                }
            }
            return null;
        }

        private async void OnUpdateWasteCards(List<Card> cards)
        {
            int cardOrderIndex = 0;
            for(int i = cards.Count() - 1; i >= 0; i--)
            {
                Card card = cards[i];
                card.SetParent(wasteTransform);
                card.gameObject.SetActive(true);
                if (cardOrderIndex >= 3)
                {
                    card.MoveToPosition(Vector2.zero + new Vector2(-40 * cardOrderIndex, 0));
                }else
                {
                   cardOrderIndex++;
                    if (false == card.IsFaceUp)
                    {
                        card.FaceUp();
                    }
                    card.MoveToPosition(Vector2.zero + new Vector2(-40 * cardOrderIndex, 0));
                   // cardOrderIndex++;
                }
            }
        }

        private async void OnResetStockCard(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                card.SetParent(stock.transform);
                card.MoveToPosition(Vector2.zero, () =>
                {
                    card.gameObject.SetActive(false);
                    card.FaceDownNoAnim();
                });
            }

        }

        //show Menu
        private void ShowMenuDetail()
        {
            DetailMenu.SetActive(isShowMenu);
        }

        public void soundUi()
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.soundUi);
        }
        //show finish game
        private void showFinishGame()
        {
          panelConfirmSubmit.SetActive(true);
            
        }
        private void OnUpdateTimer(int time)
        {
             tmpTimeSpan = System.TimeSpan.FromSeconds(time);
             txtTimer.text = $"{tmpTimeSpan.Minutes:00}:{tmpTimeSpan.Seconds:00}";
        }
        private void OnUpdatePlayerScore(int score)
        {
            newScoreadd = score - oldScore; 
            txtScore.text = score.ToString();
            oldScore = int.Parse(txtScore.text);
            if (newScoreadd != 0)
            {
                StartCoroutine(effectText());
            }
        }
        private IEnumerator effectText()
        {
            textComponent.transform.position = startposTextscore.transform.position;
            textComponent.gameObject.SetActive(true);
            if (newScoreadd > 0)
            {
                textComponent.text = "+"+ newScoreadd.ToString();
            }

            if (newScoreadd < 0)
            {
                textComponent.text = ""+ newScoreadd.ToString();
            }
          
            textComponent.transform.DOMoveY(endposTextscore.transform.position.y,2f,false);
            yield return new WaitForSeconds(1); 
            textComponent.gameObject.SetActive(false);
            
        }
        private void OnUpdateCompetitionScore(int score)
        {
            txtCompetitionScore.text = score.ToString();
        }

        private void OnTimeOut_Handler()
        {
            //SceneManager.LoadScene("Home");
            AudioManager.Instance.PlaySound(AudioManager.Instance.succesandTimeUp);
            OnUpdateTimer(0);
            panelWaitForResult.SetActive(true);
            //panelSubmitScoreTimeUp.SetActive(true);
            //txtSubmitTitle.text = "Time's Up";
        }

        private void OnEndGameResult_Handler(bool isWinner)
        {
            //SceneManager.LoadScene("Home");
            return;
            panelWaitForResult.SetActive(false);
            if (true == isWinner)
            {
                txtResult.text = "Win!!!";
            }
            else
            {
                txtResult.text = "Lose!!!";
            }
            panelResult.SetActive(true);
        }

        private void OnStartGameHC_Handler()
        {
            btnConnect_onClicked();
        }

        private void OnError_Handler(NetworkController.EGeneralError error)
        {
            panelNotification.SetActive(true);
            switch(error)
            {
                case NetworkController.EGeneralError.GE_ALREADY_LOGIN:
                    txtNotification.text = "This ID already login!!!";
                    break;
                case NetworkController.EGeneralError.GE_INVALID_DATA:
                    txtNotification.text = "Invalid data!!!";
                    break;
            }
        }

        public void OnCardClickDown(Card card)
        {
            if (null != card)
            {
                _draggingCard = card;
                _draggingCard.OnBeginDrag();
                _draggingCard.SetAnchorPosition(SGameManager.Instance.InputSystem.ScreenPosition);
            }
        }

        public void OnCancelDragCard()
        {
            _draggingCard = null;
        }

        public void OnCardClickUp()
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.xepBai1);
            if (null != _draggingCard)
            {
                Card raycastCard = RaycastForCard(SGameManager.Instance.InputSystem.ScreenPosition, _draggingCard.GetComponentsInChildren<Card>().ToList());
                CardSlot raycastCardSlot = RaycastObject<CardSlot>(SGameManager.Instance.InputSystem.ScreenPosition);
                Foundation raycastFoundation = RaycastObject<Foundation>(SGameManager.Instance.InputSystem.ScreenPosition);

                if ((null != raycastCard && true == _draggingCard.CanSnapTo(raycastCard.gameObject)))
                {
                    SGameManager.Instance.PushUndo(_draggingCard, _draggingCard.BeforeDragParent);

                    bool isSameParent = _draggingCard.BeforeDragParent == raycastCard.transform;
                    SGameManager.Instance.PushAutoFaceUpCardSlot(UpdateCardSlots());
                    SGameManager.Instance.OnUseStockCard(_draggingCard);
                    SGameManager.Instance.OnSendCardSuccess(_draggingCard, raycastCard.gameObject, 
                        Utils.GetCardParentIndex(_draggingCard.BeforeDragParent.gameObject, _draggingCard),
                        Utils.GetCardParentIndex(raycastCard.gameObject, _draggingCard));
                    _draggingCard.OnStopDrag(raycastCard.transform);
                }
                else if ((null != raycastCardSlot && true == _draggingCard.CanSnapTo(raycastCardSlot.gameObject)))
                {
                    SGameManager.Instance.PushUndo(_draggingCard, _draggingCard.BeforeDragParent);

                    SGameManager.Instance.OnSendCardSuccess(_draggingCard, raycastCardSlot.gameObject,
                        Utils.GetCardParentIndex(_draggingCard.BeforeDragParent.gameObject, _draggingCard),
                        Utils.GetCardParentIndex(raycastCardSlot.gameObject, _draggingCard));
                    _draggingCard.OnStopDrag(raycastCardSlot.transform);
                    SGameManager.Instance.PushAutoFaceUpCardSlot(UpdateCardSlots());
                    SGameManager.Instance.OnUseStockCard(_draggingCard);
                }
                else if(null != raycastFoundation && raycastFoundation == _foundation && true == TryToSendCardToFoundation(_draggingCard))
                {
                    SGameManager.Instance.PushUndo(_draggingCard, _draggingCard.BeforeDragParent);
                }
                //else if (null != raycastFoundation && true == raycastFoundation.CanSendCardToFoundation(_draggingCard))
                //{
                //    SGameManager.Instance.PushUndo(_draggingCard, _draggingCard.CurrentParent);

                //    bool isSameParent = null != _draggingCard.CurrentParent.GetComponentInParent<Foundation>();
                //    raycastFoundation.SendCardToFoundation(_draggingCard);
                //    SGameManager.Instance.PushAutoFaceUpCardSlot(UpdateCardSlots());
                //    SGameManager.Instance.OnUseStockCard(_draggingCard);
                //    SGameManager.Instance.OnSendCardSuccess(_draggingCard, raycastFoundation.gameObject, isSameParent);
                //}
                else
                {

                    _draggingCard.OnStopDrag();
                }
                _draggingCard = null;
            }
        }

        public bool TryToSendCardToFoundation(Card card)
        {
            if(true == _foundation.CanSendCardToFoundation(card))
            {
                SGameManager.Instance.PushUndo(card, card.gameObject.transform.parent);
              //  SGameManager.Instance.PushUndo(card, card.BeforeDragParent);

                bool isSameParent = false;
                if (null != card.BeforeDragParent)
                {
                    isSameParent = null != card.BeforeDragParent.GetComponentInParent<Foundation>();
                }

                _foundation.SendCardToFoundation(card);
                SGameManager.Instance.PushAutoFaceUpCardSlot(UpdateCardSlots());
                SGameManager.Instance.OnUseStockCard(card);
                SGameManager.Instance.OnSendCardSuccess(card, _foundation.gameObject,
                    Utils.GetCardParentIndex(card.CurrentParent? card.CurrentParent.gameObject: null, card),
                        Utils.GetCardParentIndex(_foundation.gameObject, card));

                return true;
            }
            return false;
        }

        #region Network controller
        public void btnConnect_onClicked()
        {
            string url = inputUrl.text;
            Debug.Log("Connect url!!! " + url);

            imgLoading.gameObject.SetActive(true);
            SGameManager.Instance.ConnectToServer(url, ()=>
            {
                btnPlayGame_onClicked();
            });
        }

        public void btnCreateRoom_onClicked()
        {
            imgLoading.gameObject.SetActive(true);
            SGameManager.Instance.CreateRoom(()=>
            {
                imgLoading.gameObject.SetActive(false);
                panelConnectToServer.SetActive(false);
            });
        }
        public void btnPlayGame_onClicked()
        {
            imgLoading.gameObject.SetActive(true);
        }

        public void btnPlayAgain_onClicked()
        {

            panelWaitForResult.SetActive(false);
            panelResult.SetActive(false);

            _draggingCard = null;

            SGameManager.Instance.ResetGame();

            btnPlayGame_onClicked();
        }
        #endregion
        public void OnPrepareToSubmitPoint_Handler()
        {
            panelConfirmSubmit.SetActive(true);
            //txtSubmitTitle.text = "Game Over";
        }

        public void OnCommitPoint_Handler()
        {
            //panelSubmitScore.SetActive(false);
            panelWaitForResult.SetActive(true);
        }

        private void UpdateAutoMode_CardList(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                if(true == _foundation.IsCardAlreadyOnFoundation(card))
                {
                    continue;
                }
                card.gameObject.SetActive(true);
                if(false == card.IsFaceUp)
                {
                    card.FaceUp();
                } 
                if (true == TryToSendCardToFoundation(card))
                {
                    return;
                }
            }
        }

        private IEnumerator UpdateAutoMode()
        {
            while (false == SGameManager.Instance.CheckForEndGame(_foundation))
            {
                UpdateAutoMode_CardList(SGameManager.Instance.StockCards.Values.ToList());
                UpdateAutoMode_CardList(SGameManager.Instance.WasteCards.Values.ToList());
                UpdateAutoMode_CardList(SGameManager.Instance.CurrentCards.Values.ToList());

                yield return new WaitForSeconds(0.2f);
            }
        }

        public void btnAuto_onClicked()
        {
            btnAuto.gameObject.SetActive(false);
            SGameManager.Instance.StartAutoMode();
            foreach (Card card in SGameManager.Instance.StockCards.Values.ToList())
            {
                if (true == _foundation.IsCardAlreadyOnFoundation(card))
                {
                    continue;
                }
                card.SetParent(wasteTransform);
                
            }
            foreach (Card card in SGameManager.Instance.WasteCards.Values.ToList())
            {
                if (true == _foundation.IsCardAlreadyOnFoundation(card))
                {
                    continue;
                }
                card.SetParent(wasteTransform);
            }
            foreach (Card card in SGameManager.Instance.CurrentCards.Values.ToList())
            {
                if (true == _foundation.IsCardAlreadyOnFoundation(card))
                {
                    continue;
                }
                card.SetParent(wasteTransform);
            }

            StartCoroutine(UpdateAutoMode());
        }

        public void btnPrepareToSubmit_onClicked()
        {
            SGameManager.Instance.PrepareToSubmitPoint();
        }

        public void btnUndo_onClicked()
        {
           SGameManager.Instance.PopUndo();
        }

        public void btnResumeGame_onClicked()
        {
            SGameManager.Instance.ResumeGame();
        }

        public void btnSubmitScore_onClicked()
        {
            SGameManager.Instance.CommitPoint();
        }

        public void btnSendMessageQuit()
        {
            SGameManager.Instance.sendMessageQuit();
        }

        public void clear()
        {
            tmpTimeSpan = TimeSpan.Zero;
        }

        public void disableButtonUndo()
        {
            btnUndo.GetComponent<ScaleOnClick>().deactive = !isCanUndo;
            btnUndo.GetComponent<Image>().sprite = isCanUndo ? UncanUndo : CanUndo;


        }
        
        
       
    }
}