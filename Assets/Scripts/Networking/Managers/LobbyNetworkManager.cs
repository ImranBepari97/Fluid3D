using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LobbyNetworkManager : NetworkManager
{

    [Scene] [SerializeField] private string menuScene = string.Empty;
    public int minPlayers = 1;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
   // public static event Action<NetworkConnection> OnServerReadied;

    public List<LobbyPlayerEntity> RoomPlayers {get; } = new List<LobbyPlayerEntity>();
    public List<GamePlayerEntity> GamePlayers {get; } = new List<GamePlayerEntity>();

    [Header("Game")]
    public GamePlayerEntity gamePlayerPrefab;


    // Start is called before the first frame update
    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn) {
        if(numPlayers >= maxConnections) {
            conn.Disconnect();
            return;
        }

        //Disconnect if the server has a game running 
        if(SceneManager.GetActiveScene().path != menuScene) {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        if(conn.identity != null) {
            LobbyPlayerEntity player;

            if(player = conn.identity.GetComponent<LobbyPlayerEntity>()) {
                RoomPlayers.Remove(player);
                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        if(SceneManager.GetActiveScene().path == menuScene) {
            bool isLeader = RoomPlayers.Count == 0;

            GameObject roomPlayerInstance = Instantiate(playerPrefab); //player prefab here is the lobby prefab
            roomPlayerInstance.GetComponent<LobbyPlayerEntity>().IsLeader = isLeader;

            //RoomPlayers.Add(roomPlayerInstance.GetComponent<LobbyPlayerEntity>());

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnStopServer() {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState() {
        foreach(LobbyPlayerEntity player in RoomPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart() {
        if(numPlayers < minPlayers)
            return false;

        foreach(LobbyPlayerEntity player in RoomPlayers) {
            if(!player.isReady)
                return false;
        }

        return true;
    }

    public void StartGame() {
        if(SceneManager.GetActiveScene().path == menuScene) {
            if(!IsReadyToStart()) {
                return;
            }

            Debug.Log("Change scene everyone");
            ServerChangeScene("Sandbox_MP");
        }
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) {
        //LevelTransitionLoader.instance.PlayTransitionFadeIn();
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }

    public override void OnClientSceneChanged(NetworkConnection conn) {
        LevelTransitionLoader.instance.PlayTransitionFadeOut();
        base.OnClientSceneChanged(conn);
        // foreach (GamePlayerEntity player in GamePlayers) {
        //     Transform newPos = GetStartPosition();
        //     player.transform.position = newPos.position;
        //     player.transform.rotation = newPos.rotation;
        // }
    }

    
    public override void OnServerSceneChanged(string sceneName) {
        LevelTransitionLoader.instance.PlayTransitionFadeOut();
        base.OnServerSceneChanged(sceneName);
        // foreach (GamePlayerEntity player in GamePlayers) {
        //     Transform newPos = GetStartPosition();
        //     player.transform.position = newPos.position;
        //     player.transform.rotation = newPos.rotation;
        // }
    }

    public override void ServerChangeScene(string newSceneName) {

        if( SceneManager.GetActiveScene().path == menuScene && !(menuScene.Contains(newSceneName)) ) {
            for(int i = RoomPlayers.Count - 1; i >= 0; i--) {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayer = Instantiate(gamePlayerPrefab); //this is the actual game prefab
                gamePlayer.SetDisplayName(RoomPlayers[i].displayName);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayer.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
        //This will call OnServerChangeScene then OnServerSceneChanged after the scene is done loading
    }

    public override void OnServerReady(NetworkConnection conn) {
        if(SceneManager.GetActiveScene().path == menuScene) {
            Debug.Log("Join lobby");
            base.OnServerReady(conn);
        } else {
            
            GamePlayerEntity gpe;
            if(gpe = conn.identity.gameObject.GetComponent<GamePlayerEntity>()) {
                Debug.Log("A client is ready, time to change their pos");
                Transform spawnPos = GetStartPosition();
                gpe.TargetSetPosition(conn, spawnPos.position, spawnPos.rotation);
            }
            
        }
    }
}
