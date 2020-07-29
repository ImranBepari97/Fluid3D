using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    public float mouseSensitivity = 1f;

    public float stickXSensitivity = 1f;
    public float stickYSensitivity = 1f;


    public GameObject target;
    Rigidbody targetRb;

    public bool isManuallyMovingCamera;
    

    // Start is called before the first frame update
    void Awake() { 
        Cursor.lockState = CursorLockMode.Locked;
        targetRb = target.GetComponent<Rigidbody>();
        isManuallyMovingCamera = false;

    }

    // Update is called once per frame
    void Update() {
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0
            || Input.GetAxis("RStick X") != 0 || Input.GetAxis("RStick Y") != 0) {
            isManuallyMovingCamera = true;
        } else {
            isManuallyMovingCamera = false;
        }
        

        gameObject.transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        gameObject.transform.Rotate(-Input.GetAxis("Mouse Y")* mouseSensitivity, 0, 0);

        gameObject.transform.Rotate(0, Input.GetAxis("RStick X") * stickXSensitivity, 0);
        gameObject.transform.Rotate(-Input.GetAxis("RStick Y") * stickYSensitivity, 0, 0);

        Vector3 eulerRotation = transform.rotation.eulerAngles;
        float clampedX = eulerRotation.x;
        
        if(clampedX > 180) {
            clampedX = Mathf.Clamp(eulerRotation.x, 275f, 360f);
        } else {
            clampedX = Mathf.Clamp(eulerRotation.x, 0f, 67.5f);
        }
         
        transform.rotation = Quaternion.Euler(clampedX, eulerRotation.y, 0); //cancel z 

        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target.transform.position, 1f);
        gameObject.transform.position = target.transform.position;

           

        if(Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    void FixedUpdate() {
        Vector3 xzMovement = targetRb.velocity;
        xzMovement.y = 0;
        if (!isManuallyMovingCamera && xzMovement.magnitude > 4f) {

            Vector3 camDirection = gameObject.transform.forward;
            camDirection.y = 0;

            //Closer DOT product to 0 the more perpendicular the camera is to movement 

            //Debug.Log("DOT camera: " + Vector3.Dot(camDirection.normalized , xzMovement.normalized));
            float camMoveDot = Vector3.Dot(camDirection.normalized, xzMovement.normalized);
            //TODO FIX IT so that small adjustments can happen
            if(camMoveDot > -0.4 && camMoveDot < 0.4f) {
                Vector3 rotated = Vector3.RotateTowards(camDirection, xzMovement, xzMovement.magnitude * Time.deltaTime * 0.1f, 2f);
                transform.rotation = Quaternion.LookRotation(rotated);
            }
            

        }
    }
}
