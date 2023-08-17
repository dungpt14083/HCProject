using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosionInfo
{
    public Vector2 Position;
    public Vector2 MaxX;
    public Vector2 MaxY;
    public BombExplosionInfo(Vector2 position, Vector2 HorizontalEplosion, Vector2 VerticalExplosion)
    {
        Position = position;
        MaxX = HorizontalEplosion;
        MaxY = VerticalExplosion;
    }
}
