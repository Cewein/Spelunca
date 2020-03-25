using UnityEngine;

[CreateAssetMenu(fileName = "NormalAmmo", menuName = "ScriptableObjects/Artefacts/Ammo", order = 1)]
public class Ammo : ScriptableObject
{
    [Tooltip("Resource type of the projectile")][SerializeField]
    private ResourceType resourceType = ResourceType.normal; 
    [Tooltip("Resource type of the projectile")][SerializeField]
    private ParticleSystem muzzleFlash = null; 
    
    public ResourceType Type => resourceType;
    public ParticleSystem MuzzleFlash => muzzleFlash;
}
