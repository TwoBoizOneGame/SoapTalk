using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Deform;
using DG.Tweening;
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
    public Vector2 anchorSeparation = new Vector2(2, 1);

    public List<WordAnchor> wordAnchors = new List<WordAnchor>();

    public async UniTask PrepareAnchors(List<Word> words)
    {
        wordAnchorParent.RemoveChildren();
        wordAnchors.Clear();
        var minX = edgeCollider2D.points.Select(x => x.x).Min() * transform.localScale.x;
        var maxX = edgeCollider2D.points.Select(x => x.x).Max() * transform.localScale.x - .25f;
        Vector2 startPoint = new Vector2(minX, 0);
        Vector2 currentPoint = startPoint;
        int lineCount = 1;
        for (int i = 0; i < words.Count; i++)
        {
            Word word = words[i];
            var anchor = Instantiate(wordAnchorPrefab, wordAnchorParent).GetComponent<WordAnchor>();
            anchor.targetText = word.textMesh.text;
            anchor.CalculateScale(word.textMesh.preferredWidth);
            var xSize = anchor.GetXSize();
            anchor.transform.localScale = Vector3.zero;
            if (currentPoint.x + xSize >= maxX)
            {
                var newY = currentPoint.y - (word.textMesh.preferredHeight + anchorSeparation.y);
                var closestPoint = edgeCollider2D.points.OrderBy(x => ((x * transform.localScale.x) - new Vector2(startPoint.x, newY)).sqrMagnitude).First();
                closestPoint *= transform.localScale.x;
                currentPoint = new Vector2(closestPoint.x, newY);
                lineCount++;
            }
            anchor.transform.position = currentPoint;
            AudioManager.instance.PlayOneShotAsync(AudioManager.instance.spawnAnchorSounds);
            currentPoint += new Vector2(xSize + anchorSeparation.x, 0);
            wordAnchors.Add(anchor);
        }
        if (lineCount >= 3)
        {
            foreach (var anchor in wordAnchors)
            {
                anchor.transform.position += Vector3.up * 2;
            }
        }
        foreach (var anchor in wordAnchors)
        {
            await anchor.transform.DOScale(Vector3.one, 0.5f).AsyncWaitForCompletion();
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