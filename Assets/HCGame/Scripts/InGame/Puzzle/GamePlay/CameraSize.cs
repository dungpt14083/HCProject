using MiniGame.MatchThree.Scripts.GamePlay;
using UnityEngine;

namespace MiniGame.MatchThree.Scripts.UI
{
    public class CameraSize : MonoBehaviour
    {
        [SerializeField] private SingleBoard board;
        [SerializeField] private float m_boardSize = 2;

        private void Start()
        {
            SetupCamera();
        }

        private void SetupCamera()
        {
            GetComponent<Transform>().localPosition = new Vector3((float)(board.width - 1) / 2f, (float)(board.height - 1) / 2f, -10f);
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            float verticalSize = (float)board.height / 2f + m_boardSize;
            float horizontaSize = ((float)board.width / 2f + m_boardSize) / aspectRatio;
            GetComponent<Camera>().orthographicSize = (verticalSize > horizontaSize) ? verticalSize : horizontaSize;
        }
    }
}
