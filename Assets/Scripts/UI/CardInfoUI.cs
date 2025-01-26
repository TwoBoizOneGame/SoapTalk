using TMPro;
using UnityEngine;

public class CardInfoUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI costText;


    public void Setup(CardBase relatedCard)
    {
        titleText.text = relatedCard.title;
        descriptionText.text = relatedCard.description;
        costText.text = $"{relatedCard.cost} gold";
    }
}