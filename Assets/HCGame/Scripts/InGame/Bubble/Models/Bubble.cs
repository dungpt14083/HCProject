using UnityEngine;
using System.Collections;

namespace BubblesShot
{
    public class Bubble
    {

        private BubbleColorType _colorType;

        public Bubble(BubbleColorType color)
        {
            this._colorType = color;

        }

        public BubbleColorType colorType
        {
            get
            {
                return this._colorType;
            }
            set
            {
                this._colorType = value;
            }
        }

    }
}
