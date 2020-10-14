using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomPlayerEntity : NetworkRoomPlayer {
    [SyncVar(hook = nameof(UpdateUI))]
    public string displayName = "Loading...";

    [SyncVar]
    public bool isLeader = false;

    public static RoomPlayerEntity localRoomPlayer;

    LobbyUI ui;

    public override void OnStartAuthority() {
        ui = LobbyUI.instance;
        CmdSetDisplayName(PlayerNameField.DisplayName);
        ui.UpdateDisplay();
    }

    [Command]
    private void CmdSetDisplayName(string name) {
        displayName = name;
    }

    public override void OnClientEnterRoom() { 
        ui = LobbyUI.instance;
        ui.UpdateDisplay();
    }

    public override void OnStartClient() {
        ui = LobbyUI.instance;
        SetStaticInstance();
        ui.UpdateDisplay();
    }

    public override void OnStopClient() {
        base.OnStopClient();
        ui.UpdateDisplay();
    }

    private void SetStaticInstance() {
        if (isLocalPlayer) {
            localRoomPlayer = this;
        }
    }

    public void UpdateUI(string oldValue, string newValue) {
        if (ui != null)
            ui.UpdateDisplay();
    }

    [TargetRpc]
    public void TargetToggleStartButton(NetworkConnection target, bool readyToStart) {
        if(isLocalPlayer) {
            ui.startGameButton.interactable = readyToStart;
        }
    }

    [Command]
    public void CmdStartGame() {
        (NetworkManager.singleton as MainRoomManager).StartGame();
    }

    public override void ReadyStateChanged(bool _, bool newReadyState) {
        if (ui != null) {
            ui.UpdateDisplay();
        }
    }

    public override void IndexChanged(int oldIndex, int newIndex) {
        if (ui != null)
            ui.UpdateDisplay();
    }
}
