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
        Entities.ForEach((ref SpiderComponent spiderComponent,ref Translation translation, ref Rotation rotation, ref LocalToWorld localToWorld) =>
        {
            RaycastHit hit;
            //-transform.up
            Vector3 direction = translation.Value - translation.Value * 0.5f;
            if (Physics.Raycast(translation.Value, direction, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))
            {
                //transform.up = Vector3.Lerp(transform.up,hit.normal, 0.5f);
                translation.Value = hit.point + hit.normal * spiderComponent.heightOffset;
            }
            else
            {
                SpiderComponent.state = SpiderState.Falling;
            }

            switch (SpiderComponent.state)
            {
                case SpiderState.Falling:
                {
                    translation.Value += (float3)(Vector3.down * 9.81f * Time.deltaTime);
                    break;
                }
                case SpiderState.Chasing:
                {
                    direction = (float3)target - translation.Value;
                    direction.Normalize();
//                    Transform t = new Transform();
//                    t.position = translation.Value;
//                    t.LookAt(target);
//                    rotation = t.rotation;
                    break;
                }
            }
        });
    }
    
}