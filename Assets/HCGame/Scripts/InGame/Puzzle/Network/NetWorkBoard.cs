using System;
using DG.Tweening;
using MiniGame.MatchThree.Scripts.GamePlay;
using MiniGame.MatchThree.Scripts.ScripTable;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweens.Plugins;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MiniGame.MatchThree.Scripts.Network
{
    public class NetWorkBoard : SingleBoard
    {
        public List<TextMeshPro> pieceSynch0;
        public List<TextMeshPro> pieceSynch1;
        public List<TextMeshPro> pieceSynch2;
        public List<TextMeshPro> pieceSynch3;
        public List<TextMeshPro> pieceSynch4;
        public List<TextMeshPro> pieceSynch5;
        public List<TextMeshPro> pieceSynch6;
        public List<TextMeshPro> pieceSynch7;
        public List<TextMeshPro> pieceSynch8;
        public List<TextMeshPro> pieceSynch9;
        public List<TextMeshPro> pieceSynch10;
        public List<TextMeshPro> pieceSynch11;










        [SerializeField] private TextMeshPro[] piece0;
        [SerializeField] private TextMeshPro[] piece1;
        [SerializeField] private TextMeshPro[] piece2;
        [SerializeField] private TextMeshPro[] piece3;
        [SerializeField] private TextMeshPro[] piece4;
        [SerializeField] private TextMeshPro[] piece5;
        [SerializeField] private TextMeshPro[] piece6;
        [SerializeField] private TextMeshPro[] piece7;
        [SerializeField] private TextMeshPro[] piece8;
        [SerializeField] private TextMeshPro[] piece9;
        [SerializeField] private TextMeshPro[] piece10;
        [SerializeField] private TextMeshPro[] piece11;

        [SerializeField] private TweenGroup groupStartGame;

        private bool isStartGame = true;
        private float swapTime = 0.15f;
        private RoyalMatch.DataGame royalMatchData;


        public override void Start()
        {
            MatchThreeGameSystem.Instance.ActStartGame += OnStartGame;
            //MatchThreeNetworkManager.Instance.SendRequestFindRoom(); 
            MatchThreeNetworkManager.Instance.SendRequestStartGame();
            HcPopupManager.Instance.ShowEightGameLoading(false);
        }
        public static NetWorkBoard instance;
        public override void Awake()
        {
            instance = this;
            base.Awake();
        }

        //Start Game
        public void OnStartGame(RoyalMatch.DataGame data)
        {
            canClearList.Clear();
            royalMatchData = data;
            if (isStartGame)
            {

                SetupTextTiles();
                MatchThreeManager.Instance.gameplayDialog.OnStartGame();
                groupStartGame.OnPlay();
                isStartGame = false;
            }
            else
            {

                SetupGame();
            }

        }

        public void SpawnSpecialTransform(List<specialTransform> specialTransformList)
        {
            Debug.LogWarning("SpawnSpecialTransform:__" + specialTransformList.ToArray().ToString());


            //stop game
            delay0 = false;
            delay1 = false;
            delay2 = false;
            delay3 = false;
            delay4 = false;

            StartCoroutine(SpawnSpecialTransform());
            IEnumerator SpawnSpecialTransform()
            {

                for (int i = 0; i < specialTransformList.Count; i++)
                {
                    yield return new WaitForSeconds(.1f);
                    ClearPieceAt(specialTransformList[i].position.x, specialTransformList[i].position.y);
                    yield return new WaitForSeconds(.1f);
                    forceFill(specialTransformList[i].position.x, specialTransformList[i].position.y, specialTransformList[i].value);

                }

                delay0 = true;
                delay1 = true;
                delay2 = true;
                delay3 = true;
                delay4 = true;
                yield return null;
            }
        }







        public float offsetNew_1_TimeWaitToClear = 0.2f;
        public void SetupGame()
        {
            if (!royalMatchData.NeedReset)
            {
                FillBoard(10, 0.5f);
                DOVirtual.Float(1, 10, offsetNew_1_TimeWaitToClear, (value) => { })
                .OnComplete(() =>
                {
                    //FillNextGrid();
                    //Kiểm tra xem Canclear có không
                    if (royalMatchData.CanClear.Count > 0)
                    {
                        m_playerInputEnable = false;
                        ClearData();
                    }
                    else// Nếu ko remove nữa thì cho user swap
                    {
                        m_playerInputEnable = true;
                        m_clickedTile = null;
                        m_targetTile = null;
                    }

                    MatchThreeManager.Instance.gameplayDialog.OnLoading(false);
                });
            }
            else//Reset lại ván cờ do không có nước đi nữa
            {
                MatchThreeManager.Instance.gameplayDialog.OnLoading(true);
                SendMessageResetBoard();
            }
        }

        #region Setup Data
        protected override void FillBoard(int falseYOffset = 0, float moveTime = 0.1F)
        {
            Debug.LogWarning("FillBoard()");
            if (royalMatchData != null)
            {
                for (int j = 0; j < royalMatchData.Grid.Count; j++)
                {
                    for (int i = 0; i < royalMatchData.Grid[j].Row_.Count; i++)
                    {
                        if (m_allPiece[i, j] == null)
                        {
                            //UnityEngine.Debug.Log("FillBoard: " + i + "," + j + ": " + data.Grid[j].Row_[i]);
                            var randomPiece = FillPieceAt(i, j, falseYOffset, moveTime);
                            m_allPiece[i, j] = randomPiece;
                        }
                    }
                }
            }
        }





        private void FillNextGrid()
        {
            Debug.LogWarning("FillNextGrid()");

            if (royalMatchData != null)
            {
                for (int j = 0; j < royalMatchData.NextGrid.Count; j++)
                {
                    for (int i = 0; i < royalMatchData.NextGrid[j].Row_.Count; i++)
                    {
                        var idx = royalMatchData.NextGrid[j].Row_[i];
                        if (idx >= 0)
                        {
                            var piece = piece0[i];
                            if (j == 0)
                                piece = piece0[i];
                            else if (j == 1)
                                piece = piece1[i];
                            else if (j == 2)
                                piece = piece2[i];
                            else if (j == 3)
                                piece = piece3[i];
                            else if (j == 4)
                                piece = piece4[i];
                            else if (j == 5)
                                piece = piece5[i];
                            else if (j == 6)
                                piece = piece6[i];
                            else if (j == 7)
                                piece = piece6[i];
                            else if (j == 8)
                                piece = piece6[i];
                            else if (j == 9)
                                piece = piece6[i];
                            piece.text = idx.ToString();
                        }
                    }
                }
            }
        }


        [Button]
        public bool IsBoardNullAt(int x, int y)
        {
            if (m_allPiece[x, y] == null) return true;
            return false;
        }

        [Button]
        public Piece GetPieceAt(int x, int y)
        {
            return m_allPiece[x, y];
        }




        [Button]
        public void LogCurBoard()
        {
            for (int i = 0; i < m_allPiece.GetLength(0); i++)
            {
                Debug.LogError(i + "__");
                for (int j = 0; j < m_allPiece.GetLength(1); j++)
                {
                    if (m_allPiece[i, j] == null)
                    {
                        //Debug.LogError("NULL AT x: " + i + "___y: " + j);  // xoa gem tai vi tri nay
                    }
                    else
                    {

                        //Debug.LogError("" + m_allPiece[i, j].txtNumber.text);
                    }
                }
            }
        }


        [Button]
        public void forceFill(int x, int y, int gemId)
        {

            if (m_allPiece[x, y] == null)
            {
                var newPiece = ForceFillAt(x, y, 0, .3f, true, gemId);
                m_allPiece[x, y] = newPiece;
            }
        }



        [Button]
        public void m_1_normalFill()
        {
            if (royalMatchData != null)
            {
                for (int j = 0; j < royalMatchData.NextGrid.Count; j++)
                {
                    for (int i = 0; i < royalMatchData.NextGrid[j].Row_.Count; i++)
                    {
                        if (m_allPiece[i, j] == null)
                        {
                            //UnityEngine.Debug.Log("RefillBoard: " + i + "," + j + ": " + data.NextGrid[j].Row_[i]);
                            var newPiece = FillPieceAt(i, j, 0, 2, true);
                            m_allPiece[i, j] = newPiece;
                        }
                    }
                }
            }

        }





        [Button]
        public void SendMessageContinueRound2()
        {
            Debug.LogWarning("ResetBoard");
            if (royalMatchData != null)
            {
                for (int j = 0; j < royalMatchData.Grid.Count; j++)
                {
                    for (int i = 0; i < royalMatchData.Grid[j].Row_.Count; i++)
                    {
                        if (m_allPiece[i, j] != null)
                            Destroy(m_allPiece[i, j].gameObject);
                    }
                }
            }

            MatchThreeNetworkManager.Instance.SendRequestAction(ROYAL_MATCH_TYPE.PLAY_GAME);
        }





        [Button]
        public void SendMessageResetBoard()
        {
            Debug.LogWarning("ResetBoard");
            if (royalMatchData != null)
            {
                for (int j = 0; j < royalMatchData.Grid.Count; j++)
                {
                    for (int i = 0; i < royalMatchData.Grid[j].Row_.Count; i++)
                    {
                        if (m_allPiece[i, j] != null)
                            Destroy(m_allPiece[i, j].gameObject);
                    }
                }
            }

            MatchThreeNetworkManager.Instance.SendRequestAction(ROYAL_MATCH_TYPE.RESET);
        }

        protected override Piece FillPieceAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1F, bool isNextGrid = false)
        {
            var sc_pice = GetPiece(x, y);
            if (isNextGrid)
                sc_pice = GetNextGridPieces(x);
            GameObject newPiece = Instantiate(sc_pice.piece, Vector3.zero, Quaternion.identity);

            if (newPiece != null)
            {
                newPiece.GetComponent<Piece>().OnInit(this, sc_pice.name);
                PlacePiece(newPiece.GetComponent<Piece>(), x, y);
                if (falseYOffset != 0)
                {
                    newPiece.transform.position = new Vector3(x, y + falseYOffset, 0);
                    newPiece.GetComponent<Piece>().SlowMove(x, y);
                }
                newPiece.transform.parent = parentPiece;
                return newPiece.GetComponent<Piece>();
            }

            return null;
        }

        public Piece ForceFillAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1F, bool isNextGrid = false, int gemId = -100)
        {
            Debug.LogWarning("FORCE FILL AT x:" + x + "__y: " + y + "__" + gemId);
            var piece = level.pieceList[0];



            level.pieceList.ForEach(item =>
            {
                if (item.name == "Piece" + gemId)
                {
                    piece = item;
                }
            });


            if (gemId == 10)
            {
                piece = level.pieceList[15];
            }
            if (gemId == 9)
            {
                piece = level.pieceList[14];
            }

            GameObject newPiece = Instantiate(piece.piece, Vector3.zero, Quaternion.identity);


            newPiece.GetComponent<Piece>().OnInit(this, piece.name);
            PlacePiece(newPiece.GetComponent<Piece>(), x, y);
            if (falseYOffset != 0)
            {
                newPiece.transform.position = new Vector3(x, y + falseYOffset, 0);
                newPiece.GetComponent<Piece>().Move(x, y, moveTime);
            }
            newPiece.transform.parent = parentPiece;
            return newPiece.GetComponent<Piece>();


        }



        private SC_Piece GetPiece(int x, int y)
        {
            var newPieceIDx = royalMatchData.Grid[y].Row_[x];
            switch (newPieceIDx)
            {
                case 41: newPieceIDx = 8; break;
                case 42: newPieceIDx = 9; break;
                case 44: newPieceIDx = 10; break;
                case 51: newPieceIDx = 11; break;
                case 55: newPieceIDx = 12; break;
                case 8: newPieceIDx = 13; break;
                case 9: newPieceIDx = 14; break;
                case 10: newPieceIDx = 15; break;
            }

            if (newPieceIDx >= 0)
            {
                if (level.pieceList[newPieceIDx] == null)
                {
                    Debug.LogError("BOARD: " + newPieceIDx + "does not contain a valid piece prefab!");
                }


                return level.pieceList[newPieceIDx];
            }
            return null;
        }

        private SC_Piece GetNextGridPieces(int x)
        {
            var randomIdx = -1;
            for (int i = 0; i < royalMatchData.NextGrid[x].Row_.Count; i++)
            {
                if (royalMatchData.NextGrid[i].Row_[x] != 0)
                {
                    randomIdx = royalMatchData.NextGrid[i].Row_[x] - 1;
                    royalMatchData.NextGrid[i].Row_[x] = 0;
                    break;
                }
            }

            if (randomIdx >= 0)
            {
                if (level.pieceList[randomIdx] == null)
                {
                    Debug.LogWarning("BOARD: " + randomIdx + "does not contain a valid piece prefab!");
                }

                return level.pieceList[randomIdx];
            }

            return null;
        }
        #endregion
        public List<Piece> canClearList;
        #region Clear Data
        private void ClearData()
        {
            var canClear = royalMatchData.CanClear;

            Debug.LogWarning("canClear" + canClear);
            if (canClear.Count > 0)
            {
                canClearList = new List<Piece>();







                for (int i = 0; i < canClear.Count; i++)
                {
                    var piece = m_allPiece[canClear[i].X, canClear[i].Y];
                    if (piece != null)
                    {
                        canClearList.Add(piece);
                    }
                }


                ClearAndRefillBoard(canClearList);
            }
        }

        public float
            offset0_FinishSwapThen_HightLight,
            offset1_hightLightTime = 0.2f,
            offset2_FinishClearThen_SpawnSpecial = 0.2f,
            offset3_SpawnSpecialThen_Collapse = 0.2f,
            offset4_FinishCollapThen_Wait = 0.2f,
            offset5_FinishAllClearCollapSpawnThen_ReFill = 0.2f,
            offset6_FinishFilledNextGridThen_SendMessageClear = 0.2f;
        protected override IEnumerator ClearAndRefillBoardRoutine(List<Piece> lstPiece)
        {
            //clear and collapse
            yield return StartCoroutine(ClearAndCollapseRoutine(lstPiece));
            yield return new WaitForSeconds(offset5_FinishAllClearCollapSpawnThen_ReFill);
            //refill
            //4.Lấy nextgrid đẩy xuống những trống
            StartCoroutine(RefillRoutine());
        }


        public bool delay0, delay1, delay2, delay3, delay4;

        protected override IEnumerator ClearAndCollapseRoutine(List<Piece> lstPiece) // luồng chơi game
        {
            Debug.LogWarning("ClearAndCollapseRoutine");
            List<Piece> movingPieces = new List<Piece>();
            List<Piece> matches = new List<Piece>();


            yield return new WaitUntil(() => delay0);
            yield return new WaitForSeconds(offset0_FinishSwapThen_HightLight);



            HighlightPiece(lstPiece);
            yield return new WaitUntil(() => delay1);
            yield return new WaitForSeconds(offset1_hightLightTime);


            //Nếu canClear có data thì remove những khối đó đi
            ClearPieceAt(lstPiece);
            yield return new WaitUntil(() => delay2);
            yield return new WaitForSeconds(offset2_FinishClearThen_SpawnSpecial);

            // fill special gem vào chỗ này
            SpawnNewSpeciaPieces();

            //Đẩy những khối nếu có phía trên remove khối xuống
            yield return new WaitUntil(() => delay3);
            yield return new WaitForSeconds(offset3_SpawnSpecialThen_Collapse);
            movingPieces = CollapseColumn(lstPiece);



            yield return new WaitUntil(() => delay4);
            yield return new WaitForSeconds(offset4_FinishCollapThen_Wait);
            //Khi rot xuong se khoang time nhin, kiem tra xem lam dung khong
            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

        }
        public bool bool1, bool2, bool3;



        public void SpawnNewSpeciaPieces()
        {
            MatchThreeGameSystem.Instance._puzzleDataJson.specialSpawn.ForEach(item =>
            {
                Debug.LogWarning("SPECIAL SPAWN piece: " + item.value);
                forceFill(item.x, item.y, item.value);
            });
        }






        public bool m_0_sapAnTuDong, m_2_fillXongAnGemGiongNhau;
        private IEnumerator RefillRoutine()
        {
            //yield return new WaitUntil(() => bool3);
            FillNextGrid();
            yield return new WaitForSeconds(offset6_FinishFilledNextGridThen_SendMessageClear);
            MatchThreeNetworkManager.Instance.SendRequestAction(ROYAL_MATCH_TYPE.CLEAR);

        }
        [Button]
        public void LateClear()
        {
            MatchThreeNetworkManager.Instance.SendRequestAction(ROYAL_MATCH_TYPE.CLEAR);
        }

        #endregion

        #region Control
        public override void ReleaseTile()
        {
            if (m_clickedTile != null && m_targetTile != null)
            {
                SwitchTiles(m_clickedTile, m_targetTile);
            }

            m_clickedTile = null;
            m_targetTile = null;
        }

        protected override IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
        {
            if (m_playerInputEnable)// Khi dang thuc hien viec xu ly cac piece thi cho hoan thanh moi lam tiep
            {
                Piece clickPiece = m_allPiece[clickedTile.xIndex, clickedTile.yIndex];
                Piece targetPiece = m_allPiece[targetTile.xIndex, targetTile.yIndex];

                if (targetPiece != null && clickPiece != null)
                {
                    m_playerInputEnable = false;
                    clickPiece.ResetHint();
                    targetPiece.ResetHint();
                    clickPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                    targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

                    yield return new WaitForSeconds(swapTime);

                    List<Piece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                    List<Piece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

                    if (clickPiece.IsSpecialPiece() || targetPiece.IsSpecialPiece())
                    {
                        SendRequestSwap(clickedTile, targetTile);
                    }
                    else if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0)
                    {
                        SendRequestSwap(clickedTile, targetTile);

                        Position pos = new Position();
                        MatchThreeGameSystem.Instance._data.points = 0; //reset data get from server
                        yield return new WaitUntil(() => MatchThreeGameSystem.Instance._data.points != 0);//wait till get new response

                        if (MatchThreeGameSystem.Instance._data.canClear.Count == 0)// no canclear swap back
                        {
                            clickPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                            targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                            //Trong thời gian swap piece thì không được click nữa
                            yield return new WaitForSeconds(swapTime);
                            m_playerInputEnable = true;
                            //MatchThreeGameSystem.Instance.veryfiAll();
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(swapTime);

                        SendRequestSwap(clickedTile, targetTile);
                    }
                }
            }
        }







        private void SendRequestSwap(Tile clickedTile, Tile targetTile)
        {
            var type = ROYAL_MATCH_TYPE.None;
            if (clickedTile.xIndex - targetTile.xIndex == 1 && clickedTile.yIndex == targetTile.yIndex)
            {
                //left
                type = ROYAL_MATCH_TYPE.SWAP_LEFT;
            }
            else if (clickedTile.xIndex - targetTile.xIndex == -1 && clickedTile.yIndex == targetTile.yIndex)
            {
                //right
                type = ROYAL_MATCH_TYPE.SWAP_RIGHT;
            }
            else if (clickedTile.xIndex == targetTile.xIndex && clickedTile.yIndex - targetTile.yIndex == 1)
            {
                //up
                type = ROYAL_MATCH_TYPE.SWAP_DOWN;
            }
            if (clickedTile.xIndex == targetTile.xIndex && clickedTile.yIndex - targetTile.yIndex == -1)
            {
                //down
                type = ROYAL_MATCH_TYPE.SWAP_UP;
            }

            if (type != ROYAL_MATCH_TYPE.None)
                MatchThreeNetworkManager.Instance.SendRequestAction(type, clickedTile.xIndex, clickedTile.yIndex);
        }
        #endregion

        private void OnEnable()
        {
            //MatchThreeGameSystem.Instance.ActStartGame += OnStartGame;
            //MatchThreeNetworkManager.Instance.SendRequestStartGame();
        }

        private void OnDestroy()
        {
            MatchThreeGameSystem.Instance.ActStartGame -= OnStartGame;

        }

        private void OnDisable()
        {
            //MatchThreeGameSystem.Instance.ActStartGame -= OnStartGame;
        }


        public GameObject tileGr;
        public void SpawnFloorGridAtBottomLeft(int row, int col, int type)
        {
            string gridName = "(" + row + "," + col + ")";
            var grid = tileGr.transform.Find(gridName);
            grid.GetComponent<Tile>().SetGrass(type);
        }
        [Button]
        public void SynchFloorGrid()
        {

            var idx = 0;
            MatchThreeGameSystem.Instance.floorGridCol0.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(0, idx, item);
                idx++;
            });
            idx = 0;
            MatchThreeGameSystem.Instance.floorGridCol1.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(1, idx, item);
                idx++;
            });
            idx = 0;
            MatchThreeGameSystem.Instance.floorGridCol2.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(2, idx, item);
                idx++;
            });
            idx = 0;
            MatchThreeGameSystem.Instance.floorGridCol3.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(3, idx, item);
                idx++;
            });
            idx = 0;
            MatchThreeGameSystem.Instance.floorGridCol4.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(4, idx, item);
                idx++;
            });
            idx = 0;
            MatchThreeGameSystem.Instance.floorGridCol5.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(5, idx, item);
                idx++;
            });
            idx = 0;

            MatchThreeGameSystem.Instance.floorGridCol6.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(6, idx, item);
                idx++;
            });
            idx = 0;

            MatchThreeGameSystem.Instance.floorGridCol7.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(7, idx, item);
                idx++;
            });
            idx = 0;

            MatchThreeGameSystem.Instance.floorGridCol8.ForEach(item =>
            {
                SpawnFloorGridAtBottomLeft(8, idx, item);
                idx++;
            });

        }


    }
}