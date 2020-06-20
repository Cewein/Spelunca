using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Camera))]
public class CameraAnimationEventHandler : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField]
    private MinerInputHandler inputHandler = null;
    
    [Header("Emotion")]
    [SerializeField][Range(1,3)] private float fear = 0;
 //   [SerializeField][Range(0,1)] private float health = 1; // TODO : caméra plus instable si peu de vie

    [Header("Breath")]
    [SerializeField] private float minHeight = .7f;
    [SerializeField] private float maxHeight = .9f;

    private float breathingMovement;

    private bool breathIn;
    //[Header("Run")]
    //[Header("Jump")]
    
    private void Start()
    {
        //inputHandler = GetComponentInParent<MinerInputHandler>();
        /*animator.SetBool("isRunning", inputHandler.isRunning()); 
        animator.SetBool("isJumping", inputHandler.isJumping()); */
    }

    private void Update()
    {
        Breath();
    }

    private void Breath()
    {
        if (breathIn)
        {
            breathingMovement = Mathf.Lerp(breathingMovement, maxHeight, Time.deltaTime*  1.7f * fear);
            transform.localPosition = new Vector3(transform.localPosition.x,breathingMovement,transform.localPosition.z);
            if (breathingMovement >= maxHeight -.005f) breathIn = false;
        }
        else
        {
            breathingMovement = Mathf.Lerp(breathingMovement, minHeight, Time.deltaTime* 1.7f * fear);
            transform.localPosition = new Vector3(transform.localPosition.x,breathingMovement,transform.localPosition.z);
            if (breathingMovement <= minHeight +.005f) breathIn = true;
        }

    }

}
