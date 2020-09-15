using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    public float mouseXSensitivity = 1f;
    public float mouseYSensitivity = 1f;

    public float stickXSensitivity = 1f;
    public float stickYSensitivity = 1f;


    public GameObject target;
    Rigidbody targetRb;

    public bool isManuallyMovingCamera;
    public static CameraRig instance;

    // Start is called before the first frame update
    void Awake() { 
        Cursor.lockState = CursorLockMode.Locked;
        targetRb = target.GetComponent<Rigidbody>();
        isManuallyMovingCamera = false;

        if(instance != null) {
            Debug.LogWarning("There are two instances of CameraRig in the scene when there shouldn't be. This GameObject will be deleted.");
            Destroy(this.gameObject);
        }  else {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update() {

        if (PauseMenu.isPaused) {
           Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            
        }

        if(Time.deltaTime == 0) {
            return;
        }

        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0
            || Input.GetAxis("RStick X") != 0 || Input.GetAxis("RStick Y") != 0) {
            isManuallyMovingCamera = true;
        } else {
            isManuallyMovingCamera = false;
        }

        if (Cursor.lockState == CursorLockMode.Locked) {
            gameObject.transform.Rotate(0, Input.GetAxis("Mouse X") * mouseXSensitivity, 0);
            gameObject.transform.Rotate(-Input.GetAxis("Mouse Y") * mouseYSensitivity, 0, 0);
        }

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

        //Auto camera rotation
        Vector3 xzMovement = targetRb.velocity;
        xzMovement.y = 0;
        if (!isManuallyMovingCamera && xzMovement.magnitude > 4f) {

            Vector3 camDirection = gameObject.transform.forward;
            camDirection.y = 0;

            //Debug.Log("DOT camera: " + Vector3.Dot(camDirection.normalized , xzMovement.normalized));
            float camMoveDot = Vector3.Dot(camDirection.normalized, xzMovement.normalized);

            float oldX = transform.rotation.eulerAngles.x;
            Vector3 rotated = Vector3.RotateTowards(camDirection, xzMovement, Mathf.Pow(xzMovement.magnitude, 1.5f) * Time.deltaTime * 0.01f / Mathf.Clamp(Mathf.Abs(camMoveDot), 0.25f, 1f), 2f);
            transform.rotation = Quaternion.LookRotation(rotated); //rotate to look at the new angle
            transform.rotation = Quaternion.Euler(oldX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z); // dont rotate X though
        }
    }

    void TryLoadPlayerPrefs() {
        if(PlayerPrefs.HasKey("mouseXInput")) {
           mouseXSensitivity = PlayerPrefs.GetFloat("mouseXInput");
        }

        if(PlayerPrefs.HasKey("mouseYInput")) {
            mouseYSensitivity = PlayerPrefs.GetFloat("mouseYInput");
        } 

        if(PlayerPrefs.HasKey("stickXInput")) {
            stickXSensitivity = PlayerPrefs.GetFloat("stickXInput");
        } 

        if(PlayerPrefs.HasKey("stickYInput")) {
            stickYSensitivity = PlayerPrefs.GetFloat("stickYInput");
        } 

        if(PlayerPrefs.HasKey("stickInverted") && PlayerPrefs.GetInt("stickInverted") != 0) {
            stickXSensitivity *= -1;
            stickYSensitivity *= -1;
        }

        if(PlayerPrefs.HasKey("mouseInverted") && PlayerPrefs.GetInt("mouseInverted") != 0) {
            mouseXSensitivity *= -1;
            mouseYSensitivity *= -1;
        }
    }
}
