using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solitaire
{
    public class CardSlot : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Card GetLastCard()
        {
            return transform.GetComponentsInChildren<Card>().Last();
        }

        public List<Card> GetCards()
        {
            return transform.GetComponentsInChildren<Card>().ToList();
        }
    }
}