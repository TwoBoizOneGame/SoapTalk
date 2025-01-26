using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image symbol;
    public TextMeshProUGUI costText;

    public CardBase relatedCard;

    public void Setup(CardBase card)
    {
        this.symbol.sprite=card.symbol;
        costText.text = $"{card.cost}";
        relatedCard=card;
    }

    public void OnHover()
    {
        transform.DOScale(Vector3.one*1.25f, 0.5f);
        GameUI.instance.ShowCardInfo(this);
    }

    public void OnHoverEnd()
    {
        transform.DOScale(Vector3.one, 0.5f);
        GameUI.instance.ShowCardInfo(null);
    }

    public void Buy()
    {
        if (GameManager.instance.goldAmount < relatedCard.cost) return;
        GameManager.instance.goldAmount-=relatedCard.cost;
        GameUI.instance.UpdateGoldAmount(GameManager.instance.goldAmount);
        relatedCard.OnBought();
        GameUI.instance.ShowCardInfo(null);
        Destroy(relatedCard);
        Destroy(gameObject);
    }
}
