using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomPlayerEntity : NetworkRoomPlayer
{
    [SyncVar (hook = nameof(UpdateUI))]
    public string displayName = "Loading...";

    public static RoomPlayerEntity localRoomPlayer;

    LobbyUI ui;

    public override void OnStartAuthority() {
        CmdSetDisplayName(PlayerNameField.DisplayName);
    }

    [Command]
    private void CmdSetDisplayName(string name) {
        displayName = name;
    } 

    public override void OnStartClient() {
        ui = LobbyUI.instance;
        Debug.Log("Starting client doing first update.");
        SetStaticInstance();
        ui.UpdateDisplay();
    }

    private void SetStaticInstance() {
        if(isLocalPlayer) {
            localRoomPlayer = this;
        }
    }

    public void UpdateUI(string oldValue, string newValue) {
        if(ui != null)
            ui.UpdateDisplay();
    }

    [Command]
    public void CmdStartGame() {
        (NetworkManager.singleton as MainRoomManager).StartGame();
    }

    public override void ReadyStateChanged(bool _, bool newReadyState) {
        if(ui != null)
            ui.UpdateDisplay();
    }

    public override void IndexChanged(int oldIndex, int newIndex) {
         if(ui != null)
            ui.UpdateDisplay();
    }
}
