﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlayerController : MonoBehaviour
{

    public Vector3 wallNormal;
    GlobalPlayerController globalPlayerController;

    bool canAct = true;

    public float initialJumpForce = 17.5f;
    public float wallSlideDownSpeed = 1f;

    public float defaultWallRunSpeed = 7.5f;

    public Vector3 wallRunDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        globalPlayerController = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        wallNormal = new Vector3(0,0,0);
        wallRunDirection = new Vector3(0,0,0);
    }

    void OnEnable() {
        rb.velocity = new Vector3(0,0,0);
        rb.useGravity = false;
        canAct = false;
        StartCoroutine(CanActCoolDown(0.15f));

        gameObject.transform.rotation = Quaternion.LookRotation(wallNormal);
        globalPlayerController.ResetJumpsAndDashes();
    }

    void OnDisable() {
        rb.useGravity = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(wallRunDirection != new Vector3(0,0,0)) {
            rb.velocity = wallRunDirection * defaultWallRunSpeed;
        } else {
            rb.AddForce(Physics.gravity * wallSlideDownSpeed);
        }
        

        if(InputController.jumpPressed && canAct) {
            rb.velocity = new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z) * initialJumpForce;
            globalPlayerController.hasRecentlyJumped = RecentJumpType.Wall;
            Debug.DrawRay(rb.position, new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z), Color.blue, 2f );
        }

        InputController.jumpPressed = false;
        InputController.dashPressed = false;
    }


    public IEnumerator CanActCoolDown(float cooldownTimeSeconds) {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        canAct = true;
    }
}
