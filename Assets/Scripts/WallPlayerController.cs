using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlayerController : MonoBehaviour
{

    public Vector3 wallNormal;
    GlobalPlayerController globalPlayerController;

    public float initialJumpForce = 17.5f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        globalPlayerController = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        wallNormal = new Vector3(0,0,0);
    }

    void OnEnable() {
        rb.velocity = new Vector3(0,0,0);
        rb.useGravity = false;
        globalPlayerController.currentJumps = globalPlayerController.extraJumps;
    }

    void OnDisable() {
        rb.useGravity = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(InputController.jumpPressed) {
            rb.velocity = new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z) * initialJumpForce;
            globalPlayerController.hasRecentlyJumped = RecentJumpType.Wall;
            Debug.DrawRay(rb.position, new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z), Color.blue, 2f );
        }
    }
}
