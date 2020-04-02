using System;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Configuration")]
    [Tooltip("The camera used to perform raycast.")] [SerializeField]
    private Camera cam = null;


    [Header("Debug")] 
    
    [Tooltip("Show raycast.")]
    public bool showRaycast = true;
    
    [Tooltip("Raycast color when the ray is hitting a target.")]
    public Color onTargetColor = Color.red;
    
    [Tooltip("Raycast color when the ray is hitting nothing.")]
    public Color outTargetColor = Color.yellow;
   
    #endregion ==========

    #region Fields ==========
    [HideInInspector]
    public RaycastHit Hit;
    [HideInInspector]
    public float scope = 100f;

    private Ray ray;
   
    #endregion ==========

    #region  Methodes ==========

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        
    }

    public void PerformRaycast()
    {
        Ray ray = cam.ScreenPointToRay(transform.position);

        if (Physics.Raycast(ray,  out Hit, scope))
        {
            if (showRaycast) Debug.DrawRay(ray.origin, ray.direction*scope, onTargetColor);
        }
        else
        {
            if (showRaycast) Debug.DrawRay(ray.origin, ray.direction*scope, outTargetColor);
        }
    }

    #endregion ==========
}
