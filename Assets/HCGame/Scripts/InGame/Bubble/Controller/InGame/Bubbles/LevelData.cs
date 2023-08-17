using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace BubblesShot
{

    public enum ModeGame
    {
        Vertical = 0,
        Rounded,
        Animals,
    }
    public class LevelData
    {
        public static LevelData Instance;

        public static int[] map = new int[11 * 70];
        public static Dictionary<int, BallColor> colorsDict = new Dictionary<int, BallColor>();
    }

}

