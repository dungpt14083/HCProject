using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BonusGame
{
    public class BonusGame_Collider_WheelItem : MonoBehaviour
    {

        public WheelItem myWheelItem;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.name.Contains(BonusGame_Constant.COLLIDER_WHEEL_ARROW))
            {
                myWheelItem.LightUp();    
            }
        }




    }
}
