﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    public float defaultRunSpeed;
    public float initialJumpForce;

    public int maxJumps = 1;

    public float airControl = 0.5f;
    int currentJumps;

    GameObject model;
    GameObject cameraRig;

    public float gravity = -9.8f;
    float currentYVelocity;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentYVelocity = 0f;
        model = transform.Find("Model").gameObject;
        cameraRig = transform.Find("CameraRig").gameObject;
        currentJumps = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //X/Z Movement
        Vector3 camForward = GetForwardRelativeToCamera();
        Vector3 camRight = new Vector3(camForward.z, 0f, -camForward.x);

        Vector3 movementX = camRight * Input.GetAxis("Horizontal");
        Vector3 movementZ = camForward * Input.GetAxis("Vertical");
        
        Vector3 moveDirection = (movementX + movementZ);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        if(moveDirection.magnitude > 0) {
            model.transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        //Y Movement
        if(cc.isGrounded) {
            currentJumps = maxJumps;
            
            if(Input.GetButtonDown("Jump") && currentJumps > 0) {
                currentYVelocity = initialJumpForce;
                currentJumps -= 1;
            } else {
                currentYVelocity = 0;
            }
        } else {

            Vector3 currentHorizontalVelocity = cc.velocity;
            currentHorizontalVelocity.y = 0;
            Debug.Log("Horizontal Airspeed: " + currentHorizontalVelocity.magnitude);           
            
            currentYVelocity += Time.deltaTime * gravity;
             if(Input.GetButtonDown("Jump") && currentJumps > 0) {
                currentYVelocity = initialJumpForce;
                currentJumps -= 1;
            }   
        }
        
        moveDirection *= defaultRunSpeed;
        moveDirection.y = currentYVelocity;
        cc.Move(moveDirection * Time.deltaTime);
    }

    Vector3 GetForwardRelativeToCamera() {
        Vector3 camDir = Camera.main.transform.forward;
        camDir.y = 0;
        camDir = Vector3.Normalize(camDir);
        
        return camDir;
    }
}
            