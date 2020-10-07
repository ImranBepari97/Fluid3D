using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LobbyJoinMenu : MonoBehaviour {

    [SerializeField] LobbyNetworkManager networkManager;
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
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    public void HostLobby() {
        networkManager.StartHost();
    }

    public void ToggleShowJoinMenu(bool show) {
        gameObject.SetActive(show);
    }
}
