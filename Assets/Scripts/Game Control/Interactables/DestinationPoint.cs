﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestinationPoint : NetworkBehaviour {

    public AudioClip pointSound;
    GameControllerArena gc;

    // Start is called before the first frame update
    void Start() {
        gc = (GameControllerArena) GameControllerCommon.instance;
    }

    void OnTriggerEnter(Collider coll) {
        if (isServer) {
            NetworkIdentity ni;
            if ((ni = coll.gameObject.GetComponent<NetworkIdentity>()) && coll.gameObject.GetComponent<GlobalPlayerController>()) {

                gc.AddPoint(ni, 1);
                gc.SetNewDestination();

                if (ni.isLocalPlayer) {
                    AudioSource.PlayClipAtPoint(pointSound, Camera.main.transform.position, 0.2f);
                    Debug.Log("+1 for :" + coll.gameObject);
                }
            }
        }
    }
}
