using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI _instance;
    public static GameUI instance
    {
        get
        {
            return _instance;
        }
    }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goldText;
    public GameObject heartPrefab;
    public Transform heartContainer;
    public Slider progressSlider;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI goText;

    public GameObject gameOverPopup;
    public TextMeshProUGUI gameOverPopupText;
    public TextMeshProUGUI gameOverHighscoreText;

    public RawImage talkerImage;
    public RawImage listenerImage;
    public RenderTexture talkerRt;
    public RenderTexture listenerRT;

    public Transform cardsContainer;
    public GameObject cardPrefab;

    public CardInfoUI cardInfoUI;

    public GameObject pauseScreen;    

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
        scoreText.transform.DOPunchScale(Vector3.one * 1.5f, .5f);
    }

    public void UpdateGoldAmount(int gold)
    {
        goldText.text = $"Gold {gold}";
    }

    public void UpdateRoundNumber()
    {
        roundText.text = $"Round {GameManager.instance.currentRound}";
        roundText.transform.DOPunchScale(Vector3.one * 2, 1);
    }

    public void UpdateProgress(float value, float timeLeft)
    {
        progressSlider.value = value;
        progressText.text = $"{Mathf.Round(timeLeft)} seconds";
    }

    public void SwapRTs()
    {
        var left = talkerImage.texture == talkerRt ? talkerRt : listenerRT;
        var right = listenerImage.texture == listenerRT ? listenerRT : talkerRt;
        talkerImage.texture = right;
        listenerImage.texture = left;
    }

    public void CreateCard(CardBase baseCard)
    {
        var card = Instantiate(cardPrefab, cardsContainer).GetComponent<CardUI>();
        card.Setup(baseCard);
    }

    public void ShowCardInfo(CardUI hoveredCard)
    {
        if (hoveredCard == null)
        {
            cardInfoUI.gameObject.SetActive(false);
            return;
        }
        cardInfoUI.Setup(hoveredCard.relatedCard);
        var hintRt = cardInfoUI.transform as RectTransform;
        var cardRt = hoveredCard.transform as RectTransform;
        hintRt.position = cardRt.position + Vector3.up * 20;

        cardInfoUI.gameObject.SetActive(true);
    }

    public void SetCardDrawerVisibility(bool visible)
    {
        RectTransform cardsContainerRt = cardsContainer.transform as RectTransform;

        Vector2 hiddenPosition = new Vector2(cardsContainerRt.anchoredPosition.x, -cardsContainerRt.rect.height);
        Vector2 visiblePosition = new Vector2(cardsContainerRt.anchoredPosition.x, 0);

        cardsContainerRt.DOAnchorPos(visible ? visiblePosition : hiddenPosition, 0.5f)
            .SetEase(Ease.InOutQuad);
    }

    public async UniTask ShowGoText()
    {
        goText.transform.localScale = Vector3.zero;
        goText.gameObject.SetActive(true);
        goText.transform.DOScale(Vector3.one * 3, 2).SetEase(Ease.OutCubic);
        await DOTween.To(() => goText.color.a, (x) => goText.color = goText.color.WithAlpha(x), 0, 2).AsyncWaitForCompletion();
        goText.gameObject.SetActive(false);
        goText.color = goText.color.WithAlpha(1);
    }

    public async void SetHeartCount(int heartCount)
    {
        if (heartContainer.childCount < heartCount)
        {
            var c = heartCount - heartContainer.childCount;
            for (int i = 0; i < c; i++)
            {
                var heart = Instantiate(heartPrefab, heartContainer);
                heart.transform.localScale = Vector3.zero;
                heart.gameObject.transform.DOScale(Vector3.one, .5f);
            }
        }
        else if (heartContainer.childCount > heartCount)
        {
            var diff = heartContainer.childCount - heartCount;
            for (int i = 0; i < diff; i++)
            {
                var c = heartContainer.GetChild(heartContainer.childCount - i - 1);
                await c.transform.DOScale(Vector3.zero, 0.5f).AsyncWaitForCompletion();
                Destroy(c.gameObject);
            }
        }
    }

    public void ShowGameOverScreen(bool isNewHighscore)
    {
        gameOverPopup.SetActive(true);
        gameOverPopupText.text = $"Score: {GameManager.instance.currentScore}";
        gameOverHighscoreText.gameObject.SetActive(isNewHighscore);
        gameOverHighscoreText.transform.DOPunchScale(Vector3.one * 1.25f, 1).SetLoops(3);
    }

    public void TogglePauseScreen(bool visible)
    {
        pauseScreen.SetActive(visible);
        if (visible) PauseGame();
    }

    public void PauseGame()
    {
        GameManager.instance.PauseGame();
    }
    public void ResumeGame()
    {
        GameManager.instance.ResumeGame();
        TogglePauseScreen(false);
    }
}
