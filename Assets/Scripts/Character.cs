using System;
using UnityEngine;

public enum CharacterRole { Talker, Listener };

public class Character : MonoBehaviour
{
    [SerializeField]
    string talkerTriggerName;
    [SerializeField]
    string listenerTriggerName;

    [SerializeField]
    Animator animator;

    [SerializeField]
    BoxCollider hearingArea;

    public CharacterRole currentRole;

    public Transform listeningEar;
    public Transform bubbleSpawnOffset;
    public GameObject mesh;

    public TalkerListener talkerListener;

    public void SetupCharacter(CharacterRole role)
    {
        talkerListener.RandomGenerate();
        SetRole(role);
    }

    public void SetRole(CharacterRole role)
    {
        currentRole=role;
    }

    public void StartListening()
    {
        animator.SetTrigger(listenerTriggerName);
    }

    public void StartTalking()
    {
        animator.SetTrigger(talkerTriggerName);
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
                    if (GetEndAreaOffset().x <= GameManager.instance.bubble.transform.position.x)
                {
                        GameManager.instance.ValidateSentence();
                }
                }
            }
        }
    }
}