using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    public float sensitivity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        gameObject.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        gameObject.transform.Rotate(-Input.GetAxis("Mouse Y")* sensitivity, 0, 0);

        Vector3 eulerRotation = transform.rotation.eulerAngles;
        float clampedX = eulerRotation.x;
        
        if(clampedX > 180) {
            clampedX = Mathf.Clamp(eulerRotation.x, 275f, 360f);
        } else {
            clampedX = Mathf.Clamp(eulerRotation.x, 0f, 67.5f);
        }
         
        transform.rotation = Quaternion.Euler(clampedX, eulerRotation.y, 0); //cancel z 


        if(Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }

    }
}
