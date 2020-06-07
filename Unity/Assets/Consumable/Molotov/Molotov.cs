using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : MonoBehaviour, ICollectible
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float blastRadius = 10;
    [SerializeField] private float blastForce = 10;
    [SerializeField] private Consumable scriptableObject = null;
    public void callback()
    {
        Debug.Log("throw a molotv");
    }

    public bool IsReachable(Ray ray, float detectionScope)
    {
        return Vector3.Distance(ray.origin , transform.position) < detectionScope;
    }

    public void Collect()
    {
        Inventory.Instance.AddConsumable(scriptableObject);
        Destroy(gameObject);
    }

    public void Emphase(bool isEmphased)
    {
        //TODO : add outlined shader (or any shader that emphasis the object)
    }
}
