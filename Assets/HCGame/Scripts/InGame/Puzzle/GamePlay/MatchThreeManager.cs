using MiniGame.MatchThree.Scripts.Network;
using MiniGame.MatchThree.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.MatchThree.Scripts.GamePlay
{
    public class MatchThreeManager : SingletonMono<MatchThreeManager>
    {
       [SerializeField] public GameplayDialogHandler gameplayDialog;
        public EndMatchPopup endMatchPopup;
        
        public void ShowEmdMatchPopup()
        {
            endMatchPopup?.Show();
        }
    }
}