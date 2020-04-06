using UnityEngine;

public class IAInputHandler : MonoBehaviour
{
    public float detectionRange = 3.0f;
    public float timeBetweenShoot = 0.5f;
    public float rotationSharpness = 10f;
    public Transform player;
    
    private bool fire;
    private bool reload;
    private bool aim;
    private float dt = 1.0f;

    private void Update()
    {
        
        if (Vector3.Distance(transform.position, player.position) < 2*detectionRange)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(player.position - transform.position), 
                                                  rotationSharpness * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            aim = true;
        }
        else
        {
            aim = false;
            fire = false;
        }

        if (aim)
        {
            dt--;
            if (dt <= timeBetweenShoot)
            {
                fire = true;
                dt = timeBetweenShoot;
            }
        }
    }

    private void Start()
    {
        if (player == null) player = GetComponent<PlayerStats>().transform;
    }


    public bool isFiringDown()
    {
        return GetIfPlayerCanPlay() && fire;
    }

    public bool isFiringHeld()
    {
        return GetIfPlayerCanPlay() &&isFiringDown();
    }

    public bool isFiringUp()
    {
        return GetIfPlayerCanPlay() && isFiringDown();
    }

    public bool isAiming(bool perform)
    {
        return perform && GetIfPlayerCanPlay() && aim;
    }

    public bool isReloading()
    {
        return GetIfPlayerCanPlay() && reload;
    }
    
    private bool GetIfPlayerCanPlay()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

}
