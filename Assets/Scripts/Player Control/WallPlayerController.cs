using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WallPlayerController : NetworkBehaviour
{

    public Vector3 wallNormal;
    GlobalPlayerController gpc;

    public List<GameObject> wallsCollidingWith;

    bool canAct = true;
    
    public GameObject lastWallTouched;

    public float initialJumpForce = 17.5f;
    public float wallRunInitialJumpForce = 12.5f;
    public float wallSlideDownSpeed = 1f;

    public float defaultWallRunSpeed = 12.3f;
    public float wallRunDuration = 2f;
    public float wallRunLogScale = 1.55f;
    public float wallRunEndSpeed = 8f;

    [SyncVar]
    public float currentWallRunDuration;

    [SyncVar]
    public bool isWallRunning;

    [SyncVar]
    public Vector3 wallRunDirection;
    
    [SyncVar]
    bool isRunningOnRightWall;

    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        gpc = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        wallNormal = new Vector3(0, 0, 0);
        wallRunDirection = new Vector3(0, 0, 0);
        currentWallRunDuration = wallRunDuration;
        isWallRunning = false;
        wallsCollidingWith = new List<GameObject>();
        isRunningOnRightWall = false;
    }

    void OnEnable()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.useGravity = false;
        canAct = false;
        currentWallRunDuration = wallRunDuration;
        StartCoroutine(CanActCoolDown(0.15f));
        CalculateWallNormalAndDirection();

        gpc.ResetJumpsAndDashes();
    }

    void OnDisable()
    {
        rb.useGravity = true;
        if(gpc.recentAction == RecentActionType.WallRunning || 
            gpc.recentAction == RecentActionType.OnWall) {
            gpc.recentAction = RecentActionType.None;
        }
    }

    void Update()
    {
        currentWallRunDuration -= Time.deltaTime;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //preparation from last physics frame
        if(gpc.recentAction != RecentActionType.OnWall) {
            CalculateWallNormalAndDirection();
        //cast a 45 degree angle downwards from the torso to check wall cling?
        } else if(!(Physics.Raycast(transform.position + (transform.up * 0.65f), transform.forward + -transform.up, 0.75f))) {
            DismountFromWall(true);
        }

        //wall calculations have determined we're not on a wall, and should cease
        if(!this.enabled) {
            return;
        }

        isWallRunning = wallRunDirection != new Vector3(0, 0, 0) && currentWallRunDuration > 0; //  && gpc.recentAction != RecentActionType.WallJump;

        //stuff to do this physics frame
        gpc.recentAction = RecentActionType.None;

        if (isWallRunning) { //regular wall running
            gpc.recentAction = RecentActionType.WallRunning;
            rb.velocity = wallRunDirection * ((defaultWallRunSpeed * currentWallRunDuration * Mathf.Log10(gpc.currentSpeedMultiplier + wallRunLogScale)) + wallRunEndSpeed);
            gameObject.transform.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(rb.velocity), 540f * Time.deltaTime);

        } else if (wallRunDirection != new Vector3(0, 0, 0)) { //ran out of running, detach from wall
            Debug.Log("dismount as ran out of wall run time");
            Debug.Log("wallRunDir: " + wallRunDirection);
            Debug.Log("wallRunDuration: " + currentWallRunDuration);
            Debug.Log("wallNormal: " +  wallNormal);
            DismountFromWall(true);

        } else { //sliding down wall
            gpc.recentAction = RecentActionType.OnWall;
            rb.AddForce(Physics.gravity * wallSlideDownSpeed);
            gameObject.transform.rotation = Quaternion.LookRotation(-wallNormal);
        }

        if (InputController.jumpPressed && canAct && isLocalPlayer) {

            gpc.IncreaseSpeedMultiplier(0.2f);
            if (isWallRunning) {
                rb.velocity = new Vector3(
                    rb.velocity.normalized.x + wallNormal.normalized.x + (InputController.moveDirection.x * 0.4f), 
                    1f, 
                    rb.velocity.normalized.z + wallNormal.normalized.z + (InputController.moveDirection.z * 0.4f)) * wallRunInitialJumpForce;
            } else {
                rb.velocity = new Vector3(
                    wallNormal.normalized.x + (InputController.moveDirection.x * 0.4f), 
                    2f, 
                    wallNormal.normalized.z + (InputController.moveDirection.z * 0.4f)).normalized * initialJumpForce * 1.25f;

                gameObject.transform.rotation = Quaternion.LookRotation(wallNormal);
            }

            gpc.recentAction = RecentActionType.WallJump;

            Debug.DrawRay(rb.position, rb.velocity, Color.blue, 2f);
            gpc.EnableDefaultControls();
        } else if(InputController.crouchPressed && isLocalPlayer) {
            InputController.crouchPressed = false;
            DismountFromWall(false);
        }


        InputController.jumpPressed = false;
        InputController.dashPressed = false;
    }

    void DismountFromWall(bool maintainMomentum = false) {
        gpc.recentAction = RecentActionType.None;
        if(maintainMomentum) {
            rb.velocity = new Vector3(rb.velocity.x + wallNormal.x, 0f, rb.velocity.z + wallNormal.z).normalized * rb.velocity.magnitude * 0.9f;
        } else {
            rb.velocity = new Vector3(rb.velocity.x + wallNormal.x, 0f, rb.velocity.z + wallNormal.z).normalized * 5f;
        }
        
        gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
        gpc.EnableDefaultControls();
    }

    void CalculateWallNormalAndDirection() {

        Vector3 horVel; 
        if(rb.velocity.magnitude > 0) {
            horVel = rb.velocity.normalized;
            horVel.y = 0;
        } else {
            horVel = transform.forward;
        }

        float rightWallDist = float.MaxValue;
        RaycastHit rightHit;
        if(Physics.Raycast(rb.position, Quaternion.AngleAxis(90f, Vector3.up) * horVel, out rightHit, 1f)) {
            if(rightHit.collider.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
                //Debug.Log("right wall");
                rightWallDist = Vector3.Distance(transform.position, rightHit.point);
                isRunningOnRightWall = true;
                //Debug.Log("closer to right wall");
            }
        }
        
        float leftWallDist = float.MaxValue;
        RaycastHit leftHit;
        if(Physics.Raycast(rb.position, Quaternion.AngleAxis(-90f, Vector3.up) * horVel, out leftHit, 1f)) {
            if(leftHit.collider.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
                leftWallDist = Vector3.Distance(transform.position, leftHit.point);

                if (leftWallDist < rightWallDist) {
                    isRunningOnRightWall = false;
                    //Debug.Log("closer to left wall");
                }
            }
        } 
        
        if(rightWallDist == float.MaxValue && leftWallDist == float.MaxValue) {
            wallRunDirection = new Vector3(0,0,0);

            RaycastHit lastHit;
            if(Physics.Raycast(transform.position, horVel.normalized, out lastHit, 1f)) {
                Debug.Log("wall in front, cling");
                return;
            }

            Debug.Log("no wall");
            DismountFromWall(true);
            return;
        }

        if(isRunningOnRightWall) {
            ApplyWallNormalFromHit(rightHit, false);
        } else {
            ApplyWallNormalFromHit(leftHit, true);
        }
        Debug.DrawRay(transform.position, wallRunDirection, Color.gray, 0.01f);

    }

    void ApplyWallNormalFromHit(RaycastHit hit, bool reverseCross) {
        if(hit.normal == wallNormal) {
            return;
        }

        float ang = Vector3.Angle(wallNormal, hit.normal);
        if(Vector3.Angle(wallNormal, hit.normal) < 39f) {
            wallNormal = hit.normal;
            Debug.Log("change normal: " + ang);
            if(reverseCross) {
                wallRunDirection = -Vector3.Cross(transform.up, wallNormal);
            } else {
                wallRunDirection = Vector3.Cross(transform.up, wallNormal);
            }
        } else {
            Debug.Log("on wall but too big angle: " + ang);
        }
    }


    public IEnumerator CanActCoolDown(float cooldownTimeSeconds)
    {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        canAct = true;
    }

    void OnCollisionStay(Collision other) {
        if (this.enabled && rb.velocity.magnitude < 1f 
        && gpc.recentAction != RecentActionType.OnWall && canAct) {
            DismountFromWall();
        }
    }
}
