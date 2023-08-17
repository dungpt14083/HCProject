using UnityEngine;
using System.Collections;

namespace NBCore {
    public class ScreenResolutionManager : Singleton<ScreenResolutionManager> {
        private const float BASESCREENWIDTH = 640f;
        private const float BASESCREENHEIGHT = 960f;

        public float GetWidthScaleRatio () {
            float targetaspect = BASESCREENWIDTH / BASESCREENHEIGHT;

            // determine the game window's current aspect ratio
            float windowaspect = (float)Screen.width / (float)Screen.height;

            // current viewport height should be scaled by this amount
            //float scaleheight = windowaspect / targetaspect;
            float scaleRatio = windowaspect / targetaspect;
            return scaleRatio;
        }

    }
}