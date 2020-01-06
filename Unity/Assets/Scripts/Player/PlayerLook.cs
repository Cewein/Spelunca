    using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform playerBody;
    public GameObject projectile;
    public float shootVelocity = 10f;
    private float yAxisClamp;

    private void Awake()
    {
        LockCursor();
        yAxisClamp = 0.0f;
    }

    private void LockCursor()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        CameraRotation();
        if (Input.GetKeyDown("a"))
        {
            Instantiate(projectile, transform.position + transform.forward, transform.rotation);
            projectile.GetComponent<Rigidbody>().velocity = transform.forward * shootVelocity;
        }
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        yAxisClamp += mouseY;
        
        if (yAxisClamp > 90.0f)
        {
            yAxisClamp = 90.0f;
            mouseY = 0f;
            ClampXAxisRotationToValue(270f);
        }else if (yAxisClamp < -90.0f)
        {
            yAxisClamp = -90.0f;
            mouseY = 0f;
            ClampXAxisRotationToValue(90f);
        }
        
        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
