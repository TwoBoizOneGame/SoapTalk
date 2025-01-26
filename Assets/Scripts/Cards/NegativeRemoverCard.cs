using UnityEngine;

[CreateAssetMenu(fileName = "NegativeRemover.asset", menuName = "Cards/Negative Remover Card")]
public class NegativeRemoverCard : CardBase
{
    public override void OnBought()
    {
        base.OnBought();
        foreach (var word in GameManager.instance.spawnedWords)
        {
            if (word.appliedModificator != null && word.appliedModificator.isNegative)
            {
                Destroy(word.appliedModificator);
                word.modificatorIcon.SetActive(false);
            }
        }
    }
}