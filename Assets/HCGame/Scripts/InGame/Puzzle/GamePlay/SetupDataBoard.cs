using MiniGame.MatchThree.Scripts.ScripTable;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
namespace MiniGame.MatchThree.Scripts.GamePlay
{
    public class SetupDataBoard : MonoBehaviour
    {
        [Header("ScriptTable")]
        [SerializeField] public SC_Level level;

        [Header("Size")]
        [SerializeField] public int width = 9;
        [SerializeField] public int height = 11;

        [Header("Tile")]
        [SerializeField] protected GameObject titlePrefab;
        [SerializeField] protected GameObject titleTextPrefab;
        [SerializeField] protected GameObject titleBoolPrefab;
        [SerializeField] protected Transform parentTile;
        [SerializeField] protected Transform parentTileText;
        [SerializeField] protected Transform parentTileBool;
        [SerializeField] protected Transform parentPiece;


        public int testDisplay;
        public Tile[,] m_allTiles;
        public Piece[,] m_allPiece;
        public GameObject[,] m_allTileBool;

        public Tile m_clickedTile;
        public Tile m_targetTile;

    

        public virtual void Awake()
        {
            width = level.width;
            height = level.height;

            m_allTiles = new Tile[width, height];
            m_allPiece = new Piece[width, height];
            m_allTileBool = new GameObject[width, height];
            Debug.Log("SetupDataBoard m_allTiles == null " + (m_allTiles == null));
        }

        public virtual void Start() { }

        #region Setup Tile  
        protected virtual void SetupTextTiles() { }

        protected virtual GameObject SetupGameobjectTile(GameObject obj, Transform parent, int i, int j)
        {
            if (parent.name.Contains("GroupBool"))
            {
                var offset = 0.07f;
                GameObject tile = Instantiate(obj, parent);
                tile.GetComponent<Transform>().localPosition = new Vector3(i + i * offset, j + j * offset, 0);
                tile.name = "(" + i + "," + j + ")";
                return tile;
            }
            else
            {
                GameObject tile = Instantiate(obj, parent);
                tile.GetComponent<Transform>().localPosition = new Vector3(i, j, 0);
                tile.name = "(" + i + "," + j + ")";
                return tile;
            }

        }
        #endregion

        #region Setup Piece
        protected SC_Piece GetRandomPiece()
        {
            int randomIdx = Random.Range(0, level.pieceList.Length);
            if (level.pieceList[randomIdx] == null)
            {
                Debug.LogWarning("BOARD: " + randomIdx + "does not contain a valid piece prefab!");
            }

            return level.pieceList[randomIdx];
        }

        public void PlacePiece(Piece piece, int x, int y)
        {
            if (piece == null)
            {
                Debug.LogWarning("BOARD:  Invalid Piece!");
                return;
            }

            piece.transform.position = new Vector3(x, y, 0);
            piece.transform.rotation = Quaternion.identity;

            if (IsWithinBounds(x, y))
                m_allPiece[x, y] = piece;

            piece.SetCoord(x, y);
        }

        protected bool IsWithinBounds(int x, int y)
        {
            return (x >= 0 && x < width &&
                    y >= 0 && y < height);
        }

        protected virtual void FillBoard(int falseYOffset = 0, float moveTime = 0.1f) { }
        #endregion

        #region Click Tile
        public virtual void ClickTile(Tile tile)
        {
            if (m_clickedTile == null)
            {
                m_clickedTile = tile;
            }
        }

        public virtual void DragToTile(Tile tile)
        {
            if (m_clickedTile != null &&
                IsNextTo(tile, m_clickedTile))
            {
                m_targetTile = tile;
            }
        }

        public virtual void ReleaseTile() { }

        private bool IsNextTo(Tile start, Tile end)
        {
            if (Mathf.Abs(start.xIndex - end.xIndex) == 1 &&
                start.yIndex == end.yIndex)
            {
                return true;
            }

            if (Mathf.Abs(start.yIndex - end.yIndex) == 1 &&
               start.xIndex == end.xIndex)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}