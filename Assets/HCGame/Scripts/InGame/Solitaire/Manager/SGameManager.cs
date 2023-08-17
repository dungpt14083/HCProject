using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NBCore;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Threading;
using HcGames.Solitaire;
using Google.Protobuf.Collections;
using UniRx;
using UnityEngine.SceneManagement;

namespace Solitaire
{
    public class SGameManager : MonoBehaviour
    {
        public static SGameManager Instance;
        public int _timePlay;

        private int _freeResetStockCard = 3;
        private bool _isPlaying = false;

        public int modeGame =1;
        private Card _checkResetCard;

        private Dictionary<CardData, Card> _stockCards;
        public Dictionary<CardData, Card> StockCards
        {
            get
            {
                return _stockCards;
            }
        }

        private Dictionary<CardData, Card> _wasteCards;
        public Dictionary<CardData, Card> WasteCards
        {
            get
            {
                return _wasteCards;
            }
        }

        private Dictionary<CardData, Card> _currentCards;

        private UndoSystem _undoSystem = new UndoSystem();

        private NetworkController _networkController = new NetworkController();

        public Dictionary<CardData, Card> CurrentCards
        {
            get { return _currentCards; }
        }

        public bool StillHaveCardOnStock
        {
            get
            {
                return _stockCards.Count > 0;
            }
        }

        //private Dictionary<GameObject, List<Card>> _pointHistoryDict = new Dictionary<GameObject, List<Card>>();

        

        //private CCData _ccData = new CCData() {MiniGameEventId = 28, Token = "28", WaitingTimeId = 0 };

        private Action _OnConnected_Callback = null;

        private bool _isAutoMode = false;
        public bool IsAutoMode
        {
            get
            {
                return _isAutoMode;
            }
        }

        public HCInputSystem InputSystem { get; private set; }

        public Action OnFinishedInit = null;

        public Action<List<Card>> OnUpdateWasteCards_Callback = null;
        public Action <List<Card>> OnResetStockCards = null;

        public Action<int> OnUpdateTimer_Callback = null;
        public Action<int> OnUpdateScore_Callback = null;
        public Action<int> OnUpdateCompetitionScore_Callback = null;
        public Action OnTimeOut_Callback = null;
        public Action OnPrepareToSubmitPoint_Callback = null;
        public Action OnCommitPoint_Callback = null;
        public Action<bool> OnEndGameResult_Callback = null;

        public Action<NetworkController.EGeneralError> OnReceiveError_Callback = null;
        public Action OnErrorHandling_Callback = null;

        public Action OnStartGameHC_Callback = null;

        private void Update()
        {
            InputSystem?.Tick();
        }

        private Card CreateCard(CardData cardData)
        {
            Debug.Log("cardPrefab " + (SGUIManager.Instance.cardPrefab == null));
            GameObject cardObj = PoolManager.Instance.spawnObject(SGUIManager.Instance.cardPrefab);
            cardObj.transform.localScale = Vector3.one;
            Card card = cardObj.GetComponent<Card>();
            card.Init(cardData);
            return card;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
            InputSystem = new HCInputSystem();
            InputSystem.Init();            
            DontDestroyOnLoad(this);
        }

        private void StartGame()
        {
        //    _timePlay = 180;
            _isAutoMode = false;
            _isPlaying = true;
            OnUpdateScore_Callback?.Invoke(0);
            OnUpdateCompetitionScore_Callback?.Invoke(0);
            
        }
        
