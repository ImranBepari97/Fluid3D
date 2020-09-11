using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

    public float jumpPadPower = 10f;
    public bool resetsDashesAndJumps = false;
    
    void OnCollisionEnter(Collision other) {
        
        Rigidbody rb;
        if(rb = other.gameObject.GetComponent<Rigidbody>()) {

            GlobalPlayerController gpc;
            if(gpc = other.gameObject.GetComponent<GlobalPlayerController>()) {
                gpc.recentAction = RecentActionType.None;
                if (resetsDashesAndJumps) {
                    gpc.ResetJumpsAndDashes();
                }
            }

            rb.AddForce(transform.up * jumpPadPower, ForceMode.VelocityChange);

        }
    }
}
