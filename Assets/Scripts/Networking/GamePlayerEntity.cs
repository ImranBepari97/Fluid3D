using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerEntity : NetworkBehaviour {

    [Header("Variables")]
    [SyncVar]
    public string displayName = "Loading...";

    public override void OnStartClient() {
        DontDestroyOnLoad(this.gameObject);
    }

    [Server]
    public void SetDisplayName(string name) {
        this.displayName = name;
    }

    [TargetRpc]
    public void TargetSetPosition(NetworkConnection target, Vector3 position, Quaternion rotation) {
        Debug.Log("New Spawn pos recieved for " + target + " is " + position); 
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;   
    }


}
