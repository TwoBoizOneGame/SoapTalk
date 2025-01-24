using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deform;
using TMPro;
using UnityEngine;

public class WordParser : MonoBehaviour
{
    public TextAsset sentencesAsset;

    public List<Sentence> sentences;

    public GameObject wordPrefab;
    public Transform bubble;
    public Transform wordInsertionPoint;
    public float insertionStrength=1.0f;
    public Deformable deformable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("SpawnSentence");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Load sentences")]
    public void ParseSentences()
    {
        sentences = JsonUtility.FromJson<SentenceCollection>(sentencesAsset.text).sentences;        
    }

    void ClearDeformers()
    {
        while (deformable.DeformerElements.Count > 0)
        {
            deformable.RemoveDeformer(
             deformable.DeformerElements[0].Component);
        }
    }

    IEnumerator SpawnSentence()
    {
        ClearDeformers();
        var sentence = sentences[Random.Range(0, sentences.Count)];
        var split = sentence.text.Split(' ');
        foreach (var word in split)
        {
            SpawnWord(word);
            yield return new WaitForSeconds(1);
        }
    }

    void SpawnWord(string word)
    {
        var obj = Instantiate(wordPrefab, wordInsertionPoint.position, Quaternion.identity);
        var textMesh = obj.GetComponentInChildren<TextMeshPro>();
        textMesh.text = word;
        var col = obj.GetComponent<BoxCollider2D>();
        col.size = new Vector2(textMesh.preferredWidth, textMesh.preferredHeight);
        var rb = obj.GetComponent<Rigidbody2D>();
        obj.transform.position += obj.transform.right*textMesh.preferredWidth;
        rb.AddForce(wordInsertionPoint.forward*insertionStrength, ForceMode2D.Impulse);
        deformable.AddDeformer(obj.GetComponentInChildren<Deformer>());
    }
}
