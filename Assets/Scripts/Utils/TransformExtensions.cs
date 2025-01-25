using UnityEngine;

public static class TransformExtensions
{
    public static void RemoveChildren(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            MonoBehaviour.Destroy(transform.GetChild(0).gameObject);
        }
    }
}