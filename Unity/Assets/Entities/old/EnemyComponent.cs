using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum EnemyBehaviourState {Disabled,Idle,Jumping,Chasing,Waiting,Fighting,Falling}
public class EnemyComponent : MonoBehaviour, IDamageable
{
    [Header("Base entity parameters")]
    [Tooltip("The entity's start health points.")] [SerializeField]
    private float startHP = 10f;
    [Tooltip("The damage dealed when attacking an other entity (such as the player).")] [SerializeField]
    private float damage = 1f;
    [Tooltip("The speed at which the entity is able to move.")] [SerializeField]
    private float movingSpeed = 3f;
    [Tooltip("The spider's renderer.")]
    public SkinnedMeshRenderer meshRenderer;
    [Tooltip("The player's GameObject.")]
    public GameObject player;
    [HideInInspector]
    public EnemyBehaviourState state;
    private Vector3 target = Vector3.zero; 
    
    //LOOT
    [Header("Loot")] 
    [Header("The script that will loot items.")] 
    private Loot loot;
    //WALK BEHAVIOUR
    [Header("Wall climbing behaviour parameters")] 
    [Tooltip("Distance from the center point up to which the Enemy is able to detect the presence of a ground surface.")] [SerializeField]
    private float surfaceDetectionDistance = 1f;
    [Tooltip("Vertical local offset distance form which the detection raycast is casted (used if the center point is set at the bottom of the 3D model.")] [SerializeField]
    private float surfaceDetectionOffset = 1f;
    [Tooltip("Distance at which entities begins to avoid each other.")] [SerializeField]
    private float avoidanceDistance = 0.1f; 
    [Tooltip("The layer considered as ground. Used to make entities stick to the cave's walls.")]
    public string groundLayer = "Default";
    [Tooltip("Vertical local offset distance used to make the 3D model touch the ground with its feet/lugs/paws etc.")]
    public float surfaceWalkingHeightOffset = 1f; 
    
    [Header("Fake Pathfinding behaviour parameters")] 
    [Tooltip("The scale of the Perlin noise.")] [SerializeField]
    private float noiseScale = 1;
    [Tooltip("The number of Perlin noise octaves wanted.")] [SerializeField]
    private float noiseOctaves = 1;
    [Tooltip("The amount of fake pathfinding effect. The greater it is, the more entities will walk randomly.")] [SerializeField]
    private float directionNoiseCoeff = 1;

    //FALLING BEHAVIOUR
    private float fallBeginingTime = 0f;
    [Header("Falling behaviour parameters")] 
    [Tooltip("The time spent falling after which the entity is considered out of bounds and will disappear.")] [SerializeField]
    private float timeBeforeOOB = 3f;
    
    //CHASING BEHAVIOUR
    [Header("Chasing behaviour parameters")]
    [Tooltip("The distance at which the entities don't feel safe. Beyond this distance they will try to avoid the player.")]
    public float proximityOffset = 1f;
    [Tooltip("The precision allowed for the entities to avoid the player.")]
    public float proximityPrecision = 0.1f;
    
    //ATTACKING BEHAVIOUR
    [Header("Attacking behaviour parameters")]
    [Tooltip("The probability for an entity to attack each frame.")] [SerializeField]
    public float attackProbability = 0.01f;
    //OTHER
    [Header("Other entity behaviour parameters")] 
    [Tooltip("The distance at which entities will begin to chase the player.")] [SerializeField]
    private float enemyDetectionDistance = 7f;
    [Tooltip("The distance from the player at which entities are asked to despawn.")] [SerializeField]
    private float outOfRangeDistance = 50f;
    


    //Private variables
    private float HP; //The entity's current HP
    private float walkingSeed; //A random seed used to prevent the spiders to move together.
    private bool isGrounded = false; //Probably not used anymore
    private Vector3 currentSurfaceNormal; //The current's surface's normal over which the entity is walking.
    private Vector3 movingDirection; //The direction toward which the entity is walking.

    //Falling behaviour

