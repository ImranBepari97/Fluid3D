using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LobbyNetworkManager : NetworkManager
{

    [Scene] [SerializeField] private string menuScene = string.Empty;
    public int minPlayers = 2;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    [SerializeField] public List<LobbyPlayerEntity> RoomPlayers {get; } = new List<LobbyPlayerEntity>();


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

            GameObject roomPlayerInstance = Instantiate(playerPrefab);
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
}
