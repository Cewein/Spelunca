using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityGenerator : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int amount = 100;

    public float radius = 10f;

    public bool onSurface = true;

    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            if (onSurface)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Random.insideUnitSphere, out hit, radius,
                    1 << LayerMask.NameToLayer("Ground")))
                {
                    GameObject obj = Instantiate(prefabToSpawn, hit.point,
                        Quaternion.Euler(0, 0, 0),transform);
                    obj.transform.up = hit.normal;
                    obj.name = "cube " + i;
                    obj.GetComponent<SurfaceWalkingComponent>().target = target;
                }
            }
            else
            {
                GameObject obj = Instantiate(prefabToSpawn, Random.insideUnitSphere * radius + transform.position,
                    Quaternion.Euler(0, Random.Range(0, 360), 0),transform);
                obj.name = "cube " + i;
                obj.GetComponent<SurfaceWalkingComponent>().target = target;

            }
            
        }
    }
    
    private void Update()
    {
        Debug.Log("TEST TARGET");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0, LayerMask.NameToLayer("Player"));
        foreach (var hit in hits)
        {
            Debug.Log("TARGET DETECTED");
            foreach (var enemy in this.GetComponentsInChildren<SurfaceWalkingComponent>())
            {
                enemy.target = hit.collider.gameObject;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}
