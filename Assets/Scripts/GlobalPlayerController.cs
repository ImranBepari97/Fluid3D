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

    public bool isGrounded;

    public RecentJumpType hasRecentlyJumped;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        defaultPlayerController = GetComponent<DefaultPlayerController>();
        wallPlayerController = GetComponent<WallPlayerController>();
        hasRecentlyJumped = RecentJumpType.None;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();

        if(isGrounded) {
            EnableDefaultControls();
        }
    }

    private void FixedUpdate() {
        switch(hasRecentlyJumped) {
            case RecentJumpType.Regular:
                StartCoroutine(JumpControlCoolDown(0.01f));
                break;
            case RecentJumpType.Wall:
                StartCoroutine(JumpControlCoolDown(0.02f));
                break;

        }
    }

    bool CheckIfGrounded() {
        RaycastHit hit;
        //Debug.DrawLine(rb.position, rb.position - Vector3.up * 1.1f);

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
            Debug.DrawRay(other.GetContact(0).point, other.GetContact(0).normal ,Color.white, 1f);

            wallPlayerController.wallNormal = other.GetContact(0).normal;

            if(!isGrounded && other.GetContact(0).normal.y > -0.3f && other.GetContact(0).normal.y < 0.5f ) {
                lastWallTouched = other.collider.gameObject;
                Debug.Log("OnWall");   
                EnableWallControls();
            }
            
        }
    }
}
