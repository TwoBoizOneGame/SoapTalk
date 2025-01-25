using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Deform;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Initial, Blowing, Talking, Moving, Reached, GameOver }

public class GameManager : MonoBehaviour
{
    const string HIGHSCORE_KEY = "highscore";

    static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }

    public GameState currentState;

    public GameObject wordPrefab;
    public GameObject bubblePrefab;
    public Bubble bubble;
    public float insertionStrength = 1.0f;
    public WordParser wordParser;
    public GameObject environment;

    List<Word> spawnedWords = new List<Word>();

    public Word currentlyDraggedWord;
    public WordAnchor currentlyHoveredAnchor;

    public Character currentTalker;
    public Character currentListener;
    public float currentMovementSpeed = 1;

    public int currentScore = 0;
    public int maxHealth=5;
    public int currentHealth=5;
    public int currentRound=1;

    Vector3 currentListenerStartingPosition;

    void Start()
    {
        _instance = this;
        currentHealth=maxHealth;
        currentState = GameState.Initial;
        currentRound=1;
        currentListenerStartingPosition=currentListener.GetEndAreaOffset();
        currentListener.SetupCharacter(CharacterRole.Listener);
        currentTalker.SetupCharacter(CharacterRole.Talker);
        wordParser.LoadSentences();
        GameUI.instance.SetHeartCount(currentHealth);
        GameUI.instance.UpdateScore(currentScore);
        SpawnBubble();

#if UNITY_EDITOR
        Debug.Log("Clearing up player prefs because we're running in the editor");
        PlayerPrefs.DeleteAll();
        #endif
    }

    void Update()
    {        
        var currentProgress = ((currentListenerStartingPosition.x-currentListener.transform.position.x)/currentListenerStartingPosition.x);
        var timeToEnd = currentListener.transform.position.x/currentMovementSpeed;

        GameUI.instance.UpdateProgress(currentProgress, timeToEnd);
        if (currentState == GameState.Moving)
        {
            environment.transform.position -= Vector3.right * currentMovementSpeed * Time.deltaTime;

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
    }

    async void SpawnBubble()
    {
        currentState=GameState.Blowing;
        bubble = Instantiate(bubblePrefab, Vector3.zero, Quaternion.identity).GetComponent<Bubble>();
        var targetScale = bubble.transform.localScale;
        bubble.transform.localScale=Vector3.zero;
        await bubble.transform.DOScale(targetScale, 1).AsyncWaitForCompletion();
        StartTalking();
    }

    async void StartTalking()
    {
        currentState = GameState.Talking;
        currentTalker.StartTalking();
        // StartCoroutine("SpawnSentence");
        await SpawnSentence();
        await UniTask.WaitForSeconds(1);
        GameUI.instance.ShowGoText();
        StartMoving();
    }

    void StartMoving()
    {
        currentState = GameState.Moving;
        currentTalker.StartListening();
    }

    async UniTask SpawnSentence()
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
            word.rigidbody2D.AddForce(bubble.wordInsertionPoint.forward * insertionStrength, ForceMode2D.Impulse);
            bubble.deformable.AddDeformer(word.GetComponentInChildren<Deformer>());
            await UniTask.WaitForSeconds(1);
        }
    }

    Word SpawnWord(string word)
    {
        var obj = Instantiate(wordPrefab, bubble.wordInsertionPoint.position, Quaternion.identity).GetComponent<Word>();
        obj.Setup(word);
        spawnedWords.Add(obj);
        return obj;
    }

    public async UniTask ValidateSentence()
    {
        currentState = GameState.Reached;
        if (currentlyDraggedWord != null)
        {
            currentlyDraggedWord.StopBeingDragged();
            currentlyDraggedWord=null;
        }

        List<Word> wordsToProcess = new List<Word>(spawnedWords);
        foreach (var anchor in bubble.wordAnchors)
        {
            if (anchor.currentlyHeldWord != null)
            {
                if (anchor.currentlyHeldWord.textMesh.text == anchor.targetText)
                {
                    currentScore += anchor.targetText.Length;
                    anchor.currentlyHeldWord.transform.DOPunchScale(Vector3.one * 1.5f, 1);
                    await DOTween.To(() => anchor.currentlyHeldWord.textMesh.color, x => anchor.currentlyHeldWord.textMesh.color = x, Color.green, 1).AsyncWaitForCompletion();
                    GameUI.instance.UpdateScore(currentScore);
                }
                else
                {
                    await anchor.currentlyHeldWord.transform.DOScale(Vector3.one * 1.5f, 0.5f).AsyncWaitForCompletion();
                    DOTween.To(() => anchor.currentlyHeldWord.textMesh.color, x => anchor.currentlyHeldWord.textMesh.color = x, Color.red, 1);
                    await anchor.currentlyHeldWord.transform.DOShakeRotation(1f).AsyncWaitForCompletion();
                    await anchor.currentlyHeldWord.transform.DOScale(Vector3.one, 0.5f).AsyncWaitForCompletion();                    
                    RemoveHeart();
                }
            }
            anchor.transform.DOScale(0, .5f);
            if (anchor.currentlyHeldWord != null)
            {
                anchor.currentlyHeldWord.transform.DOMove(currentListener.listeningEar.transform.position, 1);
                anchor.currentlyHeldWord.transform.DOScale(Vector3.zero, 1);
                wordsToProcess.Remove(anchor.currentlyHeldWord);
            }
            await UniTask.WaitForSeconds(1.5f);
        }

        foreach (var word in wordsToProcess)
        {
            DOTween.To(() => word.textMesh.color, x => word.textMesh.color = x, Color.red, 1);
            word.transform.DOScale(Vector3.zero, 1);
        }
        await bubble.transform.DOScale(0, .5f).SetEase(Ease.InOutExpo).AsyncWaitForCompletion();
        StartNewRound();
    }

    async void StartNewRound()
    {
        currentState = GameState.Initial;
        
        await environment.transform.DOMoveX(environment.transform.position.x-currentListener.bubbleSpawnOffset.position.x, 1).AsyncWaitForCompletion();
        currentTalker.transform.position = currentListener.transform.position + Vector3.right*Random.Range(60, 120);

        var temp = currentTalker;
        currentTalker = currentListener;
        currentListener = temp;
        currentTalker.name = "Talker";
        currentTalker.SetRole(CharacterRole.Talker);
        currentListener.name = "Listener";
        currentListener.SetupCharacter(CharacterRole.Listener);
        currentListenerStartingPosition=currentListener.GetEndAreaOffset();
        currentRound++;
        GameUI.instance.UpdateRoundNumber();

        SpawnBubble();
    }

    public void RemoveHeart()
    {
        currentHealth = Mathf.Max(currentHealth-1, 0);
        if (currentHealth == 0)
        {            
            EndGame();
        }
        GameUI.instance.SetHeartCount(currentHealth);
    }

    public void EndGame()
    {
        currentState=GameState.GameOver;
        var maxScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        if (currentScore > maxScore)
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, currentScore);
        }
        GameUI.instance.ShowGameOverScreen(currentScore > maxScore);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    void OnDrawGizmos()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pos, Vector3.one * .5f);
    }
}