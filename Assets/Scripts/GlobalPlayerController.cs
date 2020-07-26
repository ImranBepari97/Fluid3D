using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{

    DefaultPlayerController defaultPlayerController;
    WallPlayerController wallPlayerController;

    GameObject lastWallTouched;

    public int extraJumps = 1;
    public int currentJumps;

    public int numberOfDashes = 1;
    public int currentDashes;

    public bool isGrounded;

    public RecentJumpType hasRecentlyJumped;

    Rigidbody rb;

    bool isResetCRRunning;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        defaultPlayerController = GetComponent<DefaultPlayerController>();
        wallPlayerController = GetComponent<WallPlayerController>();
        hasRecentlyJumped = RecentJumpType.None;
        currentJumps = 0;
        currentDashes = 0;
        isResetCRRunning = false;
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

        if(!isResetCRRunning) {
            switch(hasRecentlyJumped) {
                case RecentJumpType.Regular:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.01f));
                    break;
                case RecentJumpType.Wall:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.35f));
                    break;
                case RecentJumpType.Dash:
                    isResetCRRunning = true;
                    StartCoroutine(JumpControlCoolDown(0.25f));
                    break;
            }
        }
    }

    private void FixedUpdate() {
        
    }

    bool CheckIfGrounded() {
        RaycastHit hit;

        if(Physics.Raycast(rb.position, -Vector3.up, out hit, 1.1f)) {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
               isGrounded = true;
            } 
        } else {
            isGrounded = false;
        }

        return isGrounded;
    }


    //Basically controls the boolean that see if you've jumped in the last split second
    //Generally used to give the player absolute control in this short window 
    public IEnumerator JumpControlCoolDown(float cooldownTimeSeconds) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        hasRecentlyJumped = RecentJumpType.None;
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour") && wallPlayerController.enabled) {
            Debug.Log("OffWall");
            EnableDefaultControls();
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour")) {
            //Debug.DrawRay(other.GetContact(0).point, other.GetContact(0).normal ,Color.white, 1f);

            //Are you actually touching a wall?
            bool isCorrectAngle = other.GetContact(0).normal.y > -0.3f && other.GetContact(0).normal.y < 0.5f;
            if(!isGrounded && isCorrectAngle && 
            (other.gameObject != lastWallTouched || other.GetContact(0).normal != wallPlayerController.wallNormal)) {
                Debug.DrawRay(rb.position, -(rb.position - other.GetContact(0).point), Color.yellow, 2f);
                
                Vector3 currentHorizontalVelocity = rb.velocity;
                currentHorizontalVelocity.y = 0;
                //Debug.Log(currentHorizontalVelocity);
                if(CanWallRun(Vector3.Dot((rb.position - other.GetContact(0).point).normalized, other.GetContact(0).normal.normalized))) {
                    wallPlayerController.wallRunDirection = currentHorizontalVelocity.normalized;
                } else {
                    wallPlayerController.wallRunDirection = new Vector3(0,0,0);
                }

                lastWallTouched = other.collider.gameObject;
                wallPlayerController.wallNormal = other.GetContact(0).normal;
                Debug.Log("OnWall");   
                EnableWallControls();
            }
        }
    }

    public bool CanWallRun(float angleAsDotProduct) {
        Vector3 currentHorizontalVelocity = rb.velocity;
        currentHorizontalVelocity.y = 0;

        Debug.Log("DOT: " + angleAsDotProduct + " VelXz =" +  currentHorizontalVelocity.magnitude);

        bool correctRunSpeed = currentHorizontalVelocity.magnitude > 0.5f * defaultPlayerController.defaultRunSpeed;
        bool correctDotProduct = angleAsDotProduct < 0.875f;

        if(correctRunSpeed && correctDotProduct) {
            return true;
        }
        
        return false;
    }

    public void ResetJumpsAndDashes() {
        currentJumps = extraJumps; 
        currentDashes = numberOfDashes;
    }
}
