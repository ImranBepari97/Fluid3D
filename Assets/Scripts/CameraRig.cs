using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    public float mouseSensitivity = 1f;

    public float stickXSensitivity = 1f;
    public float stickYSensitivity = 1f;


    public GameObject target;

    

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {

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
}
