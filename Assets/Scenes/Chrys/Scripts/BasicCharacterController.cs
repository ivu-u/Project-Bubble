using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = 9.81f;
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.1f;
    public float childBobFrequency = 3f;
    public float childBobAmplitude = 0.05f;
    public Transform childTransform;
    
    private CharacterController controller;
    private Vector3 moveDirection;
    private bool isGrounded;
    private float bobTimer;
    private float childBobTimer;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveZ = Input.GetAxis("Horizontal") * moveSpeed;
        float moveY = moveDirection.y;

        if (controller.isGrounded)
        {
            isGrounded = true;
            moveY = -gravity * Time.deltaTime;

            if (Input.GetButtonDown("Jump"))
            {
                moveY = jumpForce;
            }
        }
        else
        {
            isGrounded = false;
            moveY -= gravity * Time.deltaTime;
        }

        moveDirection = new Vector3(0, moveY, moveZ);
        controller.Move(moveDirection * Time.deltaTime);
        
        //scale invert
        if (moveZ < 0) {
            transform.GetChild(0).transform.localScale = new Vector3(1, 1, -1);
        }
        else if (moveZ > 0)
        {
            transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
        }
        
        //scale bobbing
        if (moveZ != 0)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float stretchFactor = 1 + Mathf.Sin(bobTimer) * bobAmplitude;
            transform.GetChild(0).localScale = new Vector3(1 / stretchFactor, stretchFactor, transform.GetChild(0).localScale.z);
        }
        
        //y-axis bobbing lol
        if (childTransform != null && moveZ != 0)
        {
            childBobTimer += Time.deltaTime * childBobFrequency;
            float bobOffset = Mathf.Sin(childBobTimer + 0.5f) * childBobAmplitude;
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, bobOffset, childTransform.localPosition.z);
        }
    }
}
