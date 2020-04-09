using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] private Renderer contentRenderer;
    [Range(0f,1f)] [SerializeField] private float fillAmount = 0.5f;
    [SerializeField] private float glassWidth = 0.1f;
    
    [Header("Wobble Effect")]
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    
    Renderer rend;
    //Wobble effect variables
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;  
    Vector3 angularVelocity;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        transform.localScale = Vector3.one * (1-glassWidth);
    }
    private void Update()
    {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));
 
        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
        
        //calcul de la scale en hauteur
        Vector3 objectSize = rend.bounds.size * (1-glassWidth);
//        Debug.Log("x : " + objectSize.x + " y : " + objectSize.y + " z : " + objectSize.z);
        
        // send it to the shader
        contentRenderer.material.SetFloat("_WobbleX", wobbleAmountX);
        contentRenderer.material.SetFloat("_WobbleZ", wobbleAmountZ);
        contentRenderer.material.SetFloat("_Height", objectSize.y);
        contentRenderer.material.SetFloat("_FillAmount", fillAmount);
        //rend.material.SetFloat("_Height", height);
 
        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;
 
 
        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
 
        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
 
 
}