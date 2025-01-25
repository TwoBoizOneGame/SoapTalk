using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FaceAnim : StateMachineBehaviour
{
    public Character character;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float talkBlend1 = 0;
    public float talkBlend2 = 0;
    public float blowingBlend = 0;
    public float listenBlend = 0;
    public float tonqueBlend = 0;
    public float earBlendR = 0;
    public float earBlendL = 0;
    public float eyebrowsBlend = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponentInParent<Character>();
        skinnedMeshRenderer = character.mesh.GetComponent<SkinnedMeshRenderer>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("Talker") == true && animator.GetBool("Listener") == false && animator.GetBool("Blowing") == false && animator.GetBool("Win") == false && animator.GetBool("Lose") == false)
        {
            Talk();
        }
        if (animator.GetBool("Listener") == true && animator.GetBool("Talker") == false && animator.GetBool("Blowing") == false && animator.GetBool("Win") == false && animator.GetBool("Lose") == false)
        {
            Listen();
        }
        if (animator.GetBool("Blowing") == true && animator.GetBool("Talker") == false && animator.GetBool("Listener") == false && animator.GetBool("Win") == false && animator.GetBool("Lose") == false)
        {
            Blow();
        }

        if (animator.GetInteger("Idle") == 1 && animator.GetBool("Talker") == false && animator.GetBool("Listener") == false && animator.GetBool("Blowing") == false && animator.GetBool("Win") == false && animator.GetBool("Lose") == false)
        {
            Idle1();
        }

        if (animator.GetInteger("Idle") == 2 && animator.GetBool("Talker") == false && animator.GetBool("Listener") == false && animator.GetBool("Blowing") == false && animator.GetBool("Win") == false && animator.GetBool("Lose") == false)
        {
            Idle2();
        }

        if(animator.GetBool("Win") == true && animator.GetBool("Listener") == true)
        {
            Win();
        }

        if (animator.GetBool("Lose") == true && animator.GetBool("Listener") == true)
        {
            Lose();
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetFace();
    }

    public void Talk()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            if (talkBlend1 < 100.0f)
            {
                talkBlend1 += Random.Range(0.5f, 5.5f);
                skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);
            }
            else
            {
                DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 0, 0.25f);
            }
        }
        else
        {
            if (talkBlend2 < 100.0f)
            {
                talkBlend2 += Random.Range(0.5f, 5.5f);
                skinnedMeshRenderer.SetBlendShapeWeight(1, talkBlend2);
            }
            else
            {
                DOTween.To(() => talkBlend2, (x) => talkBlend2 = x, 0, 0.25f);
            }
        }
    }

    public void Blow()
    {

        DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 0, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);

        if (blowingBlend < 100.0f)
        {
            blowingBlend += Random.Range(0.5f, 2.5f);
            skinnedMeshRenderer.SetBlendShapeWeight(1, blowingBlend);
        }   
        else
        {
            DOTween.To(() => blowingBlend, (x) => blowingBlend = x, 0, 0.5f);
        }
    }

    public void Listen()
    {
        DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 25, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);

        if (earBlendR < 100.0f)
        {
            earBlendR += Random.Range(0.5f, 2.5f);
            skinnedMeshRenderer.SetBlendShapeWeight(3, earBlendR);
        }
        else
        {
            DOTween.To(() => earBlendR, (x) => earBlendR = x, 25, 0.5f);
        }

        DOTween.To(() => tonqueBlend, (x) => tonqueBlend = x, 100, 2f);
        skinnedMeshRenderer.SetBlendShapeWeight(2, tonqueBlend);
    }

    public void Idle1()
    {
        DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 100, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);

        DOTween.To(() => eyebrowsBlend, (x) => eyebrowsBlend = x, 100, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(5, eyebrowsBlend);
    }

    public void Idle2()
    {
        DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 75, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);

        DOTween.To(() => eyebrowsBlend, (x) => eyebrowsBlend = x, 0, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(5, eyebrowsBlend);
    }

    public void Lose()
    {
        DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 100, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);

        DOTween.To(() => talkBlend2, (x) => talkBlend2 = x, 40, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(1, talkBlend2);

        DOTween.To(() => eyebrowsBlend, (x) => eyebrowsBlend = x, 100, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(5, eyebrowsBlend);
    }

    public void Win()
    {
        DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 0, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, talkBlend1);

        DOTween.To(() => eyebrowsBlend, (x) => eyebrowsBlend = x, 100, 0.25f);
        skinnedMeshRenderer.SetBlendShapeWeight(5, eyebrowsBlend);
    }

    public void ResetFace()
    {
        //DOTween.To(() => talkBlend1, (x) => talkBlend1 = x, 0, 0.1f);
        skinnedMeshRenderer.SetBlendShapeWeight(0, 100);

        //DOTween.To(() => talkBlend2, (y) => talkBlend2 = y, 0, 0.1f);
        skinnedMeshRenderer.SetBlendShapeWeight(1, 0);

        //DOTween.To(() => tonqueBlend, (z) => tonqueBlend = z, 0, 0.1f);
        skinnedMeshRenderer.SetBlendShapeWeight(2, 0);

        //DOTween.To(() => earBlendR, (q) => earBlendR = q, 0, 0.1f);
        skinnedMeshRenderer.SetBlendShapeWeight(3, 0);

        //DOTween.To(() => earBlendL, (w) => earBlendL = w, 0, 0.1f);
        skinnedMeshRenderer.SetBlendShapeWeight(4, 0);

       // DOTween.To(() => eyebrowsBlend, (e) => eyebrowsBlend = e, 0, 0.1f);
        skinnedMeshRenderer.SetBlendShapeWeight(5, 0);
    }
}

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
