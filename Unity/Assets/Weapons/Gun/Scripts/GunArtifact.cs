using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GunArtifact : MonoBehaviour, ICollectible
{
    [Tooltip("The artifact scriptable object linked with")][SerializeField]
    private Artifact scriptableObject; 
    private bool isEquipped => transform.parent != null;

    public bool IsReachable(Ray ray, float distance)
    {
        if (isEquipped) return false;
        return Vector3.Distance(ray.origin, transform.position) < distance;
    }

    public void Collect()
    {
        if (isEquipped) return;
        if(Inventory.Instance.AddArtifact(scriptableObject))
        {
            Destroy(gameObject);
            Inventory.Instance.NotifyArtifactStockUpdate();
        }
   
    }

    public void Emphase(bool isEmphased)
    {
        if (isEquipped) return;
    }
}
