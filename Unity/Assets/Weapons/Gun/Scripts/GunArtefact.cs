﻿using System;
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

   [Header("AI")] 
   [Tooltip("Is the miner controller an Artificial Intelligence ?")][SerializeField] 
   private bool ai = false;
    
    #endregion
    
    #region Other fields ===============================================================================================

    private int timer;
    private bool forceReload;
    private float normalFOV;
    private float lastTimeFiring;
    private MinerController miner;
    private Vector3 target;

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
    public ShootingMode ShootMode => shootMode;

    #endregion

    private void Start()
    {
        normalFOV = Camera.main.fieldOfView;
        miner = GetComponentInParent<MinerController>();
        if (magazine == null && !isPickaxe){ magazine = GetComponentInParent<GunLoader>(); }
        if (controller == null){ controller = GetComponentInParent<GunController>(); }
        controller.trigger += (down, held, up)=>Trigger(down,held,down);
        if (!ai) controller.aim += Aim;
        forceReload = false;
    }

    private void Aim(bool isAiming)
    {
        float targetFOV = isAiming && !controller.TriggerReloadAnimation  ? normalFOV * aimFovRatio : normalFOV;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    private bool Trigger(bool inputDown, bool inputHeld, bool inputUp)
    {
        switch (shootMode)
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
                

        /*  case ShootingMode.CHARGE:
                if (inputHeld){ TryBeginCharge(); }
                if (inputUp || (shootOnMaxEnergy && currentCharge >= 1f))
                {
                    return TryReleaseCharge();
                }
                return false;*/

            default:
                return false;
        }
    }

    private void LateUpdate()
    {
        if (forceReload && !isPickaxe) ForceReload();
    }

    private bool TryShoot()
    {
        if (isPickaxe){return Pick(true);}
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

    private bool Shoot()
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

        TakeRecoil();
        lastTimeFiring = Time.time;
        return true;
    }

    private void TakeRecoil()
    {
        try
        {
            miner.WeaponParent.position -= miner.WeaponParent.transform.forward*recoil;
        }
        catch (NullReferenceException e){} 
        // TODO : FPSCamera shake accordind to recoil;
    }
    
    private Vector3 SpreadBullet(Transform shootTransform)
    {
        return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,  bulletSpreadAngle / 180f);;
    }

    private void ForceReload()
    {
        timer--;
        if (timer >= 0) return;
        controller.isReloading(true);
        forceReload = false;
    }
}