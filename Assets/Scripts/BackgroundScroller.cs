using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public List<BackgroundPlane> backgroundPlanes;
    public List<Texture2D> backgroundTextures;
    public Transform screenEndLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var plane in backgroundPlanes)
        {
            SetupPlane(plane);
        }
    }

    void SetupPlane(BackgroundPlane plane)
    {
        plane.GetComponent<MeshRenderer>().material.mainTexture = backgroundTextures[Random.Range(0, backgroundTextures.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        BackgroundPlane planeToMove=null;
        foreach (var plane in backgroundPlanes)
        {
            if (plane.rightEnd.transform.position.x < screenEndLeft.transform.position.x)
            {
                planeToMove = plane;
                break;
            }
        }
        if (planeToMove != null)
        {
            var rightmostPlane = backgroundPlanes.OrderByDescending(x => x.transform.position.x).First();
            planeToMove.transform.position = new Vector3(rightmostPlane.transform.position.x+rightmostPlane.transform.localScale.x*10, planeToMove.transform.position.y, planeToMove.transform.position.z);
            SetupPlane(planeToMove);
        }
    }
}
