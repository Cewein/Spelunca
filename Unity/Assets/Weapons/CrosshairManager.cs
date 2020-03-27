using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    #region SerializedFields ============================================================================================

    [Header("Linked objects")] [Tooltip("Miner controller to know wich weapon is currently set.")] [SerializeField]
    private MinerController minerController;
    
    [Header("General settings")]
    [Tooltip("Current crosshair image")][SerializeField]
    private Image crossHair;
    [Tooltip("Crosshair animation speed. (when it point on a target, it will be animated)")]
    public float animationSpeed = 5f;
    
    [Header("Special crosshair")]
    [Tooltip("Crosser where gun magazine is empty")][SerializeField]
    private Sprite noAmmo;
    [Tooltip("Crosser when player point on a collectible")][SerializeField]
    private Sprite collectible;
    
    #endregion
    
    #region Other Fields ============================================================================================
    
    private bool isOntarget; // IDammageable or IPickable
    private bool pointOnCollectible; // ICollectible
    private RectTransform crossHairSocket;
    private Crosshair currentWeaponCrosshair;
    private Crosshair currentCrosshair; // it can be a special one, not just weapon crosshair
    
    #endregion

    void Awake()
    {
        if (minerController == null) minerController = GameObject.FindObjectOfType<MinerController>();

        OnWeaponChanged(minerController.CurrentWeapon );

        minerController.switchWeapon += OnWeaponChanged;
    }
    
    private void OnWeaponChanged(GameObject currentWeapon)
    {
        if(currentWeapon)
        {
            try
            {
                currentWeaponCrosshair = currentWeapon.GetComponentInChildren<GunArtefact>().Crosshair;
            }
            catch (NullReferenceException e)
            {
                
            }
            crossHair.enabled = true;
        }
    }

}
