using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class WordAnchor : MonoBehaviour
{
    public List<MeshRenderer> cubeRenderers;
    public Transform placementPoint;
    public BoxCollider placementArea;
    public Material defaultMaterial;
    public Material hoverMaterial;

    public Word currentlyHeldWord;
    public string targetText;

    public GameObject center;
    public GameObject rightCorner;


    void Start()
    {
        // transform.DOMoveY(transform.position.y+.4f, 1).SetLoops(-1, LoopType.Yoyo);
        SetMaterials(defaultMaterial);
    }

    public void CalculateScale(float width)
    {
        center.transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z);
        var xSize = center.transform.localScale.x+center.transform.localPosition.x;
        rightCorner.transform.localPosition = new Vector3(xSize, rightCorner.transform.localPosition.y,rightCorner.transform.localPosition.z);
        placementArea.size = new Vector3(xSize, 1, 1);
        placementArea.center = new Vector3(xSize/2,placementArea.center.y, placementArea.center.z);
        name = $"WordAnchor (w={GetXSize()})";
    }

    public float GetXSize()
    {
        return center.transform.localScale.x+(rightCorner.transform.localScale.x*2);
    }

    public void SetMaterials(Material mat)
    {
        foreach (var rend in cubeRenderers)
        {
            rend.sharedMaterial = mat;
        }
    }
}
