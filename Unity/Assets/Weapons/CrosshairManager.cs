using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    #region SerializedFields ============================================================================================

    [Header("Linked objects")] [Tooltip("Miner controller to know wich weapon is currently set.")] [SerializeField]
    private MinerController minerController;
    
    [Header("General settings")]
    [Tooltip("Crosshair animation speed. (when it point on a target, it will be animated)")]
    public float animationSpeed = 5f;
    
    [Header("Special crosshair")]
    [Tooltip("Crosser where gun magazine is empty")][SerializeField]
    private Sprite noAmmo;
    [Tooltip("Crosser when player point on a collectible")][SerializeField]
    private Sprite collectible;
    
    #endregion
    
    #region Other Fields ============================================================================================

    private Image crossHair;
    private bool isOntarget; // IDammageable or IPickable
    private bool pointOnCollectible; // ICollectible
    private RectTransform crossHairSocket;
    private Crosshair currentWeaponCrosshair;
    private Crosshair currentCrosshair; // it can be a special one, not just weapon crosshair
    
    #endregion

    private void Awake()
    {
        crossHair = GetComponentInChildren<Raycast>().GetComponent<Image>();
        crossHairSocket = crossHair.GetComponent<RectTransform>();
        if (minerController == null) minerController = GameObject.FindObjectOfType<MinerController>();
        
        OnSwitchingWeapon(minerController.CurrentWeapon );

        minerController.switchWeapon += OnSwitchingWeapon;
    }
    
    private void OnSwitchingWeapon(GameObject currentWeapon)
    {
        if(currentWeapon)
        {
            try
            {
                currentWeaponCrosshair = currentWeapon.GetComponentInChildren<GunArtefact>().Crosshair;
            }
            catch (NullReferenceException e)
            {
                try
                {currentWeaponCrosshair = currentWeapon.GetComponent<GunArtefact>().Crosshair;}
                catch (NullReferenceException ex){}
            }

            crossHair.sprite = currentWeaponCrosshair.sprite;
            crossHair.color = currentWeaponCrosshair.color;
            crossHairSocket.sizeDelta =  Vector2.one * currentWeaponCrosshair.size;
            crossHair.enabled = true;
        }
    }

}
