using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum EnemyBehaviourState {Disabled,Idle,Jumping,Chasing,Waiting,Fighting,Falling}
public class EnemyComponent : MonoBehaviour
{
    [Header("Base entity stats")]
    [SerializeField]private float startHP = 10f; //the entity's start health points
    public float baseDamage = 1f; //base damage dealed when attacking an other entiy (such as a player)
    public float movingSpeed = 3f; //Speed at which the entity is able to move

    [Header("Behaviours parameters")] 
    public EnemyBehaviourState state;
    public GameObject player; //The player
    private Vector3 target = Vector3.zero; //The position the spider should head up to
    public float surfaceDetectionDistance = 1f; //Distance from the center point up to which the Enemy is able to detect the presence of a ground surface 
    public float surfaceDetectionOffset = 1f; //Offset form which the detection raycast is casted (used if the center point is set at the bottom of the 3D model
    public float surfaceWalkingHeightOffset = 1f; //Offset used to make the 3D model touch the ground with its feet/lugs/paws etc
    public string groundLayer = "Default";
    public float avoidanceDistance = 0.1f; // Length of the raycasts casted in order to detect nearby fellow entities 
    
    public float sineScale = 1;
    public float sineOctaves = 1;
    public float directionErrorCoeff = 1;


    public float enemyDetectionDistance = 7f;
    public float outOfRangeDistance = 50f;
    
    private bool isGrounded = false;

    private Vector3 currentSurfaceNormal;

    private Vector3 movingDirection;
    //Private variables
    private float HP;
    private float walkingSeed;

    //Falling behaviour
    private float fallBeginingTime = 0f;
    public float timeBeforeOOB = 3f;
    public SkinnedMeshRenderer meshRenderer;

    private void Start()
    {
        HP = startHP;
        walkingSeed = UnityEngine.Random.Range(0,1000);
        state = EnemyBehaviourState.Idle;
        /*RaycastHit hit;
        if (Physics.SphereCast(transform.position, surfaceWalkingHeightOffset, Vector3.up, out hit, 0,
            LayerMask.NameToLayer("Ground")))
        {
            transform.up = hit.normal;
        }*/
    }
    void Update()
    {
        
        checkDistanceFromPlayer();
        if (state != EnemyBehaviourState.Disabled)
        {
            //isGrounded = false;
            wallClimbBehaviour();
            if (!isGrounded)
            {
                transform.position = transform.position - Vector3.up * 9.81f * Time.deltaTime;
                transform.up = Vector3.up;
                if (state != EnemyBehaviourState.Falling)
                {
                    fallBeginingTime = Time.time;
                }
                state = EnemyBehaviourState.Falling;
                if ((Time.time - fallBeginingTime) > timeBeforeOOB)
                {
                    state = EnemyBehaviourState.Disabled;
                }
            }
            else if (state == EnemyBehaviourState.Chasing)
            { 
                target = player.transform.position;
                checkDistanceFromPlayer();
                move();
            }
            if (HP <= 0)
            {
                state = EnemyBehaviourState.Disabled;
            }
        }
        
        
    }

    public void hit(float damages)
    {
        HP -= damages;
    }


    public void checkDistanceFromPlayer()
    {
        if (state != EnemyBehaviourState.Disabled)
        {
            Vector3 playerPos = player.transform.position;
            float distance = Math.Abs((this.transform.position - playerPos).magnitude);
            if (name == "Araignée1")
            {
                Debug.Log("Distance from player : " + distance);
            }
            if (distance < enemyDetectionDistance)
            {
                state = EnemyBehaviourState.Chasing;
            }else if (distance > outOfRangeDistance)
            {
                Debug.Log("to disable");
                state = EnemyBehaviourState.Disabled;
            }else{
                state = EnemyBehaviourState.Idle;
            }
        }
    }
    
    public void wallClimbBehaviour()
    {
        //if (Physics.SphereCast(transform.position, positionHeightOffset*1.2f, Vector3.up, out hit, 0, 1 << LayerMask.NameToLayer("Ground"))) //Doesnt Work
        //if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position + transform.up*surfaceDetectionOffset, -transform.up, out hit, surfaceDetectionDistance, 1 << LayerMask.NameToLayer(groundLayer))){
            
            isGrounded = true;
            transform.position = hit.point + hit.normal * surfaceWalkingHeightOffset;
            currentSurfaceNormal = Vector3.Lerp(transform.up,hit.normal, 0.1f);
            //il faut pouvoir rotate autour de hit.normal
        }
        else
        {
            isGrounded = false;
        }
    }

    public void move()
    {
        movingDirection = Vector3.zero;
        if (target != null)
        {
            movingDirection = target - transform.position;
            movingDirection.Normalize();
            transform.LookAt(target);
            //transform.rotation =
                    Quaternion.LookRotation(( transform.position-target).normalized, currentSurfaceNormal);
            //transform.up = currentSurfaceNormal;
            
            var toTarget = target - transform.position;
            var toUp = currentSurfaceNormal;
            float sine = 0f;
            for (int i = 1; i <= sineOctaves; i++)
            {
                sine += Mathf.PerlinNoise(walkingSeed+Time.time * (sineScale * i), i) * 2 - 1;
            }
            transform.rotation = Quaternion.LookRotation(toUp.normalized, -toTarget.normalized);
            transform.Rotate(Vector3.right, 90f, Space.Self);
            movingDirection = (sine * transform.right * directionErrorCoeff + transform.forward).normalized;
            transform.rotation = Quaternion.LookRotation(toUp.normalized, -movingDirection.normalized);
            transform.Rotate(Vector3.right, 90f, Space.Self);
            transform.Rotate(Vector3.up, 180f, Space.Self);
        }
        Vector3 avoid = Vector3.zero;//avoidanceBehaviourPosition();
        transform.position += Vector3.ClampMagnitude(Time.deltaTime * movingSpeed* Vector3.Lerp(movingDirection,avoid, 0.5f),movingSpeed);
    }
    public Vector3 avoidanceBehaviourPosition()
    {
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        
        if (Physics.Raycast(transform.position, transform.forward, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (transform.forward * avoidanceDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, -transform.forward, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (-transform.forward * avoidanceDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, transform.right, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (transform.right * avoidanceDistance);
            nAvoid++;
        }
        if (Physics.Raycast(transform.position, -transform.right, avoidanceDistance,
            1 << LayerMask.NameToLayer("Enemy")))
        {
            avoidanceMove -= (-transform.right * avoidanceDistance);
            nAvoid++;
        }


        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }
        else
        {
            avoidanceMove = Vector3.zero;
        }
        return avoidanceMove;
    }

    private void OnDrawGizmosSelected()
    {    
        Gizmos.color=Color.red;
        Gizmos.DrawRay(transform.position + transform.up*surfaceDetectionOffset,-transform.up*surfaceDetectionDistance);
        Gizmos.color=Color.green;
        Gizmos.DrawRay(transform.position,movingDirection);

    }
}
