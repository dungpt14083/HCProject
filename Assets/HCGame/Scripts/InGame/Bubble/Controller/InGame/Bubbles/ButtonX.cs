using UnityEngine;
using System.Collections;
using InitScriptName;

namespace BubblesShot
{
    public class ButtonX : MonoBehaviour
    {
        void OnMouseDown()
        {
            if (name == "Change" && GamePlay.Instance.GameStatus == GameState.Playing && !mainscript.isMovingBubble)
            {
                mainscript.Instance.ChangeBubble();
            }
        }
    }
}
