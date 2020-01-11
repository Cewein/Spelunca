using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

public class Boot : MonoBehaviour
{
    [SerializeField] private int startEntitiesCount = 2000;
    [SerializeField] private int sizeZoneX = 25;
    [SerializeField] private int sizeZoneY = 25;
    [SerializeField] private int sizeZoneZ = 10;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(RotateComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
        typeof(MoveSpeedComponent)
        );
        NativeArray<Entity> entityArray = new NativeArray<Entity>(startEntitiesCount, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);
        //entityManager.SetComponentData(entity, new RotateComponent {radiansPerSecond = Random.Range(10f,20f)});
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new RotateComponent {radiansPerSecond = Random.Range(10f,20f)});
            entityManager.SetComponentData(entity, new MoveSpeedComponent {Value = Random.Range(5f,20f)});
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
