using UnityEngine;

[CreateAssetMenu(fileName = "GoldModificator.asset", menuName = "Modificators/Gold Modificator")]
public class GoldModificator : ModificatorBase
{
    public int goldAmount;

    public AudioClip soundOnScore;

    public override void Setup(Word word)
    {
        goldAmount =  word.textMesh.text.Length;
        base.Setup(word);
    }

    public override void OnScore()
    {
        GameManager.instance.goldAmount+= goldAmount;
        GameUI.instance.UpdateGoldAmount(GameManager.instance.goldAmount);
        base.OnScore();
    }
}