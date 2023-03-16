using UnityEngine;

public class ResetIsInteracting : StateMachineBehaviour
{
    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(IsInteracting, false);
    }
    
}
