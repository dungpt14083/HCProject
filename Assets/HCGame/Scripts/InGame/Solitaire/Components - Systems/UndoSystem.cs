using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HcGames.Solitaire;
using Solitaire;

namespace Solitaire
{
    public class UndoSystem
    {
        public struct UndoData
        {
            public Card card;
            public bool isFaceUp;
            public Transform parentTransform;
            public Dictionary<CardData, Card> stockCardData;
            public Dictionary<CardData, Card> wasteCardData;
            public Dictionary<CardData, Card> availableCardData;
            public int playerScore;
            public Card autoFaceUpCardSlot;
        }

        private Stack<UndoData> _undoDatas = new Stack<UndoData>();

        private Dictionary<CardData, Card> CloneData(Dictionary<CardData, Card> originDict)
        {
            Dictionary<CardData, Card> newDict = new Dictionary<CardData, Card>();
            foreach(KeyValuePair<CardData, Card> keyValue in originDict)
            {
                newDict.Add(keyValue.Key, keyValue.Value);
            }
            return newDict;
        }

        public bool HasUndoData()
        {
            return _undoDatas.Count > 0;
        }

        public void PushUndoData(Card inputCard, Transform cardParent, Dictionary<CardData, Card> inputStockData, Dictionary<CardData, Card> inputWasteData, Dictionary<CardData, Card> inputAvailableData)
        {
            UndoData undoData = new UndoData()
            {
                card = inputCard,
                isFaceUp = inputCard.IsFaceUp,
                parentTransform = cardParent,
                stockCardData = CloneData(inputStockData),
                wasteCardData = CloneData(inputWasteData),
                availableCardData = CloneData(inputAvailableData),
                autoFaceUpCardSlot = null,
            };
            _undoDatas.Push(undoData);
        }

        public bool CanUndo(DataUndo serverUndoData)
        {
            UndoData localUndoData = _undoDatas.Peek();
            if(false == serverUndoData.ListCard.Contains(Utils.ParseCardData(localUndoData.card.cardData)))
            {
                Debug.Log("false undo data liscard");
                return false;
            }
            if(serverUndoData.To != Utils.GetCardParentIndex(localUndoData.card.transform.gameObject, localUndoData.card))
            {
                Debug.Log("false undo data From");
                return false;
            }
            if (serverUndoData.From != Utils.GetCardParentIndex(localUndoData.parentTransform.gameObject, localUndoData.card))
            {
                Debug.Log("false undo data To");
                return false;
            }
            Debug.Log("true undo");
            return false;
        }

        public UndoData PopData()
        {
            UndoData undoData = _undoDatas.Pop();
            if(false == undoData.isFaceUp)
            {
                undoData.card.FaceDown2();
            }
            if(null != undoData.autoFaceUpCardSlot)
            {
                undoData.autoFaceUpCardSlot.FaceDown();
            }
            undoData.card.SetParent(undoData.parentTransform);
            undoData.card.MoveToPosition(Vector2.zero);

            return undoData;
        }

        public void PushAutoFaceUpCardSlot(Card card)
        {
            UndoData undoData = _undoDatas.Pop();
            undoData.autoFaceUpCardSlot = card;
            _undoDatas.Push(undoData);
        }
    }
}