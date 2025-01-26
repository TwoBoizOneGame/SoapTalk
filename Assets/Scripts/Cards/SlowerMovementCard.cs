using UnityEngine;

[CreateAssetMenu(fileName = "SlowerMovement.asset", menuName = "Cards/Slower Movement Card")]

public class SlowerMovementCard : CardBase
{
    public override void OnBought()
    {
        base.OnBought();
        GameManager.instance.currentMovementSpeed /= 2;
    }
}