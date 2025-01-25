using UnityEngine;

public class FaceAnim : StateMachineBehaviour
{
    public Character character;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float talkBlend = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponentInParent<Character>();
        skinnedMeshRenderer = character.mesh.GetComponent<SkinnedMeshRenderer>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Talk();
    }

    public void Talk()
    {
        if (talkBlend < 100.0f)
        {
            talkBlend += Random.Range(0.5f, 1.5f);
            skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend);
        }else
        {
            talkBlend = 0;
        }
        Debug.Log(talkBlend);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
