using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerEntity : NetworkBehaviour {

    [Header("Variables")]
    [SyncVar]
    public string displayName = "Loading...";

    private LobbyNetworkManager lobbyManager;
    private LobbyNetworkManager LobbyManager {
        get {
            if (lobbyManager != null) return lobbyManager;

            if(NetworkManager.singleton is LobbyNetworkManager) {
                return lobbyManager = NetworkManager.singleton as LobbyNetworkManager;
            }

            return null;
        }
    }

    void Awake() {
        if(NetworkManager.singleton is OfflineNetworkManager) {
            Destroy(this);
        }
    }

    public override void OnStartClient() {

        DontDestroyOnLoad(this.gameObject);
        LobbyManager.GamePlayers.Add(this);
    }

    public override void OnStopClient() {

        LobbyManager.GamePlayers.Remove(this);
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
