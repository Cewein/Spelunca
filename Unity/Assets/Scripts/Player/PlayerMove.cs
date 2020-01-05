using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 10f;
    public float jumpForce = 10f;
    public Animator cameraAnimation;

    private Vector3 velocity = Vector3.zero;

    private Rigidbody rb;
    private bool isJump = false;

    public void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");
        
        if (zMov != 0 || xMov != 0)
        {
            cameraAnimation.SetBool("isRunning", true);
        }
        else
        {
            cameraAnimation.SetBool("isRunning", false);
        }
        if (Input.GetAxis("Jump") > 0 )
        {
            isJump = true;
            rb.AddForce(Vector3.up * jumpForce);
            cameraAnimation.SetBool("isJumping", true);
        }
        else
        {
            cameraAnimation.SetBool("isJumping", false);
        }

        
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;
        //      On conserve l'ancienne accélération verticale   
        velocity = Vector3.up * rb.velocity.y + (moveHorizontal + moveVertical).normalized * acceleration* Time.fixedDeltaTime;
    }
    void FixedUpdate()
    {
        PlayerMovement();
    }
    void PlayerMovement()
    {
        if (velocity != Vector3.zero)
        {
            //Vector3 newPos = rb.position + velocity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (isJump && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("reset");
            isJump = false;
        }
    }
}
