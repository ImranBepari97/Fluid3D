using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class GlobalPlayerController : MonoBehaviour
{

    DefaultPlayerController defaultPlayerController;
    WallPlayerController wallPlayerController;

    GrindPlayerController grindPlayerController;

    PlayerHealth health;

    public GameObject lastWallTouched;

    public int extraJumps = 1;

    
    public int currentJumps;

    public int numberOfDashes = 1;
    public int currentDashes;

    public float currentSpeedMultiplier = 1f;
    public float maxSpeedMultiplier = 2f;
    public bool isGrounded;

    public RecentActionType recentAction;

    Rigidbody rb;
    CapsuleCollider cc;

    bool isResetCRRunning; //bool for checking if the coroutines for resetting actions is active

    public float dotProductOfNearestWall; //the angle of the players velocity compared to the nearest wall they're facing

    public Vector3 floorNormal; //the normal of the floor the player is standing on 

    


    // Start is called before the first frame update
    void Awake()
    {
        isGrounded = false;
        health = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody>();
        defaultPlayerController = GetComponent<DefaultPlayerController>();
        wallPlayerController = GetComponent<WallPlayerController>();
        grindPlayerController = GetComponent<GrindPlayerController>();
        cc = GetComponent<CapsuleCollider>();
        recentAction = RecentActionType.None;
        currentJumps = 0;
        currentDashes = 0;
        isResetCRRunning = false;
        dotProductOfNearestWall = 0f;
    }

    // Update is called once per frame
    void Update() {

        CheckIfGrounded();
        CheckFloorNormal();

        if(isGrounded) {
            ResetJumpsAndDashes();
            EnableDefaultControls();
            wallPlayerController.wallNormal = new Vector3(0,0,0);
        }

        RaycastHit hit;
        Vector3 xzVelocity = rb.velocity;
        xzVelocity.y = 0f;
        Debug.DrawRay(rb.position, xzVelocity.normalized, Color.white, 0.01f);
        if (Physics.Raycast(rb.position, xzVelocity.normalized, out hit, 5f)) {
            dotProductOfNearestWall = Mathf.Abs(Vector3.Dot(hit.normal, xzVelocity.normalized));
        } else {
            dotProductOfNearestWall = 1f;
        }

        if (!isResetCRRunning) {
            switch(recentAction) {
                case RecentActionType.RegularJump:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.01f, recentAction));
                    break;
                case RecentActionType.SlideJump:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.35f, recentAction));
                    break;
                case RecentActionType.WallJump:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCooldownClampSpeed(0.35f, defaultPlayerController.currentMaxSpeed, recentAction));
                    break;
                case RecentActionType.Dash:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCooldownClampSpeed(0.25f, defaultPlayerController.currentMaxSpeed, recentAction));
                    break;
                case RecentActionType.Slide:
                    if(!InputController.crouchPressed) {
                        isResetCRRunning = true;
                        StartCoroutine(JumpControlCoolDown(0.35f, recentAction));
                    }
                    break;
            }
        }
    }

    bool CheckIfGrounded() {

        RaycastHit hit;
        if (Physics.SphereCast(rb.position, cc.radius, -Vector3.up, out hit, 0.49f)) {
            int layerTag = hit.collider.gameObject.layer;
            if (layerTag == LayerMask.NameToLayer("Parkour") || layerTag == LayerMask.NameToLayer("Floor")) {
                //Debug.Log("ground dot: " + Vector3.Dot(hit.normal, Vector3.up));
                if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f) {
                    isGrounded = true;

                    if (recentAction == RecentActionType.None) {
                        currentSpeedMultiplier = 1.0f;
                    }

                    return isGrounded;
                }
            } 
        }

        isGrounded = false;
        return isGrounded;
    }

    void CheckFloorNormal() {
        RaycastHit hit;
        Debug.DrawLine(rb.position, rb.position + (-Vector3.up * 1f), Color.red, 0.01f);
        if (Physics.Raycast(rb.position, -Vector3.up, out hit, 1f)) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Parkour") ||
               hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor")) {
                floorNormal = hit.normal;
                isGrounded = true;
            }
        } else {
            floorNormal = new Vector3(0, 0, 0);
        }
    }

    //Basically controls the boolean that see if you've jumped in the last split second
    //Generally used to give the player absolute control in this short window 
    public IEnumerator JumpControlCoolDown(float cooldownTimeSeconds, RecentActionType entryAction) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        if(recentAction == entryAction) { //check playerstate
            recentAction = RecentActionType.None;
        }
        isResetCRRunning = false;
    }
    
    public IEnumerator JumpControlCooldownClampSpeed(float cooldownTimeSeconds, float maxSpeed, RecentActionType entryAction) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        if(recentAction == entryAction) { //check if something else changed the player state
            rb.velocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), maxSpeed);
            recentAction = RecentActionType.None;
        }
        isResetCRRunning = false;
    }

    public static Vector3 GetForwardRelativeToCamera() {
        Vector3 camDir = Camera.main.transform.forward;
        camDir.y = 0;
        camDir = Vector3.Normalize(camDir);
        
        return camDir;
    }

    public void EnableDefaultControls() {

        defaultPlayerController.enabled = true;
        wallPlayerController.enabled = false;
        grindPlayerController.enabled = false;
    }

    public void EnableWallControls() {
        defaultPlayerController.enabled = false;
        wallPlayerController.enabled = true;
        grindPlayerController.enabled = false;
    }

    public void EnableGrindControls() {
        rb.velocity = new Vector3(0,0,0);
        defaultPlayerController.enabled = false;
        wallPlayerController.enabled = false;
        grindPlayerController.enabled = true;
    }


    public void DisableAllControls() {
        rb.velocity = new Vector3(0,0,0);
        defaultPlayerController.enabled = false;
        wallPlayerController.enabled = false;
        grindPlayerController.enabled = false;
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour") || other.gameObject.tag == "Grind") {
            // Debug.Log("OffWall");
            if (wallPlayerController.wallsCollidingWith.Contains(other.gameObject)) {
                wallPlayerController.wallsCollidingWith.Remove(other.gameObject);
            }

            if(wallPlayerController.wallsCollidingWith.Count == 0 && recentAction != RecentActionType.Grind) {
                EnableDefaultControls();
            }
        }
    }

    void OnCollisionEnter(Collision other) {

        //Grind Detection
        if(other.gameObject.tag == "Grind" && recentAction != RecentActionType.Grind) {
            wallPlayerController.wallsCollidingWith.Add(other.gameObject);
            
            if(other.GetContact(0).normal.y > 0) {
                PathCreator pc;
                if(pc = other.gameObject.transform.parent.gameObject.GetComponent<PathCreator>())  {
                    grindPlayerController.currentRail = pc;
                    grindPlayerController.dstTravelled = pc.path.GetClosestDistanceAlongPath(other.GetContact(0).point);
                    grindPlayerController.roadMeshCreator = pc.gameObject.GetComponent<RoadMeshCreator>();

                    Vector3 horVel = rb.velocity;
                    horVel.y = 0; 

                    Debug.Log(Vector3.Dot(horVel.normalized, pc.path.GetDirectionAtDistance(grindPlayerController.dstTravelled)));
                    grindPlayerController.isReversed = Vector3.Dot(horVel.normalized, pc.path.GetDirectionAtDistance(grindPlayerController.dstTravelled)) > 0 ? false : true;
                    lastWallTouched = null;


                    ResetJumpsAndDashes();
                    EnableGrindControls();
                    
                }
            }
        }

        //Fall Damage Check
        if(other.gameObject.layer == LayerMask.NameToLayer("Parkour") || 
            other.gameObject.layer == LayerMask.NameToLayer("Floor")) {
            if(Vector3.Dot(other.GetContact(0).normal, Vector3.up) > 0.5f) {
                Debug.Log("fallspeed = " + other.relativeVelocity.y);
                health.HandleFallDamage(other.relativeVelocity.y);
                health.deathVelocity = other.relativeVelocity;
            }
        }

        //Wall Running Check
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
            //Debug.DrawRay(other.GetContact(0).point, other.GetContact(0).normal ,Color.white, 1f);
            wallPlayerController.wallsCollidingWith.Add(other.gameObject);

            //Are you actually touching a wall?
            bool isCorrectAngle = other.GetContact(0).normal.y > -0.3f && other.GetContact(0).normal.y < 0.5f; // range for tilted walls
            if(!isGrounded && isCorrectAngle && 
            (other.gameObject != lastWallTouched || other.GetContact(0).normal != wallPlayerController.wallNormal)) {
                Debug.DrawRay(rb.position, -(rb.position - other.GetContact(0).point), Color.yellow, 2f);
                
                Vector3 currentHorizontalVelocity = rb.velocity;
                currentHorizontalVelocity.y = 0;
                //Debug.Log(currentHorizontalVelocity);
                if (CanWallRun(dotProductOfNearestWall)) {
                    wallPlayerController.wallRunDirection = currentHorizontalVelocity.normalized;
                } else {
                    wallPlayerController.wallRunDirection = new Vector3(0,0,0);
                }

                lastWallTouched = other.collider.gameObject;
                wallPlayerController.wallNormal = other.GetContact(0).normal;
                //Debug.Log("OnWall");   
                wallPlayerController.currentWallRunDuration += 0.1f;
                EnableWallControls();

            }
        }
    }

    public bool CanWallRun(float angleAsDotProduct) {
        Vector3 currentHorizontalVelocity = rb.velocity;
        currentHorizontalVelocity.y = 0;

        //Debug.Log("DOT: " + angleAsDotProduct + " VelXz =" +  currentHorizontalVelocity.magnitude);

        bool correctRunSpeed = currentHorizontalVelocity.magnitude > 0.4f * defaultPlayerController.defaultRunSpeed;
        bool correctDotProduct = angleAsDotProduct < 0.76f;

        //if we're already wallrunning, then it's okay if we're slower than the threshold
        if((wallPlayerController.isWallRunning || correctRunSpeed) && correctDotProduct) {
            return true;
        }
        
        return false;
    }

    public void ResetJumpsAndDashes() {
        currentJumps = extraJumps; 
        currentDashes = numberOfDashes;
    }

    public void IncreaseSpeedMultiplier(float speedIncrease) {
        currentSpeedMultiplier += speedIncrease;
        if(currentSpeedMultiplier > maxSpeedMultiplier) {
            currentSpeedMultiplier = maxSpeedMultiplier;
        }
    }
}
