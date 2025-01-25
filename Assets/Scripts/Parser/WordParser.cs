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

    [ContextMenu("Load sentences")]
    public void LoadSentences()
    {
        sentences = JsonUtility.FromJson<SentenceCollection>(sentencesAsset.text).sentences;        
    }
}
