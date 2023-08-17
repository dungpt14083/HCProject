using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

namespace Solitaire
{
    public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            { 
                return _rectTransform;
            }
        }

        private Image _image;
        private CardData _cardData;
        public CardData cardData
        {
            get { return _cardData; }
        }

        private Transform _BeforeDragParent = null;
        public Transform BeforeDragParent
        {
            get
            {
                return _BeforeDragParent;
            }
        }

        private Transform _currentParent = null;
        public Transform CurrentParent
        {
            get
            {
                return _currentParent;
            }
        }

        private Vector2 _currentAnchorMin;
        private Vector2 _currentAnchorMax;
        private Vector2 _currentPivot;
        private Vector2 _dragPivot = new Vector2(0.5f, 0.5f);
        private Vector2 _currentAnchorPosition = new Vector2();

        private bool _isFaceUp = false;
        public bool IsFaceUp
        {
            get
            {
                return _isFaceUp;
            }
        }

        private Vector2 OffsetPosition
        {
            get
            {
                if (null != transform.parent && null == transform.parent.GetComponent<Card>())
                {
                    return new Vector2(0, -1 * _rectTransform.sizeDelta.y / 2);
                }
                else
                {
                    return new Vector2(0, -1 * (_rectTransform.sizeDelta.y / 2 + 60.0f));
                }
            }
        }

        public bool CanBeSnapTo
        {
            get
            {
                if (null != transform.parent && (null != transform.parent.GetComponent<Card>() || null != transform.parent.GetComponent<CardSlot>()))
                {
                    return true;
                }
                return false;
            }
        }

        private const float ANIM_TIME_ROTATE = 0.1f;
        private const float ANIM_TIME_MOVE = 0.15f;

        private Vector2 _pointerPosition = Vector2.zero;

        private bool _hasAddPoint_Card = false;
        public bool _isFirstCard=true;
        public bool HasAddPoint_Card
        {
            get
            {
                return _hasAddPoint_Card;
            }
            set
            {
                if(false == _hasAddPoint_Card)
                {
                    _hasAddPoint_Card = value;
                }
            }
        }

        private bool _hasAddPoint_Foundation = false;
        public bool HasAddPoint_Foundation
        {
            get
            {
                return _hasAddPoint_Foundation;
            }
            set
            {
                if(false == _hasAddPoint_Foundation)
                {
                    _hasAddPoint_Foundation = value;
                }
            }
        }

        private bool _isMoving = false;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();

            _currentAnchorMin = _rectTransform.anchorMin;
            _currentAnchorMax = _rectTransform.anchorMax;
            _currentPivot = _rectTransform.pivot;
        }

        private bool IsCardClicked()
        {
            return Vector2.Distance(_pointerPosition, SGameManager.Instance.InputSystem.ScreenPosition) <= 10.0f;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(CardData cardData)
        {
            _cardData = cardData;
        }

        public void OnBeginDrag()
        {
            if(null == _rectTransform)
            {
                return;
            }
            this.transform.DOScale(scaleCard,0.5f);

            _BeforeDragParent = transform.parent;
            _currentParent = transform.parent;

            _currentAnchorPosition = _rectTransform.anchoredPosition - OffsetPosition;

            SetParent(SGUIManager.Instance.transform);
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.zero;
            _rectTransform.pivot = _dragPivot;
        }

        public void OnStopDrag(Transform newParent = null)
        {
            if(null == _rectTransform)
            {
                return;
            }
            this.transform.DOScale(orignalScale,0.5f);
            Vector3 tempPos = transform.position;

            _rectTransform.anchorMin = _currentAnchorMin;
            _rectTransform.anchorMax = _currentAnchorMax;
            _rectTransform.pivot = _currentPivot;
            transform.position = tempPos;

            if(null == newParent)
            {
                SetParent(_BeforeDragParent);
            }
            else
            {
                SetParent(newParent);
                _currentAnchorPosition = Vector2.zero;
            }
            _BeforeDragParent = null;
            MoveToPosition(_currentAnchorPosition);
        }

        public void MoveToPosition(Vector2 moveToPosition, Action onComepleteMove = null)
        {
            _isMoving = true;
            rectTransform.DOAnchorPos(moveToPosition + OffsetPosition, ANIM_TIME_MOVE, true).onComplete += () =>
            {
                _isMoving = false;
                onComepleteMove?.Invoke();
            };
        }

        public async UniTask FaceUp()
        {
            _isFaceUp = true;
            Sprite cardSprite = await SGameManager.Instance.LoadCardSprite(_cardData.Type, _cardData.Rank);
            if(null == cardSprite)
            {
                Debug.LogError("Can't load card sprite!!! " + _cardData.Type + "===" + _cardData.Rank);
                return;
            }
            rectTransform.DORotate(new Vector3(0, 90, 0), ANIM_TIME_ROTATE, RotateMode.Fast).onComplete += () => 
            {
                _image.sprite = cardSprite;
                rectTransform.DORotate(Vector3.zero, ANIM_TIME_ROTATE, RotateMode.Fast);
            };
        }

        public async void FaceDownNoAnim()
        {
            
            _isFaceUp = false;
            Sprite cardSprite = await SGameManager.Instance.LoadBackCardSprite();
            if (null == cardSprite)
            {
                Debug.LogError("Can't load back card sprite!!!");
                return;
            }
            _image.sprite = cardSprite;
        }

        public async UniTask FaceDown()
        {
            _isFaceUp = false;
            Sprite cardSprite = await SGameManager.Instance.LoadBackCardSprite();
            if (null == cardSprite)
            {
                Debug.LogError("Can't load back card sprite!!!");
                return;
            }
            rectTransform.DORotate(new Vector3(0, 90, 0), ANIM_TIME_ROTATE, RotateMode.Fast).onComplete += () =>
            {
                _image.sprite = cardSprite;
                rectTransform.DORotate(Vector3.zero, ANIM_TIME_ROTATE, RotateMode.Fast).onComplete+= () =>
                {
                    //_image.gameObject.SetActive(false);
                };
            };
        }
        public async UniTask FaceDown2()
        {
            _isFaceUp = false;
            Sprite cardSprite = await SGameManager.Instance.LoadBackCardSprite();
            if (null == cardSprite)
            {
                Debug.LogError("Can't load back card sprite!!!");
                return;
            }
            rectTransform.DORotate(new Vector3(0, 90, 0), ANIM_TIME_ROTATE, RotateMode.Fast).onComplete += () =>
            {
                _image.sprite = cardSprite;
                rectTransform.DORotate(Vector3.zero, ANIM_TIME_ROTATE, RotateMode.Fast).onComplete+= () =>
                {
                    _image.gameObject.SetActive(false);
                };
            };
        }

        public bool IsLastCard()
        {
            return 1 == transform.GetComponentsInChildren<Card>().Length;
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
        }
        public void SetAnchorPosition(Vector2 mousePos)
        {
            // Vector2 canvasMousePos = new Vector2(mousePos.x / SGUIManager.Instance.CanvasScale.x, mousePos.y / SGUIManager.Instance.CanvasScale.y);
            // rectTransform.anchoredPosition = canvasMousePos;
            
            
            // Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 100)); // 10 là khoảng cách giữa camera và canvas
            // Vector2 canvasMousePos = Vector2.zero;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, worldPos, Camera.main, out canvasMousePos);
            // rectTransform.anchoredPosition = canvasMousePos;
            
            // Chuyển vị trí chuột thành vị trí trên Canvas
            Vector2 canvasMousePos = mousePos / SGUIManager.Instance.GetCanvasScale();

            // Thiết lập vị trí cho lá bài
            rectTransform.anchoredPosition = canvasMousePos;
        }

        public bool CanSnapTo(GameObject inputObj)
        {
            Card card = inputObj.GetComponent<Card>();
            if(null != card)
            {
                if(false == card.IsFaceUp)
                {
                    return false;
                }
                if(false == card.CanBeSnapTo)
                {
                    return false;
                }
                if(false == card.IsLastCard())
                {
                    return false;
                }
                if(cardData.Color == card.cardData.Color)
                {
                    return false;
                }
                if((int)cardData.Rank + 1 != (int)card.cardData.Rank)
                {
                    return false;
                }
                return true;
            }

            CardSlot slot = inputObj.GetComponent<CardSlot>();
            if(null != slot && 0 == slot.transform.childCount && ECardRank.King == cardData.Rank)
            {
                return true;
            }

            Foundation foundation = inputObj.GetComponent<Foundation>();
            if(null != foundation)
            {
                return true;
            }

            return false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (true == SGameManager.Instance.IsAutoMode)
            {
                return;
            }

            Waste waste = GetComponentInParent<Waste>();
            if (false == _isMoving && true == _isFaceUp && (null == waste || (null != waste && this == waste.GetComponentsInChildren<Card>().Last())))
            {
                SGameManager.Instance.InputSystem.UpdateTouchData();
                _pointerPosition = SGameManager.Instance.InputSystem.ScreenPosition;
                SGUIManager.Instance.OnCardClickDown(this);
            }
        }

        private Vector3 scaleCard = new Vector3(1.2f, 1.2f, 1.2f);
        private Vector3 orignalScale = new Vector3(1f, 1f, 1f);

        public void OnPointerUp(PointerEventData eventData)
        {
            SGUIManager.Instance.OnCardClickUp();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (true == SGameManager.Instance.IsAutoMode)
            {
                return;
            }

            SGameManager.Instance.InputSystem.UpdateTouchData();
            if (true == IsCardClicked())
            {
                SGUIManager.Instance.TryToSendCardToFoundation(this);
            }
        }
    }
}