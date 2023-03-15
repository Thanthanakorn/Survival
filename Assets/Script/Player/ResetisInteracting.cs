using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetisInteracting : StateMachineBehaviour
{
    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(IsInteracting, false);
    }
    
}
