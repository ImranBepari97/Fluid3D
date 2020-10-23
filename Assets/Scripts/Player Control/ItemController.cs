using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class ItemController : NetworkBehaviour
{

    public ItemBase currentItem;
    CinemachineVirtualCamera aimCam;

    public Transform lookAtTarget;

    GlobalPlayerController gpc;
    DefaultPlayerController dpc;

    public bool isAiming;

    void Start() {
        aimCam = GameObject.FindGameObjectWithTag("AimCamera").GetComponent<CinemachineVirtualCamera>();
        lookAtTarget = GameObject.FindGameObjectWithTag("LookAtTarget").transform;
        gpc = GetComponent<GlobalPlayerController>();
        dpc = GetComponent<DefaultPlayerController>();
        isAiming = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Aim") > 0) {

            if(currentItem == null || !currentItem.requiresAim) {
                Unaim();
                return;
            }

            if(gpc.recentAction == RecentActionType.None || gpc.recentAction == RecentActionType.Grind || gpc.recentAction == RecentActionType.RegularJump) {
                Aim();
            } else {
                Unaim();
            }
        } else {
            Unaim();
        }

        if(Input.GetButtonDown("Fire")) {
            if(currentItem != null && (!currentItem.requiresAim || (currentItem.requiresAim && isAiming))) {
                currentItem.Use();
            }
        }
    }

    void Aim() {
        isAiming = true;
        aimCam.Priority = 11;
        dpc.defaultRunSpeed = 7.5f;

        Vector3 lookAtTargetNoY = lookAtTarget.position;
        lookAtTargetNoY.y = transform.position.y;
        gameObject.transform.rotation = Quaternion.LookRotation(lookAtTargetNoY - transform.position);

    }

    void Unaim() {
        isAiming = false;

        aimCam.Priority = 9;
        dpc.defaultRunSpeed = 10f;
    }
}
