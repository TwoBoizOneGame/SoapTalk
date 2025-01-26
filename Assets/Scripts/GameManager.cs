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

    public string[] currentSentence;
    public List<Word> spawnedWords = new List<Word>();

    public Word currentlyDraggedWord;
    public WordAnchor currentlyHoveredAnchor;

    public Character currentTalker;
    public Character currentListener;

    public float defaultMovementSpeed = 1;
    public float currentMovementSpeed = 1;

    public int currentScore = 0;
    public int goldAmount = 0;
    public int maxHealth = 5;
    public int currentHealth = 5;
    public int currentRound = 1;

    Vector3 totalDistanceToEndPoint;

    public List<CardBase> allCards = new List<CardBase>();
    public List<CardBase> availableCards = new List<CardBase>();
    public int maxCardsAvailable = 2;

    void Start()
    {
        _instance = this;
        currentHealth = maxHealth;
        currentState = GameState.Initial;
        currentRound = 0;
        currentListener.SetupCharacter(CharacterRole.Listener);
        currentTalker.SetupCharacter(CharacterRole.Talker);
        wordParser.LoadSentences();
        GameUI.instance.SetHeartCount(currentHealth);
        GameUI.instance.UpdateScore(currentScore);
        GameUI.instance.UpdateGoldAmount(goldAmount);
        StartNewRound();

#if UNITY_EDITOR
        Debug.Log("Clearing up player prefs because we're running in the editor");
        PlayerPrefs.DeleteAll();
#endif
    }

    void Update()
    {
        if (bubble != null)
        {
            var endArea = currentListener.GetEndAreaOffset();
            var remainingDistance = endArea.x - bubble.rightEnd.transform.position.x;
            var currentProgress = 1 - (remainingDistance / totalDistanceToEndPoint.x);
            var timeToEnd = remainingDistance / currentMovementSpeed;

            GameUI.instance.UpdateProgress(currentProgress, timeToEnd);
        }
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
                        currentlyHoveredAnchor.SetMaterials(currentlyHoveredAnchor.defaultMaterial);
                    wa.SetMaterials(wa.hoverMaterial);
                    currentlyHoveredAnchor = wa;
                }
            }
            else
            {
                if (currentlyHoveredAnchor != null)
                {
                    currentlyHoveredAnchor.SetMaterials(currentlyHoveredAnchor.defaultMaterial);
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
                            // Debug.Log($"Hit {word.transform.name}");
                            if (word != null && !word.isBeingDragged)
                            {
                                // Debug.Log($"Dragging {word.name}");
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
        currentState = GameState.Blowing;
        currentTalker.SetBlowing(true);
        bubble = Instantiate(bubblePrefab, Vector3.zero, Quaternion.identity).GetComponent<Bubble>();
        var targetScale = bubble.transform.localScale;
        bubble.transform.localScale = Vector3.zero;
        await bubble.transform.DOScale(targetScale, 1).AsyncWaitForCompletion();
        AudioManager.instance.PlayOneShotAsync(AudioManager.instance.stretchSounds);
        currentTalker.SetBlowing(false);
        totalDistanceToEndPoint = currentListener.GetEndAreaOffset() - bubble.rightEnd.transform.position;
        StartTalking();
    }

    async void StartTalking()
    {
        currentState = GameState.Talking;
        await SpawnSentence();
        currentTalker.SetIdlePose();
        GameUI.instance.SetCardDrawerVisibility(true);
        await UniTask.WaitForSeconds(1);
        GameUI.instance.ShowGoText();
        StartMoving();
    }

    void StartMoving()
    {
        currentState = GameState.Moving;
        currentTalker.SetIdlePose();
        foreach (var word in spawnedWords)
        {
            if (word.appliedModificator != null)
            {
                word.appliedModificator.OnStartMovement();
            }
        }
    }

    string[] GenerateSentence()
    {
        var sentence = wordParser.sentences[Random.Range(0, wordParser.sentences.Count)];
        var split = sentence.text.Split(' ');
        return split;
    }

    async UniTask SpawnSentence()
    {
        bubble.ClearDeformers();
        spawnedWords.Clear();

        List<Word> words = new();
        foreach (var word in currentSentence)
        {
            var w = SpawnWord(word);
            w.gameObject.SetActive(false);
            words.Add(w);
        }
        currentTalker.SetBlowing(true);
        await bubble.PrepareAnchors(words);
        currentTalker.SetBlowing(false);
        currentTalker.SetTalking(true);
        var audio = AudioManager.instance.PlayOneShot(AudioManager.instance.talkingSound, true);
        foreach (var word in words)
        {
            word.gameObject.SetActive(true);
            word.rigidbody2D.linearVelocity = Vector2.zero;
            word.rigidbody2D.angularVelocity = 0;
            word.transform.position += word.transform.right * word.textMesh.preferredWidth;
            word.rigidbody2D.AddForce(bubble.wordInsertionPoint.forward * insertionStrength, ForceMode2D.Impulse);
            bubble.deformable.AddDeformer(word.GetComponentInChildren<Deformer>());
            word.transform.localScale = Vector3.zero;
            word.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
            await UniTask.WaitForSeconds(1);
        }
        currentTalker.SetTalking(false);
        Destroy(audio.gameObject);
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
        GameUI.instance.SetCardDrawerVisibility(false);
        currentListener.SetListening(true);
        if (currentlyDraggedWord != null)
        {
            currentlyDraggedWord.StopBeingDragged();
            currentlyDraggedWord.OnStopHover();
            currentlyDraggedWord = null;
        }

        List<Word> wordsToProcess = new List<Word>(spawnedWords);
        bool isPerfect = true;
        foreach (var anchor in bubble.wordAnchors)
        {
            if (anchor.currentlyHeldWord != null)
            {
                if (anchor.currentlyHeldWord.textMesh.text == anchor.targetText)
                {
                    currentScore += anchor.targetText.Length;
                    anchor.currentlyHeldWord.transform.DOPunchScale(Vector3.one * 1.5f, 1);
                    AudioManager.instance.PlayOneShotAsync(AudioManager.instance.goodAnswerSound);
                    await DOTween.To(() => anchor.currentlyHeldWord.textMesh.color, x => anchor.currentlyHeldWord.textMesh.color = x, Color.green, 1).AsyncWaitForCompletion();
                    GameUI.instance.UpdateScore(currentScore);
                    if (anchor.currentlyHeldWord.appliedModificator != null)
                    {
                        anchor.currentlyHeldWord.appliedModificator.OnScore();
                    }
                }
                else
                {
                    await anchor.currentlyHeldWord.transform.DOScale(Vector3.one * 1.5f, 0.5f).AsyncWaitForCompletion();
                    DOTween.To(() => anchor.currentlyHeldWord.textMesh.color, x => anchor.currentlyHeldWord.textMesh.color = x, Color.red, 1);
                    AudioManager.instance.PlayOneShotAsync(AudioManager.instance.badAnswerSound);
                    await anchor.currentlyHeldWord.transform.DOShakeRotation(1f).AsyncWaitForCompletion();
                    await anchor.currentlyHeldWord.transform.DOScale(Vector3.one, 0.5f).AsyncWaitForCompletion();
                    isPerfect = false;
                }
            }
            anchor.transform.DOScale(0, .5f);
            if (anchor.currentlyHeldWord != null)
            {
                anchor.currentlyHeldWord.transform.DOMove(currentListener.listeningEar.transform.position, 1);
                AudioManager.instance.PlayOneShotAsync(AudioManager.instance.whooshSounds);
                anchor.currentlyHeldWord.transform.DOScale(Vector3.zero, 1);
                wordsToProcess.Remove(anchor.currentlyHeldWord);
            }
            await UniTask.WaitForSeconds(1.5f);
        }
        if (!isPerfect)
        {
            RemoveHeart();
        }

        foreach (var word in wordsToProcess)
        {
            DOTween.To(() => word.textMesh.color, x => word.textMesh.color = x, Color.red, 1);
            word.transform.DOScale(Vector3.zero, 1);
        }
        if (currentHealth > 0)
        {
            currentListener.SetWin(true);
        }
        AudioManager.instance.PlayOneShotAsync(AudioManager.instance.popSounds);
        await bubble.transform.DOScale(0, .5f).SetEase(Ease.InOutExpo).AsyncWaitForCompletion();
        bubble.DestroyBubble();
        if (currentHealth > 0)
        {
            currentListener.SetWin(false);
            StartNewRound();
        }
    }

    async void StartNewRound()
    {
        for (int i = 0; i < spawnedWords.Count; i++)
        {
            Destroy(spawnedWords[i].gameObject);
        }
        spawnedWords.Clear();

        currentState = GameState.Initial;
        currentMovementSpeed = defaultMovementSpeed;
        currentSentence = GenerateSentence();
        if (currentRound > 0)
        {
            await environment.transform.DOMoveX(environment.transform.position.x - currentListener.bubbleSpawnOffset.position.x, 1).AsyncWaitForCompletion();
            var temp = currentTalker;
            currentTalker = currentListener;
            currentListener = temp;
            GameUI.instance.SwapRTs();
        }

        var dist = currentSentence.Length * 5 * currentMovementSpeed;
        var minimumOffset = 5;
        currentListener.name = "Listener";
        currentListener.SetupCharacter(CharacterRole.Listener);
        currentListener.transform.position = currentTalker.transform.position + Vector3.right * (minimumOffset + dist);
        currentTalker.name = "Talker";
        currentTalker.SetRole(CharacterRole.Talker);
        currentTalker.SetIdlePose();
        currentListener.SetIdlePose();
        currentRound++;
        GameUI.instance.UpdateRoundNumber();
        GenerateCards();

        SpawnBubble();
    }

    public void GenerateCards()
    {
        for (int i = 0; i < availableCards.Count; i++)
        {
            if (availableCards[i] != null)
                Destroy(availableCards[i]);
        }
        availableCards.Clear();
        GameUI.instance.cardsContainer.RemoveChildren();

        for (int i = 0; i < maxCardsAvailable; i++)
        {
            var r = allCards[Random.Range(0, allCards.Count)];
            var card = Instantiate(r);
            GameUI.instance.CreateCard(card);
            availableCards.Add(card);
        }
    }

    public void RemoveHeart()
    {
        currentHealth = Mathf.Max(currentHealth - 1, 0);
        if (currentHealth == 0)
        {
            EndGame();
        }
        GameUI.instance.SetHeartCount(currentHealth);
    }

    public void EndGame()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;
        currentListener.SetFail(true);
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