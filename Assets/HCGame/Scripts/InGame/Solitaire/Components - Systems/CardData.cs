using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire
{
    public class CardData
    {
        public ECardType Type { get; set; }
        public ECardRank Rank { get; set; }

        public EColor Color 
        {
            get
            {
                if (ECardType.Club == Type || ECardType.Spade == Type)
                {
                    return EColor.Black;
                }
                return EColor.Red;
            }
        }
    }
}