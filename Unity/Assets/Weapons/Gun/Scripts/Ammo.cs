using UnityEngine;

public class Ammo : MonoBehaviour
{
    [Tooltip("Resource type of the projectile")][SerializeField]
    private ResourceType resourceType = ResourceType.normal; 
    [Tooltip("Resource type of the projectile")][SerializeField]
    private ParticleSystem muzzleFlash = null; 
    
    public ResourceType Type => resourceType;
    public ParticleSystem MuzzleFlash => muzzleFlash;
}
