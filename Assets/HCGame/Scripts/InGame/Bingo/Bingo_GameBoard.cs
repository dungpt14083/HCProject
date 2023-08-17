using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

namespace Bingo
{
    public class Bingo_GameBoard : MonoBehaviour
    {

        public static Bingo_GameBoard instance;
        public GameObject contents;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }




        [SerializeField]
        public BingoGameData bingoData;

        [Button]
        public void OnReceivedNewWebsocketMessage_UpdateGameBoard(BingoGameData bingoDataParam)
        {
            bingoData = bingoDataParam;
            int index = 0;
            
            bingoData.boards.ForEach(board =>
            {
                boardList[index].SetValue(board.value, board.slotType, 0);
                index++;
            });
            int index2 = 0;
            bingoData.boards.ForEach(board =>
            {
                boardList2[index2].SetValue(board.value, board.slotType, 0);
                index2++;
            });
            Bingo_Bot_Booster.instance.ShowButonBingo(CheckCanBingo());
        }


        public List<Bingo_GameBoardSingleItem> boardList;
        public List<Bingo_GameBoardSingleItem> boardList2;


        bool CheckCanBingo()
        {
            if (CheckDiagonal1())
            {
                Debug.Log("==========Duong cheo 1===============");
                return true;
            }
                
            if (CheckDiagonal2())
            {
                Debug.Log("==========Duong cheo 2===============");
                return true;
            }
                
            for (int i=0; i< 5; i++)
            {
                if (CheckRow(i))
                {
                    Debug.Log($"==========Duong ngang {i}===============");
                    return true;
                }
            }
            for(int i =0; i< 5; i++)
            {
                if (CheckCol(i))
                {
                    Debug.Log($"==========Duong doc {i}===============");
                    return true;
                }
            }

            if (CheckCorners())
            {
                Debug.Log("==========Goc ===============");
                return true;
            }
            return false;
        }
        
        bool CheckCorners()
        {
            if(CheckCellType(0,0)&&CheckCellType(0,4)&&CheckCellType(4,0)&&CheckCellType(4,4))
            {
                return false;
            }
            if (CheckCell(0, 0)&&CheckCell(0, 4)&&CheckCell(4, 0)&&CheckCell(4, 4))
            {
                return true;
                
            }
           

            return false;
        }
        bool CheckCellType(int row, int col)
        {
          
            
            if (boardList[row * 5 + col].mySlotType==3)
            {
                return true;
            }

            return false;
        }
        bool CheckCell(int row, int col)
        {
            bool isExistType1 = false, isBingo = true;
            var cell = boardList[row * 5 + col]; // Lấy đối tượng Cell tại vị trí (row, col)
            
             if (cell.mySlotType == 1||cell.mySlotType==3)
            {
                isExistType1 = true;
            }

            if(cell.mySlotType <= 0)
            {
                isBingo = false;
               
            }

            return isExistType1 && isBingo;
        }

        bool CheckRow(int index)
        {
            bool isExistType1 = false, isBingo = true;
            for (int i=0; i< 5; i++)
            {
                int idexCell = index * 5 + i;
                if (boardList[idexCell].mySlotType == 1)
                {
                    isExistType1 = true;
                }
                if(boardList[idexCell].mySlotType <= 0)
                {
                    isBingo = false;
                    break;
                }

            }
            return isBingo && isExistType1;
        }
        bool CheckCol(int index)
        {
            bool isExistType1 = false, isBingo = true;
            
            for (int i = 0; i < 5; i++)
            {
                int idexCell = index + (5 * i);
                if (boardList[idexCell].mySlotType == 1)
                {
                    isExistType1 = true;
                }
                if (boardList[idexCell].mySlotType <= 0)
                {
                    isBingo = false;
                    break;
                }
            }
            return isBingo && isExistType1;
        }

        bool CheckDiagonal1()
        {
            bool isExistType1 = false, isBingo = true;
            for (int i=0; i < 5; i++)
            {
                if (boardList[i * 5 + i].mySlotType == 1)
                {
                    isExistType1 = true;
                }
                if (boardList[i * 5 + i].mySlotType <= 0)
                {
                    isBingo = false;
                    break;
                }
            }
            return isBingo && isExistType1;
        }
        bool CheckDiagonal2()
        {
            bool isExistType1 = false, isBingo = true;
            for (int i = 0; i < 5; i++)
            {
                if (boardList[i * 5 + (4-i)].mySlotType == 1)
                {
                    isExistType1 = true;
                }
                if (boardList[i * 5 + (4 - i)].mySlotType <= 0)
                {
                    isBingo = false;
                    break;
                }
            }
            return isBingo && isExistType1;
        }
    }
    [Serializable]
    public class BingoGameData
    {
        public int gamePhase;
        //0 là chưa bắt đầu, mới chỉ load cái bàn ra thôi
        //1 là đếm ngược 1 2 3 go
        //2 là bắt đầu chơi được tương tác được, trừ thời gian, gửi target các thứ...
        //3 là kết thúc không chơi được nữa, không cho bấm bất kì nút gì cả, lúc này diễn đồ họa thắng thua các kiểu
        //4 hoàn thành game, trở về ui replay hay thoát

        public scoreDetail scoreDetail;
        public string currentScore;
        public int secondTimeLeft;
        public string choseTarget;//B_23
        public int choseTargetSecondTime;

        [TableList(ShowIndexLabels = true)]
        public List<BingoBoard> boards;
        public List<BingoBoard> boards2;
        public List<BingoBoosterBar> boosterBalls;
        
    }
    [Serializable]
    public class BingoBoard
    {
        public int slotType;
        public int value;
        //0 là màu trắng, chạm vào để gọi số
        //1 là màu xanh, đã được chạm vào và đúng
        //2 là kim cương
        //3 là ngôi sao(bingo)
    }

    [Serializable]
    public class BingoBoosterBar
    {
        public int loading;
        public string booster;
    }

    [Serializable]
    public class BingoGameDataPlayer2
    {
        public ScoreBingo scores;
    }
    [Serializable]
    public class ScoreBingo
    {
        public string header;
        public int player2_score;
        public string player2_name;
        public string player2_token;
    }


    public class scoreDetail
    {
        public Score Doubs;
        public Score Bingos;
        public Score MultiBingos;
        public Score DoubleScore;
        public Score Penalties;

    }
    public class Score
    {
        public int times;
        public int point;
    }

}