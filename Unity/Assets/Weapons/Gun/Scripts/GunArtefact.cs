using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

/// <summary>
/// Manual : need to press input each time to trigger ( ex : fusil à pompe )
/// Automatic : trigger while input is helding ( ex : mitraillette )
/// Charge : held input to charge energy and trigger when release (ex : les attaques chargées quoi )
/// semi-auto : I don't really know but some gun are like this in real life/video game so I put it
/// just in case.
/// </summary>
public enum ShootingMode{ MANUAL , AUTO, SEMI, CHARGE }

[System.Serializable]
public struct Crosshair
{
    [Tooltip("Sprite used to display the crosshair.")]
    public Sprite sprite;
    [Tooltip("The size of the crosshair image")]
    public int size;
    [Tooltip("The color of the crosshair image")]
    public Color color;
    [Tooltip("Distance the raycast can be perform")][SerializeField]
    public float scope;
}

public class GunArtefact : MonoBehaviour
{
    #region SerializedField ============================================================================================

    [Header("Linked objects")]
    [Tooltip("The gun magazine that stock ammo.")][SerializeField]
    private GunLoader magazine;
    [Tooltip("The gun magazine that stock ammo.")][SerializeField]
    private GunController controller;
    
    [Header("Pickaxe only")]
    [Tooltip("Is that the miner pickaxe ? ")][SerializeField]
    private bool isPickaxe = false;
    [Tooltip("Pickax damage")][SerializeField]
    private float pickaxeDamage = 5f;
    
    [Header("Data")]
    [Tooltip("Artefact name.")][SerializeField]
    private string name;
    [Tooltip("Artefact icon used in every UI")][SerializeField]
    private Sprite icon;
    [Tooltip("Crosshair parameters")][SerializeField]
    private Crosshair crosshair;

    [Header("Projectile")]
    [Tooltip("Normal resource projectile prefab")][SerializeField]
    private Ammo normalResourceAmmo = null;
    [Tooltip("Resource type projectile prefab tab")][SerializeField]
    private Ammo[] AmmoType = null;
    [Tooltip("Point transform where muzzle flash will be spawned.")][SerializeField]
    private Transform muzzle = null;


    [Header("Shoot Parameters")] 
    
    [Tooltip("How the the trigger ammo when we trigger it.")][SerializeField]
    private ShootingMode shootMode;
   // [Tooltip("The projectile prefab")]
    //public Projectile projectilePrefab;
    [Tooltip("Delay between two shot")][SerializeField]
    private float firingRate = 0.5f;
    [Tooltip("Cone angle where the bullet will spread while shooting.")][SerializeField]
    private float bulletSpreadAngle = 0f;
    [Tooltip("Amount of bullets per shot")][SerializeField]
    private int bulletsPerShot = 1;
    [Tooltip("gun recoil")][SerializeField]
    [Range(0f, 2f)]
    private float recoil = 1;
    
    [Header("Aiming Parameters")]
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")][SerializeField]
    [Range(0f, 1f)]
    private float aimFovRatio = 0.8f;
    [Tooltip("How fast the smooth aiming zoom animaiton occured.")][SerializeField]
    private float zoomSpeed = 10f;

   /* [Header("Charging (charging weapons only)")]
    [Tooltip("Auto-shooting or not when max charge is reached.")][SerializeField]
    private bool shootOnMaxEnergy;
    [Tooltip("Time needed to reach max energy charged")][SerializeField]
    private float chargeTime = 2f;*/
    
    #endregion
    
    #region Other fields ===============================================================================================

    public ShootingMode ShootMode => shootMode;
    private float normalFOV;
    private float aimingFOV;
    private float lastTimeFiring;
    private Ammo currentAmmo;
    private Ammo CurrentAmmo
    {
        get
        {
            if (isPickaxe) return normalResourceAmmo;
            if (currentAmmo == null || !currentAmmo.Type.Equals(magazine.CurrentResource.Type))
                currentAmmo = magazine.CurrentResource.Type.Equals(ResourceType.normal) ?
                                                            normalResourceAmmo : 
                                                            AmmoType.First(ammo => ammo
                                                                           .Type
                                                                           .Equals(magazine.CurrentResource.Type));
            return currentAmmo;
        }
    }

