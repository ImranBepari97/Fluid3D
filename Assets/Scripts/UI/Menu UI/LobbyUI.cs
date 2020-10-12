using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour {

    public static LobbyUI instance;

    public NetworkRoomManager managerExt;
    public RoomPlayerEntity playerEntity;

    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text[] playerReadyTexts;

    public Button startGameButton = null;
    public TMP_Text readyButtonText = null;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log("There should not be two LobbUIs, deleting this one.");
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (managerExt == null) {
            if (NetworkManager.singleton != null && NetworkManager.singleton is NetworkRoomManager) {
                managerExt = NetworkManager.singleton as NetworkRoomManager;
            }

            return;
        }

        if (playerEntity == null) {
            playerEntity = RoomPlayerEntity.localRoomPlayer;
        }
    }

    public void UpdateDisplay() {

        for(int j = 0; j < playerNameTexts.Length; j++ ) {
            playerNameTexts[j].text = "Waiting for Player ...";
            playerReadyTexts[j].gameObject.SetActive(false);
        }

        for (int i = 0; i < managerExt.roomSlots.Count; i++) {
            playerReadyTexts[i].gameObject.SetActive(true);
            RoomPlayerEntity roomPlayerEntity;
            if (roomPlayerEntity = managerExt.roomSlots[i].GetComponent<RoomPlayerEntity>()) {
                playerNameTexts[i].text = roomPlayerEntity.displayName;

                playerReadyTexts[i].text = roomPlayerEntity.readyToBegin ?
                "<color=green>Ready</color>" : "<color=red> Not Ready</color>";
                
                if(managerExt.roomSlots[i].isLocalPlayer) {
                    if(managerExt.roomSlots[i].readyToBegin) {
                        readyButtonText.text = "UNREADY";
                    } else {
                        readyButtonText.text = "READY UP";
                    }

                    //if you're the leader and the local player
                    if(managerExt.roomSlots[i].index == 0) {
                        startGameButton.gameObject.SetActive(true);
                    } else {
                        startGameButton.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void ToggleReady() {
        playerEntity.CmdChangeReadyState(!playerEntity.readyToBegin);
    }

    public void StartGame() {
        playerEntity.CmdStartGame();
    }
}
