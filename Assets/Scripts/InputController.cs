using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update

    GlobalPlayerController gpc;

    public static bool jumpPressed;
    public static Vector3 moveDirection;
    public static bool dashPressed;


    void Start()
    {   
        gpc = GetComponent<GlobalPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!jumpPressed) {
            jumpPressed = Input.GetButtonDown("Jump");
        }

        if(!dashPressed) {
            dashPressed = Input.GetButtonDown("AirDash");
            
        }

        Vector3 camForward = GlobalPlayerController.GetForwardRelativeToCamera();
        Vector3 camRight = new Vector3(camForward.z, 0f, -camForward.x);

        Vector3 movementX = camRight * Input.GetAxis("Horizontal");
        Vector3 movementZ = camForward * Input.GetAxis("Vertical");
        moveDirection = (movementX + movementZ);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
    }
}