    private void Start()
    {
        HP = startHP; //Setting the HPs to the base HPs
        walkingSeed = UnityEngine.Random.Range(0,1000); //Randomly setting the walking behaviour's fake path finding's seed
        state = EnemyBehaviourState.Idle; //Setting the entity's state as Idle
    }
    void Update()
    {
        
        checkDistanceFromPlayer();//checking if the entity is too far or too close. If so, it wil change it's state
        if (state != EnemyBehaviourState.Disabled)
        {
            if (HP <= 0)//mort de l'araignée
            {
                state = EnemyBehaviourState.Disabled;
                loot.lootItems();
            }
            //tries to hold to any cave surface. if not possible, the entity is considered falling.
            wallClimbBehaviour();
            if (!isGrounded)//isOnGround is set to false if no surface is detected from walkClimbBehaviour().
                            //We can't just use state.Falling because we need to detect the switch to falling state to init the timer.
            {
                transform.position = transform.position - Vector3.up * 9.81f * Time.deltaTime;
                transform.up = Vector3.up;
                if (state != EnemyBehaviourState.Falling)
                {
                    fallBeginingTime = Time.time; //Reseting the timer.
                }
                state = EnemyBehaviourState.Falling;
                if ((Time.time - fallBeginingTime) > timeBeforeOOB) //If the entity has spent too much time of the walls, gets disabled
                {
                    state = EnemyBehaviourState.Disabled;
                }
            }
            else if (state == EnemyBehaviourState.Chasing) //if the entity is chasing the player.
            { 
                target = player.transform.position;
                target -= (player.transform.position - transform.position).normalized*proximityOffset; //slight offset in order to let the spiders wait arround the player and not under hes feets
            
                checkDistanceFromPlayer();// checking the distance from player (required because of wallClimbBehaviour() )
                move(); //moves the spider in direction to target
            }else if (state == EnemyBehaviourState.Fighting)
            {
                Debug.DrawLine(transform.position,player.transform.position);
                //Debug.Log(name + " is in Fighting state");

                if (UnityEngine.Random.Range(0f, 1f) < attackProbability)
                {
                    //Debug.Log(name + " is attacking the player !"); 
                    // TODO : compute attack direction to display hit mark on the HUD
                    player.GetComponent<PlayerStats>().SetDamage(10, Vector3.zero);
                }
            }
        }
        
        
    }


    public void checkDistanceFromPlayer()
    {
        if (state != EnemyBehaviourState.Disabled)
        {
            Vector3 playerPos = player.transform.position;
            float distance = Math.Abs((this.transform.position - playerPos).magnitude);
            if (name == "Araignée1")
            {
                //Debug.Log("Distance from player : " + distance);
            }
            if (distance < enemyDetectionDistance)
            {
                if (state != EnemyBehaviourState.Fighting)
                {
                    state = EnemyBehaviourState.Chasing;
                }
            }else if (distance > outOfRangeDistance)
            {
                //Debug.Log("to disable");
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
            Vector3 toPlayerDirection = player.transform.position - transform.position;
            if (movingDirection.magnitude > 1f)
            {
                movingDirection.Normalize();
            }
            
            var toTarget = target - transform.position;
            var toUp = currentSurfaceNormal;
            float sine = 0f;
            if (state == EnemyBehaviourState.Chasing)
            {
                for (int i = 1; i <= noiseOctaves; i++)
                {
                    sine += Mathf.PerlinNoise(walkingSeed+Time.time * (noiseScale * i), i) * 2 - 1;
                }
            }
            transform.rotation = Quaternion.LookRotation(toUp.normalized, -toTarget.normalized);
            transform.Rotate(Vector3.right, 90f, Space.Self);
            movingDirection = (sine * transform.right * directionNoiseCoeff + transform.forward).normalized;
            if (Vector3.Dot(toPlayerDirection, movingDirection) < 0) // if the spider is not facing the player
            {
                transform.rotation = Quaternion.LookRotation(toUp.normalized, movingDirection.normalized);
            }else
            {
                transform.rotation = Quaternion.LookRotation(toUp.normalized, -movingDirection.normalized);
            }
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
        Vector3 playerPos = player.transform.position;
        float distance = Math.Abs((this.transform.position - playerPos).magnitude);
        Debug.Log("distance from entity to player " + distance);
        if (distance > this.proximityOffset - this.proximityPrecision && distance < this.proximityOffset + this.proximityPrecision)
        {
            Debug.Log("ASK FOR ATTACK ");
        }
    }

    public void setDamage(RaycastHit hit, ParticleSystem damageEffect, float damage, ResourceType type)
    {
        HP -= damage; 
        ParticleSystem d = Instantiate(damageEffect, hit.point, Quaternion.identity);
        d.Play();
    }
}
