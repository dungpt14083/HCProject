using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using System;
using DG.Tweening;
using MiniGame.MatchThree.Scripts.Network;
using MiniGame.MatchThree.Scripts.ScripTable;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweens.Plugins;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MiniGame.MatchThree.Scripts.GamePlay
{
    public class SingleBoard : SetupDataBoard
    {
        [SerializeField] private float swapTime = 0.5f;

        protected bool m_playerInputEnable = true;

        public override void Start()
        {
            base.Start();
            SetupTextTiles();
            FillBoard(10, 0.5f);
            MatchThreeManager.Instance.gameplayDialog.OnStartGame();
        }

        #region Setup Piece
        protected override void SetupTextTiles()
        {
            Debug.Log("SetupTextTiles m_allTiles == null " + (m_allTiles == null));
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    try
                    {
                        var tile = SetupGameobjectTile(titlePrefab, parentTile, i, j);
                        //var tileText = SetupGameobjectTile(titleTextPrefab, parentTileText, i, j);
                        //tileText.GetComponent<TextMeshPro>().text = i + "," + j;
                        m_allTiles[i, j] = tile.GetComponent<Tile>();
                        m_allTiles[i, j].OnInit(i, j, this);

                        var tileBool = SetupGameobjectTile(titleBoolPrefab, parentTileBool, i, j);
                        m_allTileBool[i, j] = tileBool;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("SetupTextTiles " + e.StackTrace);
                    }
                }
            }
        }

        //create original pieces that are not duplicated
        protected override void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
        {
            int maxInterations = 100;
            int interations = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (m_allPiece[i, j] == null)
                    {
                        Piece piece = FillPieceAt(i, j, falseYOffset, moveTime);
                        interations = 0;

                        while (HasMatchFill(i, j))
                        {
                            ClearPieceAt(i, j);
                            piece = FillPieceAt(i, j, falseYOffset, moveTime);
                            interations++;

                            if (interations >= maxInterations)
                            {
                                Debug.Log("BREAK ====================");
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected virtual Piece FillPieceAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f, bool isNextGrid = false)
        {
            var sc_pice = GetRandomPiece();
            GameObject randomPiece = Instantiate(sc_pice.piece, Vector3.zero, Quaternion.identity);

            if (randomPiece != null)
            {
                randomPiece.GetComponent<Piece>().OnInit(this, sc_pice.name);
                PlacePiece(randomPiece.GetComponent<Piece>(), x, y);
                if (falseYOffset != 0)
                {
                    randomPiece.transform.position = new Vector3(x, y + falseYOffset, 0);
                    randomPiece.GetComponent<Piece>().Move(x, y, moveTime);
                }
                randomPiece.transform.parent = parentPiece;
                return randomPiece.GetComponent<Piece>();
            }

            return null;
        }

        private bool HasMatchFill(int x, int y, int minLength = 3)
        {
            List<Piece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
            List<Piece> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

            if (leftMatches == null)
            {
                leftMatches = new List<Piece>();
            }

            if (downwardMatches == null)
            {
                downwardMatches = new List<Piece>();
            }


            return (leftMatches.Count > 0 || downwardMatches.Count > 0);
        }
        #endregion

        #region Click Tile
        public virtual void ReleaseTile()
        {
            if (m_clickedTile != null && m_targetTile != null)
            {
                SwitchTiles(m_clickedTile, m_targetTile);
            }

            m_clickedTile = null;
            m_targetTile = null;
        }

        protected void SwitchTiles(Tile clickedTile, Tile targetTile)
        {
            StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
        }

        protected virtual IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
        {
            if (m_playerInputEnable)// Khi dang thuc hien viec xu ly cac piece thi cho hoan thanh moi lam tiep
            {
                Piece clickPiece = m_allPiece[clickedTile.xIndex, clickedTile.yIndex];
                Piece targetPiece = m_allPiece[targetTile.xIndex, targetTile.yIndex];

                if (targetPiece != null && clickPiece != null)
                {
                    clickPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                    targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

                    yield return new WaitForSeconds(swapTime);

                    List<Piece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                    List<Piece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

                    if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0)
                    {
                        clickPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                        targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                    }
                    else
                    {
                        yield return new WaitForSeconds(swapTime);

                        ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());
                    }
                }
            }
        }

        #endregion

        #region Match
        // specify a starting coordinate (startX, startY) and use a Vector2 for direction
        // (1,0) = right, (-1,0) = left, (0,1) = up, (0,-1) = down; minLength is minimum number to be considered
        private List<Piece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLenght = 3)
        {
            List<Piece> matches = new List<Piece>();
            Piece startPiece = null;

            if (IsWithinBounds(startX, startY))
            {
                startPiece = m_allPiece[startX, startY];
            }

            if (startPiece != null)
            {
                matches.Add(startPiece);
            }
            else
            {
                return null;
            }

            int nextX;
            int nextY;
            int maxValue = (width > height) ? width : height;
            for (int i = 1; i < maxValue - 1; i++)
            {
                nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
                nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

                if (!IsWithinBounds(nextX, nextY))
                {
                    break;
                }

                Piece nextPiece = m_allPiece[nextX, nextY];

                if (nextPiece == null)
                {
                    break;
                }
                else
                {
                    if (nextPiece.name == startPiece.name && !matches.Contains(nextPiece))
                    {
                        matches.Add(nextPiece);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matches.Count >= minLenght)
            {
                return matches;
            }

            return null;
        }

        private List<Piece> FindVerticalMatches(int startX, int startY, int minLength = 3)
        {
            List<Piece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
            List<Piece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

            if (upwardMatches == null)
            {
                upwardMatches = new List<Piece>();
            }

            if (downwardMatches == null)
            {
                downwardMatches = new List<Piece>();
            }

            var combineMatches = upwardMatches.Union(downwardMatches).ToList();
            return (combineMatches.Count >= minLength) ? combineMatches : null;
        }

        private List<Piece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
        {
            List<Piece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
            List<Piece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

            if (rightMatches == null)
            {
                rightMatches = new List<Piece>();
            }

            if (leftMatches == null)
            {
                leftMatches = new List<Piece>();
            }

            var combineMatches = rightMatches.Union(leftMatches).ToList();

            return (combineMatches.Count >= minLength) ? combineMatches : null;
        }

        protected List<Piece> FindMatchesAt(int x, int y, int minLength = 3)
        {
            List<Piece> horizontalMatches = FindHorizontalMatches(x, y, minLength);
            List<Piece> verticalMatches = FindVerticalMatches(x, y, minLength);

            if (horizontalMatches == null)
            {
                horizontalMatches = new List<Piece>();
            }

            if (verticalMatches == null)
            {
                verticalMatches = new List<Piece>();
            }

            var combineMatches = horizontalMatches.Union(verticalMatches).ToList();
            return combineMatches;
        }

        protected List<Piece> FindMatchesAt(List<Piece> lstPiece, int minLength = 3)
        {
            List<Piece> matches = new List<Piece>();
            foreach (Piece piece in lstPiece)
            {
                matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
            }
            return matches;
        }

        protected List<Piece> FindAllMatches()
        {
            List<Piece> combineMatches = new List<Piece>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    List<Piece> matches = FindMatchesAt(i, j);
                    combineMatches = combineMatches.Union(matches).ToList();
                }
            }
            return combineMatches;
        }
        #endregion

        #region Highlight

        private void HighLightMatches()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    HightlightMatchesAt(i, j);
                }
            }
        }

        private void HightlightMatchesAt(int i, int j)
        {
            HighlightTileBoolOff(i, j);
            List<Piece> combineMatches = FindMatchesAt(i, j);
            if (combineMatches.Count > 0)
            {
                foreach (Piece piece in combineMatches)
                {
                    HighlightTileBoolOn(piece.xIndex, piece.yIndex);
                }
            }
        }

        private void HighlightTileBoolOff(int x, int y)
        {
            SpriteRenderer spriteRenderer = m_allTileBool[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }

        private void HighlightTileBoolOn(int x, int y)
        {
            SpriteRenderer spriteRenderer = m_allTileBool[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        }

        protected void HighlightPiece(List<Piece> lstPiece)
        {
            foreach (Piece piece in lstPiece)
            {
                if (piece != null)
                {
                    HighlightTileBoolOn(piece.xIndex, piece.yIndex);
                }
            }
        }

        #endregion

        #region Clear Piece

        protected void ClearPieceAt(int x, int y)
        {
            Piece pieceToClear = m_allPiece[x, y];  // clear piece
            if (pieceToClear != null)
            {
                m_allPiece[x, y] = null;

                if (pieceToClear.transform.childCount == 2)
                {
                    var particle = pieceToClear.gameObject.transform.GetChild(1).gameObject;
                    particle.SetActive(true);
                    particle.transform.parent = null;
                }

                Destroy(pieceToClear.gameObject);
            }

            HighlightTileBoolOff(x, y);
        }

        protected void ClearPieceAt(List<Piece> lstPiece)
        {


            MatchThreeGameSystem.Instance._puzzleDataJson.specialTransform.ForEach(item =>
            {
                var piece = m_allPiece[item.position.x, item.position.y];
                lstPiece.Add(piece);
            });



            foreach (Piece piece in lstPiece)
            {
                if (piece != null)
                {
                    ClearPieceAt(piece.xIndex, piece.yIndex);
                }
            }
        }

        private void ClearBoard()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ClearPieceAt(i, j);
                }
            }
        }

        #endregion

        #region Collapse Column
        //Tha roi manh pice nam phia tren, lap day co o trong phia duoi. e.d 00---00 => ---0000
        private List<Piece> CollapseColumn(int column, float collapseTime = 0.1f)
        {
            List<Piece> movingPiece = new List<Piece>();
            for (int i = 0; i < height - 1; i++)
            {
                if (m_allPiece[column, i] == null)
                {
                    for (int j = i + 1; j < height; j++)
                    {
                        if (m_allPiece[column, j] != null)
                        {
                            //m_allPiece[column, j].Move(column, i, collapseTime * (j - i));
                            m_allPiece[column, j].SlowMoveColapse(column, i, collapseTime * (j - i));
                            m_allPiece[column, i] = m_allPiece[column, j];
                            m_allPiece[column, i].SetCoord(column, i);

                            if (!movingPiece.Contains(m_allPiece[column, i]))
                            {
                                movingPiece.Add(m_allPiece[column, i]);
                            }

                            m_allPiece[column, j] = null;

                            break;
                        }
                    }
                }
            }
            DOVirtual.DelayedCall(1.3f, () => { NetWorkBoard.instance.delay0 = true; });
            return movingPiece;
        }

        public List<int> columnsToCollapse;
        protected List<Piece> CollapseColumn(List<Piece> lstPiece)
        {
            List<Piece> movingPieces = new List<Piece>();
            columnsToCollapse = GetColumns(lstPiece);
            foreach (int column in columnsToCollapse)
            {
                movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
            }
            return movingPieces;
        }

        private List<int> GetColumns(List<Piece> lstPiece)
        {
            List<int> columns = new List<int>();

            foreach (Piece piece in lstPiece)
            {
                if (!columns.Contains(piece.xIndex))
                {
                    columns.Add(piece.xIndex);
                }
            }

            return columns;
        }

        protected void ClearAndRefillBoard(List<Piece> lstPiece)
        {
            StartCoroutine(ClearAndRefillBoardRoutine(lstPiece));
        }
        public bool bool2;
        protected virtual IEnumerator ClearAndRefillBoardRoutine(List<Piece> lstPiece)
        {
            Debug.LogError("All piêce " + m_allPiece);
            m_playerInputEnable = false;
            List<Piece> matches = lstPiece;
            do
            {
                //clear and collapse
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
                yield return null;

                //yield return new WaitUntil(() => bool2);
                //refill
                yield return StartCoroutine(RefillRoutine());
                matches = FindAllMatches();

                yield return new WaitForSeconds(0.5f);
            }
            while (matches.Count != 0);

            m_playerInputEnable = true;
        }

        private IEnumerator RefillRoutine()
        {
            FillBoard(10, 0.5f);
            yield return null;
        }

        public bool bool1;
        protected virtual IEnumerator ClearAndCollapseRoutine(List<Piece> lstPiece)
        {
            List<Piece> movingPieces = new List<Piece>();
            List<Piece> matches = new List<Piece>();

            HighlightPiece(lstPiece);
            yield return new WaitForSeconds(0.5f);
            bool isFinished = false;

            while (!isFinished)
            {
                ClearPieceAt(lstPiece);
                yield return new WaitForSeconds(0.25f);
                movingPieces = CollapseColumn(lstPiece);
                //Khi rot xuong se khoang time nhin, kiem tra xem lam dung khong

                //yield return new WaitUntil(() => bool1);

                while (!IsCollapsed(movingPieces))
                {
                    yield return null;
                }
                yield return new WaitForSeconds(0.2f);
                matches = FindMatchesAt(movingPieces);
                if (matches.Count == 0)
                {
                    isFinished = true;
                    break;
                }
                else
                {
                    yield return StartCoroutine(ClearAndCollapseRoutine(matches));
                }
            }
            Debug.LogError(m_allPiece);

            yield return null;
        }

        protected bool IsCollapsed(List<Piece> lstPiece)
        {
            foreach (Piece piece in lstPiece)
            {
                if (piece != null)
                {
                    if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}