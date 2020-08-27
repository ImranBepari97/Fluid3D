using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    Animator animator;
    GlobalPlayerController gpc;
    Rigidbody rb;
    CapsuleCollider cc;

    bool isRightWall;
    public Vector3 defaultOffset;
    public Vector3 crouchOffset;

    public Vector3 wallLeftOffset;
    public Vector3 wallRightOffset;

    public Vector3 wallHoldOffset;

    // Start is called before the first frame update
    void Start()
    {
        isRightWall = false;
        gpc = transform.parent.GetComponent<GlobalPlayerController>();
        animator = GetComponent<Animator>();
        rb = transform.parent.GetComponent<Rigidbody>();
        cc = transform.parent.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update() {

        bool gameStarted;
        if(GameController.instance != null) {
           gameStarted = GameController.instance.gameState == GameState.PLAYING;
        } else {
            gameStarted = true;
        }

        FindWallClosest();

        animator.SetBool("isRightWallClosest", isRightWall);
        animator.SetBool("isWallRunning", gpc.recentAction == RecentActionType.WallRunning);
        animator.SetBool("isDashing", gpc.recentAction == RecentActionType.Dash);
        animator.SetBool("isSliding", gpc.recentAction == RecentActionType.Slide);
        animator.SetBool("isJumping", gpc.recentAction == RecentActionType.RegularJump);
        animator.SetBool("isGrinding", gpc.recentAction == RecentActionType.Grind);
        animator.SetBool("isGrounded", gpc.isGrounded);
        animator.SetBool("isSlideJumping", gpc.recentAction == RecentActionType.SlideJump);
        animator.SetBool("isOnWall", gpc.recentAction == RecentActionType.OnWall);
        animator.SetBool("gameStarted", gameStarted);
        animator.SetFloat("playerHeight", cc.height);

        Vector3 horVel = rb.velocity;
        horVel.y = 0;

        animator.SetFloat("horizontalVelocity", horVel.magnitude);
        animator.SetFloat("verticalVelocity", rb.velocity.y);

        InterpToLocation();
    }

    void InterpToLocation() {

        float crouchYOffset = RangeRemap(cc.height, 0, 1.5f, crouchOffset.y, defaultOffset.y);

        Vector3 offset = new Vector3(0, crouchYOffset, 0);

        if(gpc.recentAction == RecentActionType.WallRunning) {
            if(isRightWall) {
                offset.x = wallRightOffset.x;
            } else {
                offset.x = wallLeftOffset.x;
            }
        }

        if(gpc.recentAction == RecentActionType.OnWall) {
            offset.z = wallHoldOffset.z;
        }

        transform.localPosition = offset;
        
    }

    void FindWallClosest() {
        RaycastHit hitRight;
        RaycastHit hitLeft;


        Debug.DrawRay(rb.position, transform.right, Color.red, 0.01f);
        Debug.DrawRay(rb.position, -transform.right, Color.blue, 0.01f);
        float rightWallDist = 4f;
        if(Physics.Raycast(rb.position, transform.right,  out hitRight, 3f)) {
            if(hitRight.collider.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
                //Debug.Log("right wall");
                isRightWall = true;
                rightWallDist = Vector3.Distance(rb.position, hitRight.point);
            }
        }

        if(Physics.Raycast(rb.position, -transform.right,  out hitLeft, 3f)) {
            if(hitLeft.collider.gameObject.layer == LayerMask.NameToLayer("Parkour") &&
                Vector3.Distance(rb.position, hitLeft.point) < rightWallDist) {
                //Debug.Log("left wall");
                isRightWall = false;
            }
        }
    }

    public static float RangeRemap (float value, float from1, float to1, float from2, float to2) {
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}
}
