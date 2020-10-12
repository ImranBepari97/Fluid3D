using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerEntity : NetworkBehaviour {

    [Header("Variables")]
    [SyncVar]
    public string displayName = "Loading...";

    // public override void OnStartClient() {
    //     DontDestroyOnLoad(this.gameObject);
    // }

    public override void OnStartAuthority() {
        Debug.Log("trying add player to leaderboard");
        CmdAddPlayerToLeaderBoard(this.GetComponent<NetworkIdentity>());

        Debug.Log("Setting cutscene");
        GameObject cutscene = GameObject.FindGameObjectWithTag("Cutscene");
        cutscene.transform.position = transform.position;
        cutscene.transform.rotation = transform.rotation;
    }

    [Command]
    public void CmdAddPlayerToLeaderBoard(NetworkIdentity playerToAdd) {
        Debug.Log("Adding " + playerToAdd.gameObject + " to leaderboard");
        (GameControllerCommon.instance as GameControllerArena).AddPlayerToScoreboard(playerToAdd);
        (NetworkManager.singleton as MainRoomManager).playersToGoIngame.Remove(playerToAdd.connectionToClient);
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
