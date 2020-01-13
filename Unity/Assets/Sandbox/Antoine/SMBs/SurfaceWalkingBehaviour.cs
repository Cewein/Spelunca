using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceWalkingBehaviour : StateMachineBehaviour
{
    private EnemyComponent ec;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ec = animator.GetComponent<EnemyComponent>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        ec.wallClimbBehaviour();
        ec.move();   
    }
//    
//    private void wallClimbBehaviour(Transform transform)
//    {
//        //if (Physics.SphereCast(transform.position, positionHeightOffset*1.2f, Vector3.up, out hit, 0, 1 << LayerMask.NameToLayer("Ground"))) //Doesnt Work
//        //if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))
//        RaycastHit hit;
//        
//        if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground"))){
//            transform.up = Vector3.Lerp(transform.up,hit.normal, lerpCoeff);
//            transform.position = hit.point + hit.normal * positionHeightOffset;
//            currentNormal = hit.normal;
//        }
//    }
//
//    private void move(Transform transform)
//    {
//        Vector3 direction = Vector3.zero;
//        if (target != null)
//        {
//            direction = target.transform.position - transform.position;
//            direction.Normalize();
//            transform.LookAt(target.transform);
//            transform.up = currentNormal; //FIXME: Il faut réussir à le faire tourner autour de l'axe de la normale sans fournir de degrés genre avec un Look At
//        }
//        Vector3 avoid = avoidanceBehaviourPosition(transform);
//        transform.position += Vector3.ClampMagnitude(Time.deltaTime * movingSpeed* (direction*(1-coeffAvoid) + avoid*coeffAvoid),movingSpeed);
//    }
//    private Vector3 avoidanceBehaviourPosition(Transform transform)
//    {
//        Vector3 avoidanceMove = Vector3.zero;
//        int nAvoid = 0;
//        
//        if (Physics.Raycast(transform.position, transform.forward, avoidDistance,
//            1 << LayerMask.NameToLayer("Enemy")))
//        {
//            avoidanceMove -= (transform.forward * avoidDistance);
//            nAvoid++;
//        }
//        if (Physics.Raycast(transform.position, -transform.forward, avoidDistance,
//            1 << LayerMask.NameToLayer("Enemy")))
//        {
//            avoidanceMove -= (-transform.forward * avoidDistance);
//            nAvoid++;
//        }
//        if (Physics.Raycast(transform.position, transform.right, avoidDistance,
//            1 << LayerMask.NameToLayer("Enemy")))
//        {
//            avoidanceMove -= (transform.right * avoidDistance);
//            nAvoid++;
//        }
//        if (Physics.Raycast(transform.position, -transform.right, avoidDistance,
//            1 << LayerMask.NameToLayer("Enemy")))
//        {
//            avoidanceMove -= (-transform.right * avoidDistance);
//            nAvoid++;
//        }
//
//
//        if (nAvoid > 0)
//        {
//            avoidanceMove /= nAvoid;
//        }
//        /*
//            Debug.DrawRay(transform.position,transform.forward,Color.red);
//            Debug.DrawRay(transform.position,-transform.forward,Color.red);
//            Debug.DrawRay(transform.position,transform.right,Color.red);
//            Debug.DrawRay(transform.position,-transform.right,Color.red);
//        */
//        return avoidanceMove;
//    }
}
