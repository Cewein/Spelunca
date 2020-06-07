using System;
using UnityEngine;

public class LifePotion : MonoBehaviour, ICollectible
{
   [SerializeField] private int healPoints = 1;
   [SerializeField] private Consumable scriptableObject = null;
   public void callback()
   {
      FindObjectOfType<PlayerStats>().RestoreLife(healPoints); // TODO : not very good to use findObjectOfType
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
