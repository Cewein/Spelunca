using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class SmoothMouseLook : MonoBehaviour
{
    Vector2 rotation = new Vector2(0, 0);
    public float speed = 3;
    public Texture2D crosshair;

    public Vector2 clampX = new Vector2(-89,89);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        rotation.y += Input.GetAxis("Mouse X") * speed;
        rotation.x += -Input.GetAxis("Mouse Y") * speed;
        rotation.x = Mathf.Clamp(rotation.x, clampX.x, clampX.y);
        transform.eulerAngles = rotation * speed;
        Camera.main.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
       
    }

    private void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshair.width / 2);
        float yMin = (Screen.height / 2) - (crosshair.height / 2);

        GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
    }
}