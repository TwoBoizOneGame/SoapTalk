using System;
using UnityEngine;

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

    public Transform listeningEar;
    public Transform bubbleSpawnOffset;
    public GameObject mesh;

    public void StartListening()
    {
        animator.SetTrigger(listenerTriggerName);
    }

    public void StartTalking()
    {
        animator.SetTrigger(talkerTriggerName);
    }

    void Update()
    {
        if (GameManager.instance != null) {
            if (GameManager.instance.currentState == GameState.Moving)
            {
                if (hearingArea.bounds.Contains(GameManager.instance.bubble.transform.position))
                {
                    GameManager.instance.ValidateSentence();
                }
            }
        }
    }
}