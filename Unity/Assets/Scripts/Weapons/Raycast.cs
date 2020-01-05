using System;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Configuration")]
    [Tooltip("The camera used to perform raycast.")] [SerializeField]
    private Camera cam = null;

    [Tooltip("The targets layer mask name.")] [SerializeField]
    private string layerMaskName = "Enemy";

    [Header("Debug")] 
    
    [Tooltip("Show raycast.")]
    public bool showRaycast = true;
    
    [Tooltip("Raycast length.")]
    public int rayLenght = 1000;
    
    [Tooltip("Raycast color when the ray is hitting a target.")]
    public Color onTargetColor = Color.red;
    
    [Tooltip("Raycast color when the ray is hitting nothing.")]
    public Color outTargetColor = Color.yellow;
   
   #endregion ==========

   #region Fields ==========

   private RaycastHit hit;
   private Ray ray;
   
   #endregion ==========

    #region  Methodes ==========

   
    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(transform.position);

        if (Physics.Raycast(ray,  out hit, Mathf.Infinity, LayerMask.GetMask(layerMaskName)))
        {
            if (showRaycast) Debug.DrawRay(ray.origin, ray.direction*rayLenght, onTargetColor);
        }
        else
        {
            if (showRaycast) Debug.DrawRay(ray.origin, ray.direction*rayLenght, outTargetColor);
        }
    }

    #endregion ==========
}
