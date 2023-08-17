using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solitaire
{
    public class Stock : MonoBehaviour
    {
        [SerializeField] private Sprite _sprite_card;
        [SerializeField] private Sprite _sprite_Reset;
        [SerializeField] private Button BtnStockCard;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
            BtnStockCard.onClick.AddListener(CLickDraw);
        }

        private void Start()
        {
            SGameManager.Instance.OnUpdateWasteCards_Callback += OnDrawNewCard;
            SGameManager.Instance.OnResetStockCards += OnResetStockCard;
        }

        private void OnDestroy()
        {
            SGameManager.Instance.OnUpdateWasteCards_Callback -= OnDrawNewCard;
            SGameManager.Instance.OnResetStockCards -= OnResetStockCard;
        }

        private void OnDrawNewCard(List<Card> card)
        {
            Debug.Log("click on drawNewCard");
            if(true == SGameManager.Instance.StillHaveCardOnStock)
            {
                _image.sprite = _sprite_card;
            }
            else
            {
                _image.sprite = _sprite_Reset;
            }
        }

        private void OnResetStockCard(List<Card> cards)
        {
            _image.sprite = _sprite_card;
        }

        public void CLickDraw()
        {
            Debug.Log("click new card");
            if(true == SGameManager.Instance.StillHaveCardOnStock)
            {
                SGameManager.Instance.DrawNewCard();
            }
            else
            {
                SGameManager.Instance.ResetStockCards();
            }
        }

    }
}