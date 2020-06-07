using System;
using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent callback;
    
    public string Name => name;

    public Sprite Icon => icon;

    #endregion
    public bool IsReachable(Ray ray, float detectionScope)
    {
        return Vector3.Distance(ray.origin , transform.position) < detectionScope;
    }

    public void Collect()
    {
         ConsumableStock.Instance.SetConsumable(this);
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public void Emphase(bool isEmphased)
    {
        //TODO : add outlined shader (or any shader that emphasis the object)
    }

    public void Use()
    {
        callback.Invoke();
        ConsumableStock.Instance.TakeConsumable(this);
    }
    
    
}
