using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputController : NetworkBehaviour
{
    // Start is called before the first frame update
    public bool jumpPressed;

    public Vector3 moveDirection;

    public bool dashPressed;

    public bool crouchPressed;

    // Update is called once per frame
    void Update()
    {
        if(Time.deltaTime == 0 || (GameControllerCommon.instance != null && GameControllerCommon.instance.gameState != GameState.PLAYING) || PauseMenu.isPaused) {
            return;
        }

        if(!jumpPressed) {
            jumpPressed = Input.GetButtonDown("Jump");
        }

        if(!dashPressed) {
            dashPressed = Input.GetButtonDown("AirDash");
        }

        crouchPressed = Input.GetButton("Crouch");

        Vector3 camForward = GlobalPlayerController.GetForwardRelativeToCamera();
        Vector3 camRight = new Vector3(camForward.z, 0f, -camForward.x);

        Vector3 movementX = camRight * Input.GetAxis("Horizontal");
        Vector3 movementZ = camForward * Input.GetAxis("Vertical");
        moveDirection = (movementX + movementZ);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
    }

    [Client]
    public void CmdResetJumpAndDashInput() {
        jumpPressed = false;
        dashPressed = false;
    }
}
