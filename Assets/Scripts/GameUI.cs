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
    public GameObject heartPrefab;
    public Transform heartContainer;
    public Slider progressSlider;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI goText;

    void Awake()
    {
        _instance = this;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
        scoreText.transform.DOPunchScale(Vector3.one * 1.5f, .5f);
    }

    public async UniTask ShowGoText()
    {
        goText.transform.localScale = Vector3.zero;
        goText.gameObject.SetActive(true);
        goText.transform.DOScale(Vector3.one * 3, 2).SetEase(Ease.OutCubic);
        await DOTween.To(() => goText.color.a, (x) => goText.color = goText.color.WithAlpha(x), 0, 2).AsyncWaitForCompletion();
        goText.gameObject.SetActive(false);
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
}
