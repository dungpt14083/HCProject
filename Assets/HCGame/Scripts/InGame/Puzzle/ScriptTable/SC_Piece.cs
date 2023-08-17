using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.MatchThree.Scripts.ScripTable
{
    [CreateAssetMenu()]
    public class SC_Piece : ScriptableObject
    {
        public string pieceName;
        public GameObject piece;
    }
}