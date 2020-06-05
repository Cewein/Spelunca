using System;
using Unity.Transforms;
using UnityEngine;

public class ResourceCollectible : MonoBehaviour, IPickable
{
   [SerializeField] private ResourceType type;
   [SerializeField] private float quantity;
   [SerializeField] private float pv;
   public Action<ResourceType,float> pick;

   public void Pickax(RaycastHit hit,float damage)
   {
      if ((pv -= damage) <= 0){Pick();}
   }

   private void Pick()
   {
      pick?.Invoke(type , quantity);
      ResourcesStock.Instance.setResource(type, quantity);
      Debug.Log(ResourcesStock.Instance.ToString());
      Destroy(transform.parent.gameObject);
   }
}
