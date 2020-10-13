using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MainRoomManager : NetworkRoomManager {


    public List<NetworkConnection> playersToGoIngame = new List<NetworkConnection>();


    /// <summary>
    /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
    /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
    /// into the GamePlayer object as it is about to enter the Online scene.
    /// </summary>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer) {
        GamePlayerEntity gpe = gamePlayer.GetComponent<GamePlayerEntity>();
        gpe.displayName = roomPlayer.GetComponent<RoomPlayerEntity>().displayName;

        gpe.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        gpe.GetComponent<Rigidbody>().useGravity = false;
        gpe.GetComponent<GlobalPlayerController>().DisableAllControls();

        return true;
    }

    public override void OnRoomClientDisconnect(NetworkConnection conn) {
        base.OnRoomClientDisconnect(conn);
        playersToGoIngame.Clear();
    }

    public override void OnRoomServerDisconnect(NetworkConnection conn) {
        base.OnRoomServerDisconnect(conn);

        if (playersToGoIngame.Contains(conn)) {
            playersToGoIngame.Remove(conn);
        }
    }

    public override void OnRoomStopClient() {
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopClient();

    }

    public override void OnRoomStopServer() {
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopServer();

        playersToGoIngame.Clear();
    }

    public void StartGame() {
        ServerChangeScene(GameplayScene);
    }

    public override void ServerChangeScene(string newSceneName) {
        LevelTransitionLoader.instance.PlayTransitionFadeIn();

        if (newSceneName != RoomScene) {
            foreach (NetworkRoomPlayer nrp in roomSlots) {
                playersToGoIngame.Add(nrp.connectionToClient);
            }
        }

        base.ServerChangeScene(newSceneName);

        string[] sceneDefaultName = newSceneName.Split('_');
        Debug.Log("Switching to " + sceneDefaultName[0]);
        // Debug.Log("Is " + sceneDefaultName[0] + "_Geometry.unity valid? " + SceneManager.GetSceneByName(sceneDefaultName[0] + "_Geometry.unity").IsValid());
        if (Application.CanStreamedLevelBeLoaded(sceneDefaultName[0] + "_Geometry.unity")) {
            Debug.Log("Loading geometry for " + sceneDefaultName[0]);
            SceneManager.LoadSceneAsync(sceneDefaultName[0] + "_Geometry.unity", LoadSceneMode.Additive);
            NetworkServer.SendToAll(new SceneMessage { sceneName = sceneDefaultName[0] + "_Geometry.unity", sceneOperation = SceneOperation.LoadAdditive });
        }
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) {

        if(sceneOperation != SceneOperation.LoadAdditive) {
            LevelTransitionLoader.instance.PlayTransitionFadeIn();
        }
        
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }

    bool showStartButton;

    public override void OnRoomServerPlayersReady() {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
        showStartButton = true;
        (roomSlots[0] as RoomPlayerEntity).TargetToggleStartButton((roomSlots[0] as RoomPlayerEntity).connectionToClient, true);
#endif
    }

    public override void OnRoomServerPlayersNotReady() {
        (roomSlots[0] as RoomPlayerEntity).TargetToggleStartButton((roomSlots[0] as RoomPlayerEntity).connectionToClient, false);
    }

    public string GetRoomScene() {
        return RoomScene;
    }
}
