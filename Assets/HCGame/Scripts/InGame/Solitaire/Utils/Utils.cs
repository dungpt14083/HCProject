using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Solitaire;
using HcGames.Solitaire;

namespace Solitaire
{
    public class Utils
    {
        private static List<CardSlot> cardSlots;
        public static List<CardSlot> CardSlots
        {
            set
            {
                cardSlots = value;
            }
        }

        public static List<CardData> ParseCardData(RepeatedField<HcGames.Solitaire.Card> inputList)
        {
            List<CardData> cards = new List<CardData>();
            foreach (HcGames.Solitaire.Card card in inputList)
            {
                CardData cardData = new CardData();
                cardData.Rank = (ECardRank)card.Number;
                switch (card.Type)
                {
                    case 1:
                        cardData.Type = ECardType.Heart;
                        break;
                    case 2:
                        cardData.Type = ECardType.Diamond;
                        break;
                    case 3:
                        cardData.Type = ECardType.Club;
                        break;
                    case 4:
                        cardData.Type = ECardType.Spade;
                        break;
                }
                cards.Add(cardData);
            }
            return cards;
        }

        public static HcGames.Solitaire.Card ParseCardData(CardData cardData)
        {
            HcGames.Solitaire.Card card = new HcGames.Solitaire.Card();
            card.Number = (int)cardData.Rank;
            switch(cardData.Type)
            {
                case ECardType.Heart:
                    card.Type = 1;
                    break;
                case ECardType.Diamond:
                    card.Type = 2;
                    break;
                case ECardType.Club:
                    card.Type = 3;
                    break;
                case ECardType.Spade:
                    card.Type = 4;
                    break;
            }
            return card;
        }

        public static int GetCardParentIndex(GameObject inputParentObj, Card inputCard)
        {
            if (null != inputParentObj)
            {
                CardSlot cardSlot = inputParentObj.GetComponentInParent<CardSlot>();
                if (null != inputParentObj.GetComponentInParent<Waste>())
                {
                    return 0;
                }
                if (null != cardSlot)
                {
                    return (cardSlots.IndexOf(cardSlot) + 1);
                }
            }
            switch (inputCard.cardData.Type)
            {
                case ECardType.Club:
                    return 11;
                    break;
                case ECardType.Diamond:
                    return 8;
                    break;
                case ECardType.Heart:
                    return 10;
                    break;
                case ECardType.Spade:
                    return 9;
                    break;
            }
            return -1;
        }
    }
}
