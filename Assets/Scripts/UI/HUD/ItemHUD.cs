using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHUD : MonoBehaviour {

    Animator crosshairAnimator;
    GlobalPlayerController gpc;
    ItemController ic;

    bool localIsAiming; //we check the difference to only play animation once

    // Start is called before the first frame update
    void Awake() {
        crosshairAnimator = GetComponent<Animator>();
        localIsAiming = false;
    }

    // Update is called once per frame
    void Update() {
        if (gpc == null || ic == null) {
            TryFindDefaultPlayerTarget();
            return;
        }

        if(ic.isAiming != localIsAiming) {
            localIsAiming = ic.isAiming;

            if(localIsAiming) {
                crosshairAnimator.ResetTrigger("TurnCrosshairOff");
                crosshairAnimator.SetTrigger("TurnCrosshairOn");
            } else {
                crosshairAnimator.ResetTrigger("TurnCrosshairOn");
                crosshairAnimator.SetTrigger("TurnCrosshairOff");
            }
        }
    }

    void TryFindDefaultPlayerTarget() {
        if (GlobalPlayerController.localInstance != null) {
            gpc = GlobalPlayerController.localInstance;
            ic = gpc.GetComponent<ItemController>();
        }
    }
}
