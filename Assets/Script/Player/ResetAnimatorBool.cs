using UnityEngine;

public class ResetAnimatorBool : StateMachineBehaviour
{

    public string targetBool;
    public bool status;

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(targetBool, status);
    }
    
}
