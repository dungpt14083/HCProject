using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.Asteroids;

namespace Solitaire
{
    public class Foundation : MonoBehaviour
    {
        [SerializeField] private Transform clubFoundation;
        [SerializeField] private Transform heartFoundation;
        [SerializeField] private Transform spadeFoundation;
        [SerializeField] private Transform diamondFoundation;

        private Transform _foundationSnapTo = null;

        private ECardRank CurrentFoundationRank(Transform inputFoundation)
        {
            if(0 == inputFoundation.childCount)
            {
                return ECardRank.One;
            }
            else
            {
                List<Card> currentCardList = new List<Card>(inputFoundation.GetComponentsInChildren<Card>());
                ECardRank currentRank = ECardRank.One;
                foreach(Card card in currentCardList)
                {
                    if((int)card.cardData.Rank > (int)currentRank)
                    {
                        currentRank = card.cardData.Rank;
                    }
                }
                return currentRank;
            }
        }

        public ECardRank GetFoundationRankByCardType(ECardType type)
        {
            Transform foundation = null;
            switch (type)
            {
                case ECardType.Heart:
                    foundation = heartFoundation;
                    break;
                case ECardType.Spade:
                    foundation = spadeFoundation;
                    break;
                case ECardType.Diamond:
                    foundation = diamondFoundation;
                    break;
                case ECardType.Club:
                    foundation = clubFoundation;
                    break;
            }

            return CurrentFoundationRank(foundation);
        }

        public bool IsCardAlreadyOnFoundation(Card inputCard)
        {
            Transform foundation = null;
            switch (inputCard.cardData.Type)
            {
                case ECardType.Heart:
                    foundation = heartFoundation;
                    break;
                case ECardType.Spade:
                    foundation = spadeFoundation;
                    break;
                case ECardType.Diamond:
                    foundation = diamondFoundation;
                    break;
                case ECardType.Club:
                    foundation = clubFoundation;
                    break;
            }

            List<Card> currentCardList = new List<Card>(foundation.GetComponentsInChildren<Card>());
            return currentCardList.Contains(inputCard);
        }

        public bool CanSendCardToFoundation(Card card)
        {
            if (false == card.IsFaceUp)
            {
                return false;
            }
            if (card.transform.childCount > 0)
            {
                return false;
            }

            _foundationSnapTo =null;
            switch(card.cardData.Type)
            {
                case ECardType.Heart:
                    _foundationSnapTo = heartFoundation;
                    break;
                case ECardType.Spade:
                    _foundationSnapTo = spadeFoundation;
                    break;
                case ECardType.Diamond:
                    _foundationSnapTo = diamondFoundation;
                    break;
                case ECardType.Club:
                    _foundationSnapTo = clubFoundation;
                    break;
            }

            ECardRank foundationRank = GetFoundationRankByCardType(card.cardData.Type);
            if((_foundationSnapTo.childCount > 0 && (int)card.cardData.Rank == (int)foundationRank + 1) || (_foundationSnapTo.childCount == 0 && (ECardRank.One == card.cardData.Rank && card.cardData.Rank == foundationRank)))
            {
                return true;
            }
            return false;
        }

        public void SendCardToFoundation(Card card)
        {
            card.OnStopDrag(_foundationSnapTo);
        }
    }
}