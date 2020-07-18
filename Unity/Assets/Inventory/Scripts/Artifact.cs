using UnityEngine;

[CreateAssetMenu(fileName = "Artifact", menuName = "ScriptableObjects/Inventory/Items/Artifact")]
public class Artifact : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;

    public GameObject Prefab => prefab;
    public Sprite Sprite     => icon;
    public string Name       => name;
    
}
