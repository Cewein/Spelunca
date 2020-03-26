using System;
using UnityEngine;
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
}

public class GunArtefact : MonoBehaviour
{
    #region SerializedField ============================================================================================

    [Header("Linked objects")]
    [Tooltip("The gun magazine that stock ammo.")][SerializeField]
    private GunLoader magazine;
    [Tooltip("The gun magazine that stock ammo.")][SerializeField]
    private GunController controller;
    
    [Header("Data")]
    [Tooltip("Artefact name.")][SerializeField]
    private string name;
    [Tooltip("Artefact icon used in every UI")][SerializeField]
    private Sprite icon;
    [Tooltip("Crosshair parameters")][SerializeField]
    private Crosshair crosshairDataDefault;

    [Header("Projectile")]
    [Tooltip("Normal resource projectile prefab")][SerializeField]
    private Ammo normalResourceAmmo = null;
    [Tooltip("Resource type projectile prefab tab")][SerializeField]
    private Ammo[] AmmoType = null;
    [Tooltip("Point transform where muzzle flash will be spawned.")][SerializeField]
    private Transform muzzle = null;
    
    [Header("Shoot Parameters")]
    [Tooltip("How the the trigger ammo when we trigger it.")][SerializeField]
    private ShootingMode shootType;
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
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")][SerializeField]
    [Range(0f, 1f)]
    private float aimFovRatio = 1f;
    [Tooltip("Translation to apply to weapon arm when aiming with this weapon")][SerializeField]
    private Vector3 aimOffset;

   /* [Header("Charging (charging weapons only)")]
    [Tooltip("Auto-shooting or not when max charge is reached.")][SerializeField]
    private bool shootOnMaxEnergy;
    [Tooltip("Time needed to reach max energy charged")][SerializeField]
    private float chargeTime = 2f;*/
    
    #endregion
    
    #region Other fields ===============================================================================================

    private float lastTimeFiring;
    private Ammo currentAmmo;
    #endregion

    private void Start()
    {
        if (magazine == null){ magazine = GetComponentInParent<GunLoader>(); }
        controller.trigger += (down, held, up)=>Trigger(down,held,down);
        currentAmmo = normalResourceAmmo; // TODO : change on resource switching

    }

    private bool Trigger(bool inputDown, bool inputHeld, bool inputUp)
    {
        switch (shootType)
        {
            case ShootingMode.MANUAL:
                if (inputDown){ return TryShoot(); }
                return false;

            case ShootingMode.AUTO:
                if (inputHeld){ return TryShoot(); }
                return false;
            
            case ShootingMode.SEMI:break; // I don't know for the moment

         /*   case ShootingMode.CHARGE:
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
        if (magazine.CurrentResourceQuantity >= 0f && lastTimeFiring + firingRate < Time.time)
        {
            Shoot();
            return true;
        }

        return false;
    }

    private void Shoot()
    {
        // spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = SpreadBullet(muzzle); 
            Instantiate(currentAmmo, muzzle.position, Quaternion.LookRotation(shotDirection));
        }

        // muzzle flash
        if (currentAmmo.MuzzleFlash != null)
        {
            var muzzleFlashInstance = Instantiate(currentAmmo.MuzzleFlash, muzzle.position, muzzle.rotation, muzzle.transform); 
            // TODO : test  -> Destroy(Instantiate(muzzleFlashPrefab, weaponMuzzle.position, weaponMuzzle.rotation, weaponMuzzle.transform), 2f);

            Destroy(muzzleFlashInstance, 2f);
        }

        lastTimeFiring = Time.time;
    }
    
    private Vector3 SpreadBullet(Transform shootTransform)
    {
        return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,  bulletSpreadAngle / 180f);;
    }
}
