using UnityEngine;
using DG.Tweening;

public class CoinAnim : MonoBehaviour
{

    public float rotation = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotation++;
        transform.eulerAngles = new Vector3(0, rotation, 0);
        if (rotation > 180)
        {
            rotation = -180;
        }

    }
}
