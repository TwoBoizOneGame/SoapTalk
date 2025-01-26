using UnityEngine;

[CreateAssetMenu(fileName = "HealthCard.asset", menuName = "Cards/Health Card")]
public class HealthCard : CardBase
{
    public override void OnBought()
    {
        base.OnBought();
        GameManager.instance.currentHealth = Mathf.Min(GameManager.instance.currentHealth+1, GameManager.instance.maxHealth);
        GameUI.instance.SetHeartCount(GameManager.instance.currentHealth);
    }
}