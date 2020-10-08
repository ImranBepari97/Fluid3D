using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LobbyJoinMenu : MonoBehaviour {

    // Start is called before the first frame update

    [Header("UI")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button joinButton;

    private void OnEnable() {
        LobbyNetworkManager.OnClientConnected += HandleClientConnected;
        LobbyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() {
        LobbyNetworkManager.OnClientConnected -= HandleClientConnected;
        LobbyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientConnected() {
        joinButton.interactable = true;

        gameObject.SetActive(false);
    }

    private void HandleClientDisconnected() {
        joinButton.interactable = true;
    }

    public void JoinLobby() {
        string ipAddress = ipInputField.text;
        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    public void HostLobby() {
        NetworkManager.singleton.StartHost();
    }

    public void ToggleShowJoinMenu(bool show) {
        gameObject.SetActive(show);
    }
}
