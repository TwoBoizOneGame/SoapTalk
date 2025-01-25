using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void RemoveChildren(this Transform transform)
    {
        List<Transform> children = new();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        foreach (Transform child in children)
        {
            MonoBehaviour.Destroy(child.gameObject);
        }
    }
}