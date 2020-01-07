using System;
using UnityEngine;
using UnityEngine.Serialization;

public class OrientationController : MonoBehaviour
{
    
    [Header("Inputs")]
    
    [Tooltip("The horizontal orientation input name as it defined in Edit > Project Settings > Inputs Manager.")][SerializeField]
    private string yawInputName = "Mouse X";

    [Tooltip("The horizontal orientation input name as it defined in Edit > Project Settings > Inputs Manager.")] [SerializeField] 
    private string pitchInputName = "Mouse Y";
   
    [Tooltip("The horizontal mouse sensitivity.")][SerializeField]
    private float horizontalMouseSensitivity = 150f; 
    
    [Tooltip("The vertical mouse sensitivity.")][SerializeField]
    private float verticalMouseSensitivity = 150f;

    [Header("Configurations")] 
    
    [Tooltip("If selected, clamp vertical rotation between the two angles value defined in pitch amplitude.")][SerializeField]
    private bool clampVerticalRotation = true;
    
    [Tooltip("If selected, clamp horizontal rotation between the two angles value defined in yaw amplitude.")][SerializeField]
    private bool clampHorizontalRotation = false;
    

    [Tooltip("The minimum and the maximum yaw value in degree.")][SerializeField]
    private Vector2 yawAmplitude = new Vector2(-45,45);
    
    [Tooltip("The minimum and the maximum pitch value in degree.")][SerializeField]
    private Vector2 pitchAmplitude = new Vector2(90,-45);


    [Header("Debug")]
    
    [Tooltip("Show axes.")] [SerializeField]
    private bool showAxes = false;
    
    [Tooltip("Axes length")] [SerializeField]
    private int axeLength = 2;
    
    
    private float yaw;
    private float pitch;
    private float row;
    
    public float Yaw
    {
        set => yaw = value;
    }

    public float Pitch
    {
        set => pitch = value;
    }

    public float Row
    {
        set => row = value;
    }

  public void Update()
    {
       rotate();
       setRotation();
    }

    private void rotate()
    {
        if ( clampHorizontalRotation ) yaw =  Mathf.Clamp(yaw,yawAmplitude.x,yawAmplitude.y) ;
        if ( clampVerticalRotation ) pitch = Mathf.Clamp(pitch,pitchAmplitude.x,pitchAmplitude.y) ;
         
        transform.localRotation  = Quaternion.Euler(new Vector3(pitch, yaw, 0.0f));
    }

    private void OnDrawGizmos()
    {    
        if (showAxes)
        {
            Debug.DrawRay(transform.localPosition, (transform.up*(!clampHorizontalRotation ? 1 : yawAmplitude.y)).normalized*axeLength, Color.magenta);
            Debug.DrawRay(transform.localPosition, (transform.right*(!clampVerticalRotation ? 1 :pitchAmplitude.y)).normalized*axeLength, Color.magenta);
        }
    }

    private void setRotation()
    {
         yaw += Input.GetAxis(yawInputName) * horizontalMouseSensitivity * Time.deltaTime;
         pitch -= Input.GetAxis(pitchInputName) * verticalMouseSensitivity * Time.deltaTime;
    }
}
