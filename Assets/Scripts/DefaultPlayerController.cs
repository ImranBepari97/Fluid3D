﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlayerController : MonoBehaviour
{
    //general variables
    GlobalPlayerController gpc;
    Rigidbody rb;
    GameObject model;

    //control variables
    public float gravityScale = -35f;
    public float defaultRunSpeed = 10f; //the slowest run speed the player has
    public float maxPossibleSpeed = 20f; //the fastest speed the player has
    float currentMaxSpeed; //current speed the player get's to move at
    public float initialJumpForce = 17.5f;
    public float airControl = 0.5f;
    float yVel;

    //Input management and error correcting variables
    Vector3 moveDirection;    

    //this is a successful jump, where we cannot jump again, yet maintain 100% air control for a split second 
    Vector3 currentHorizontalVelocity;


    // Start is called before the first frame update
    void Start()
    {
        gpc = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        model = transform.Find("Model").gameObject;
        gpc.currentJumps = 0;
        currentMaxSpeed = defaultRunSpeed;
    }

    // Update is called once per frame, we should get all input as constantly as possible
    void Update() {

        yVel = rb.velocity.y;
        currentHorizontalVelocity = rb.velocity;
        currentHorizontalVelocity.y = 0;

        moveDirection = InputController.moveDirection; //current input left and right, relative to the camera

        if(moveDirection.magnitude > 0.01f) {
            gameObject.transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        
       
    }

    void FixedUpdate() {
        rb.AddForce(Physics.gravity * gravityScale); //custom gravity

        if(gpc.isGrounded) {
            gpc.currentJumps = gpc.extraJumps;

            if(InputController.jumpPressed) {
                gpc.hasRecentlyJumped = RecentJumpType.Regular;
                yVel = initialJumpForce;
            } 

            //running is always locked at a default speed to encourage air movement
            rb.velocity = new Vector3(
                moveDirection.x * defaultRunSpeed,
                rb.velocity.y,
                moveDirection.z * defaultRunSpeed
            );

        } else {

            if(gpc.hasRecentlyJumped == RecentJumpType.None) { //general air drift
                
                rb.AddForce(moveDirection * currentMaxSpeed * airControl, ForceMode.Force);
                rb.velocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), currentMaxSpeed);

                if(InputController.jumpPressed && gpc.currentJumps > 0) {
                    gpc.hasRecentlyJumped = RecentJumpType.Regular;;
                    yVel = initialJumpForce;
                    gpc.currentJumps -= 1;
                } 

            } else if(gpc.hasRecentlyJumped == RecentJumpType.Regular) {
                rb.velocity = new Vector3(
                    moveDirection.x * defaultRunSpeed * 0.5f,
                    rb.velocity.y,
                    moveDirection.z * defaultRunSpeed * 0.5f
                );
            }
        }
        
        //put it all together
        rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);

        //reset inputs
        InputController.jumpPressed = false;
    }


    
    
}
            