        public IEnumerator FixedUpdateSec()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (true == _isPlaying)
                {
                    if (_timePlay > 0)
                    {
                        _timePlay--;
                    }
                    else
                    {
                        _timePlay = 0;
                        _isPlaying = false;
                    }
                    OnUpdateTimer_Callback?.Invoke(_timePlay);
                }
                else
                {
                    break;
                }
            }
        }
        private async UniTask FixedUpdatePerSec()
        {
            while(true)
            {
                await UniTask.Delay(new TimeSpan(0, 0, 1), true);
                if (true == _isPlaying)
                {
                    if (_timePlay > 0)
                    {
                        _timePlay--;
                    }
                    else
                    {
                        _timePlay = 0;
                        _isPlaying = false;
                    }
                    OnUpdateTimer_Callback?.Invoke(_timePlay);
                }
            }
        }
        public void Ready()
        {
            _networkController.Ready();
        }
        private void OnDestroy()
        {
            Debug.Log("Soliraite OnDestroy");
            _networkController.OnConnectedCallback = null;
            _networkController.OnReceiveCardDataCallback = null;
            _networkController.OnReceivedEndGameWinnerCallback = null;
            _networkController.OnTimeOutCallback = null;

            _networkController.OnUpdateUserPointCallback = null;
            _networkController.OnUpdateCompetitionPointCallback = null;			

            _networkController.OnUndoCardsCallback = null;

            _networkController.OnErrorCallback = null;
        }
        public void Disconnect()
        {
            Debug.Log("Soliraite Disconnect");
            _networkController.CloseNetwork();
        }
        //private void PushCardToHistory(GameObject inputObj, Card inputCard)
        //{
        //    List<Card> cardList = null;
        //    if(false == _pointHistoryDict.ContainsKey(inputObj))
        //    {
        //        cardList = new List<Card>();
        //    }
        //    else
        //    {
        //        cardList = _pointHistoryDict[inputObj];
        //    }
        //    cardList.Add(inputCard);

        //    _pointHistoryDict[inputObj] = cardList;
        //}

        public bool CheckForEndGame(Foundation foundation)
        {
            if (ECardRank.King != foundation.GetFoundationRankByCardType(ECardType.Spade))
            {
                return false;
            }
            if (ECardRank.King != foundation.GetFoundationRankByCardType(ECardType.Diamond))
            {
                return false;
            }
            if (ECardRank.King != foundation.GetFoundationRankByCardType(ECardType.Heart))
            {
                return false;
            }
            if (ECardRank.King != foundation.GetFoundationRankByCardType(ECardType.Club))
            {
                return false;
            }
            PrepareToSubmitPoint();
            return true;
        }

        private void OnError_Handler(NetworkController.EGeneralError error)
        {
            OnReceiveError_Callback?.Invoke(error);
        }

        public async UniTask<Sprite> LoadCardSprite(ECardType cardType, ECardRank cardRank)
        {
            string cardName = cardType.ToString();
            switch(cardRank)
            {
                case ECardRank.One:
                    cardName += "01";
                    break;
                case ECardRank.Two:
                    cardName += "02";
                    break;
                case ECardRank.Three:
                    cardName += "03";
                    break;
                case ECardRank.Four:
                    cardName += "04";
                    break;
                case ECardRank.Five:
                    cardName += "05";
                    break;
                case ECardRank.Six:
                    cardName += "06";
                    break;
                case ECardRank.Seven:
                    cardName += "07";
                    break;
                case ECardRank.Eight:
                    cardName += "08";
                    break;
                case ECardRank.Nine:
                    cardName += "09";
                    break;
                case ECardRank.Ten:
                    cardName += "10";
                    break;
                case ECardRank.Jack:
                    cardName += "11";
                    break;
                case ECardRank.Queen:
                    cardName += "12";
                    break;
                case ECardRank.King:
                    cardName += "13";
                    break;
            }

            return await HCGameResource.LoadAssetFromResources<Sprite>("PlayingCards/" + cardName);
        }
        public async UniTask<Sprite> LoadBackCardSprite()
        {
            return await HCGameResource.LoadAssetFromResources<Sprite>("PlayingCards/BackColor_Black");
        }

        public void DrawNewCard()
        {
            if(null != AudioManager.Instance)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.xaoBai);
            }
            if(true == StillHaveCardOnStock)
            {
                
                CardData cardData = _stockCards.Keys.First();
                Card card = _stockCards[cardData];
                sendREMAIN_ACTION();
                PushUndo(card, card.transform.parent);
                _wasteCards.Add(cardData, card);
                _stockCards.Remove(cardData);

                OnUpdateWasteCards_Callback?.Invoke(_wasteCards.Values.ToList());
            }
        }

        public void ResetStockCards()
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.xaoBai);
            _stockCards.Clear();
            foreach(KeyValuePair<CardData, Card> keyValue in _wasteCards)
            {
                _stockCards.Add(keyValue.Key, keyValue.Value);
            }
            _wasteCards.Clear();
            sendREMAIN_ACTION();
            Debug.Log("restockcard");
            OnResetStockCards?.Invoke(_stockCards.Values.ToList());

            _freeResetStockCard--;
            if(_freeResetStockCard < 0)
            {
                _freeResetStockCard = 0;
                //PlayerScore -= 20;

            }
        }

        public void OnUseStockCard(Card card)
        {
            if(false == _wasteCards.ContainsKey(card.cardData))
            {
                return;
            }
            _wasteCards.Remove(card.cardData);
            _currentCards.Add(card.cardData, card);
        }

        public void OnSendCardSuccess(Card card, GameObject inputParentCard, int fromCardSlot, int toCardSlot)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.anTien);
            StartCoroutine(DisplayObjectLayerChatOnCanvasCoroutine());
            Card parentCard = inputParentCard.GetComponent<Card>();
            CardSlot raycastCardSlot = inputParentCard.GetComponent<CardSlot>();
            //if (null != parentCard && (false == _pointHistoryDict.ContainsKey(inputParentCard) || false == _pointHistoryDict[inputParentCard].Contains(card)))
            if (null != parentCard || null != raycastCardSlot)
            {
                if(false == card.HasAddPoint_Card)
                {
                    card.HasAddPoint_Card = true;
                    //PlayerScore += 20;
                }

                //PushCardToHistory(inputParentCard, card);
                Debug.Log("Send move card data!!! " + fromCardSlot + "===" + toCardSlot);
                _networkController.SendMoveCardData(card.gameObject.GetComponentsInChildren<Card>(), fromCardSlot, toCardSlot);
            }


            //if (null != parentCard && false == isSameParent)
            //{
            //    PlayerScore += 20;
            //}

            Foundation foundation = inputParentCard.GetComponent<Foundation>();

            //if (null != foundation && (false == _pointHistoryDict.ContainsKey(inputParentCard) || false == _pointHistoryDict[inputParentCard].Contains(card)))
            if (null != foundation)
            {
                if(false == card.HasAddPoint_Foundation)
                {
                    card.HasAddPoint_Foundation = true;
                    //PlayerScore += 120;
                    //PushCardToHistory(inputParentCard, card);
                  //  PushUndo(parentCard,parentCard.transform);
                }

                Debug.Log("Send move card data to foundation!!! " + fromCardSlot + "===" + toCardSlot);
                _networkController.SendMoveCardData(card.gameObject.GetComponentsInChildren<Card>(), fromCardSlot, toCardSlot);

                if (ECardRank.King == foundation.GetFoundationRankByCardType(card.cardData.Type))
                {
                    //PlayerScore += 100;
                    CheckForEndGame(foundation);
                }
            }
            //if (null != foundation && false == isSameParent)
            //{
            //    PlayerScore += 120;

            //    if(ECardRank.King == foundation.GetFoundationRankByCardType(card.cardData.Type))
            //    {
            //        PlayerScore += 100;
            //    }
            //}
        }
        public IEnumerator DisplayObjectLayerChatOnCanvasCoroutine()
        {
            SGUIManager.Instance.layerChatGreat.SetActive(true);
            yield return new WaitForSeconds(1); // chờ 1 giây
            SGUIManager.Instance.layerChatGreat.SetActive(false);
         
        }


        public void OnTimeOut_Handler()
        {
            _isPlaying = false;

            _networkController.CommitPoint();
            OnTimeOut_Callback?.Invoke();
        }

        public void PrepareToSubmitPoint()
        {
            _isPlaying = false;

            OnPrepareToSubmitPoint_Callback?.Invoke();
        }

        public void ResumeGame()
        {
            _isPlaying = true;
        }

        public void CommitPoint()
        {
            _networkController.CommitPoint();
            OnCommitPoint_Callback?.Invoke();
            _networkController.CloseNetwork();
        }

        public void PushUndo(Card inputCard, Transform cardParent)
        {
            _undoSystem.PushUndoData(inputCard, cardParent, _stockCards, _wasteCards, _currentCards);
        }

        public void PopUndo()
        {
            if(false == _undoSystem.HasUndoData())
            {
                Debug.Log("pos undo that bai");
                return;
            }
            //dung test
            // UndoSystem.UndoData undoData = _undoSystem.PopData();
            // _stockCards = undoData.stockCardData;
            // _wasteCards = undoData.wasteCardData;
            // _currentCards = undoData.availableCardData;
            // Debug.Log("pos undo thanh cong");
            //code cu
            _networkController.RequestSOLITAIRE_UNDO_ACTION();
            //PlayerScore = undoData.playerScore;
        }

        private void OnUndoCardsHandler(DataUndo undoCardData)
        {
            if(true == _undoSystem.CanUndo(undoCardData))
            {
                UndoSystem.UndoData undoData = _undoSystem.PopData();
                _stockCards = undoData.stockCardData;
                _wasteCards = undoData.wasteCardData;
                _currentCards = undoData.availableCardData;
            }
            else
            {
               // OnReceiveError_Callback?.Invoke(NetworkController.EGeneralError.GE_DESYNC_UNDO_DATA);
                Debug.Log("Error Undo Data: "+NetworkController.EGeneralError.GE_DESYNC_UNDO_DATA);
            }
        }
        private void OnUndoCardsClick(DataUndo dataUndo)
        {
            UndoSystem.UndoData undoData = _undoSystem.PopData();
            _stockCards = undoData.stockCardData;
           _wasteCards = undoData.wasteCardData;
           _currentCards = undoData.availableCardData;


        }

        public void PushAutoFaceUpCardSlot(Card card)
        {
            if (false == _undoSystem.HasUndoData())
            {
                return;
            }
            _undoSystem.PushAutoFaceUpCardSlot(card);
        }
        private void OnConnected()
        {
            Debug.Log("On network controller connected!!!");
            _OnConnected_Callback?.Invoke();
        }

        private void AddCardToTab(Dictionary<CardData, Card> cardDict, RepeatedField<HcGames.Solitaire.Card> inputList)
        {
            foreach (CardData cardData in Utils.ParseCardData(inputList))
            {
                cardDict.Add(cardData, CreateCard(cardData));
                cardDict[cardData].gameObject.SetActive(true);
            }
        }

        private void OnReceiveCardData_Handler(CardData_Soli cardDatas)
        {
            //Initialize cards
            Debug.Log("Soliraite NetworkHeader.OnReceiveCardData_Handler");
            _currentCards = new Dictionary<CardData, Card>();

            AddCardToTab(_currentCards, cardDatas.Colum1);
            AddCardToTab(_currentCards, cardDatas.Colum2);
            AddCardToTab(_currentCards, cardDatas.Colum3);  
            AddCardToTab(_currentCards, cardDatas.Colum4);
            AddCardToTab(_currentCards, cardDatas.Colum5);
            AddCardToTab(_currentCards, cardDatas.Colum6);
            AddCardToTab(_currentCards, cardDatas.Colum7);

            _wasteCards = new Dictionary<CardData, Card>();
            _stockCards = new Dictionary<CardData, Card>();

            AddCardToTab(_stockCards, cardDatas.RemainCards);
            foreach(Card cardObj in _stockCards.Values)
            {
                cardObj.gameObject.SetActive(false);
            }

            _timePlay = cardDatas.TimePlay;
            Debug.Log("VARCheck2: "+_timePlay);
            Debug.Log("Soliraite NetworkHeader.OnReceiveCardData_Handler OnFinishedInit");
            OnFinishedInit?.Invoke();
        }

        private void OnReceivedEndGameWinner_Handler(bool isWinner)
        {
            OnEndGameResult_Callback?.Invoke(isWinner);
        }

        private void OnUpdateUserPoint_Handler(int point)
        {
            OnUpdateScore_Callback?.Invoke(point);
        }

        private void OnUpdateCompetitionPoint_Handler(int point)
        {
            OnUpdateCompetitionScore_Callback?.Invoke(point);
        }

        public void ConnectToServer(string url, Action connectToServerCallback)
        {
            _networkController.OnConnectedCallback = OnConnected;
            _networkController.OnReceiveCardDataCallback = OnReceiveCardData_Handler;
            _networkController.OnReceivedEndGameWinnerCallback = OnReceivedEndGameWinner_Handler;
            _networkController.OnTimeOutCallback = OnTimeOut_Handler;

            _networkController.OnUpdateUserPointCallback = OnUpdateUserPoint_Handler;
            _networkController.OnUpdateCompetitionPointCallback = OnUpdateCompetitionPoint_Handler;
            _OnConnected_Callback = connectToServerCallback;
            _networkController.InitNetwork(url);
			

            _networkController.OnUndoCardsCallback = OnUndoCardsHandler;
            _networkController.OnUndoCardsClickCallback = OnUndoCardsClick;

            _networkController.OnErrorCallback = OnError_Handler;
        }

        public void CreateRoom(Action onOtherJoinRoom)
        {
            _networkController.CreateRoom(()=>
            {
                Debug.Log("Other join room!!!");
                StartGame();
                onOtherJoinRoom?.Invoke();
            });
        }
        public void StartFindRoom(string gameUrl, string usercodeID, int mmr, HcGames.CCData ccData, int userLevel)
        {
            SGameManager.Instance.ConnectToServer(gameUrl, () =>
            {
                Debug.Log($"StartFindRoom mmr {mmr} gameUrl {gameUrl}");
                SceneManager.LoadScene("Game Matching");
                SGameManager.Instance.FindRoom((int)GameType.Solitaire, userLevel, usercodeID, mmr, ccData, (doFoundRoom) =>
                {
                    Debug.Log($"Solitaire_GameScene doFoundRoom {doFoundRoom}");
                    if (true == doFoundRoom)
                    {
                        //SceneManager.LoadScene("Solitaire_GameScene");
                        HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Solitaire);
                    }
                    else
                    {
                        Debug.LogError("1111111111");
                        SceneManager.LoadScene("Home");
                    }
                });
            });
            
        }

        public async void FindRoom(int gameType, int level, string userCodeID, int mmr, HcGames.CCData ccData, Action<bool> findRoomCallback)
        {
            Action<bool> _OnFindRoom_Callback = findRoomCallback;

            HcGames.FindingRoomResponse findRoomResponse = await _networkController.FindRoom(gameType, level, userCodeID, mmr, ccData);
            HCAppController.Instance.findingRoomResponse = findRoomResponse;
            Debug.Log("Solitaire findRoomResponse " + findRoomResponse.ToString());
            if(null != findRoomResponse && false == string.IsNullOrEmpty(findRoomResponse.RoomId.Trim()))
            {
                Debug.Log("Found room!!! " + findRoomResponse.RoomId + " === " + findRoomResponse.MasterName+" == "+findRoomResponse.OtherPlayerName+"=="+findRoomResponse.Mode );
                modeGame = findRoomResponse.Mode;
                StartGame();
                _OnFindRoom_Callback?.Invoke(true);
               
            }
            else
            {
                _OnFindRoom_Callback?.Invoke(false);
            }

        }

        public void ResetGame()
        {
            _timePlay = 300;

            OnUpdateScore_Callback?.Invoke(0);
            OnUpdateCompetitionScore_Callback?.Invoke(0);

            _freeResetStockCard = 3;
            _isPlaying = false;

            foreach (Card card in _stockCards.Values)
            {
                card.FaceDownNoAnim();
                PoolManager.Instance.releaseObject(card.gameObject);
            }
            _stockCards.Clear();

            foreach (Card card in _wasteCards.Values)
            {
                card.FaceDownNoAnim();
                PoolManager.Instance.releaseObject(card.gameObject);
            }
            _wasteCards.Clear();

            foreach (Card card in _currentCards.Values)
            {
                card.FaceDownNoAnim();
                PoolManager.Instance.releaseObject(card.gameObject);
            }
            _currentCards.Clear();
        }

        //public void Init(long miniGameEventId, string token)
        //{
        //    _ccData.MiniGameEventId = (ulong)miniGameEventId;
        //    _ccData.Token = token;
        //    OnStartGameHC_Callback?.Invoke();
        //}
        public void OnUserConfirmErrorMessage()
        {
            OnErrorHandling_Callback?.Invoke();

            //TODO: after user confirm error message, handling error here
            Application.Quit();
        }

        public void StartAutoMode()
        {
            _isAutoMode = true;
            sendMessegaAutoComplte();
        }

        public void sendMessageQuit()
        {
            _networkController.SendMessageQuit();
        }

        public void sendUndoAction()
        {
            _networkController.RequestSOLITAIRE_UNDO_ACTION();
        }

        public void sendREMAIN_ACTION()
        {
            _networkController.RequestSOLITAIRE_ROLL_REMAIN_ACTION();
        }

        public void sendMessegaAutoComplte()
        {
            _networkController.SendMessageAutoComplte();
        }
        public void CancelMatching()
        {
            _networkController.SendCancelMatching();
        }
    }
}