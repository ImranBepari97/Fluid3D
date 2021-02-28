using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class ItemController : NetworkBehaviour {

    [SyncVar(hook = nameof(OnItemChanged))] public NetworkIdentity currentItem;
    CinemachineVirtualCamera aimCam;

    public Transform lookAtTarget;

    GlobalPlayerController gpc;
    DefaultPlayerController dpc;

    public Transform restWeaponBone;
    public Transform useWeaponBone;

    public bool isAiming;

    ItemBase currentItemBase;

    void Start() {
        aimCam = GameObject.FindGameObjectWithTag("AimCamera").GetComponent<CinemachineVirtualCamera>();
        lookAtTarget = GameObject.FindGameObjectWithTag("LookAtTarget").transform;
        gpc = GetComponent<GlobalPlayerController>();
        dpc = GetComponent<DefaultPlayerController>();
        isAiming = false;
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) {
            // exit from update if this is not the local player
            return;
        }

        if (Input.GetAxis("Aim") > 0) {

            if (currentItemBase == null || !currentItemBase.requiresAim) {
                CmdUnaim();
                return;
            }

            if (gpc.recentAction == RecentActionType.None || gpc.recentAction == RecentActionType.Grind || gpc.recentAction == RecentActionType.RegularJump) {
                CmdAim();
            } else {
                CmdUnaim();
            }
        } else {
            CmdUnaim();
        }

        if (Input.GetButtonDown("Fire")) {
            if (currentItemBase != null && (!currentItemBase.requiresAim || (currentItemBase.requiresAim && isAiming))) {
                Debug.Log("Trying to fire");
                currentItemBase.TryUse();
            }
        }
    }

    [Command]
    void CmdAim() {
        isAiming = true;
        aimCam.Priority = 11;
        dpc.defaultRunSpeed = 7.5f;

        Vector3 lookAtTargetNoY = lookAtTarget.position; 
        lookAtTargetNoY.y = transform.position.y;
        gameObject.transform.rotation = Quaternion.LookRotation(lookAtTargetNoY - transform.position); //basically point the waist and legs in the direction of the aim, but not torso

    }

    [Command]
    void CmdUnaim() {
        isAiming = false;

        aimCam.Priority = 9;
        dpc.defaultRunSpeed = 10f;
    }

    void OnItemChanged(NetworkIdentity _, NetworkIdentity newValue) {
        Debug.Log("Received item: " + newValue.gameObject);
        currentItemBase = newValue.GetComponent<ItemBase>();
    }

}
