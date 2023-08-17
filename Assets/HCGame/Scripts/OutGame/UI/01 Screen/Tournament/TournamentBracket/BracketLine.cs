using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class BracketLine : MonoBehaviour
{
    public UILineRenderer lineRenderer;

    public void ShowLineRender(Vector3 start, Vector3 end, Color color, float width)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(start);

        var midPoint1 = new Vector2((start.x + end.x) / 2.0f, start.y);
        points.Add(midPoint1);

        var midPoint2 = new Vector2(midPoint1.x, end.y);
        points.Add(midPoint2);


        points.Add(end);
        lineRenderer.Points = points.ToArray();

        lineRenderer.LineThickness = 8;
        lineRenderer.color = color;
    }
}