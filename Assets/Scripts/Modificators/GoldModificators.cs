using UnityEngine;

[CreateAssetMenu(fileName = "GoldModificator.asset", menuName = "Modificators/Gold Modificator")]
public class GoldModificator : ModificatorBase
{
    public int goldAmount;

    public override void Setup(Word word)
    {
        goldAmount =  word.textMesh.text.Length;
        base.Setup(word);
    }

    public override void OnScore()
    {
        GameManager.instance.goldAmount+= goldAmount;
        base.OnScore();
    }
}