using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BubbleColliderGenerator : MonoBehaviour
{
    public MeshRenderer sphereRenderer;
    public EdgeCollider2D edgeCollider2D;
    public int pointCount = 36;

    [ContextMenu("Generate Collider")]
    void GenerateCollider()
    {
        var radius = sphereRenderer.bounds.extents.x/sphereRenderer.transform.localScale.x;
        var step = 360/pointCount;
        List<Vector2> points = new();
        for (int i = 0; i <= pointCount; i++)
        {
            var p = new Vector2(Mathf.Sin(i*step*Mathf.Deg2Rad)*radius, Mathf.Cos(i*step*Mathf.Deg2Rad)*radius);
            points.Add(p);
        }
        edgeCollider2D.SetPoints(points);
    }
}
