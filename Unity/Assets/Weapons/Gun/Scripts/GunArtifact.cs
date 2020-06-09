using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Weapon reloading mode.
/// Manual : need to press input each time to trigger and reload after each shoot ( ex : shotgun )
/// Automatic : shoot while input is pressed until the magazine is empty ( ex : machine gun )
/// Semi-auto : need to press input each time to trigger but no need to reload if the magazine isn't empty (ex : sniper)
/// Charge : held input to charge energy and trigger when release (ex : plasma canon )
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

public class GunArtifact : MonoBehaviour, ICollectible
{
    #region Fields =====================================================================================================
    
    [Header("Linked objects")]
    [Tooltip("The gun magazine that stock ammo.")][SerializeField]
    private GunLoader magazine;
    [Tooltip("The gun controller.")][SerializeField]
    private GunController controller;
    [Tooltip("The artifact scriptable object wich represent this artifact.")][SerializeField]
    private Artifact scriptableObject;
    
    [Header("Shoot Parameters")] 
    [Tooltip("Hit point caused by this artifact (without type scaling).")]
    public int damage = 5;
    [Tooltip("Crosshair parameters")][SerializeField]
    private Crosshair crosshair;
    [Tooltip("Weapon reloading mode.")][SerializeField]
    private ShootingMode shootingMode;
    [Tooltip("Delay between two shot")][SerializeField]
    private float firingRate = 0.5f;
    [Tooltip("Cone angle where the bullet will spread while shooting.")][SerializeField]
    private float bulletSpreadAngle = 0f;
    [Tooltip("Amount of bullets per shot")][SerializeField]
    private int bulletsPerShot = 1;
    [Tooltip("gun recoil")][SerializeField]
    [Range(0f, 2f)]
    private float recoil = 1;
    [Tooltip("Time between a shot and the reloading action ( Manual weapon only )")][SerializeField] 
    private int reloadCooldown = 30;
    
    [Header("Aiming Parameters")]
    [Tooltip("Default field of view, when it isn't aiming.")][SerializeField]
    private float normalFOV = 75;
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")][SerializeField]
    [Range(0f, 1f)]
    private float aimFovRatio = 0.8f;
    [Tooltip("How fast the smooth aiming zoom animaiton occured.")][SerializeField]
    private float zoomSpeed = 10f;
    
    [Header("Projectile")]
    [Tooltip("Normal resource projectile prefab")][SerializeField]
    private Ammo normalResourceAmmo = null;
    [Tooltip("Resource type projectile prefab tab")][SerializeField]
    private Ammo[] AmmoType = null;
    [Tooltip("Point transform where muzzle flash will be spawned.")] [SerializeField]
    private Transform muzzle = null;

    private bool isEquipped => transform.parent != null;
    public Artifact ScriptableObject => scriptableObject;
    public Crosshair Crosshair => crosshair;
    public ShootingMode ShootingMode => shootingMode;
    
    
    private int timer;
    private bool forceReload;
    private float lastTimeFiring;
    private Vector3 target;

    private Ammo currentAmmo;
    private Ammo CurrentAmmo
    {
        get
        {
            if (currentAmmo == null || !currentAmmo.Type.Equals(magazine.CurrentResource.Type))
                currentAmmo = magazine.CurrentResource.Type.Equals(ResourceType.normal) ?
                    normalResourceAmmo : 
                    AmmoType.First(ammo => ammo
                        .Type
                        .Equals(magazine.CurrentResource.Type));
            return currentAmmo;
        }
    }
    #endregion


    #region ICollectible ===============================================================================================
    public bool IsReachable(Ray ray, float distance)
    {
        if (isEquipped) return false;
        return Vector3.Distance(ray.origin, transform.position) < distance;
    }

    public void Collect()
    {
        if (isEquipped) return;
        if(Inventory.Instance.AddArtifact(scriptableObject))
        {
            Destroy(gameObject);
            Inventory.Instance.NotifyArtifactStockUpdate();
        }
    }

    public void Emphase(bool isEmphased)
    {
        if (isEquipped) return;
    }
    #endregion

    #region Function ===================================================================================================

    private void Start()
    {
        if (!isEquipped)
        {
            gameObject.layer =  LayerMask.NameToLayer("Default");
            return;
        }
        magazine   = GetComponentInParent<GunLoader>();
        controller = GetComponentInParent<GunController>();
        controller.trigger += adaptTrigger;
        forceReload = false;
        controller.aim += Aim;
        controller.GetComponentInParent<MinerController>().NotifyArtifactEquipped();
        Destroy(GetComponent<BoxCollider>());
        gameObject.layer =  LayerMask.NameToLayer("AvoidClipping");
    }
    
    private void adaptTrigger(bool down,bool  held, bool up)
    {
        if (isEquipped) Trigger(down, held, down);
    }
    
    private bool Trigger(bool inputDown, bool inputHeld, bool inputUp)
    {
        switch (shootingMode)
        {
            case ShootingMode.MANUAL:
                if (inputDown)
                {
                    if (TryShoot())
                    {
                        timer = reloadCooldown;
                        forceReload = true;
                        return forceReload;
                    }
                }
                return false;

            case ShootingMode.AUTO:
                if (inputHeld){ return TryShoot(); }
                return false;
            
            case ShootingMode.SEMI:
                if (inputDown){ return TryShoot(); }
                return false;
            
            default:
                return false;
      }
  }
    
    private bool TryShoot()
    {
        if (magazine.CurrentResourceQuantity >= 0f
            && lastTimeFiring + firingRate < Time.time
            && !controller.TriggerReloadAnimation
            && !forceReload)
        {
            Shoot();
            return true;
        }
        return false;
    }
    
    private bool Shoot()
    {
        // spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = SpreadBullet(muzzle); 
            Instantiate(CurrentAmmo, muzzle.position, Quaternion.LookRotation(shotDirection));
            magazine.isConsuming(true, 1);
        }

        // muzzle flash
        if (CurrentAmmo.MuzzleFlash != null)
        {
            Instantiate(CurrentAmmo.MuzzleFlash, muzzle.position, muzzle.rotation, muzzle.transform);
        }
        try
        {
            if (controller.Hit.transform != null)
            {
                controller.Hit.transform.gameObject.GetComponent<IDamageable>()
                    .setDamage( controller.Hit, 
                        CurrentAmmo.DamageEffect,
                        damage, magazine.CurrentResource.Type
                    );
            }
               
        }
        catch (NullReferenceException e){}

        lastTimeFiring = Time.time;
        return true;
    }
    
    private Vector3 SpreadBullet(Transform shootTransform)
    {
        return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,  bulletSpreadAngle / 180f);;
    }
    
    private void LateUpdate()
    {
        if (isEquipped && forceReload) ForceReload();
    }
    
    private void ForceReload()
    {
        timer--;
        if (timer >= 0) return;
        controller.isReloading(true);
        forceReload = false;
    }

    private void OnDestroy()
    {
        if(!isEquipped) return;
        controller.trigger -= adaptTrigger;
        controller.aim -= Aim;
    }
    
    private void Aim(bool isAiming)
    {
        if (!isEquipped) return;
        float targetFOV = isAiming && !controller.TriggerReloadAnimation  ? normalFOV * aimFovRatio : normalFOV;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }
    
  #endregion
}
