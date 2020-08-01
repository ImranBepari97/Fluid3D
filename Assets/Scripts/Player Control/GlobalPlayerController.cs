using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{

    DefaultPlayerController defaultPlayerController;
    WallPlayerController wallPlayerController;

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

    bool isResetCRRunning;

    float dotProductOfNearestWall;

    public Vector3 floorNormal;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        defaultPlayerController = GetComponent<DefaultPlayerController>();
        wallPlayerController = GetComponent<WallPlayerController>();
        recentAction = RecentActionType.None;
        currentJumps = 0;
        currentDashes = 0;
        isResetCRRunning = false;
        dotProductOfNearestWall = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();

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
        }

        if (!isResetCRRunning) {
            switch(recentAction) {
                case RecentActionType.RegularJump:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.01f));
                    break;
                case RecentActionType.SlideJump:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.35f));
                    break;
                case RecentActionType.WallJump:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCooldownClampSpeed(0.35f, defaultPlayerController.currentMaxSpeed));
                    break;
                case RecentActionType.Dash:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCooldownClampSpeed(0.25f, defaultPlayerController.currentMaxSpeed));
                    break;
                case RecentActionType.Slide:
                    if(!InputController.crouchPressed) {
                        isResetCRRunning = true;
                        StartCoroutine(JumpControlCoolDown(0.35f));
                    }
                    break;
            }
        }
    }

    bool CheckIfGrounded() {
        RaycastHit hit;

        if(Physics.Raycast(rb.position, -Vector3.up, out hit, 1.1f)) {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
                isGrounded = true;
                floorNormal = hit.normal;
            
                if(recentAction == RecentActionType.None) {
                    currentSpeedMultiplier = 1.0f;
                }
            } 
        } else {
            isGrounded = false;
            floorNormal = new Vector3(0, 0, 0);
        }

        return isGrounded;
    }

    //Basically controls the boolean that see if you've jumped in the last split second
    //Generally used to give the player absolute control in this short window 
    public IEnumerator JumpControlCoolDown(float cooldownTimeSeconds) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        recentAction = RecentActionType.None;
        isResetCRRunning = false;
    }
    

    public IEnumerator JumpControlCooldownClampSpeed(float cooldownTimeSeconds, float maxSpeed) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        rb.velocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), maxSpeed);
        recentAction = RecentActionType.None;
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
    }

    public void EnableWallControls() {
        defaultPlayerController.enabled = false;
        wallPlayerController.enabled = true;
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
            // Debug.Log("OffWall");
            if (wallPlayerController.wallsCollidingWith.Contains(other.gameObject)) {
                wallPlayerController.wallsCollidingWith.Remove(other.gameObject);
            }

            if(wallPlayerController.wallsCollidingWith.Count == 0) {
                EnableDefaultControls();
            }
        }
    }

    void OnCollisionEnter(Collision other) {
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
                IncreaseSpeedMultiplier(0.1f);
                EnableWallControls();

            }
        }
    }

    public bool CanWallRun(float angleAsDotProduct) {
        Vector3 currentHorizontalVelocity = rb.velocity;
        currentHorizontalVelocity.y = 0;

        Debug.Log("DOT: " + angleAsDotProduct + " VelXz =" +  currentHorizontalVelocity.magnitude);

        bool correctRunSpeed = currentHorizontalVelocity.magnitude > 0.4f * defaultPlayerController.defaultRunSpeed;
        bool correctDotProduct = angleAsDotProduct < 0.76f;

        if(correctRunSpeed && correctDotProduct) {
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
