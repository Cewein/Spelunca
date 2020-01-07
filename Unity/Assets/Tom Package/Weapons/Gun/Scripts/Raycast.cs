using System;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    #region SerializeFields ==========

    [Header("Configuration")]
    [Tooltip("The camera used to perform raycast.")] [SerializeField]
    private Camera cam = null;

    [Tooltip("The raycast scope.")] [SerializeField]
    private int scope = 100;

    [Header("Debug")] 
    
    [Tooltip("Show raycast.")]
    public bool showRaycast = true;
    
    [Tooltip("Raycast color when the ray is hitting a target.")]
    public Color onTargetColor = Color.red;
    
    [Tooltip("Raycast color when the ray is hitting nothing.")]
    public Color outTargetColor = Color.yellow;
   
    #endregion ==========

    #region Fields ==========

    private RaycastHit hit;

    public RaycastHit Hit;
 
    private Ray ray;
   
    #endregion ==========

    #region  Methodes ==========
   
    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(transform.position);

        if (Physics.Raycast(ray,  out hit, scope))
        {
            if (showRaycast) Debug.DrawRay(ray.origin, ray.direction*scope, onTargetColor);
            Hit = hit;
        }
        else
        {
            if (showRaycast) Debug.DrawRay(ray.origin, ray.direction*scope, outTargetColor);
        }
    }

    #endregion ==========
}
