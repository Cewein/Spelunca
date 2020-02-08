using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ElevateSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //Entities.ForEach((LevelComponent levelComponent) => { });
        Entities.ForEach((ref Translation translation, ref MoveSpeedComponent moveSpeedComponent) =>
        {
            translation.Value.y += moveSpeedComponent.Value * Time.DeltaTime;
            if (translation.Value.y < -10f)
            {
                moveSpeedComponent.Value = math.abs(moveSpeedComponent.Value);
            }
            else if(translation.Value.y > 10f)
            {
                moveSpeedComponent.Value = -math.abs(moveSpeedComponent.Value);
            }
        });
    }
}
