using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "BombModificator.asset", menuName = "Modificators/Bomb Modificator")]
public class BombModificator : ModificatorBase
{
    public Vector2 explosionDelayRange = new Vector2(5,9);
    public float explosionDelay=5;

    bool isActive=false;
    float startTime;

    public override void Setup(Word word)
    {
        base.Setup(word);
        explosionDelay=Random.Range(explosionDelayRange.x,explosionDelayRange.y);
    }

    public override void OnStartMovement()
    {
        isActive=true;
        startTime=Time.time;
        base.OnStartMovement();
    }

    public override void OnUpdate()
    {
        if (isActive && Time.time > startTime+explosionDelay)
        {
            if (GameManager.instance.currentState == GameState.Moving)
            {
                isActive=false;
                word.canBePickedUp=false;
                Explode();
            }
        }
        base.OnUpdate();
    }

    async UniTask Explode()
    {
        await word.modificatorIcon.transform.DOScale(Vector3.one*5, 0.5f).AsyncWaitForCompletion();
        AudioManager.instance.PlayOneShotAsync(AudioManager.instance.explosionSounds);
        word.gameObject.SetActive(false);
    }

    public override void OnPlace(WordAnchor anchor)
    {
        word.modificatorIcon.SetActive(false);
        isActive=false;
        base.OnPlace(anchor);
    }
}