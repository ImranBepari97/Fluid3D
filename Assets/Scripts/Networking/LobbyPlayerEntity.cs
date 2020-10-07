using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayerEntity : NetworkBehaviour
{
    private bool isLeader;

    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts;

    [SerializeField] private TMP_Text[] playerReadyTexts;

    [SerializeField] private Button startGameButton = null;

    [Header("Variables")]

    [SyncVar (hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;

    [SyncVar (hook = nameof(HandleDisplayNameChanged))]
    public string displayName = "Loading...";

    private LobbyNetworkManager lobbyManager; 
    private LobbyNetworkManager LobbyManager {
        get {
            if(lobbyManager != null) return lobbyManager;
            return lobbyManager = NetworkManager.singleton as LobbyNetworkManager;
        }
    }

    public bool IsLeader {
        set { 
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    void Start() {
        if(!isLocalPlayer) {
            lobbyUI.SetActive(false);
        }
    }

    public override void OnStartAuthority() {
        CmdSetDisplayName(PlayerNameField.DisplayName);
        if(!isLocalPlayer) {
            lobbyUI.SetActive(true);
        }
    }

    [Command]
    private void CmdSetDisplayName(string name) {
        displayName = name;
    } 

    [Command]
    public void CmdReadyUp() {
        isReady = !isReady;

        LobbyManager.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame() {
        if(LobbyManager.RoomPlayers[0].connectionToClient != connectionToClient) {
            return;
        }

        Debug.Log("Received command to start game");
        //Start
    }

    private void HandleReadyStatusChanged(bool oldValue, bool newValue) {
        UpdateDisplay();
    }

    private void HandleDisplayNameChanged(string oldValue, string newValue) {
        UpdateDisplay();
    }

    public override void OnStartClient() {
        LobbyManager.RoomPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient() {
        LobbyManager.RoomPlayers.Remove(this);
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if(!isLocalPlayer) {
            foreach(LobbyPlayerEntity player in LobbyManager.RoomPlayers) {
                if(player.hasAuthority) {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for(int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text  = string.Empty;
        }

        for(int i = 0; i < LobbyManager.RoomPlayers.Count; i++) {
            playerNameTexts[i].text = LobbyManager.RoomPlayers[i].displayName;
             playerReadyTexts[i].text = LobbyManager.RoomPlayers[i].isReady ? 
                "<color=green>Ready</color>" : "<color=red> Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart) {
        if(!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

}
