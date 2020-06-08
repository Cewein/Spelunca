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

    [Tooltip("Pointer object detection distance")] [SerializeField]
    private float pointerScope;
    
    [Header("Special crosshair")]
    [Tooltip("Crosser where gun magazine is empty")][SerializeField]
    private Sprite noAmmo;
    [Tooltip("Crosser when player point on a collectible")][SerializeField]
    private Crosshair collectible;
    
    #endregion
    
    #region Other Fields ============================================================================================

    private Image crossHair;
    private Raycast raycaster;
    private bool isOntarget; // IDammageable or IPickable
    private bool pointOnCollectible; // ICollectible
    private RectTransform crossHairSocket;
    private Crosshair currentWeaponCrosshair;
    private Crosshair currentCrosshair; // it can be a special one, not just weapon crosshair
    private RaycastHit hit;
    private MinerInputHandler inputHandler;
    
    #endregion

    private void Awake()
    {
        raycaster = GetComponentInChildren<Raycast>();
        crossHair = raycaster.GetComponent<Image>();
        crossHairSocket = crossHair.GetComponent<RectTransform>();
        if (minerController == null) minerController = GameObject.FindObjectOfType<MinerController>();
        
        OnSwitchingWeapon(minerController.CurrentWeapon);

        minerController.switchWeapon += OnSwitchingWeapon;
        inputHandler = minerController.gameObject.GetComponent<MinerInputHandler>();
    }
    
    private void OnSwitchingWeapon(GameObject currentWeapon)
    {
        if(currentWeapon && !pointOnCollectible)
        {
            try
            {
              //  currentWeaponCrosshair = currentWeapon.GetComponentInChildren<GunArtefact>().Crosshair;
            }
            catch (NullReferenceException e)
            {
               /* try
                {currentWeaponCrosshair = currentWeapon.GetComponent<GunArtefact>().Crosshair;}
                catch (NullReferenceException ex){}*/
            }
            raycaster.scope  = currentWeaponCrosshair.scope;
            crossHair.sprite = currentWeaponCrosshair.sprite;
            crossHair.color  = currentWeaponCrosshair.color;
            crossHairSocket.sizeDelta =  Vector2.one * currentWeaponCrosshair.size;
            crossHair.enabled = true;
        }
    }

    private void Update()
    {
        PointerRaycast();
        Pointer();
    }

    private void PointerRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(raycaster.transform.position);
        if (Physics.Raycast(ray, out hit, pointerScope )) // TODO : layers
        {
            try
            {
                var item = hit.transform.gameObject.GetComponent<ICollectible>();
                pointOnCollectible = item.IsReachable(ray, collectible.scope);
                if (pointOnCollectible)
                {
                    item.Emphase(true);
                    if (inputHandler.isInteracting())
                        item.Collect(); 
                }
                else item.Emphase(false);

            }
            catch (NullReferenceException e){ pointOnCollectible = false;}
        }
        else { pointOnCollectible = false; }
    }

    private void Pointer()
    {
        if (pointOnCollectible)
        {
            crossHair.sprite = collectible.sprite;
            crossHair.color  = collectible.color;
            crossHairSocket.sizeDelta =  Vector2.one * collectible.size;
        }
        else
        {
            crossHair.sprite = currentWeaponCrosshair.sprite;
            crossHair.color  = currentWeaponCrosshair.color;
            crossHairSocket.sizeDelta =  Vector2.one * currentWeaponCrosshair.size;

        }
    }

}
