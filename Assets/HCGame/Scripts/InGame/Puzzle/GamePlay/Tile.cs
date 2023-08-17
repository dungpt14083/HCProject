using UnityEngine;

namespace MiniGame.MatchThree.Scripts.GamePlay
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] public int xIndex;
        [SerializeField] public int yIndex;
        [SerializeField] public SpriteRenderer grassSlot;
        [SerializeField] public Sprite ghost;
        [SerializeField] public Sprite grass1;
        [SerializeField] public Sprite grass2;
        private SingleBoard m_board;

        public void OnInit(int x, int y, SingleBoard board)
        {
            xIndex = x;
            yIndex = y;
            m_board = board;
        }

        public void OnPointerEnter()
        {
            if (m_board != null)
            {
                m_board.DragToTile(this);
            }
        }

        public void OnPointerDown()
        {
            if (m_board != null)
            {
                m_board.ClickTile(this);
            }
        }

        public void OnPointerUp()
        {
            if (m_board != null)
            {
                m_board.ReleaseTile();
            }
        }
        public void SetGrass(int grassID)
        {
            if (grassID == 0) grassSlot.sprite = null;
            if (grassID == 6) grassSlot.sprite = grass1;
            if (grassID == 7) grassSlot.sprite = grass2;
        }
    }
}