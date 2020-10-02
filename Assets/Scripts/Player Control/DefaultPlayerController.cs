using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DefaultPlayerController : NetworkBehaviour
{
    //general variables
    GlobalPlayerController gpc;
    Rigidbody rb;
    CapsuleCollider col;
    GameObject model;

    //control variables
    public float gravityScale = -35f;
    public float defaultRunSpeed = 10f; //the slowest run speed the player has
    public float currentMaxSpeed; //current speed the player get's to move at
    public float initialJumpForce = 17.5f;
    public float airControl = 0.5f;
    public float airDashSpeed = 25f;
    public float crouchSpeedMultiplier = 0.5f;
    public float slideControl = 10f;

    //Input management and error correcting variables
    //Vector3 moveDirection;

    [SyncVar]
    Vector3 dashDirection;

    [SyncVar]
    float yVel;

    [SyncVar]
    Vector3 currentHorizontalVelocity;



    // Start is called before the first frame update
    void Awake()
    {
        col = GetComponent<CapsuleCollider>();
        gpc = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        model = transform.Find("Model").gameObject;
        currentMaxSpeed = defaultRunSpeed;
    }

    void Start() {
        if(!isLocalPlayer) {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame, we should get all input as constantly as possible
    void Update() {
        if (!isLocalPlayer) {
            // exit from update if this is not the local player.
            rb.isKinematic = true;
            return;
        }

        currentMaxSpeed = defaultRunSpeed * gpc.currentSpeedMultiplier;
        //moveDirection = gpc.input.moveDirection; //current input left and right, relative to the camera

        if(gpc.input.moveDirection.magnitude > 0.1f && gpc.recentAction != RecentActionType.Dash 
            && gpc.recentAction != RecentActionType.Slide && gpc.recentAction != RecentActionType.SlideJump) {
            gameObject.transform.rotation = Quaternion.LookRotation(gpc.input.moveDirection);
        }
    }

    void FixedUpdate() {

        if (!isLocalPlayer) {
            // exit from update if this is not the local player
            return;
        }

        CmdDoFixedUpdate(gpc.input.moveDirection, gpc.input.jumpPressed, gpc.input.dashPressed, gpc.input.crouchPressed);

        //reset inputs
        gpc.input.jumpPressed = false;
        gpc.input.dashPressed = false;
    }    

    void CmdDoFixedUpdate(Vector3 moveDirection, bool jump, bool dash, bool crouch) {
        yVel = rb.velocity.y;
        currentHorizontalVelocity = rb.velocity;
        currentHorizontalVelocity.y = 0;

        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (gpc.recentAction != RecentActionType.Dash) { //custom gravity, turn off while dashing
            rb.AddForce(Physics.gravity * gravityScale * Time.fixedDeltaTime * 60);
        }
        
        //these methods will edit yVel for the later statement
        if(gpc.isGrounded) {
            HandleGrounded(moveDirection, jump, dash, crouch);
        } else {
            HandleInAir(moveDirection, jump, dash, crouch);
        }
        
        //put it all together
        rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);
    }

    void ShrinkPlayer() {
        // col.height = Mathf.Lerp(col.height, 0f, 0.1f);
        // col.radius = Mathf.Lerp(col.radius, 0.4f, 0.1f); 
        col.height = 0.01f;
        col.radius = 0.4f;
    }

    void UnshrinkPlayer() {
        RaycastHit hit;
        //only try to unshrink if you have free space above so player doesn't get stuck
        if(Physics.SphereCast(transform.position + (Vector3.up * col.height / 2), col.radius + 0.1f, Vector3.up, out hit, 1.1f) &&
            hit.collider.gameObject.tag != "Player") {
            return;
        } else {
            // col.height = Mathf.Lerp(col.height, 1.5f, 0.1f);
            // col.radius = Mathf.Lerp(col.radius, 0.5f, 0.1f);
            col.height = 1.5f;
            col.radius = 0.5f;
        }
    }

    IEnumerator SlideActCooldown(float cooldownTimeSeconds) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
    }

    private void HandleInAir(Vector3 moveDirection, bool jump, bool dash, bool crouch) {
        UnshrinkPlayer();
            //check what the players doing
        if (gpc.recentAction == RecentActionType.None) { //general air drift

            if(currentHorizontalVelocity.magnitude < currentMaxSpeed * 1.1f) {
                rb.AddForce(moveDirection * currentMaxSpeed * airControl, ForceMode.Force);
                rb.velocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), currentMaxSpeed);
            }

            if(jump && gpc.currentJumps > 0) { //multi jump if you can
                //gpc.recentAction = RecentActionType.RegularJump;
                gpc.CmdSetRecentAction(RecentActionType.RegularJump);
                yVel = initialJumpForce;
                gpc.currentJumps -= 1;
            } else if (dash && gpc.currentDashes > 0) { //dash if you can
                CmdStartDash(moveDirection);
            }

        } else if(gpc.recentAction == RecentActionType.RegularJump) { //if you just jumped, you still have air control for a split second

            float dot = Vector3.Dot(
                currentHorizontalVelocity.normalized, 
                moveDirection.normalized
            );
            //Debug.Log(dot);
            if(dot > 0.85f) {
                rb.velocity = new Vector3(
                    moveDirection.x * currentHorizontalVelocity.magnitude * 0.85f,
                    rb.velocity.y,
                    moveDirection.z * currentHorizontalVelocity.magnitude * 0.85f
                );
            } else {
                rb.velocity = new Vector3(
                    moveDirection.x * currentHorizontalVelocity.magnitude * 0.5f,
                    rb.velocity.y,
                    moveDirection.z * currentHorizontalVelocity.magnitude * 0.5f
                );
            }
            
        } else if(gpc.recentAction == RecentActionType.Dash) { //if dashing continue doing that 
            rb.velocity = new Vector3(dashDirection.x, 0, dashDirection.z) * airDashSpeed;
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    private void HandleGrounded(Vector3 moveDirection, bool jump, bool dash, bool crouch) {
        if (jump) { //initial jump fine
            yVel = initialJumpForce;
            if (gpc.recentAction == RecentActionType.Slide) {
                //gpc.recentAction = RecentActionType.SlideJump;
                gpc.CmdSetRecentAction(RecentActionType.SlideJump);
                rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);
            } else {
                //gpc.recentAction = RecentActionType.RegularJump;
                gpc.CmdSetRecentAction(RecentActionType.RegularJump);
            }       
        }

        //running is always locked at a default speed to encourage air movement
        if (crouch && currentHorizontalVelocity.magnitude > 0.9f * defaultRunSpeed
            && gpc.recentAction == RecentActionType.None) { //slide if you're moving fast enough on the ground
            //Debug.Log("first crouch");
            ShrinkPlayer();
            gpc.recentAction = RecentActionType.Slide;
            gpc.CmdSetRecentAction(RecentActionType.Slide);
            rb.velocity = new Vector3(
                Mathf.Clamp(rb.velocity.x * 1.8f, 1.8f * -currentMaxSpeed, 1.8f * currentMaxSpeed),
                rb.velocity.y,
                Mathf.Clamp(rb.velocity.z * 1.8f, 1.8f * -currentMaxSpeed, 1.8f * currentMaxSpeed)
            );
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
        } else if (gpc.recentAction == RecentActionType.Slide && currentHorizontalVelocity.magnitude < defaultRunSpeed * 0.25f) {
            UnshrinkPlayer();
            //Debug.Log("slide is ending");
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
            gpc.CmdSetRecentAction(RecentActionType.None);

        } else if (crouch && gpc.recentAction == RecentActionType.None) { //move with crouched speed
            ShrinkPlayer();
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z) * currentMaxSpeed * crouchSpeedMultiplier;
            //Debug.Log("crouch walk");
        } else if (crouch && gpc.recentAction == RecentActionType.Slide && gpc.floorNormal != new Vector3(0, 1, 0)) { //slide time
            //Debug.Log("slide");
            ShrinkPlayer();
            Vector3 slideDir = gpc.floorNormal;
            slideDir.y = 0;
            rb.AddForce((slideDir * currentMaxSpeed * 1.2f) + (moveDirection * slideControl));
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);

        } else if (gpc.recentAction == RecentActionType.Slide) { //getting up from slide
            //Debug.Log("slide getup");
            ShrinkPlayer();
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z) * 0.975f;
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);

        } else if (gpc.recentAction != RecentActionType.SlideJump) { //move normally
            UnshrinkPlayer();
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z) * currentMaxSpeed;
            //fine if we're still crouched stay slow
            if(col.height < 1.4f) {
                rb.velocity *= crouchSpeedMultiplier;
            }

            //if we're clearly not moving and grounded, then dont move on the Y axis
            //stops slopes 

            if ((!isLocalPlayer || moveDirection.magnitude < 0.15f) && rb.velocity.magnitude < 2f && 
            gpc.recentAction == RecentActionType.None && gpc.floorNormal != new Vector3(0, 1, 0)) {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }
        } 
    }
    
    private void CmdStartDash(Vector3 moveDirection) {
        gpc.recentAction = RecentActionType.Dash;
        gpc.CmdSetRecentAction(RecentActionType.Dash);
        yVel = 0;
        gpc.currentDashes -= 1;
                //Debug.DrawRay(rb.position, moveDirection.normalized, Color.cyan, 2f );

        if (moveDirection.magnitude > 0) { //dash in your current facing direction if you have no directional input 
            dashDirection = moveDirection.normalized;
        } else {
            dashDirection = rb.transform.forward;
        }
    
        rb.velocity = new Vector3(dashDirection.x, 0, dashDirection.z) * airDashSpeed;
    }
}
            