    public Crosshair Crosshair => crosshair;

    #endregion

    private void Start()
    {
        normalFOV = Camera.main.fieldOfView;
        aimingFOV = normalFOV * aimFovRatio;
        if (magazine == null && !isPickaxe){ magazine = GetComponentInParent<GunLoader>(); }
        if (controller == null){ controller = GetComponentInParent<GunController>(); }
        controller.trigger += (down, held, up)=>Trigger(down,held,down);
        controller.aim += Aim;
    }

    private void Aim(bool isAiming)
    {
        float targetFOV = isAiming ? aimingFOV : normalFOV;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);

    }

    private bool Trigger(bool inputDown, bool inputHeld, bool inputUp)
    {
        switch (shootMode)
        {
            case ShootingMode.MANUAL:
                if (inputDown){ return TryShoot(); }
                return false;

            case ShootingMode.AUTO:
                if (inputHeld){ return TryShoot(); }
                return false;
            
            /*case ShootingMode.SEMI:break; // I don't know for the moment

          case ShootingMode.CHARGE:
                if (inputHeld){ TryBeginCharge(); }
                if (inputUp || (shootOnMaxEnergy && currentCharge >= 1f))
                {
                    return TryReleaseCharge();
                }
                return false;*/

            default:
                return false;
        }

        return false;
    }
    
    private bool TryShoot()
    {
        if (isPickaxe)
        {
            return Pick(true);
        }
        if (magazine.CurrentResourceQuantity >= 0f && lastTimeFiring + firingRate < Time.time)
        {
            Shoot();
            return true;
        }

        return false;
    }
    
    private bool Pick(bool isShooting)
    {
        if (!isShooting || !GetComponent<Animator>().GetBool("canAttack"))
            return isShooting && GetComponent<Animator>().GetBool("canAttack");
        try
        {
            if (controller.Hit.transform != null)
            {
                       
                try
                {
                    controller.Hit.transform.gameObject.GetComponent<IPickable>().Pickax(controller.Hit, pickaxeDamage);
                }
                catch (NullReferenceException e){}

                try
                {
                    if (controller.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick.GetInvocationList()
                            .Length > 1)
                    {
                        controller.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick -= CollectResource;
                    }
              
                }
                catch (NullReferenceException e)
                {
                    controller.Hit.transform.gameObject.GetComponent<ResourceCollectible>().pick += CollectResource;
                }
            }
        }
        catch (NullReferenceException e){}

        return isShooting && GetComponent<Animator>().GetBool("canAttack");
    }

    private void CollectResource(ResourceType type, float quantity)
    {
        ResourcesStock.Instance.setResource(type,quantity);
    }

    private void Shoot()
    {
        // spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = SpreadBullet(muzzle); 
            Instantiate(CurrentAmmo, muzzle.position, Quaternion.LookRotation(shotDirection));
        }

        // muzzle flash
        if (CurrentAmmo.MuzzleFlash != null)
        {
            Instantiate(CurrentAmmo.MuzzleFlash, muzzle.position, muzzle.rotation, muzzle.transform);
        }
        
        try
        {
           if (controller.Hit.transform != null)
               controller.Hit.transform.gameObject.GetComponent<IDamageable>()
                                                  .setDamage( controller.Hit, 
                                                              CurrentAmmo.DamageEffect,
                                                      5, magazine.CurrentResource.Type
                                                  );
        }
        catch (NullReferenceException e){}
        


        lastTimeFiring = Time.time;
    }
    
    private Vector3 SpreadBullet(Transform shootTransform)
    {
        return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,  bulletSpreadAngle / 180f);;
    }
}
