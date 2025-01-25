using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Deform;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }

    public GameObject wordPrefab;
    public Bubble bubble;
    public Transform wordInsertionPoint;
    public float insertionStrength = 1.0f;
    public Deformable bubbleDeformable;

    public WordParser wordParser;

    List<Word> spawnedWords = new List<Word>();

    public Word currentlyDraggedWord;
    public WordAnchor currentlyHoveredAnchor;

    void Start()
    {
        _instance = this;
        wordParser.LoadSentences();
        StartCoroutine("SpawnSentence");
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 5);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var wa = hit.collider.GetComponentInParent<WordAnchor>();
            if (wa != null && wa.currentlyHeldWord == null)
            {
                if (currentlyHoveredAnchor != null)
                    currentlyHoveredAnchor.renderer.sharedMaterial = currentlyHoveredAnchor.defaultMaterial;
                wa.renderer.sharedMaterial = wa.hoverMaterial;
                currentlyHoveredAnchor = wa;
            }
        }
        else
        {
            if (currentlyHoveredAnchor != null)
            {
                currentlyHoveredAnchor.renderer.sharedMaterial = currentlyHoveredAnchor.defaultMaterial;
                currentlyHoveredAnchor = null;
            }
        }

        foreach (var word in spawnedWords)
        {
            var pos = ray.GetPoint(-Camera.main.transform.position.z);
            if (word.CheckMouseOverlap(pos))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (currentlyDraggedWord == null)
                    {
                        Debug.Log($"Hit {word.transform.name}");
                        if (word != null && !word.isBeingDragged)
                        {
                            Debug.Log($"Dragging {word.name}");
                            word.BeginBeingDragged();
                            currentlyDraggedWord = word;
                        }

                    }
                }
                else if (Input.GetMouseButtonUp(0) && currentlyDraggedWord != null)
                {
                    currentlyDraggedWord.StopBeingDragged();
                    if (currentlyHoveredAnchor != null)
                    {
                        if (currentlyHoveredAnchor.currentlyHeldWord == null)
                        {
                            currentlyDraggedWord.PlaceAtAnchor(currentlyHoveredAnchor);
                        }
                    }
                    currentlyDraggedWord = null;
                }
            }
        }
    }



    IEnumerator SpawnSentence()
    {
        bubble.ClearDeformers();
        spawnedWords.Clear();

        var sentence = wordParser.sentences[Random.Range(0, wordParser.sentences.Count)];
        var split = sentence.text.Split(' ');
        List<Word> words = new();
        foreach (var word in split)
        {
            var w = SpawnWord(word);
            w.gameObject.SetActive(false);
            words.Add(w);
        }
        bubble.PrepareAnchors(words);
        foreach (var word in words)
        {
            word.gameObject.SetActive(true);
            word.rigidbody2D.linearVelocity = Vector2.zero;
            word.rigidbody2D.angularVelocity = 0;
            word.transform.position += word.transform.right * word.textMesh.preferredWidth;
            word.rigidbody2D.AddForce(wordInsertionPoint.forward * insertionStrength, ForceMode2D.Impulse);
            bubble.deformable.AddDeformer(word.GetComponentInChildren<Deformer>());
            yield return new WaitForSeconds(1);
        }
    }

    Word SpawnWord(string word)
    {
        var obj = Instantiate(wordPrefab, wordInsertionPoint.position, Quaternion.identity).GetComponent<Word>();
        obj.Setup(word);
        spawnedWords.Add(obj);
        return obj;
    }

    void OnDrawGizmos()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pos, Vector3.one * .5f);
    }
}