using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WordPlacer.asset", menuName = "Cards/Word Placer Card")]

public class WordPlacerCard : CardBase
{
    public override void OnBought()
    {
        base.OnBought();
        var emptyAnchors = GameManager.instance.bubble.wordAnchors.Where(x => x.currentlyHeldWord == null).ToList();
        var anchor = emptyAnchors[Random.Range(0, emptyAnchors.Count)];
        var relatedWord = GameManager.instance.spawnedWords.First(x => x.textMesh.text == anchor.targetText);
        relatedWord.PlaceAtAnchor(anchor);
    }
}