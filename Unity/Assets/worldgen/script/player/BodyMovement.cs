using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMovement : MonoBehaviour
{

    public Transform player;
    public float moveSpeed = 5;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Z))
        {
            player.position += new Vector3(Camera.main.transform.forward.x * Time.deltaTime * moveSpeed, 0, Camera.main.transform.forward.z * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            player.position -= new Vector3(Camera.main.transform.forward.x * Time.deltaTime * moveSpeed, 0, Camera.main.transform.forward.z * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 pos = Vector3.Cross(Camera.main.transform.forward, Camera.main.transform.up) * Time.deltaTime * moveSpeed;
            player.position += new Vector3(pos.x, 0, pos.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 pos = Vector3.Cross(Camera.main.transform.forward, Camera.main.transform.up) * Time.deltaTime * moveSpeed;
            player.position -= new Vector3(pos.x, 0, pos.z);
        }


    }
}
