using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

    public float jumpPadPower = 10f;
    
    void OnCollisionEnter(Collision other) {
        
        Rigidbody rb;
        if(rb = other.gameObject.GetComponent<Rigidbody>()) {
            //rb.velocity = rb.velocity + (transform.up * jumpPadPower);
            

            GlobalPlayerController gpc;
            if(gpc = other.gameObject.GetComponent<GlobalPlayerController>()) {
                gpc.recentAction = RecentActionType.None;
            }

            rb.AddForce(transform.up * jumpPadPower, ForceMode.VelocityChange);

        }
    }
}
