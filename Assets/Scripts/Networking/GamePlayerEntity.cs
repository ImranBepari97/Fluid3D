using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerEntity : NetworkBehaviour {

    [Header("Variables")]
    [SyncVar]
    public string displayName = "Loading...";


    public override void OnStartAuthority() {
        Debug.Log("Trying add player to leaderboard, sending command");
        CmdAddPlayerToLeaderBoard(this.GetComponent<NetworkIdentity>());

        //LevelTransitionLoader.instance.PlayTransitionFadeOut();
        GameObject cutscene;
        if (cutscene = GameObject.FindGameObjectWithTag("Cutscene")) {
            cutscene.transform.position = transform.position;
            cutscene.transform.rotation = transform.rotation;
        }

        GameObject cameraRig = GameObject.FindObjectOfType<CameraRig>().gameObject;
        cameraRig.transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    [Command]
    public void CmdAddPlayerToLeaderBoard(NetworkIdentity playerToAdd) {
        Debug.Log("Command received, addding " + playerToAdd.gameObject + " to leaderboard");
        if (GameControllerCommon.instance != null && GameControllerCommon.instance is GameControllerArena) {
            (GameControllerCommon.instance as GameControllerArena).AddPlayerToScoreboard(playerToAdd);
        }

        if (NetworkManager.singleton is MainRoomManager) {
            (NetworkManager.singleton as MainRoomManager).playersToGoIngame.Remove(playerToAdd.connectionToClient);
        }
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

    public override void OnStopServer() {
        //Remove from scoreboard
        (GameControllerCommon.instance as GameControllerArena).scoreboard.Remove(GetComponent<NetworkIdentity>());
        base.OnStopServer();
    }

    // public override void OnStopClient() {
    //     //Add to disconnected players, so technically still on scoreboard
    //     KeyValuePair<NetworkIdentity, int> dc = new KeyValuePair<NetworkIdentity, int>
    //         (GetComponent<NetworkIdentity>(), 
    //         (GameControllerCommon.instance as GameControllerArena).scoreboard[GetComponent<NetworkIdentity>()]
    //     );

    //     base.OnStopClient();
    // }
}
