using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//using RaycastHit = Unity.Physics.RaycastHit;
//using Physics = Unity.Physics;

public class SpiderSystem : ComponentSystem
{
    public Vector3 target = new Vector3(5,2,10);
    public float detectionDistance = 0.5f;
    protected override void OnUpdate()
    {
        //Entities.ForEach((LevelComponent levelComponent) => { });
        Entities.ForEach((ref SpiderComponent spiderComponent,ref Translation translation, ref Rotation rotation, ref Scale scale, ref LocalToWorld localToWorld) =>
        {
            
            switch (spiderComponent.state)
            {
                case SpiderState.Falling:
                {
                    translation.Value += (float3)(Vector3.down * 9.81f * Time.DeltaTime);
                    break;
                }
                case SpiderState.Chasing:
                {
                    WalkClimbComponent(ref spiderComponent, ref translation, ref rotation, ref localToWorld);
                    Vector3 direction = (float3)target - translation.Value;
                    direction.Normalize();
                    translation.Value = translation.Value + (float3)direction * spiderComponent.movingSpeed * Time.DeltaTime;
//                    Transform t = new Transform();
//                    t.position = translation.Value;
//                    t.LookAt(target);
//                    rotation = t.rotation;
                    break;
                }
            }
        });
    }

    private void WalkClimbComponent(ref SpiderComponent spiderComponent, ref Translation translation,
        ref Rotation rotation, ref LocalToWorld localToWorld)
    {
        float3 direction = -localToWorld.Up;

        RaycastHit hit;
        if (Physics.Raycast(translation.Value, direction, out hit, 1f, 1 << LayerMask.NameToLayer("Ground")))
        {
            translation.Value = hit.point + hit.normal * spiderComponent.heightOffset;
        }
        else
        {
            spiderComponent.state = SpiderState.Falling;
        }
    }
    
}