using System;
using UnityEngine;

public class Consumable : MonoBehaviour, ICollectible
{
    #region SerializedField ============================================================================================

    [Header("General settings")]
    [Tooltip("Consumable name displayed on UI.")][SerializeField]
    private string name = "Unamed";
    [Tooltip("Icon used on UI.")] [SerializeField]
    private Sprite icon = null;
    [Tooltip("Description to help the player")] [SerializeField]
    private string description;
    
    #endregion
    public bool IsReachable(Ray ray, float detectionScope)
    {
        return Vector3.Distance(ray.origin , transform.position) < detectionScope;
    }

    public void Collect(GameObject container)
    {
         //TODO : add to consumable stock
         
         Destroy(gameObject);


    }

    public void Emphase(bool isEmphased)
    {
        throw new System.NotImplementedException();
    }
}
