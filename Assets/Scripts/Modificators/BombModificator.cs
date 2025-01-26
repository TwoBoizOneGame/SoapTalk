using UnityEngine;

[CreateAssetMenu(fileName = "BombModificator.asset", menuName = "Modificators/Bomb Modificator")]
public class BombModificator : ModificatorBase
{
    public float explosionDelay=5;

    bool isActive=false;
    float startTime;

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
                word.gameObject.SetActive(false);
            }
        }
        base.OnUpdate();
    }

    public override void OnPlace(WordAnchor anchor)
    {
        word.modificatorIcon.SetActive(false);
        isActive=false;
        base.OnPlace(anchor);
    }
}