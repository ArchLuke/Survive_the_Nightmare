using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : StateMachineBehaviour
{
    public SharedBool rifleShoot = null;
    public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        rifleShoot.value = true;
    }
}
