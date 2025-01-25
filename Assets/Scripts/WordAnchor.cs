using Unity.VisualScripting;
using UnityEngine;

public class WordAnchor : MonoBehaviour
{
    public MeshRenderer renderer;
    public Transform placementPoint;
    public BoxCollider placementArea;
    public Material defaultMaterial;
    public Material hoverMaterial;

    public Word currentlyHeldWord;
    public string targetText;

}
