using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/Inventory/Items/Consumable")]
public class Consumable : ScriptableObject
{
    [SerializeField] private string name = "";
    [SerializeField] private Sprite icon = null;
    [SerializeField] private string description = "";
    [SerializeField] private GameObject prefab = null;
    public UnityEvent callback;
    public string Name => name;
    public Sprite Icon => icon;
    public string Description => description;
    public GameObject Prefab => prefab;
    
    public void Use()
    {
        callback.Invoke();
        ConsumableStock.Instance.TakeConsumable(this);
    }
}
