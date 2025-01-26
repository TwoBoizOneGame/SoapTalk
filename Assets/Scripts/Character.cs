using UnityEngine;

public enum CharacterRole { Talker, Listener };

public class Character : MonoBehaviour
{
    [SerializeField]
    string talkingAnimationVariable;
    [SerializeField]
    string listeningAnimationVariable;
    [SerializeField]
    string blowingAnimationVariable;
    [SerializeField]
    string failAnimationVariable;
    [SerializeField]
    string winAnimationVariable;

    [SerializeField]
    Animator animator;

    [SerializeField]
    BoxCollider hearingArea;

    [SerializeField]
    int idleAnimCount=2;

    public CharacterRole currentRole;

    public Transform listeningEar;
    public Transform bubbleSpawnOffset;
    public GameObject mesh;

    public TalkerListener talkerListener;

    public void SetupCharacter(CharacterRole role)
    {
        talkerListener.RandomGenerate();
        SetIdlePose();
        SetRole(role);
    }

    public void SetRole(CharacterRole role)
    {
        currentRole=role;        
    }

    public void SetListening(bool listening)
    {
        animator.SetBool(listeningAnimationVariable, listening);
    }

    public void SetTalking(bool talking)
    {   
        animator.SetBool(talkingAnimationVariable, talking);
    }

    public void SetBlowing(bool blowing)
    {
        animator.SetBool(blowingAnimationVariable, blowing);
    }

    public void SetWin(bool win)
    {
        animator.SetBool(winAnimationVariable, win);
    }

    public void SetFail(bool fail)
    {
        animator.SetBool(failAnimationVariable, fail);
    }

    public void SetIdlePose()
    {
        SetListening(false);
        SetTalking(false);
        animator.SetInteger("Idle", Random.Range(0, idleAnimCount));
    }

    public Vector3 GetEndAreaOffset()
    {
        return hearingArea.transform.position;
    }

    void Update()
    {
        if (GameManager.instance != null) {
            if (currentRole == CharacterRole.Listener)
            {
                if (GameManager.instance.currentState == GameState.Moving)
            {
                // if (hearingArea.bounds.Contains(GameManager.instance.bubble.transform.position))
                    if (GetEndAreaOffset().x <= GameManager.instance.bubble.rightEnd.position.x)
                {
                        GameManager.instance.ValidateSentence();
                }
                }
            }
        }
    }
}