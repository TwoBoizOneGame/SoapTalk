
using UnityEngine;

[CreateAssetMenu(fileName = "JumperModificator.asset", menuName = "Modificators/Jumper Modificator")]
public class JumperModificator : ModificatorBase
{
    public float jumpStrength=100;
    bool isActive;

    public override void Setup(Word word)
    {
        base.Setup(word);
        isActive=true;
    }

    public override void OnPlace(WordAnchor anchor)
    {
        base.OnPlace(anchor);
        if (!isActive) return;

        anchor.currentlyHeldWord=null;
        word.StopBeingDragged();
        word.rigidbody2D.AddForce(word.transform.up*jumpStrength);
        AudioManager.instance.PlayOneShotAsync(AudioManager.instance.jumpSounds);
        word.modificatorIcon.SetActive(false);
        isActive=false;
    }
}