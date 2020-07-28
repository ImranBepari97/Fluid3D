using System.Collections;
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
    public float airDashSpeed = 25f;
    float yVel;

    //Input management and error correcting variables
    Vector3 moveDirection;
    Vector3 dashDirection;

    //this is a successful jump, where we cannot jump again, yet maintain 100% air control for a split second 
    Vector3 currentHorizontalVelocity;


    // Start is called before the first frame update
    void Start()
    {
        gpc = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        model = transform.Find("Model").gameObject;
        currentMaxSpeed = defaultRunSpeed;
    }

    // Update is called once per frame, we should get all input as constantly as possible
    void Update() {

        yVel = rb.velocity.y;
        currentHorizontalVelocity = rb.velocity;
        currentHorizontalVelocity.y = 0;

        moveDirection = InputController.moveDirection; //current input left and right, relative to the camera

        if(moveDirection.magnitude > 0.1f) {
            gameObject.transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void FixedUpdate() {

        if(gpc.hasRecentlyJumped != RecentJumpType.Dash) { //custom gravity, turn off while dashing
            rb.AddForce(Physics.gravity * gravityScale);
        }
        
        if(gpc.isGrounded) {
            if(InputController.jumpPressed) { //initial jump fine
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

            //check what the players doing
            if(gpc.hasRecentlyJumped == RecentJumpType.None) { //general air drift
                
                rb.AddForce(moveDirection * currentMaxSpeed * airControl, ForceMode.Force);
                rb.velocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), currentMaxSpeed); //make sure you can't drift too fast

                if(InputController.jumpPressed && gpc.currentJumps > 0) { //multi jump if you have the resources
                    gpc.hasRecentlyJumped = RecentJumpType.Regular;
                    yVel = initialJumpForce;
                    gpc.currentJumps -= 1;
                } else if (InputController.dashPressed && gpc.currentDashes > 0) { //dash if you can
                    gpc.hasRecentlyJumped = RecentJumpType.Dash;
                    yVel = 0;
                    gpc.currentDashes -= 1;
                    Debug.DrawRay(rb.position, moveDirection.normalized, Color.cyan, 2f );
                    dashDirection = moveDirection.normalized;
                    rb.velocity = new Vector3(
                        dashDirection.x * airDashSpeed,
                        0,
                        dashDirection.z * airDashSpeed
                    );
                }

            } else if(gpc.hasRecentlyJumped == RecentJumpType.Regular) {
                rb.velocity = new Vector3(
                    moveDirection.x * defaultRunSpeed * 0.5f,
                    rb.velocity.y,
                    moveDirection.z * defaultRunSpeed * 0.5f
                );
            } else if(gpc.hasRecentlyJumped == RecentJumpType.Dash) { //if dashing continue doing that 
                rb.velocity = new Vector3(
                    dashDirection.x * airDashSpeed,
                    0,
                    dashDirection.z * airDashSpeed
                );
            }
        }
        
        //put it all together
        rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);

        //reset inputs
        InputController.jumpPressed = false;
        InputController.dashPressed = false;
    }    
    
}
            