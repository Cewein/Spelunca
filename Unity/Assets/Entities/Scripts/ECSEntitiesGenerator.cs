using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Random = UnityEngine.Random;

public class ECSEntitiesGenerator : MonoBehaviour
{
    [SerializeField] private int startEntitiesCount = 2000;
    [SerializeField] private int sizeZoneX = 25;
    [SerializeField] private int sizeZoneY = 25;
    [SerializeField] private int sizeZoneZ = 10;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(RotateComponent),
            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(SpiderComponent)
        );
        NativeArray<Entity> entityArray = new NativeArray<Entity>(startEntitiesCount, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);
        //entityManager.SetComponentData(entity, new RotateComponent {radiansPerSecond = Random.Range(10f,20f)});
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new Scale {Value = 10f});
            entityManager.SetComponentData(entity, new SpiderComponent
            {
                startHP = 10f,
                baseDamage = 3f,
                movingSpeed = 1.5f,
                HP = 10f,
                surfaceDetectionDistance  = 0.5f,
                surfaceDetectionOffset = 0.2f,
                surfaceWalkingHeightOffset = 0.5f,
                avoidanceDistance = 0.2f,
                state = SpiderState.Chasing
            });
            entityManager.SetComponentData(entity, new Translation {Value = new Vector3(Random.Range(-sizeZoneX,sizeZoneX),Random.Range(-sizeZoneY,sizeZoneY),Random.Range(-sizeZoneZ,sizeZoneZ))});
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
        }

        entityArray.Dispose();
        
    }

}

