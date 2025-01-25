using Deform;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Word : MonoBehaviour
{
    public Deformer deformer;
    public TextMeshPro textMesh;
    public BoxCollider2D boxCollider2D;
    public Rigidbody2D rigidbody2D;

    public bool isBeingDragged=false;
    public Color highlightColor = Color.green;
    public Color normalColor = Color.white;

    public WordAnchor relatedAnchor;

    public void Setup(string word)
    {
        textMesh.text = word;        
        boxCollider2D.size = new Vector2(Mathf.Min(1, textMesh.preferredWidth), Mathf.Min(1, textMesh.preferredHeight));
        this.name = $"Word ({word})";
    }

    Vector3 previousPos;
    public void FixedUpdate()
    {
        if (isBeingDragged)
        {
            previousPos = transform.position;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mousePos = ray.GetPoint(-Camera.main.transform.position.z);
            mousePos = new Vector3(mousePos.x, mousePos.y,0);

            var targetPos = new Vector3(mousePos.x, mousePos.y);

            var bubble = GameManager.instance.bubble; 

            var maxDistFromCenter=(bubble.renderer.bounds.extents.x)*.8f;
            var distFromCenter = targetPos-bubble.transform.position;
            // Debug.Log($"{mousePos} {distFromCenter.magnitude} {maxDistFromCenter}");
            if (distFromCenter.sqrMagnitude > maxDistFromCenter*maxDistFromCenter)
            {
                targetPos = bubble.transform.position+(distFromCenter.normalized*maxDistFromCenter)*.85f;
            }

            rigidbody2D.linearVelocity=Vector2.zero;
            transform.position=targetPos;
        }
    }

    public bool CheckMouseOverlap(Vector3 pos)
    {
        var result = boxCollider2D.OverlapPoint(pos);
        if (result) OnHover();
        else OnStopHover();

        return result;
    }

    public void OnHover()
    {
        textMesh.color = highlightColor;
    }

    public void OnStopHover()
    {
        textMesh.color=normalColor;
    }

    public void BeginBeingDragged()
    {
        isBeingDragged=true;
        if (relatedAnchor != null)
        {
            relatedAnchor.currentlyHeldWord = null;
            relatedAnchor = null;
        }
    }

    public void StopBeingDragged()
    {
        isBeingDragged=false;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        var velocityDiff =transform.position-previousPos;
        var defaultGravity = Vector2.down * 0.1f;
        rigidbody2D.linearVelocity = Vector2.Max(velocityDiff, defaultGravity);
        if (relatedAnchor != null)
        {
            relatedAnchor.currentlyHeldWord=null;
            relatedAnchor=null;
        }
    }

    public void PlaceAtAnchor(WordAnchor anchor)
    {
        anchor.currentlyHeldWord=this;
        relatedAnchor=anchor;
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.angularVelocity=0;
        rigidbody2D.linearVelocity=Vector2.zero;
        transform.position = anchor.placementPoint.position+boxCollider2D.bounds.extents.y*Vector3.up;
        transform.rotation=Quaternion.identity;
    }
}
