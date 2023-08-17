using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace MiniGame.MatchThree.Scripts.ScripTable
{
    [CreateAssetMenu()]
    public class SC_Level : ScriptableObject
    {
        public int width;
        public int height;
        public GoalType goalType;
        public int targetTimeSecond;
        [TableList(ShowIndexLabels =true)]
        public SC_Piece[] pieceList;
        public List<LevelGridPosition> levelGridPositionList;

        [System.Serializable]
        public class LevelGridPosition
        {

            public SC_Piece piece;
            public int x;
            public int y;
        }

        public enum GoalType
        {
            Time,
        }
    }
}
