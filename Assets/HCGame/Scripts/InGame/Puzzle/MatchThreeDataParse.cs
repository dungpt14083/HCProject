using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.MatchThree.Scripts
{

    public class MatchThreeDataParse : MonoBehaviour
    {


        public DataGame _MatchThreeDataParse;


    }



    [System.Serializable]
    public class DataGame
    {
        public int status;
        public List<Row> nextGrid;
        public List<Row> grid;
        public List<Position> canClear;
        public int points;
        public bool needReset;
        public int numberSwap;
    }



    [System.Serializable]

    public class Row
    {
        public List<int> row;

    }
    [System.Serializable]

    public class Position
    {
        public int x;
        public int y;

    }





}
