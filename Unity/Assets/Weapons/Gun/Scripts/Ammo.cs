using System;
using System.Collections;
using UnityEngine;

    public class Ammo : MonoBehaviour
{
    [Tooltip("Resource type of the projectile")][SerializeField]
    private ResourceType resourceType = ResourceType.normal; 
    [Tooltip("Resource type of the projectile")][SerializeField]
    private ParticleSystem muzzleFlash = null;
    [Tooltip("The default damage effect particle system")][SerializeField] 
    private ParticleSystem damageEffect = null;
   
    public ResourceType Type => resourceType;
    public ParticleSystem MuzzleFlash => muzzleFlash;

    public ParticleSystem DamageEffect => damageEffect;

    private void Awake()
    {
        StartCoroutine(AutoDestroy());
    }
    
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
