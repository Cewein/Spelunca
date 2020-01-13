using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class FallingBehaviour : StateMachineBehaviour
{
    private Rigidbody rigidbody;
    private float timer = 0.0f;
    private int seconds;
 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigidbody = animator.GetComponent<Rigidbody>();
        

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        seconds = (int)(timer % 60);
        animator.transform.position += -9.81f * Vector3.up * Time.deltaTime;
    }
}
