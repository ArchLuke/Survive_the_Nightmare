using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : StateMachineBehaviour
{
    public SharedBool _knife = null;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _knife.value = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _knife.value = false;
        animator.SetBool("attack", false);
    }
}
