using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame.MatchThree.Scripts.UI
{
    public class ResultDialogHandler : MonoBehaviour
    {
        public void OnHome()
        {
            SceneManager.LoadScene("M3_FindMatch");
        }    
    }
}