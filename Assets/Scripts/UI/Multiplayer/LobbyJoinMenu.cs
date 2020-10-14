using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Text.RegularExpressions;

public class LobbyJoinMenu : MonoBehaviour {

    // Start is called before the first frame update

    [Header("UI")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;
    [SerializeField] GameObject youSureWindow;


    void Start() {
        ipInputField.text = PlayerPrefs.GetString("LastJoinedIp", "localhost");
    }


    void Update() {
        if(Input.GetButtonDown("Cancel")) {
            ToggleExitWindow(!youSureWindow.activeSelf);
        }
    }

    public void ToggleExitWindow(bool visible) {
        youSureWindow.SetActive(visible);
        if (visible) {
            ipInputField.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            Selectable first = youSureWindow.GetComponentInChildren<Selectable>();
            first.Select();
            first.OnSelect(null);
        } else {
            ipInputField.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
            joinButton.Select();
            joinButton.OnSelect(null);
        }
    }

    public void ReturnToMain() {
        LevelTransitionLoader.instance.LoadSceneWithTransition("MainMenu");
    }

    public void JoinLobby() {
        string ipAddress = ipInputField.text;
        NetworkManager.singleton.networkAddress = ipAddress;
        ipAddress = Regex.Replace(ipAddress, @"\s", "");
        Debug.Log(ipAddress);
        if (!NetworkServer.active) {
            NetworkManager.singleton.StartClient();
            PlayerPrefs.SetString("LastJoinedIp", ipAddress);
            PlayerPrefs.Save();
        }

        joinButton.interactable = false;
    }

    public void HostLobby() {
        if (!NetworkServer.active) {
            NetworkManager.singleton.StartHost();
        }
    }

    public void ToggleShowJoinMenu(bool show) {
        gameObject.SetActive(show);
    }
}
