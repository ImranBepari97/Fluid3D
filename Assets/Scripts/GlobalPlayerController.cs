using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerController : MonoBehaviour
{

    DefaultPlayerController defaultPlayerController;
    WallPlayerController wallPlayerController;

    public int extraJumps = 1;
    public int currentJumps;

    public bool isGrounded;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        defaultPlayerController = GetComponent<DefaultPlayerController>();
        wallPlayerController = GetComponent<WallPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();

        if(isGrounded) {
            EnableDefaultControls();
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

            Debug.DrawLine(rb.position, other.GetContact(0).point, Color.white, 1f);
            //Physics.Raycast( out hit);

            float angleDot = Vector3.Dot(Vector3.up, rb.position - other.GetContact(0).point);
            
            if(angleDot > - 0.2f && angleDot < 0.35f && !isGrounded) {
                Debug.Log("OnWall");   
                EnableWallControls();
            }

            
        }

    }
}
