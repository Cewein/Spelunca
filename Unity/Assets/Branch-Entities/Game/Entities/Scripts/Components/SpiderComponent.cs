using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum SpiderState
{
    Idle,
    Chasing,
    Jumping,
    Falling,
    Weaving
}

public struct SpiderComponent : IComponentData
{
    
    [Header("Base entity stats")]
    public float startHP; //the entity's start health points
    public float baseDamage; //base damage dealed when attacking an other entiy (such as a player)
    public float movingSpeed; //Speed at which the entity is able to move
    public float HP; //The Spider's health points
    [Header("Behaviours parameters")]
    public GameObject target; //The target of the entity
    public float surfaceDetectionDistance; //Distance from the center point up to which the Enemy is able to detect the presence of a ground surface 
    public float surfaceDetectionOffset; //Offset form which the detection raycast is casted (used if the center point is set at the bottom of the 3D model
    public float surfaceWalkingHeightOffset; //Offset used to make the 3D model touch the ground with its feet/lugs/paws etc
    public float avoidanceDistance; // Length of the raycasts casted in order to detect nearby fellow entities 
    public float heightOffset;
    public static SpiderState state;
}
