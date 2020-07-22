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
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);


        if(Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }

    }
}
