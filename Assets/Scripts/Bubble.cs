using System.Collections.Generic;
using System.Linq;
using Deform;
using Unity.VisualScripting;
using UnityEngine;

public class Bubble : MonoBehaviour 
{
    public EdgeCollider2D edgeCollider2D;
    public MeshRenderer renderer;
    public Deformable deformable;

    public Transform wordInsertionPoint;
    public Transform wordAnchorParent;
    public GameObject wordAnchorPrefab;

    public Transform rightEnd;
    public float anchorSeparation=0.1f;

    public List<WordAnchor> wordAnchors= new List<WordAnchor>();

    public void PrepareAnchors(List<Word> words)
    {
        wordAnchorParent.RemoveChildren();
        wordAnchors.Clear();
        var minX = edgeCollider2D.points.Select(x=> x.x).Min()*transform.localScale.x;
        var maxX = edgeCollider2D.points.Select(x=>x.x).Max()*transform.localScale.x;
        Vector2 startPoint = new Vector2(minX, 0);
        Vector2 currentPoint = startPoint;
        for (int i = 0; i < words.Count; i++)
        {
            Word word = words[i];
            var anchor = Instantiate(wordAnchorPrefab, wordAnchorParent).GetComponent<WordAnchor>();
            anchor.targetText=word.textMesh.text;
            anchor.transform.localScale = new Vector3(word.textMesh.preferredWidth, anchor.transform.localScale.y, anchor.transform.localScale.z);
            var xSize = anchor.transform.localScale.x;
            if (currentPoint.x+xSize > maxX)
            {
                currentPoint = new Vector2(startPoint.x, currentPoint.y-(word.textMesh.preferredHeight+anchorSeparation));
            }
            if (i == 0)
            {
                currentPoint += new Vector2(word.textMesh.preferredWidth,0);
            }
            anchor.transform.position = currentPoint;
            currentPoint += new Vector2(xSize+anchorSeparation,0);
            wordAnchors.Add(anchor);
        }
    }

    public void DestroyBubble()
    {
        foreach (Transform child in transform)
        {
            child.RemoveChildren();
        }
        Destroy(gameObject);
    }

    public void ClearDeformers()
    {
        var deformersToClear = deformable.DeformerElements.Where(x => x.Component is MagnetDeformer).ToList();
        foreach (var deformer in deformersToClear)
        {
            deformable.RemoveDeformer(
             deformer.Component);
        }
    }
}