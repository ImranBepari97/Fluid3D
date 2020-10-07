using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OfflineNetworkManager : NetworkManager
{

    public static OfflineNetworkManager instance;
    GameControllerArena gca;

    new void Awake() {
        base.Awake();
        StartHost();

        if(instance == null) {
            instance = this;
        } else {
            Debug.Log("Seems there are multiple OfflineNetworkManagers, deleting this one.");
            Destroy(this.gameObject);
        }

    }

    new void Start() {
        base.Start();
        if(GameControllerCommon.instance != null && GameControllerCommon.instance is GameControllerArena) {
            gca = (GameControllerArena) GameControllerCommon.instance;
        }
    }

    new void StartHost() {
        base.StartHost();
        NetworkServer.dontListen = true;

        // if(gca != null) {
            
        // }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
