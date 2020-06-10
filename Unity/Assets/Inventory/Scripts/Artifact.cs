using UnityEngine;

[CreateAssetMenu(fileName = "Artifact", menuName = "ScriptableObjects/Inventory/Items/Artifact")]
public class Artifact : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;
    public Sprite Icon     => icon;
    public string Name       => name;

    public void Equipped(Transform parent)
    { 
        try{Destroy(parent.GetChild(0).gameObject);}
        catch(UnityException){}
        Instantiate(prefab, parent);
    }

    public void Throw(Transform parent)
    {
        GameObject equippedArtifact = parent.GetChild(0).gameObject;
        if (equippedArtifact != null && equippedArtifact.GetComponent<GunArtifact>().ScriptableObject.name == name)
        {
            Destroy(equippedArtifact);
        }
        Inventory.Instance.TakeArtifact(this); //TODO : the prefab must appear on the map again
    }
}
